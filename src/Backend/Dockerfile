FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /Backend
COPY OrderProcessing.Api/OrderProcessing.Api.csproj OrderProcessing.Api/
COPY OrderProcessing.Application/OrderProcessing.Application.csproj OrderProcessing.Application/
COPY OrderProcessing.Domain/OrderProcessing.Domain.csproj OrderProcessing.Domain/
COPY OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj OrderProcessing.Infrastructure/
RUN dotnet restore OrderProcessing.Api/OrderProcessing.Api.csproj
COPY . .

WORKDIR /Backend/OrderProcessing.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderProcessing.Api.dll"]
