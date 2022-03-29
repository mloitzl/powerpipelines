FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY . .
RUN find . -type f
RUN dotnet restore
RUN dotnet publish -o /app/publish blogdeployments.agent/blogdeployments.agent.csproj

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/publish /app
ENTRYPOINT [ "dotnet", "/app/blogdeployments.agent.dll" ]