FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /
ENV ASPNETCORE_URLS http://*:5000
# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "MyCodeCamp.dll"]
