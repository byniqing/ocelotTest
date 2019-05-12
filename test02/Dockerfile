FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY *.csproj .
#COPY . .
RUN dotnet restore
COPY . .
RUN dotnet build  -c Release -o /out

FROM build AS publish
#WORKDIR /pub
RUN dotnet publish  -c Release -o /pub

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=publish /pub .
EXPOSE 80
ENTRYPOINT ["dotnet", "test02.dll"]
