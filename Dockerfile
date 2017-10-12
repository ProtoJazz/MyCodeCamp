FROM microsoft/dotnet:latest

COPY . /app
WORKDIR /app/MyCodeCamp
RUN ["dotnet", "restore"]
ENV ASPNETCORE_URLS http://*:5004
EXPOSE 5004
ENTRYPOINT ["dotnet", "run"]