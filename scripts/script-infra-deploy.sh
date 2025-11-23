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

# ==================== VALIDAR SENHA FORTE ====================
echo ""
echo "🔐 Validando senha do SQL Server..."
if [[ ! "$DB_PASSWORD" =~ [A-Z] ]] || [[ ! "$DB_PASSWORD" =~ [a-z] ]] || [[ ! "$DB_PASSWORD" =~ [0-9] ]] || [[ ! "$DB_PASSWORD" =~ [^a-zA-Z0-9] ]] || [ ${#DB_PASSWORD} -lt 8 ]; then
    echo "❌ ERRO: Senha do SQL Server deve ter no mínimo 8 caracteres, incluindo:"
    echo "   - Letras maiúsculas (A-Z)"
    echo "   - Letras minúsculas (a-z)"
    echo "   - Números (0-9)"
    echo "   - Caracteres especiais (!@#\$%)"
    echo ""
    echo "Exemplo de senha válida: MyS3cur3P@ss!"
    exit 1
fi
echo "✅ Senha validada!"

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
echo "🗄️ Configurando SQL Server Container..."
SQL_CONTAINER_NAME="sqlserver-meandai"

# Deletar se já existe para recriar limpo
if az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME >/dev/null 2>&1; then
    echo "⚠️  SQL Container já existe. Deletando para recriar..."
    az container delete --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --yes
    sleep 15
fi

echo "Criando SQL Server Container com recursos adequados..."
az container create \
    --resource-group $RESOURCE_GROUP \
    --name $SQL_CONTAINER_NAME \
    --image mcr.microsoft.com/mssql/server:2022-latest \
    --dns-name-label "sql-meandai-$(date +%s)" \
    --ports 1433 \
    --cpu 2.0 \
    --memory 4.0 \
    --os-type Linux \
    --environment-variables \
        "ACCEPT_EULA=Y" \
        "MSSQL_SA_PASSWORD=$DB_PASSWORD" \
        "MSSQL_PID=Express" \
        "MSSQL_COLLATION=Latin1_General_CI_AS" \
    --restart-policy Always

echo "⏳ Aguardando SQL Server inicializar (90 segundos)..."
sleep 90

# Verificar se SQL está rodando
echo ""
echo "🔍 Verificando estado do SQL Server..."
SQL_STATE=$(az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --query "containers[0].instanceView.currentState.state" --output tsv)
echo "Estado do SQL: $SQL_STATE"

if [ "$SQL_STATE" != "Running" ]; then
    echo "❌ SQL Server não está rodando corretamente!"
    echo ""
    echo "📋 Logs do SQL Container:"
    az container logs --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --tail 50
    echo ""
    echo "📊 Detalhes do container:"
    az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --query "containers[0].instanceView"
    echo ""
    echo "💡 Possíveis causas:"
    echo "   1. Senha muito fraca (deve ter maiúsculas, minúsculas, números e caracteres especiais)"
    echo "   2. Recursos insuficientes (SQL Server 2022 precisa de 2 CPUs e 4GB RAM)"
    echo "   3. Problemas de rede ou configuração do Azure"
    exit 1
fi

echo "✅ SQL Server rodando com sucesso!"

# Obter FQDN do SQL Server
SQL_FQDN=$(az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --query "ipAddress.fqdn" --output tsv)
SQL_IP=$(az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --query "ipAddress.ip" --output tsv)
SQL_SERVER_FULL="$SQL_FQDN,1433"

echo "🔗 SQL Server FQDN: $SQL_SERVER_FULL"
echo "🔗 SQL Server IP: $SQL_IP"

# Testar conectividade na porta 1433
echo ""
echo "🧪 Testando conectividade na porta 1433..."
for i in {1..10}; do
    if timeout 5 bash -c "cat < /dev/null > /dev/tcp/$SQL_FQDN/1433" 2>/dev/null; then
        echo "✅ Porta 1433 acessível!"
        break
    fi
    if [ $i -eq 10 ]; then
        echo "⚠️  Porta 1433 não está respondendo após 10 tentativas"
        echo "💡 Continuando deploy, mas pode haver problemas de conectividade..."
    else
        echo "⏳ Tentativa $i/10 - Aguardando porta 1433..."
        sleep 10
    fi
done

# ==================== DEPLOY CONTAINER INSTANCE ====================
echo ""
echo "🔍 Verificando se container da API já existe..."
if az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME >/dev/null 2>&1; then
    echo "⚠️  Container já existe. Deletando para recriar..."
    az container delete --resource-group $RESOURCE_GROUP --name $ACI_NAME --yes
    sleep 15
fi

echo ""
echo "📱 Criando Container Instance da API no Azure (usando ACR)..."

# String de conexão
CONNECTION_STRING="Server=$SQL_SERVER_FULL;Database=$DB_NAME;User Id=sa;Password=$DB_PASSWORD;TrustServerCertificate=true;Encrypt=true;"

echo "🔗 Connection String: Server=$SQL_SERVER_FULL;Database=$DB_NAME;User Id=sa;Password=***;TrustServerCertificate=true;Encrypt=true;"

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
        "MEANDAI_DB_CONNECTION=$CONNECTION_STRING" \
    --cpu 1.0 \
    --memory 2.0 \
    --os-type Linux \
    --restart-policy Always

echo ""
echo "⏳ Aguardando container da API inicializar (45 segundos)..."
sleep 45

# ==================== VERIFICAÇÃO E RESULTADOS ====================
echo ""
echo "🔍 Verificando estado do container da API..."
CONTAINER_STATE=$(az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query "containers[0].instanceView.currentState.state" --output tsv)
echo "Estado: $CONTAINER_STATE"

if [ "$CONTAINER_STATE" != "Running" ]; then
    echo "⚠️  Container não está rodando. Verificando logs..."
    az container logs --resource-group $RESOURCE_GROUP --name $ACI_NAME --tail 30
fi

FQDN=$(az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query "ipAddress.fqdn" --output tsv)
IP=$(az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query "ipAddress.ip" --output tsv)

echo ""
echo "✅ =============================================="
echo "✅ Deploy concluído!"
echo "✅ =============================================="
echo ""
echo "📊 Informações da aplicação:"
echo "🌐 URL Swagger: http://$FQDN:8080/swagger"
echo "🌐 URL API: http://$FQDN:8080"
echo "🔢 IP Público API: $IP"
echo ""
echo "🗄️  Informações do SQL Server:"
echo "🔗 SQL Server: $SQL_SERVER_FULL"
echo "🔢 SQL Server IP: $SQL_IP"
echo "💾 Database: $DB_NAME"
echo "👤 Usuário: sa"
echo "🔐 Senha: ******* (mascarada)"
echo ""
echo "🐳 Informações do ACR:"
echo "📦 ACR: $ACR_LOGIN_SERVER"
echo "🏷️  Imagem: $FINAL_IMAGE"
echo ""
echo "🧪 Comandos de teste:"
echo "# Testar API:"
echo "curl http://$FQDN:8080/swagger"
echo ""
echo "# Testar conectividade SQL (requer sqlcmd instalado):"
echo "sqlcmd -S $SQL_SERVER_FULL -U sa -P '$DB_PASSWORD' -Q 'SELECT @@VERSION'"
echo ""
echo "📋 Comandos úteis:"
echo ""
echo "# Ver logs da API:"
echo "az container logs --resource-group $RESOURCE_GROUP --name $ACI_NAME --follow"
echo ""
echo "# Ver logs do SQL Server:"
echo "az container logs --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --follow"
echo ""
echo "# Ver estado da API:"
echo "az container show --resource-group $RESOURCE_GROUP --name $ACI_NAME --query 'containers[0].instanceView'"
echo ""
echo "# Ver estado do SQL:"
echo "az container show --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME --query 'containers[0].instanceView'"
echo ""
echo "# Reiniciar API:"
echo "az container restart --resource-group $RESOURCE_GROUP --name $ACI_NAME"
echo ""
echo "# Reiniciar SQL:"
echo "az container restart --resource-group $RESOURCE_GROUP --name $SQL_CONTAINER_NAME"
echo ""
echo "# Ver imagens no ACR:"
echo "az acr repository list --name $ACR_NAME --output table"
echo ""
echo "# Deletar tudo:"
echo "az group delete --name $RESOURCE_GROUP --yes --no-wait"
echo ""

# ==================== TESTE DE CONECTIVIDADE ====================
echo "🧪 Testando conectividade da API..."
sleep 15

TEST_SUCCESS=false
for i in {1..5}; do
    if curl -s --connect-timeout 10 http://$FQDN:8080 >/dev/null 2>&1; then
        echo "✅ API respondendo corretamente!"
        TEST_SUCCESS=true
        break
    fi
    echo "⏳ Tentativa $i/5 - API ainda não respondeu..."
    sleep 10
done

if [ "$TEST_SUCCESS" = true ]; then
    echo ""
    echo "🎉 =============================================="
    echo "🎉 DEPLOY CONCLUÍDO COM SUCESSO!"
    echo "🎉 Aplicação acessível em: http://$FQDN:8080"
    echo "🎉 =============================================="
else
    echo ""
    echo "⚠️  =============================================="
    echo "⚠️  API ainda está inicializando..."
    echo "⚠️  Aguarde mais alguns instantes e teste:"
    echo "⚠️  http://$FQDN:8080/swagger"
    echo "⚠️  =============================================="
    echo ""
    echo "💡 Para verificar problemas:"
    echo "az container logs --resource-group $RESOURCE_GROUP --name $ACI_NAME"
fi

# Salvar variáveis para a pipeline (se estiver rodando no Azure DevOps)
if [ -n "$SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" ]; then
    echo "##vso[task.setvariable variable=APP_URL]http://$FQDN:8080"
    echo "##vso[task.setvariable variable=APP_FQDN]$FQDN"
    echo "##vso[task.setvariable variable=APP_IP]$IP"
    echo "##vso[task.setvariable variable=SQL_SERVER_FULL]$SQL_SERVER_FULL"
    echo "##vso[task.setvariable variable=SQL_SERVER_IP]$SQL_IP"
    echo "##vso[task.setvariable variable=ACR_NAME]$ACR_NAME"
    echo "##vso[task.setvariable variable=ACR_LOGIN_SERVER]$ACR_LOGIN_SERVER"
fi

echo ""
echo "🎊 Script finalizado!"