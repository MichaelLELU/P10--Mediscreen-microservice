FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["NoteService.Api/NoteService.Api.csproj", "NoteService.Api/"]
RUN dotnet restore "NoteService.Api/NoteService.Api.csproj"

COPY . .
WORKDIR "/src/NoteService.Api"

RUN dotnet publish \
    "NoteService.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NoteService.Api.dll"]