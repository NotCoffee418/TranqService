# TranqService
Background service to watch YouTube playlists and download any videos added to them.


## Installation Prerequisites
1. Install [.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.8-windows-x64-installer)
2. Install [Powershell 7+](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows)
3. Create a discord server for bot logs with a webhook for the channel.
4. Get a [YouTube Data API v3](https://console.cloud.google.com/apis/library/youtube.googleapis.com) key.

## Installation
### Windows
1. Move latest release build to MEGA/Services/TranqService/deploy
2. Set up the config file at MEGA/Services/TranqService/data/appsettings.json
3. Point sqlite path to MEGA/Services/TranqService/data/TranqService.sqlite
4. Configure all the settings you want.
    - Paste the YouTube Data API v3 key into the config file.
    - Extract `DiscordWebhookId` and `DiscordWebhookSecret` from the webhook URL.
    - Point `SqliteFilePath` to a file (that does not yet exist) in the same directory as the config file. Use the filename TranqService.sqlite. **(Use double backslash!)**
    - Under VideoPlaylists and MusicPlaylists, add the playlist guids and the local directory in which to save the files. **(Use double backslash!)**
    The playlist ID can be found on youtube by opening a playlist and looking at the URL:
    https://www.youtube.com/watch?v=VideoIdHere&list=PlaylistIdHere
5. Powershell: `New-Service -Name "TranqService" -BinaryPathName '"MEGAFOLDER\Services\deploy\TranqService.exe" --configPath "MEGAFOLDER\Services\data\appsettings.json"'`

### Linux
1. Ensure you have .NET SDK installed (https://docs.microsoft.com/en-us/dotnet/core/install/linux)
2. Clone the repo and cd into it
3. `chmod +x install-linux.sh`
4. `sudo ./install-linux.sh -u linuxRunnerUser -c "/path/to/appsettings.json" -d "/deploy/directory/here"`

# Update
1. Stop the service
2. Place new relase in MEGA/Services/TranqService/deploy
3. Restart service or pc

## Removal
You may need to [update powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows)
Powershell: `Remove-Service -Name "TranqService"`
