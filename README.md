# mastodon-bot
Mastodon Bot Template

## Prerequisites

- .net SDK v7+
- A Mastodon account created for your bot (make sure you tick off that it's a bot on the profile)

## Getting Started

- Do a .net restore
`dotnet restore`

- If debugging locally, copy `appsettings.json` and rename the resulting file as `local.appsettings.json`

- In `appsettings.json`/`local.appsettings.json`, fill in the following details:
  - `appName`: A unique name for your bot on the instance
  - `instance`: The Mastodon instance on which your bot lives
  - `email`: The email account associated with your bot
  - `password`: The password for your bot

## Extending

The included functionality is currently intentionally basic. The bot will respond with one of five random phrases if tagged and follow the user. The hooks are there to extend this functionality however you see fit.
