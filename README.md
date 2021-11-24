# WellBot

This is a Telegram bot for various chat interactions (intended only for fun).

## Database migrations

To update the databae schema / add a new migration, run from command line:

```
dotnet ef migrations add MigrationName -s WellBot.Web -p WellBot.Infrastructure.DataAccess
```

or run from package manager console:

```
Add-Migration MigrationName -p WellBot.Infrastructure.DataAccess
```

## Tests

To run the application tests, execute the following command:
`dotnet test src`

Or if you are in `src` folder, just run `dotnet test`.

## Commands

This project contains some helper commands that can be used to initialize / interact with basic project data.

General syntax for executing a command (from command line): `WellBot.Web.exe command-name parameters`

### Create User

Creates a new user in the system.

Arguments:
- `--email` - user's email
- `--password` user's password

Example usage:

`WellBot.Web.exe create-user --email=jdoe@example.com --password=Exampl3`