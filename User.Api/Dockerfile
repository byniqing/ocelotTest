FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
#WORKDIR /app
RUN dotnet build  -c Release -o out

FROM build AS publish
RUN dotnet publish  -c Release -o /app

#FROM base AS final
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=publish /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "User.Api.dll"]
