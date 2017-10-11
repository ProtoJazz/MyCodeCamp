# Sample contents of Dockerfile
 # Stage 1
 FROM microsoft/aspnetcore-build AS builder
 WORKDIR /app


 RUN dotnet restore

 # copies the rest of your code
 COPY . .
 RUN dotnet publish --output /app/ --configuration Release

 # Stage 2
 FROM microsoft/aspnetcore
 WORKDIR /app
 COPY --from=builder /app .
 ENTRYPOINT ["dotnet", "MyCodeCamp.dll"]
