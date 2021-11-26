#!/bin/bash

while getopts u:h:s: flag
do
    case "${flag}" in
        u) username=${OPTARG};;
        h) host=${OPTARG};;
        s) service=${OPTARG};;
    esac
done

targetarch=linux-arm

source=$service
target=${source/.//}

rm -rf ./publish/
dotnet publish $source -r $targetarch  -c Release -o publish/$source --self-contained
/opt/homebrew/bin/rsync -avh ./publish/$source $username@$host:~/
ssh $username@$host sudo systemctl restart $source.service
