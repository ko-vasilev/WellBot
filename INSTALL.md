## Required software

- Visual Studio 2019 (https://www.visualstudio.com/downloads/download-visual-studio-vs.aspx) or JetBrains Rider
- .NET SDK 5 (https://dotnet.microsoft.com/download/dotnet/5.0)
- git

## Project initialization

1. Copy `src\WellBot.Web\appsettings.json.template` file to `src\WellBot.Web\appsettings.Development.json`

2. Update the `ConnectionStrings:AppDatabase` setting in that file to target your local development server/database

The database will be created automatically upon application start (if it does not exist yet).
