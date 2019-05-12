FROM microsoft/aspnetcore-build as build-dev
WORKDIR /code 
COPY *.csproj /code 
RUN dotnet restore
COPY . /code
RUN dotnet publish -c Release -o out 

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-dev /code/out /app

EXPOSE 80
ENTRYPOINT ["dotnet","test01.dll"] #入口，是容器运行的时候的入口，如果是run，是镜像打包执行运行

#microsoft/dotnet:2.2.0-aspnetcore-runtime
