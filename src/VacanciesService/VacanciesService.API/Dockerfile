FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VacanciesService.API/VacanciesService.API.csproj", "VacanciesService.API/"]
COPY ["VacanciesService.Application/VacanciesService.Application.csproj", "VacanciesService.Application/"]
COPY ["VacanciesService.Domain/VacanciesService.Domain.csproj", "VacanciesService.Domain/"]
COPY ["VacanciesService.Infrastructure/VacanciesService.Infrastructure.csproj", "VacanciesService.Infrastructure/"]
COPY ["VacanciesService.Presentation/VacanciesService.Presentation.csproj", "VacanciesService.Presentation/"]
RUN dotnet restore "./VacanciesService.API/VacanciesService.API.csproj"
COPY . .
WORKDIR "/src/VacanciesService.API"
RUN dotnet build "./VacanciesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VacanciesService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VacanciesService.API.dll"]