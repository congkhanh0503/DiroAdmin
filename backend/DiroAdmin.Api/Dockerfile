FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["DiroAdmin.Api.csproj", "./"]
RUN dotnet restore "DiroAdmin.Api.csproj"

COPY . .
RUN dotnet publish "DiroAdmin.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5020
ENV DB_PATH=/app/data/diroadmin.db

EXPOSE 5020

ENTRYPOINT ["dotnet", "DiroAdmin.Api.dll"]
