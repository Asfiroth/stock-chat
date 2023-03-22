FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ["StockChat.Stock/StockChat.Stock.csproj", "StockChat.Stock/"]
RUN dotnet restore "StockChat.Stock/StockChat.Stock.csproj"
COPY . .
WORKDIR "StockChat.Stock"
RUN dotnet build "StockChat.Stock.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockChat.Stock.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockChat.Stock.dll"]