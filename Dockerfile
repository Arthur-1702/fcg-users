# Estágio 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
# Usa imagem Alpine do SDK .NET 8 para compilação (imagem menor que as padrões)

WORKDIR /app

COPY . .

# Restaura pacotes NuGet
RUN dotnet restore fcg-users.sln

# Compila e publica em um único comando
RUN dotnet publish API/API.csproj -c Release -o /app/publish \
    # Eficiente: Evita restauração redundante de pacotes
    --no-restore \ 
    # Produz aplicação dependente do runtime (imagem menor)
    --self-contained false \
    # Desativa trimming (mantém compatibilidade)
    /p:PublishTrimmed=false \
    # Mantém arquivos separados (mais fácil para debugging)
    /p:PublishSingleFile=false

# Estágio 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
# Usa imagem Alpine runtime (muito menor que a imagem SDK, ~100MB vs ~500MB)

# Set working directory
WORKDIR /app

# Instala bibliotecas ICU para suporte a globalização, necessário para versão Alpine
RUN apk add --no-cache icu-libs icu-data-full

# Instala dependências adicionais para SQL Client
RUN apk add --no-cache krb5-libs libgcc libstdc++

# Instala suporte a globalização e fuso horário
RUN apk add --no-cache icu-libs tzdata

# São instaladas apenas as bibliotecas necessárias, mantendo a imagem leve e segura

# Define variáveis de ambiente para globalização (brasileiro)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=America/Sao_Paulo
ENV LC_ALL=pt_BR.UTF-8
ENV LANG=pt_BR.UTF-8

# Multi-stage build copia apenas artefatos finais (reduz tamanho)
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]
