FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
COPY . /app
EXPOSE 80
ENTRYPOINT ["dotnet", "test02.dll"]