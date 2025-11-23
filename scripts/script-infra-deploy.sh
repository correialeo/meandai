#!/bin/bash

# deploy-meandai.sh - Deploy usando ACR (Azure Container Registry)

set -e

echo "🚀 Iniciando deploy da Meandai API no Azure (ACR)..."

# ==================== CONFIGURAÇÕES ====================
RESOURCE_GROUP="${RESOURCE_GROUP:-rg-meandai-sprint}"
LOCATION="West US"
ACI_NAME="aci-meandai-api"

# ACR Configuration
ACR_NAME="${ACR_NAME:-acrmeandai$(date +%s | tail -c 6)}"
DOCKER_HUB_IMAGE="${DOCKER_IMAGE:-correialeo/meandai.api:latest}"

# Configurações do banco
if [ -z "$DB_SERVER" ]; then
    DB_SERVER_NAME="sqlserver-meandai-$(date +%s)"
else
    DB_SERVER_NAME="$DB_SERVER"
fi
DB_NAME="${DB_NAME}"
DB_ADMIN="${DB_USER}"
DB_PASSWORD="${DB_PASSWORD}"

echo "📋 Configurações:"
echo "Resource Group: $RESOURCE_GROUP"
echo "ACR Name: $ACR_NAME"
echo "Docker Hub Image: $DOCKER_HUB_IMAGE"
echo "DB Server: $DB_SERVER_NAME"
echo "DB Name: $DB_NAME"

# ==================== REGISTRAR PROVIDERS ====================
echo ""
echo "📂 Registrando providers necessários..."
az provider register --namespace Microsoft.ContainerInstance --wait
az provider register --namespace Microsoft.Sql --wait
az provider register --namespace Microsoft.ContainerRegistry --wait

echo "✅ Providers registrados!"

# ==================== CRIAR RESOURCE GROUP ====================
echo ""
echo "📦 Verificando Resource Group..."
if ! az group show --name $RESOURCE_GROUP >/dev/null 2>&1; then
    echo "Criando Resource Group..."
    az group create --name $RESOURCE_GROUP --location "$LOCATION"
else
    echo "Resource Group já existe."
fi

# ==================== CRIAR/VERIFICAR ACR ====================
echo ""
echo "🐳 Configurando Azure Container Registry..."
if ! az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP >/dev/null 2>&1; then
    echo "Criando ACR: $ACR_NAME..."
    az acr create \
        --resource-group $RESOURCE_GROUP \
        --name $ACR_NAME \
        --sku Basic \
        --admin-enabled true \
        --location "$LOCATION"
    
    echo "✅ ACR criado com sucesso!"
else
    echo "ACR já existe: $ACR_NAME"
    # Garantir que admin está habilitado
    az acr update --name $ACR_NAME --admin-enabled true
fi

# Obter credenciais do ACR
echo ""
echo "🔐 Obtendo credenciais do ACR..."
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query username --output tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query "passwords[0].value" --output tsv)
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query loginServer --output tsv)

echo "✅ Credenciais obtidas: $ACR_LOGIN_SERVER"

# ==================== IMPORTAR IMAGEM DO DOCKER HUB PARA ACR ====================
echo ""
echo "📥 Importando imagem do Docker Hub para ACR..."
echo "Origem: $DOCKER_HUB_IMAGE"
echo "Destino: $ACR_LOGIN_SERVER/meandai-api:latest"

az acr import \
    --name $ACR_NAME \
    --source docker.io/$DOCKER_HUB_IMAGE \
    --image meandai-api:latest \
    --resource-group $RESOURCE_GROUP \
    --force || {
        echo "⚠️  Falha na importação. Tentando método alternativo..."
        
        # Método alternativo: pull + push usando docker
        echo "Login no ACR..."
        echo "$ACR_PASSWORD" | docker login $ACR_LOGIN_SERVER -u $ACR_USERNAME --password-stdin
        
        echo "Pull da imagem do Docker Hub..."
        docker pull $DOCKER_HUB_IMAGE
        
        echo "Tag da imagem para ACR..."
        docker tag $DOCKER_HUB_IMAGE $ACR_LOGIN_SERVER/meandai-api:latest
        
        echo "Push para ACR..."
        docker push $ACR_LOGIN_SERVER/meandai-api:latest
    }

echo "✅ Imagem disponível no ACR!"

# Verificar se imagem existe no ACR
echo ""
echo "🔍 Verificando imagem no ACR..."
az acr repository show --name $ACR_NAME --image meandai-api:latest

# Imagem final no ACR
FINAL_IMAGE="$ACR_LOGIN_SERVER/meandai-api:latest"
echo "✅ Imagem final: $FINAL_IMAGE"

# ==================== CRIAR SQL SERVER CONTAINER ====================
echo ""
echo "🗄️ Criando SQL Server Container..."
SQL_CONTAINER_NAME="sqlserver-meandai"

# Verificar se container já existe
if az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME >/dev/null 2>&1; then
    echo "SQL Server Container já existe: $SQL_CONTAINER_NAME"
else
    echo "Criando SQL Server Container..."
    az container create \
        --resource-group $RESOURCE_GROUP \
        --name $SQL_CONTAINER_NAME \
        --image mcr.microsoft.com/mssql/server:2022-latest \
        --dns-name-label "sql-meandai-$(date +%s)" \
        --ports 1433 \
        --cpu 1.0 \
        --memory 2.0 \
        --environment-variables \
            "ACCEPT_EULA=Y" \
            "MSSQL_SA_PASSWORD=$DB_PASSWORD" \
            "MSSQL_PID=Express" \
        --restart-policy Always
    
    echo "✅ SQL Server Container criado!"
    sleep 30
fi

# Obter FQDN do SQL Server
SQL_FQDN=$(az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --query "ipAddress.fqdn" --output tsv)
SQL_SERVER_FULL="$SQL_FQDN,1433"

echo "🔗 SQL Server Container: $SQL_SERVER_FULL"

# ==================== DEPLOY CONTAINER INSTANCE ====================
echo ""
echo "🔍 Verificando se container já existe..."
if az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME >/dev/null 2>&1; then
    echo "⚠️  Container já existe. Deletando para recriar..."
    az container delete --resource-group $RESOURCE_GROUP --name $ACI_NAME --yes
    sleep 15
fi

echo ""
echo "📱 Criando Container Instance no Azure (usando ACR)..."

# String de conexão
CONNECTION_STRING="Server=$SQL_SERVER_FULL;Database=$DB_NAME;User Id=sa;Password=$DB_PASSWORD;TrustServerCertificate=true;Encrypt=true;"

# Criar container usando ACR
az container create \
    --resource-group $RESOURCE_GROUP \
    --name $ACI_NAME \
    --image $FINAL_IMAGE \
    --registry-login-server $ACR_LOGIN_SERVER \
    --registry-username $ACR_USERNAME \
    --registry-password "$ACR_PASSWORD" \
    --dns-name-label "meandai-api-$(date +%s)" \
    --ports 8080 80 443 \
    --protocol TCP \
    --ip-address Public \
    --environment-variables \
        "ASPNETCORE_URLS=http://+:8080" \
        "ASPNETCORE_HTTP_PORTS=8080" \
        "DOTNET_RUNNING_IN_CONTAINER=true" \
        "MEANDAI_DB_CONNECTION=Server=$SQL_SERVER_FULL;Database=$DB_NAME;User Id=sa;Password=$DB_PASSWORD;TrustServerCertificate=true;Encrypt=true;" \
    --cpu 1.0 \
    --memory 2.0 \
    --os-type Linux \
    --restart-policy Always

echo ""
echo "⏳ Aguardando container inicializar..."
sleep 30

# ==================== VERIFICAÇÃO E RESULTADOS ====================
echo ""
echo "🔍 Verificando estado do container..."
CONTAINER_STATE=$(az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query "containers[0].instanceView.currentState.state" --output tsv)
echo "Estado: $CONTAINER_STATE"

FQDN=$(az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query "ipAddress.fqdn" --output tsv)
IP=$(az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query "ipAddress.ip" --output tsv)

echo ""
echo "✅ =============================================="
echo "✅ Deploy concluído com sucesso!"
echo "✅ =============================================="
echo ""
echo "📊 Informações da aplicação:"
echo "🌐 URL Swagger: http://$FQDN:8080/swagger"
echo "🌐 URL API: http://$FQDN:8080"
echo "🔢 IP Público: $IP"
echo "🐳 ACR: $ACR_LOGIN_SERVER"
echo "📦 Imagem: $FINAL_IMAGE"
echo "🗄️ SQL Server Container: $SQL_SERVER_FULL"
echo "💾 Database: $DB_NAME"
echo "👤 Usuário: sa"
echo ""
echo "🧪 Comandos de teste:"
echo "# Testar API via FQDN:"
echo "curl http://$FQDN:8080/swagger"
echo ""
echo "# Testar API via IP:"
echo "curl http://$IP:8080"
echo ""
echo "📋 Comandos úteis:"
echo "# Ver logs em tempo real:"
echo "az container logs --resource-group $RESOURCE_GROUP --name $ACI_NAME --follow"
echo ""
echo "# Ver estado atual:"
echo "az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query 'containers[0].instanceView.currentState'"
echo ""
echo "# Reiniciar container:"
echo "az container restart --resource-group $RESOURCE_GROUP --name $ACI_NAME"
echo ""
echo "# Ver imagens no ACR:"
echo "az acr repository list --name $ACR_NAME --output table"
echo ""

# ==================== TESTE DE CONECTIVIDADE ====================
echo "🧪 Testando conectividade..."
sleep 20

if curl -s --connect-timeout 10 http://$FQDN:8080 >/dev/null 2>&1; then
    echo "✅ API respondendo corretamente!"
    echo "🎉 Deploy concluído e aplicação acessível!"
else
    echo "⚠️  API ainda está inicializando..."
    echo "💡 Aguarde mais alguns instantes e teste manualmente"
    echo ""
    echo "Para verificar logs:"
    echo "az container logs --resource-group $RESOURCE_GROUP --name $ACI_NAME"
fi

# Salvar variáveis para a pipeline (se estiver rodando no Azure DevOps)
if [ -n "$SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" ]; then
    echo "##vso[task.setvariable variable=APP_URL]http://$FQDN:8080"
    echo "##vso[task.setvariable variable=APP_FQDN]$FQDN"
    echo "##vso[task.setvariable variable=APP_IP]$IP"
    echo "##vso[task.setvariable variable=DB_SERVER_FULL]$DB_SERVER_NAME.database.windows.net"
    echo "##vso[task.setvariable variable=ACR_NAME]$ACR_NAME"
    echo "##vso[task.setvariable variable=ACR_LOGIN_SERVER]$ACR_LOGIN_SERVER"
fi

echo ""
echo "🎊 Script finalizado!"