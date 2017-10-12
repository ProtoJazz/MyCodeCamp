FROM microsoft/aspnetcore-build:2.0 AS build-env

COPY . /app
WORKDIR /app/MyCodeCamp
RUN ["dotnet", "restore"]

EXPOSE 5004
ENTRYPOINT ["dotnet", "run"]