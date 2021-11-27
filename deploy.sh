#!/bin/bash

while getopts u:h:s:r:t: flag
do
    case "${flag}" in
        u) username=${OPTARG};;
        h) host=${OPTARG};;
        s) service=${OPTARG};;
        r) restart=${OPTARG};;
        t) targetarch=${OPTARG}
    esac
done

source=$service
target=${source/.//}

rm -rf ./publish/
dotnet publish $source -r $targetarch  -c Release -o publish/$source --self-contained
/opt/homebrew/bin/rsync -avh ./publish/$source $username@$host:~/

if [[ $restart == "true" ]]
then
  ssh $username@$host sudo systemctl restart $source.service
fi
