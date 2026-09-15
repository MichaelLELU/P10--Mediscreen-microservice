FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["AuthenticationService.Api/AuthenticationService.Api.csproj", "AuthenticationService.Api/"]
RUN dotnet restore "AuthenticationService.Api/AuthenticationService.Api.csproj"

COPY . .
WORKDIR "/src/AuthenticationService.Api"

RUN dotnet publish \
    "AuthenticationService.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuthenticationService.Api.dll"]