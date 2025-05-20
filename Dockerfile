# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copie o .csproj e restaure as dependências
COPY *.csproj ./
RUN dotnet restore "semantic-kernel-agent-api.csproj"

# Copie o restante dos arquivos
COPY . .

# Publique a aplicação
RUN dotnet publish "semantic-kernel-agent-api.csproj" -c Release -o /app/publish

# Estágio Final/Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponha a porta usada se precisar
EXPOSE 8080

# Ponto de entrada
ENTRYPOINT ["dotnet", "semantic-kernel-agent-api.dll"]