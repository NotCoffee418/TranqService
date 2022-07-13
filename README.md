# TranqService
Periodically save youtube videos to a local directory when saved to a playlist.


## Installation
1. Move latest release build to MEGA/Services/TranqService/deploy
2. Set up the config file at MEGA/Services/TranqService/data/appsettings.json
3. Point sqlite path to MEGA/Services/TranqService/data/TranqService.sqlite
4. Powershell: `New-Service -Name "TranqServiceTest1" -BinaryPathName '"MEGAFOLDER\Services\deploy\TranqService.exe" --configPath "MEGAFOLDER\Services\data\appsettings.json"'`
Test the Mind the quotes, spaced paths should have them.

# Update
1. Stop the service
2. Throw new files at MEGA/Services/TranqService/deploy

## Removal
You may need to [update powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows)
Powershell: `Remove-Service -Name "TranqService"`
