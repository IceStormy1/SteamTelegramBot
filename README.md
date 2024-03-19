
# SteamTelegramBot

## Description
SteamTelegramBot is a Telegram bot that tracks prices of applications on Steam and notifies users when the price drops.

## Demonstration
![Alt text](demo.gif)

## Setup
Make sure you have .NET 8 or newer installed  
There are two projects in the solution: **SteamTelegramBot** and **SteamTelegramBot.Jobs**
1. SteamTelegramBot - provides webhook endpoint for the Telegram Bot;
2. SteamTelegramBot.Jobs - cron jobs

### SteamTelegramBot
1. Change the connection string in appsettings.json
```json
 "ConnectionStrings": {
   "SteamConnectionString": "User ID=postgres;Password=123;Server=localhost;Port=5432;Database=SteamTelegramBot;Include Error Detail=True"
 }
```
2. Change BotConfiguration
- You have to specify your Bot token in appsettings.json. Replace **{BotToken}** in **appsettings.json** with actual Bot token (You can get it from [@BotFather](https://t.me/BotFather)). 
- Also you have to specify endpoint, to which Telegram will send new updates with `HostAddress` parameter
- Set OwnerUsername - your username in the telegram
```json
"BotConfiguration": {
  "BotToken": "*",
  "HostAddress": "https://mydomain.com",
  "Route": "/bot",
  "SecretToken": "*",
  "IsActive": true,
  "OwnerUsername": "*"
}
```
### SteamTelegramBot.Jobs
The same setting as for SteamTelegramBot, except for `Quartz`. In **appsettings.json** change cron if it necessary (by default every 6 hours)
``` json
"Quartz": {
  "SteamTelegramBot.Jobs.Jobs.CheckingSteamAppsJob": "0 0 0/6 1/1 * ? *"
}
```

## Usage
Most interactions happen through Telegram buttons (InlineKeyboardMarkup), except for adding a game to the watchlist, which is done using the command "/addgame GameName".

## Functionality
* Tracking prices of applications on Steam.
* User receives a notification if the price of tracked application drop.

## Limitations
### Steam API limitations.
Based on the testing of the steam API (the suggests request is used, because there is the least restriction on requests), it allows you to make no more than 60 requests in 5 seconds
### Telegram API limitations 
See [Telegram documentation](https://core.telegram.org/bots/faq#my-bot-is-hitting-limits-how-do-i-avoid-this)
