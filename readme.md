## Introduction

alarm is a  command line application to play an audio file after a specified delay.

## Download

Downloads are available as [Github Releases](https://github.com/btigi/alarm/releases/latest)

## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/alarm

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```

## Usage

swavewall is a command line application and should be run from a terminal session. Application usage is

```
Usage: alarm -t <time> [-f <audiofile>] [-m <message>]

  -t: Time offset (e.g., 5m for 5 minutes, 30s for 30 seconds)

  -f: Audio file to play (optional, uses default from appsettings.json if not specified)

  -m: Message to display (optional)
  ```


Usage examples:

 ```alarm -t 60 -f alarm.mp3```

 ```alarm -t 5m -f alarm.mp3 -m do the thing```

## Licencing

alarm is licenced under the MIT license. Full license details are available in license.md