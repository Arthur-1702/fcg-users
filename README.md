# 👥 Users API - FCG (FIAP Cloud Games)

API para gerenciamento de usuários com arquitetura de microserviços e comunicação orientada a eventos. Parte da plataforma FCG que oferece um ecossistema completo para jogos em nuvem.

## 🚀 Funcionalidades

### Gestão de Usuários

- ✅ Criação e gerenciamento de usuários
- ✅ Autenticação JWT com tokens de longa duração
- ✅ Hash seguro de senhas com PBKDF2 e salt
- ✅ Validação de senha forte
- ✅ Validação de formato de e-mail
- ✅ Controle de permissões administrativas
- ✅ Notificações de eventos em tempo real via Azure Service Bus

### Segurança

- ✅ Middleware global de tratamento de erros
- ✅ Respostas padronizadas com error tracking
- ✅ Logs com RequestId único para rastreamento
- ✅ Autorização por endpoint com JWT
- ✅ CORS configurado para segurança
- ✅ Rate limiting e proteção contra ataques

### Observabilidade

- ✅ Testes unitários completos com cobertura alta
- ✅ Logging centralizado via New Relic
- ✅ Rastreamento de requisições
- ✅ Métricas de performance

## 🧪 Testes

- Testes unitários completos com xUnit
- Cobertura de regras de domínio, autenticação e serviços
- Cenários válidos e inválidos
- Mocks de repositórios e serviços com Moq
- FluentAssertions para leitura clara dos testes

## 🛠 Tecnologias Utilizadas

| Camada                       | Tecnologias                                 |
| ---------------------------- | ------------------------------------------- |
| **Framework**                | .NET 8                                      |
| **ORM**                      | Entity Framework Core com Migrations        |
| **Banco de Dados**           | SQL Server                                  |
| **Autenticação**             | JWT (JSON Web Tokens)                       |
| **Testes**                   | xUnit, Moq, FluentAssertions                |
| **API Documentation**        | Swashbuckle.AspNetCore (Swagger)            |
| **Segurança**                | PBKDF2 para hash de senhas                  |
| **Logging**                  | Middleware customizado + New Relic          |
| **Containerização**          | Docker com multi-stage build                |
| **Monitoramento**            | New Relic APM                               |
| **Mensageria**               | Azure Service Bus (Tópicos e Subscriptions) |
| **Processamento Assíncrono** | Hosted Services, Azure Functions            |
| **Orquestração**             | Azure Container Apps                        |
| **API Gateway**              | Azure API Management                        |
| **CI/CD**                    | GitHub Actions / Azure DevOps               |

## ⚙️ Pré-requisitos

- .NET 8 SDK ou superior
- SQL Server 2019+ (local ou Azure SQL Database)
- Docker (para containerização)
- Git
- Visual Studio 2022 ou VS Code com C# extensions

## 🛠️ Como Executar Localmente

### 1. Clonar o repositório

```bash
git clone https://github.com/seu-repo/fcg-users.git
cd fcg-users
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Configurar o banco de dados

Atualize a connection string em `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FCGUsersDb;User Id=sa;Password=YourPassword;"
  }
}
```

### 4. Executar as Migrations

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

### 5. Executar a aplicação

```bash
dotnet run --project API
```

A API estará disponível em: `https://localhost:5001`

### 6. Acessar Swagger

```
https://localhost:5001/swagger
```

## 🐳 Executar com Docker

```bash
docker build -t fcg-users:latest .
docker run -p 5001:5001 -e ASPNETCORE_ENVIRONMENT=Production fcg-users:latest
```

Ou usando docker-compose:

```bash
docker-compose up -d
```

## 🔐 Autenticação

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@example.com",
  "password": "SenhaForte123!"
}
```

**Response (200 OK)**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "usuario@example.com",
    "name": "João Silva"
  }
}
```

### Usar o Token

Adicione o token no header `Authorization` de requisições protegidas:

```http
Authorization: Bearer {seu_token_aqui}
```

## 📚 Endpoints Principais

### Usuários

- `POST /api/users` - Criar usuário
- `GET /api/users/{id}` - Obter usuário
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Deletar usuário
- `GET /api/users` - Listar usuários (admin)

### Autenticação

- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/logout` - Logout

### Health Check

- `GET /health` - Status da aplicação

## 📁 Estrutura do Projeto

```
fcg-users/
├── API/                          # Camada de Apresentação
│   ├── Controllers/              # Endpoints da API
│   ├── Middlewares/              # Error handling, logging
│   ├── Models/                   # Request/Response models
│   ├── Program.cs                # Configuração da aplicação
│   └── appsettings.json          # Configurações
│
├── Application/                  # Camada de Aplicação
│   ├── Services/                 # Lógica de negócio
│   ├── Interfaces/               # Contratos de serviços
│   ├── DTO/                      # Data Transfer Objects
│   ├── Mappings/                 # AutoMapper profiles
│   └── Exceptions/               # Exceções customizadas
│
├── Domain/                       # Camada de Domínio
│   ├── Entities/                 # Modelos de domínio
│   ├── Enums/                    # Enumerações
│   ├── Exceptions/               # Exceções de negócio
│   └── Repositories/             # Interfaces de repositórios
│
├── Infrastructure/               # Camada de Infraestrutura
│   ├── Context/                  # DbContext do EF
│   ├── Repositories/             # Implementação de repositórios
│   ├── Migrations/               # Migrações do banco
│   ├── Services/                 # Serviços externos
│   └── Configurations/           # Configurações do EF
│
├── Tests/                        # Testes Automatizados
│   └── UnitTests/                # Testes unitários
│
└── k8s/                          # Manifesto Kubernetes
    ├── deployment.yaml           # Configuração de deployment
    ├── service.yaml              # Serviço
    ├── configmap.yaml            # Variáveis de configuração
    └── secret.yaml               # Secrets
```

## 🚀 Deployment

## 🚀 Deployment

### Azure Container Apps

1. **Build da imagem Docker**

```bash
az acr build --registry {seu-registry} --image fcg-users:latest .
```

2. **Deploy com Kubernetes**

```bash
kubectl apply -f k8s/
```

3. **Verificar status**

```bash
kubectl get pods
kubectl logs -f deployment/fcg-users
```

### Variáveis de Ambiente

Configure as seguintes variáveis:

```env
ASPNETCORE_ENVIRONMENT=Production
DATABASE_CONNECTION_STRING=Server=...;Database=...
JWT_SECRET_KEY=sua-chave-secreta-muito-segura
JWT_EXPIRATION_MINUTES=1440
NEW_RELIC_LICENSE_KEY=seu-license-key
AZURE_SERVICE_BUS_CONNECTION_STRING=Endpoint=...
LOG_LEVEL=Information
```

## ☁️ Infraestrutura Azure

- **Banco de Dados**: Azure SQL Database
- **Container Registry**: Azure Container Registry (ACR)
- **Orquestração**: Azure Container Apps
- **Mensageria**: Azure Service Bus
- **Serverless**: Azure Functions (para processamento assíncrono)
- **API Gateway**: Azure API Management
- **Monitoramento**: New Relic APM
- **CI/CD**: GitHub Actions (workflows em `.github/workflows/`)

## 🧪 Executar Testes

```bash
# Todos os testes
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true

# Teste específico
dotnet test --filter "CategoryName=UserServiceTests"
```

## 📊 Monitoramento

### New Relic

- Dashboard de performance
- Rastreamento de transações
- Alertas automáticos
- Análise de logs

### Health Check

```http
GET /health
```

Retorna status da aplicação e dependências:

```json
{
  "status": "Healthy",
  "timestamp": "2026-01-09T10:30:00Z",
  "database": "Connected",
  "servicebus": "Connected"
}
```

## 📝 Logging

Todos os logs são centralizados via New Relic. O middleware customizado adiciona:

- RequestId único
- Timestamp
- HTTP Method e Path
- Status code
- Duração da requisição
- Erros detalhados

## 🔗 Links Úteis

- [Documentação .NET 8](https://learn.microsoft.com/pt-br/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/pt-br/ef/core/)
- [JWT.io](https://jwt.io/)
- [Azure Documentation](https://learn.microsoft.com/pt-br/azure/)
- [New Relic Docs](https://docs.newrelic.com/)

## 🤝 Contribuindo

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob licença MIT. Veja o arquivo LICENSE para mais detalhes.

## 👥 Autores

- **Projeto**: FIAP Cloud Games (FCG)
- **Mantido por**: Time de Desenvolvimento

## 📞 Suporte

Para problemas, dúvidas ou sugestões, abra uma issue no repositório ou entre em contato com o time de desenvolvimento.
