# sevdesk-exchange-sync
A tool for synchronizing sevDesk contacts and tasks to Exchange Online (Office 365) accounts. The project is written in dotnet Core using C#.

## Usage

This tool can syncronize contacts and tasks from a comany's sevDesk account to individual Exchange Online accounts of employees. The configuration includes methods to choose which contact categories to synchronize, which users in Exchange Online participate in the synchronization process and more.

The console application has two modes: 
* In the *interactive mode*, a wizard guides you through the steps to create a configuration file containing the credentials for communicating with sevDesk and Exchange.
* In the *synchronization mode*, the console application uses a configuration file to actually synchronizes the data.

## Installation

Please make sure that you installed the dotnet Core SDK on your machine. To build the project, navigate into the `SevDeskExchangeSync.Console` folder of the project and run:

```bash
dotnet restore
dotnet build
```

To run the console application in the interactive mode, run:

```bash
dotnet run 
dotnet build
```

## Deployment

Publish the project for you platform, e.g. use the RID `win10-x64` for Windows-based servers:

```bash
dotnet publish -c Release -r win10-x64 --self-contained
```

0. To go the publish folder (`bin/Release/netcoreapp1.1/win10-x64/publish`) and copy all contents to your server.
0. Afterwards, add the configuration file that you created in the interactive mode to this folder.
0. Add a cronjob to your server's task scheduler that executes the console app in a specific interval with the name of the configuration file as command line parameter.

## State of the Project

This project is in beta state, so please make sure that you test it in a development environment before publishing it to a project server using production credentials for sevDesk and Exchange Online. Feel free to submit bugs and make feature requests.