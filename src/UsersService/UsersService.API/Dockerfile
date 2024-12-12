FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/UsersService/UsersService.API/UsersService.API.csproj", "src/UsersService/UsersService.API/"]
COPY ["src/UsersService/UsersService.Application/UsersService.Application.csproj", "src/UsersService/UsersService.Application/"]
COPY ["src/UsersService/UsersService.Domain/UsersService.Domain.csproj", "src/UsersService/UsersService.Domain/"]
COPY ["src/UsersService/UsersService.Infrastructure/UsersService.Infrastructure.csproj", "src/UsersService/UsersService.Infrastructure/"]
COPY ["tests/UsersService.Tests/UsersService.Tests.csproj", "tests/UsersService.Tests/"]
COPY ["shared/Jobly.Protobufs/Jobly.Protobufs.csproj", "shared/Jobly.Protobufs/"]
RUN dotnet restore "./src/UsersService/UsersService.API/UsersService.API.csproj"
COPY . .
WORKDIR "/src/UsersService/UsersService.API"
RUN dotnet build "./UsersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UsersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.API.dll"]