#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Parus.WebUI/Parus.WebUI.csproj", "Parus.WebUI/"]
COPY ["Parus.Core/Parus.Core.csproj", "Parus.Core/"]
COPY ["Parus.Common/Parus.Common.csproj", "Parus.Common/"]
COPY ["Parus.Infrastructure/Parus.Infrastructure.csproj", "Parus.Infrastructure/"]
RUN dotnet restore "./Parus.WebUI/./Parus.WebUI.csproj"
COPY . .
WORKDIR "/src/Parus.WebUI"
RUN dotnet build "./Parus.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Parus.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parus.WebUI.dll"]