FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/MessagesService/MessagesService.API/MessagesService.API.csproj", "src/MessagesService/MessagesService.API/"]
COPY ["src/MessagesService/MessagesService.Application/MessagesService.Application.csproj", "src/MessagesService/MessagesService.Application/"]
COPY ["src/MessagesService/MessagesService.Core/MessagesService.Core.csproj", "src/MessagesService/MessagesService.Core/"]
COPY ["src/MessagesService/MessagesService.DataAccess/MessagesService.DataAccess.csproj", "src/MessagesService/MessagesService.DataAccess/"]
COPY ["src/MessagesService/MessagesService.Presentation/MessagesService.Presentation.csproj", "src/MessagesService/MessagesService.Presentation/"]
COPY ["tests/MessagesService.Tests/MessagesService.Tests.csproj", "tests/MessagesService.Tests/"]
COPY ["shared/Jobly.Protobufs/Jobly.Protobufs.csproj", "shared/Jobly.Protobufs/"]
COPY ["shared/Jobly.Brokers/Jobly.Brokers.csproj", "shared/Jobly.Brokers/"]


RUN dotnet restore "./src/MessagesService/MessagesService.API/MessagesService.API.csproj"
COPY . .
COPY ./src /src
COPY ./shared /shared

WORKDIR /src/MessagesService/MessagesService.API
RUN dotnet build "MessagesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MessagesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish /app
ENTRYPOINT ["dotnet", "MessagesService.API.dll"]