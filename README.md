# Airport control tower. 
Revolutionizing airport operations with an intelligent control tower system that enables real-time, autonomous aircraft communication.


## Applications
This project contains the following applications

- AirportControlTower.API - serves as the Airports Control tower that aircrafts use to communicate it's positional data and intentions (e.g. intention to land).
- AirportControlTower.Dashboard- a Blazor Server Application serving as the Admin Dashboard app to display operations that happen in the control tower

## Setup
To make it easier to start project, a docker compose configuration is in the root folder of the project solution

**Requirements** 
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) <sup>optional</sup>
- [Microsoft Sql Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) *
- [Visual Studio](https://visualstudio.microsoft.com/downloads/)/ [VSCode](https://code.visualstudio.com/download) *
- [.Net 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) *

<sup>*</sup> required when projects are started without Docker

> [!TIP]
> Easier to run applications with Docker/docker compose.

Project can be run by: 
1. **Docker**
   - Open a terminal in the root of the project solution then Run the command, `docker compose up -d`
   - API swagger can be found at http://localhost:58364/swagger/index.html
   - Blazor app can be located at http://localhost:58374
   - Other project settings can be found in the compose file

2. **Individual project**
   - Navigate to the root folder of API and/or Blazor app then run the command `dotnet run -v q`
   - API swagger can be located at http://localhost:5181/swagger/index.html
   - Blazor app can be located at http://localhost:5257
   
## Usage
> [!IMPORTANT]
> Use this Admin account credentials to login to the Admin Dashboard
> - Email: `admin@controltower.com`, password: `12345`