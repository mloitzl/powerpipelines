#!/bin/bash
rm -rf ./publish/
dotnet publish ./blogdeployments.api/blogdeployments.api.csproj -r linux-arm -c Release -o publish/api
/opt/homebrew/bin/rsync -avh ./publish/api/ pi@192.168.1.123:~/blogdeployment/api
ssh pi@192.168.1.123 sudo systemctl restart blogdeployment.api.service
# scp -r ./publish/api/* pi@192.168.1.123:~/blogdeployment/api
