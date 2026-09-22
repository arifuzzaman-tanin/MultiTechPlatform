FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/MultiTech.Platform.Api/MultiTech.Platform.Api.csproj src/MultiTech.Platform.Api/
COPY src/MultiTech.Platform.Application/MultiTech.Platform.Application.csproj src/MultiTech.Platform.Application/
COPY src/MultiTech.Platform.Contracts/MultiTech.Platform.Contracts.csproj src/MultiTech.Platform.Contracts/
COPY src/MultiTech.Platform.Domain/MultiTech.Platform.Domain.csproj src/MultiTech.Platform.Domain/
COPY src/MultiTech.Platform.Infrastructure/MultiTech.Platform.Infrastructure.csproj src/MultiTech.Platform.Infrastructure/

RUN dotnet restore src/MultiTech.Platform.Api/MultiTech.Platform.Api.csproj

COPY src/ src/

RUN dotnet publish src/MultiTech.Platform.Api/MultiTech.Platform.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MultiTech.Platform.Api.dll"]
