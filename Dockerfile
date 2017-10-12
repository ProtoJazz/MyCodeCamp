FROM microsoft/dotnet:sdk AS build-env
COPY . /app
WORKDIR /app/MyCodeCamp
RUN ["dotnet", "restore"]
ENV ASPNETCORE_URLS http://*:5004
EXPOSE 5004
ENTRYPOINT ["dotnet", "run"]