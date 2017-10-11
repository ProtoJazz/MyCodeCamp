FROM microsoft/dotnet:latest
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "run", "--project", "/app/MyCodeCamp/bin/Debug/netcoreapp1.0/MyCodeCamp.dll"]
