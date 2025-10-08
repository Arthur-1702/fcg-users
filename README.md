# Users API

API para gerenciamento de usuários com arquitetura de microserviços e comunicação orientada a eventos.

## 🚀 Funcionalidades

### Gestão de Usuários
- Criação de usuários
- Autenticação JWT
- Hash seguro de senhas com salt
- Validação de senha forte
- Validação de formato de e-mail
- Controle de permissões administrativas

### Segurança
- Middleware global de tratamento de erros
- Respostas padronizadas
- Logs com RequestId único
- Autorização por endpoint com JWT

## 🧪 Testes

- Testes unitários completos
- Cobertura de regras de domínio, autenticação e serviços
- Cenários válidos e inválidos
- Mocks de repositórios e serviços

## ⚙️ Pré-requisitos

- .NET 8 SDK
- SQL Server

## 🛠️ Configuração

1. Configure a connection string no `appsettings.json` ou variáveis de ambiente
2. Execute as migrations para criar o banco de dados
3. Execute a aplicação
4. Acesse a documentação Swagger em `http://localhost:<porta>/swagger`

## 🔐 Autenticação

1. Faça login em `/auth/login`
2. Use o token Bearer retornado no header `Authorization` das requisições protegidas

## 📁 Estrutura do Projeto

```
FCG.Users/
├── API/                 # Controllers e Middlewares
├── Application/         # Serviços, DTOs e Interfaces
├── Domain/             # Entidades e Regras de Negócio
├── Infrastructure/     # EF, Repositórios, Migrations
├── Tests/              # Testes Unitários e de Integração
└── Documentation/      # Documentação do projeto
```

## ☁️ Infraestrutura Azure

- **Banco de Dados**: Azure SQL Database
- **Containerização**: Azure Container Registry & Container Apps
- **API**: Azure API Management
- **Mensageria**: Azure Service Bus
- **Serverless**: Azure Functions
- **Monitoramento**: New Relic (configurado via Dockerfile)
