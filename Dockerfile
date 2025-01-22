# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establece el directorio de trabajo
WORKDIR /app

# Copia los archivos del proyecto y restaura las dependencias
COPY ["currencyApi/currencyApi.csproj", "currencyApi/"]
RUN dotnet restore "currencyApi/currencyApi.csproj"

# Copia todo el código fuente y publica la aplicación
COPY . .
RUN dotnet publish "currencyApi/currencyApi.csproj" -c Release -o /app/publish

# Etapa 2: Ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Establece el directorio de trabajo
WORKDIR /app

# Copia los archivos compilados desde la etapa de construcción
COPY --from=build /app/publish .

# Expone el puerto 80 para la API
EXPOSE 80

# Configura el comando para iniciar la API
ENTRYPOINT ["dotnet", "currencyApi.dll"]
