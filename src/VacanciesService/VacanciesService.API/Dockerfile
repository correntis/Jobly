FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/VacanciesService/VacanciesService.API/VacanciesService.API.csproj", "src/VacanciesService/VacanciesService.API/"]
COPY ["src/VacanciesService/VacanciesService.Application/VacanciesService.Application.csproj", "src/VacanciesService/VacanciesService.Application/"]
COPY ["src/VacanciesService/VacanciesService.Domain/VacanciesService.Domain.csproj", "src/VacanciesService/VacanciesService.Domain/"]
COPY ["src/VacanciesService/VacanciesService.Infrastructure/VacanciesService.Infrastructure.csproj", "src/VacanciesService/VacanciesService.Infrastructure/"]
COPY ["src/VacanciesService/VacanciesService.Presentation/VacanciesService.Presentation.csproj", "src/VacanciesService/VacanciesService.Presentation/"]
COPY ["tests/VacanciesService.Tests/VacanciesService.Tests.csproj", "tests/VacanciesService.Tests/"]
COPY ["shared/Jobly.Protobufs/Jobly.Protobufs.csproj", "shared/Jobly.Protobufs/"]
COPY ["shared/Jobly.Brokers/Jobly.Brokers.csproj", "shared/Jobly.Brokers/"]

RUN dotnet restore "./src/VacanciesService/VacanciesService.API/VacanciesService.API.csproj"
COPY . .
COPY ./src /src
COPY ./shared /shared

WORKDIR /src/VacanciesService/VacanciesService.API
RUN dotnet build "VacanciesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VacanciesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish /app
ENTRYPOINT ["dotnet", "VacanciesService.API.dll"]
