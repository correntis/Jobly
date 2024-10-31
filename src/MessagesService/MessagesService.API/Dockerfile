FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MessagesService.API/MessagesService.API.csproj", "MessagesService.API/"]
COPY ["MessagesService.Application/MessagesService.Application.csproj", "MessagesService.Application/"]
COPY ["MessagesService.DataAccess/MessagesService.DataAccess.csproj", "MessagesService.DataAccess/"]
COPY ["MessagesService.Core/MessagesService.Core.csproj", "MessagesService.Core/"]
RUN dotnet restore "./MessagesService.API/MessagesService.API.csproj"
COPY . .
WORKDIR "/src/MessagesService.API"
RUN dotnet build "./MessagesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MessagesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessagesService.API.dll"]