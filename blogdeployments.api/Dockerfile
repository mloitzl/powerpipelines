FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/publish blogdeployments.api/blogdeployments.api.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/publish /app
ENTRYPOINT [ "dotnet", "/app/blogdeployments.api.dll" ]