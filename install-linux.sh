#!/bin/sh

echo "-- Checking config path"
# Assign input parameters to variables
while getopts 'c:d:u': flag
do
    case "${flag}" in
        c) confPath=${OPTARG};;
        d) deployPath=${OPTARG};;
        u) unixUser=${OPTARG};;
    esac
done

# Validate config path parameter
if [ -z "$confPath" ] || [ -z "$deployPath" ] || [ -z "$unixUser" ] 
then
    echo "$confPath"
    echo "Missing parameter. -c path to appsettings.json -d path to deploy directory -u unix user"
    exit 1
fi

# Ensure the config file exists at that path
if ! test -f "$confPath"; 
then
    echo "Config file $confPath does not exist."
    exit 1
fi

# Create publish build at -d path
echo "-- Building TranqService"
dotnet publish -c Release -r linux-x64 -o "$deployPath"
chown -R $unixUser:$unixUser "$deployPath"

#
cat << EOF > /etc/systemd/system/tranq.service
[Unit]
Description=Test passing multiple arguments

[Service]
WorkingDirectory=$deployPath
User=$unixUser
ExecStart=/bin/bash -c "\"$deployPath/TranqService\" --configPath \"$confPath\""

[Install]
WantedBy=multi-user.target
EOF

# Enable and start service
sudo systemctl start tranq.service
sudo systemctl enable tranq.service

# Notify complete
echo "-- TranqService installed"