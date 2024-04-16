# Mythic Empire - Backend server

## Description
Mythic Empire - Backend server is a backend project built with ASP.Net Core to create an API system interacting with data in a MongoDB database. This project provides endpoints to manage and retrieve data related to [Mythic Empire](https://github.com/viethuynh713/TowerDefense-Unity).

## Requirements
- [Dotnet version 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [ASP.Net Core platform](https://dotnet.microsoft.com/en-us/apps/aspnet)
- [MongoDB version 6](https://www.mongodb.com/docs/v6.0/introduction/)

## Service API Documentation

### AuthenControl

- GET /api/AuthenControl: Returns an array of User Models.

- POST /api/AuthenControl/register: Registers a new user.

- GET /api/AuthenControl/login: Logs in a user.

- GET /api/AuthenControl/login-id: Logs in a user by user ID.

- PUT /api/AuthenControl/reset-password: Resets the password of a user.

- GET /api/AuthenControl/send-otp: Sends an OTP to a user's email.

- POST /api/AuthenControl/valid-otp: Validates an OTP entered by the user.

### CardControl

- GET /api/CardControl: Returns an array of Card Models.

- POST /api/CardControl/add-card-to-database: Adds a new card to the database.

- PUT /api/CardControl/upgrade-card/{userId}: Upgrades a card for a user.

- POST /api/CardControl/buy-gacha/{userId}: Buys a gacha pack for a user.

- POST /api/CardControl/buy-card/{userId}: Buys a specific card for a user.

### UserControl

- POST /api/UserControl/update-nickname/{userId}: Updates the nickname of a user.

- POST /api/UserControl/create-gamesession: Creates a new game session.

- GET /api/UserControl/get-gamesession: Gets the game session for a user.

## Installation and Running
1. Clone this repository.
2. Set up a MongoDB database.
3. Open the solution in Visual Studio.
4. Configure the connection to the MongoDB database in the `appsettings.json` file with the following items:
```json
"DatabaseConfig": {
  "ConnectionString": "",
  "DatabaseName": "",
  "UserModelCollectionName": "",
  "GameSessionModelCollectionName": "",
  "CardModelCollectionName": ""
}
```
- ConnectionString: connection string to connect to the configured MongoDB.
- DatabaseName: Name of the database.
- UserModelCollectionName: Collection name to store player information.
- GameSessionModelCollectionName: Collection name to store completed game sessions.
- CardModelCollectionName: Collection name to store cards in the game.
5. Build and run the project.

## Contributions
We welcome contributions from the community. Please create a Pull Request to contribute to this project.

## Author
Contact: viethuynh713@gmail.com
