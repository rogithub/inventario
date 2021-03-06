FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build     
WORKDIR /app

# copy csproj and restore as distinct layers
COPY Ro.Inventario.sln .
COPY nuget.config .
COPY Ro.Inventario.Web/*.csproj ./Ro.Inventario.Web/
COPY Ro.SQLite.Data/*.csproj ./Ro.SQLite.Data/
RUN dotnet restore
RUN dotnet dev-certs https -ep /app/.aspnet/https/inventario_gordopechocho.pfx -p gordopechocho
RUN dotnet dev-certs https --trust

# copy everything else and build app
COPY . ./
WORKDIR /app/Ro.Inventario.Web
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY dep-raspi/* ./
# ===============Not Required=====================
# no se required si usas podman_deploy.sh script 
## COPY Ro.Inventario.Web/Db/inventario.db ./
# ===============Not Required=====================

# Change timezone to local time
ENV TZ=America/Mexico_City
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Prepare everything for sqlite
RUN apt-get update && apt-get -y upgrade \
    && apt-get install -y --allow-unauthenticated \
    sqlite3 

COPY --from=build /app/Ro.Inventario.Web/out ./
COPY --from=build /app/.aspnet/https /root/.aspnet/DataProtection-Keys/
ENTRYPOINT ["dotnet", "Ro.Inventario.Web.dll", "--urls", "https://0.0.0.0:5002;http://0.0.0.0:5003"]