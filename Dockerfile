# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY EnergyCo.Loyalty/EnergyCo.Loyalty.Api.csproj             EnergyCo.Loyalty/
COPY src/EnergyCo.Loyalty.Domain/EnergyCo.Loyalty.Domain.csproj           src/EnergyCo.Loyalty.Domain/
COPY src/EnergyCo.Loyalty.Application/EnergyCo.Loyalty.Application.csproj src/EnergyCo.Loyalty.Application/
COPY src/EnergyCo.Loyalty.Infrastructure/EnergyCo.Loyalty.Infrastructure.csproj src/EnergyCo.Loyalty.Infrastructure/

RUN dotnet restore EnergyCo.Loyalty/EnergyCo.Loyalty.Api.csproj

COPY . .

RUN dotnet publish EnergyCo.Loyalty/EnergyCo.Loyalty.Api.csproj \
    -c Release \
    -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EnergyCo.Loyalty.Api.dll"]
