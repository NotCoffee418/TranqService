#!/bin/sh

printf "-- Checking config path"
# Assign input parameters to variables
while getopts 'c:d': flag
do
    case "${flag}" in
        c) confPath=${OPTARG};;
        d) deployPath=${OPTARG};;
    esac
done

# Validate config path parameter
if [ -z "$confPath" ] || [ -z "$deployPath" ] 
then
    echo "$confPath"
    echo "missing -c or -d parameter"
    exit 1
fi

# Ensure the config file exists at that path
if ! test -f "$confPath"; 
then
    echo "Config file $confPath does not exist."
    exit 1
fi

# Create publish build at -d path
printf "-- Building TranqService"
dotnet publish -c Release -r linux-x64 -o "$deployPath"

## todo: create service etc, gtg