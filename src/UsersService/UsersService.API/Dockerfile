FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UsersService.API/UsersService.API.csproj", "UsersService.API/"]
COPY ["UsersService.Application/UsersService.Application.csproj", "UsersService.Application/"]
COPY ["UsersService.Domain/UsersService.Domain.csproj", "UsersService.Domain/"]
COPY ["UsersService.Infrastructure/UsersService.Infrastructure.csproj", "UsersService.Infrastructure/"]
RUN dotnet restore "./UsersService.API/UsersService.API.csproj"
COPY . .
WORKDIR "/src/UsersService.API"
RUN dotnet build "./UsersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UsersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.API.dll"]