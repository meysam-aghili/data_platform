FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./app/src/WebApi.csproj .
RUN dotnet restore "./WebApi.csproj"
COPY ./app/src ./
WORKDIR /src/src
RUN dotnet build "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
COPY --from=publish /app/publish .
USER root
# COPY ./config/certs/ /usr/local/share/ca-certificates/
# RUN update-ca-certificates
RUN groupadd -g 1001 docker && \
    useradd -u 1656 -g docker apiuser
USER apiuser
CMD ["dotnet", "WebApi.dll"]