FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ["StockChat.Identity/StockChat.Identity.csproj", "StockChat.Identity/"]
RUN dotnet restore "StockChat.Identity/StockChat.Identity.csproj"
COPY . .
WORKDIR "StockChat.Identity"
RUN dotnet build "StockChat.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockChat.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockChat.Identity.dll"]
