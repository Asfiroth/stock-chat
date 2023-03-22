FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ["StockChat.Web/StockChat.Web.csproj", "StockChat.Web/"]
RUN dotnet restore "StockChat.Web/StockChat.Web.csproj"
COPY . .
WORKDIR "StockChat.Web"
RUN dotnet build "StockChat.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockChat.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockChat.Web.dll"]
