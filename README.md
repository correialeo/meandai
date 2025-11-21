# MeandAI

API RESTful para gerenciamento de usuários, habilidades e trilhas de aprendizado com autenticação JWT, API Key e arquitetura limpa.

👨‍💻Participantes
-------------------
- Julia Brito - RM 558831
- Leandro Correia - RM 556203
- Victor Antonopoulos - RM 556313

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture** com separação clara de responsabilidades:

```
MeandAI/
├── MeandAI.Api/          # Controllers, DTOs, Swagger
├── MeandAI.Application/  # Services, Interfaces, Use Cases
├── MeandAI.Domain/       # Entities, Domain Logic
├── MeandAI.Infrastructure/ # EF Core, Repositories, External Services
└── MeandAI.Tests/        # Unit Tests
```

### Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - API RESTful
- **Entity Framework Core** - ORM
- **SQL Server** - Banco de dados
- **JWT Bearer Authentication** - Autenticação via Token
- **API Key Authentication** - Autenticação via Header
- **BCrypt.Net** - Hash de senhas
- **Swagger/OpenAPI** - Documentação
- **xUnit + Moq** - Testes unitários
- **Docker** - Containerização

## 🚀 Configuração Rápida

### 1. Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://docs.docker.com/get-docker/)
- [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (opcional)

### 2. Configurar o Banco de Dados

Inicie o SQL Server com Docker:

```bash
docker-compose up -d
```

Aguarde o container iniciar (pode levar alguns minutos). O banco estará disponível em:
- **Server:** `localhost,1433`
- **User:** `sa`
- **Password:** `MeandAI@123456`

### 3. Configurar Variáveis de Ambiente

Crie o arquivo `.env` na pasta `MeandAI.Api`:

```bash
cd MeandAI.Api
cp .env.example .env
```

Configure as variáveis no arquivo `.env`:

```env
JWT_KEY=sua_chave_secreta_muito_longa_aqui
JWT_ISSUER=MeandAI
JWT_AUDIENCE=MeandAI_Users
JWT_TOKEN_EXPIRATION_HOURS=24

# API Key Configuration
API_KEY=sua_chave_de_api_secreta_aqui

# Connection String
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=MeandAI;User Id=sa;Password=MeandAI@123456;TrustServerCertificate=true;
```

### 4. Executar as Migrations

```bash
dotnet ef database update --project MeandAI.Infrastructure --startup-project MeandAI.Api
```

### 5. Iniciar a API

```bash
dotnet run --project MeandAI.Api
```

A API estará disponível em: `http://localhost:5231`

## 📚 Documentação da API

### Swagger UI

Acesse a documentação interativa: `http://localhost:5231/swagger`

### Endpoints Principais

#### Autenticação

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/v1/auth/login` | Login e geração de token JWT |

**Exemplo de Login:**
```bash
curl -X POST "http://localhost:5231/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@exemplo.com",
    "password": "senha123"
  }'
```

#### Usuários

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `POST` | `/api/v1/users` | Criar novo usuário | ❌ |
| `GET` | `/api/v1/users` | Listar todos os usuários | ✅ |
| `GET` | `/api/v1/users/{id}` | Obter usuário por ID | ✅ |
| `PUT` | `/api/v1/users/{id}` | Atualizar perfil do usuário | ✅ |
| `DELETE` | `/api/v1/users/{id}` | Excluir usuário | ✅ |

**Exemplo de Registro:**
```bash
curl -X POST "http://localhost:5231/api/v1/users" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@exemplo.com",
    "currentRole": "Desenvolvedor",
    "desiredArea": "IA",
    "password": "senha123"
  }'
```

#### Habilidades

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `GET` | `/api/v1/skills` | Listar habilidades | ✅ |
| `POST` | `/api/v1/skills` | Criar nova habilidade | ✅ |
| `GET` | `/api/v1/skills/{id}` | Obter habilidade por ID | ✅ |
| `PUT` | `/api/v1/skills/{id}` | Atualizar habilidade | ✅ |
| `DELETE` | `/api/v1/skills/{id}` | Excluir habilidade | ✅ |

#### Trilhas de Aprendizado

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `GET` | `/api/v1/learning-paths` | Listar trilhas | ✅ |
| `POST` | `/api/v1/learning-paths` | Criar nova trilha | ✅ |
| `GET` | `/api/v1/learning-paths/{id}` | Obter trilha por ID | ✅ |
| `PUT` | `/api/v1/learning-paths/{id}` | Atualizar trilha | ✅ |
| `DELETE` | `/api/v1/learning-paths/{id}` | Excluir trilha | ✅ |

### Autenticação

A API suporta dois métodos de autenticação:

#### 1. JWT Bearer Token

1. Faça login para obter um token
2. Inclua o token no header `Authorization` para acessar endpoints protegidos:

```bash
curl -X GET "http://localhost:5231/api/v1/users" \
  -H "Authorization: Bearer SEU_TOKEN_JWT_AQUI"
```

#### 2. API Key (Novo)

Para acessos automatizados ou integrações de sistema:

1. Configure a variável `API_KEY` no seu arquivo `.env`
2. Inclua a key no header `X-API-Key`:

```bash
curl -X GET "http://localhost:5231/api/v1/users" \
  -H "X-API-Key: SUA_API_KEY_AQUI"
```

**Prioridade de Autenticação:**
- Se `X-API-Key` for fornecida e válida → usa API Key
- Se não tiver API Key, mas tiver JWT válido → usa JWT
- Se não tiver nenhum → retorna 401 (para endpoints protegidos)

**Exemplos de uso:**

```bash
# Com API Key (prioridade)
curl -X GET "http://localhost:5231/api/v1/users" \
  -H "X-API-Key: sua-chave-secreta" \
  -H "Authorization: Bearer token-jwt"  # ignorado se API Key for válida

# Com JWT apenas
curl -X GET "http://localhost:5231/api/v1/users" \
  -H "Authorization: Bearer seu-token-jwt"

# Sem autenticação (endpoint público)
curl -X POST "http://localhost:5231/api/v1/users" \
  -H "Content-Type: application/json" \
  -d '{"name": "João", "email": "joao@teste.com"}'
```

## 🧪 Testes

### Executar Testes Unitários

```bash
dotnet test
```

### Cobertura de Testes

Os testes cobrem as principais funcionalidades:
- ✅ **AuthService**: Validação de credenciais e geração de tokens
- ✅ **JwtService**: Geração e validação de tokens JWT
- ✅ **UsersService**: CRUD de usuários e gerenciamento de habilidades

### Exemplos de Testes

```bash
# Executar testes com detalhes
dotnet test --verbosity normal

# Executar testes de um arquivo específico
dotnet test --filter "FullyQualifiedName~AuthServiceTests"

# Gerar relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Comandos Úteis

### Entity Framework

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project MeandAI.Infrastructure --startup-project MeandAI.Api

# Aplicar migrations
dotnet ef database update --project MeandAI.Infrastructure --startup-project MeandAI.Api

# Remover última migration
dotnet ef migrations remove --project MeandAI.Infrastructure --startup-project MeandAI.Api
```

### Docker

```bash
# Iniciar banco de dados
docker-compose up -d

# Parar banco de dados
docker-compose down

# Ver logs do container
docker-compose logs sqlserver

# Acessar o SQL Server no container
docker exec -it meandai-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P MeandAI@123456
```

### API

```bash
# Build do projeto
dotnet build

# Executar em modo de desenvolvimento
dotnet run --project MeandAI.Api

# Executar em modo de watch (auto-restart)
dotnet watch --project MeandAI.Api

# Publicar para produção
dotnet publish -c Release -o ./publish
```

## 🏛️ Fluxo de Autenticação

### Método 1: JWT (Para usuários)

1. **Registro de Usuário**
   - `POST /api/v1/users` (público)
   - Senha é criptografada com BCrypt
   - Usuário é salvo no banco

2. **Login**
   - `POST /api/v1/auth/login` (público)
   - Valida email e senha contra o banco
   - Gera token JWT se credenciais válidas

3. **Acesso Protegido**
   - Inclua `Authorization: Bearer {token}` nas requisições
   - Token é validado a cada requisição
   - Token expira em 24h (configurável)

### Método 2: API Key (Para sistemas)

1. **Configuração**
   - Defina `API_KEY` no arquivo `.env`
   - Chave deve ser mantida em segredo

2. **Uso**
   - Inclua `X-API-Key: {key}` nas requisições
   - Válido para todos os endpoints protegidos
   - Não expira, ideal para integrações automatizadas

3. **Prioridade**
   - API Key tem precedência sobre JWT
   - Se ambos forem enviados, API Key será usada

## 📊 Estrutura do Banco de Dados

### Tabelas Principais

- **Users**: Informações dos usuários
- **Skills**: Catálogo de habilidades
- **UserSkills**: Relacionamento usuário x habilidade
- **LearningPaths**: Trilhas de aprendizado
- **UserLearningPaths**: Progresso dos usuários

### Diagrama Simplificado

```
Users (1) -----> (N) UserSkills (N) <----- (1) Skills
  |                                              |
  |                                              |
  +-----> (N) UserLearningPaths (N) <----- LearningPaths
```

## 🚀 Deploy

### Docker Production

```bash
# Build da imagem
docker build -t meandai-api .

# Executar em produção
docker run -d -p 8080:8080 --name meandai-prod meandai-api
```

### Environment Variables

Produção:
- `ASPNETCORE_ENVIRONMENT=Production`
- `JWT_KEY`: Use uma chave forte e única
- `API_KEY`: Chave para autenticação via header
- `ConnectionStrings__DefaultConnection`: String de conexão do banco

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🐛 Problemas Conhecidos

- Usuários criados antes da implementação de senhas podem ter `PasswordHash` vazio
- O container SQL Server pode levar até 2 minutos para iniciar completamente

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/correialeo/meandai/issues)
- **Documentação**: `http://localhost:5000/swagger`

---

**MeandAI** - Desenvolvido usando .NET 8.0 e Clean Architecture
