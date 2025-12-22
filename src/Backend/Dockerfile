FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /Backend
COPY OrderProcessing.Api/OrderProcessing.Api.csproj OrderProcessing.Api/
COPY OrderProcessing.Application/OrderProcessing.Application.csproj OrderProcessing.Application/
COPY OrderProcessing.Domain/OrderProcessing.Domain.csproj OrderProcessing.Domain/
COPY OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj OrderProcessing.Infrastructure/
RUN dotnet restore OrderProcessing.Api/OrderProcessing.Api.csproj
COPY . .
WORKDIR /src/OrderProcessing.Api
RUN dotnet publish -c Release -o /app/publish
