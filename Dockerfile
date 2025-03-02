FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7021

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChessCore/ChessCore.csproj", "ChessCore/"]

RUN dotnet restore "ChessCore/ChessCore.csproj"
COPY . .
WORKDIR "/src/ChessCore"
RUN dotnet build "ChessCore.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChessCore.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copie des fichiers publiés
COPY --from=publish /app/publish .

# Modification des permissions pour le répertoire ToDownloadedFiles
USER root
RUN mkdir -p /app/wwwroot/UploadedFiles && chmod -R 777 /app/wwwroot/UploadedFiles
RUN mkdir -p /app/wwwroot/ToDownloadedFiles && chmod -R 777 /app/wwwroot/ToDownloadedFiles

# Revenir à l'utilisateur non-root
USER $APP_UID
ENTRYPOINT ["dotnet", "ChessCore.dll"]
