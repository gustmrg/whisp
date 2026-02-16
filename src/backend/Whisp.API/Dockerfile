FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dev

WORKDIR /src

COPY Whisp.sln ./
COPY Whisp.API/Whisp.API.csproj Whisp.API/
RUN dotnet restore Whisp.API/Whisp.API.csproj

COPY . .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

CMD ["dotnet", "watch", "--non-interactive", "--project", "Whisp.API/Whisp.API.csproj", "run", "--urls", "http://0.0.0.0:8080"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY Whisp.sln ./
COPY Whisp.API/Whisp.API.csproj Whisp.API/
RUN dotnet restore Whisp.API/Whisp.API.csproj

COPY . .

RUN dotnet build Whisp.API/Whisp.API.csproj -c Release --no-restore

FROM build AS publish

RUN dotnet publish Whisp.API/Whisp.API.csproj -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=publish /app/publish ./

ENTRYPOINT ["dotnet", "Whisp.API.dll"]
