# TranqService
Background service to watch YouTube playlists and download any videos added to them.


## Installation
1. Move latest release build to MEGA/Services/TranqService/deploy
2. Set up the config file at MEGA/Services/TranqService/data/appsettings.json
3. Point sqlite path to MEGA/Services/TranqService/data/TranqService.sqlite
4. Powershell: `New-Service -Name "TranqService" -BinaryPathName '"MEGAFOLDER\Services\deploy\TranqService.exe" --configPath "MEGAFOLDER\Services\data\appsettings.json"'`

# Update
1. Stop the service
2. Place new relase in MEGA/Services/TranqService/deploy

## Removal
You may need to [update powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows)
Powershell: `Remove-Service -Name "TranqService"`
