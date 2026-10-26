# La imagen se arma en dos etapas. La primera trae el SDK entero para compilar; la segunda,
# solo lo necesario para correr. Lo que queda en la imagen final es la segunda: el SDK pesa
# más de un giga y para correr la aplicación no hace falta.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS construir
WORKDIR /obra

# El .csproj primero y solo el .csproj: bajar los paquetes es el paso lento, y así esa capa
# se rehace únicamente cuando cambian las dependencias. Tocar una vista no vuelve a bajarlos.
COPY SCA-MVC.csproj ./
RUN dotnet restore SCA-MVC.csproj

COPY . ./
RUN dotnet publish SCA-MVC.csproj -c Release -o /publicado --no-restore


# aspnet y no runtime, que no trae ASP.NET Core.
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# QuestPDF dibuja los reportes con Skia, que en Linux necesita fontconfig para buscar las
# tipografías. La imagen base no lo trae, y sin él los PDF fallan recién al generarse.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=construir /publicado ./

# Sin root: si alguien vulnerara la aplicación, no tendría control del contenedor
USER app

# Desde .NET 8 las imágenes escuchan en el 8080 y no en el 80
EXPOSE 8080
ENTRYPOINT ["dotnet", "SCA-MVC.dll"]
