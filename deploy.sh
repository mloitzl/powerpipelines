#!/bin/bash

while getopts u:h:f:t: flag
do
    case "${flag}" in
        u) username=${OPTARG};;
        h) host=${OPTARG};;
        f) source=${OPTARG};;
        t) target=${OPTARG};;
    esac
done

targetarch=linux-arm

rm -rf ./publish/
dotnet publish $source -r $targetarch  -c Release -o publish/$source
/opt/homebrew/bin/rsync -avh ./publish/$source $username@$host:$target
ssh $username@$host sudo systemctl restart blogdeployment.api.service
