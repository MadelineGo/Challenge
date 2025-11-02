FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia de archivos de proyecto para aprovechar la cache de capas
COPY ChallengeApp.Api/ChallengeApp.Api.csproj ChallengeApp.Api/
COPY ChallengeApp.Application/ChallengeApp.Application.csproj ChallengeApp.Application/
COPY ChallengeApp.Application.Test/ChallengeApp.Application.Test.csproj ChallengeApp.Application.Test/
COPY ChallengeApp.Domain/ChallengeApp.Domain.csproj ChallengeApp.Domain/
COPY ChallengeApp.Infrastructure/ChallengeApp.Infrastructure.csproj ChallengeApp.Infrastructure/
COPY ChallengeApp.IntegrationTests/ChallengeApp.IntegrationTests.csproj ChallengeApp.IntegrationTests/
COPY ChallengeApp.sln .

RUN dotnet restore ChallengeApp.sln

# Copia del resto de los archivos y publicación
COPY . .
WORKDIR /src/ChallengeApp.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "ChallengeApp.Api.dll"]
