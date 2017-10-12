FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY MyCodeCamp/*.csproj ./
COPY MyCodeCamp.Data/*.csproj ./
RUN dotnet restore MyCodeCamp.csproj
RUN dotnet restore MyCodeCamp.Data.csproj
# Copy everything else and build
COPY . ./
RUN dotnet publish MyCodeCamp.csproj -c Release -o out
RUN dotnet publish MyCodeCamp.Data.csproj -c Release -o out
# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MyCodeCamp.dll"]