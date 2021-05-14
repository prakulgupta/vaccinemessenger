# Vaccine Messenger

This is a prototype project of how to get Vaccine Notification in Telegram when any slot is available in a particular district of a city.

## Description
This is a c# console application. It uses multitasking concept to trigger APIs parallelly and at different time intervals as per need. It uses Co-WIN public APIs to get information about vaccine availability. And sends the notification to user using Telegram Bot.

## Steps
1. Clone this repository.
2. Create a Telegram Bot - Following the instrunctions from [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-telegram?view=azure-bot-service-4.0).
3. Copy the Access Token and use it inside the Program.cs file.
4. Make necessary changes in the Program.cs file regarding adding requires users ChatId. 
5. How to get users chatId. Follow the instrunctions from [here](https://stackoverflow.com/a/32572159).
6. For ChatId either you can use Postman/Fiddler or can also use the GetTelegramBotQueries method written inside Program.cs file. 
7. Build and Run the console app.

## Technology/Tools
1. Visual Studio 2019
2. C# Console App
3. Postman
4. [Co-WIN Public APIs](https://apisetu.gov.in/public/marketplace/api/cowin) 

## Futurescope 
Automatically reading queries from bot and mapping with the logic to overcome manual part of adding user chatIds for a particular district. 
