FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PatientService.Api/PatientService.Api.csproj", "PatientService.Api/"]
RUN dotnet restore "PatientService.Api/PatientService.Api.csproj"

COPY . .
WORKDIR "/src/PatientService.Api"

RUN dotnet publish \
    "PatientService.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PatientService.Api.dll"]