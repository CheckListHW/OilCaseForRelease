FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OilCaseApi.csproj", "."]
RUN dotnet restore "./OilCaseApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "OilCaseApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OilCaseApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Files ./Files

ENV ASPNETCORE_ENVIRONMENT Staging

ENTRYPOINT ["dotnet", "OilCaseApi.dll", "--environment=Staging"]