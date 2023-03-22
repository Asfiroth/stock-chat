FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ["StockChat.Api/StockChat.Api.csproj", "StockChat.Api/"]
RUN dotnet restore "StockChat.Api/StockChat.Api.csproj"
COPY . .
WORKDIR "StockChat.Api"
RUN dotnet build "StockChat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockChat.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockChat.Api.dll"]
