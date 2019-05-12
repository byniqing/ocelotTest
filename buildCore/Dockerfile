From microsoft/dotnet:sdk AS build 
WORKDIR /project
COPY *.csproj /project 
RUN dotnet restore 

COPY . /project 
RUN dotnet publish -c Release -o out 


FROM microsoft/dotnet:runtime
WORKDIR /app
COPY --from=build /project/out /app 
ENTRYPOINT ["dotnet","buildCore.dll"] # 因为是发布后的文件，所有就不是run了。而是具体的dll程序集