 

FROM microsoft/aspnetcore
WORKDIR /app
COPY . /app 

EXPOSE 80
ENTRYPOINT ["dotnet","User.Api.dll"] #入口，是容器运行的时候的入口，如果是run，是镜像打包执行运行
