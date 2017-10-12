FROM microsoft/dotnet:1.0.0-core
RUN sudo apt-get install dotnet-dev-1.0.4
COPY . /app
WORKDIR /app/MyCodeCamp
RUN ["/usr/bin/dotnet", "restore"]
ENV ASPNETCORE_URLS http://*:5004
EXPOSE 5004
ENTRYPOINT ["/usr/bin/dotnet", "run"]