# Codename Lisergy - A Civilization style MMO with turn based auto-battles and configurable battle IA's.

[![LisergyPipeline](https://github.com/Ziden/Lisergy/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Ziden/Lisergy/actions/workflows/dotnet.yml)

## To run the server:

Requirements: .net core 2.1, docker

## Running Server in Standalone Mode

Running Server:
```
cd StandaloneServer/
dotnet build
dotnet run
```
Or start the Standalone Server project.

## Running Server in Clustered Mode

#### Running RabbitMQ

```
docker run -d --hostname my-rabbit --name ecomm-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```

#### Running the Server
```
cd LisergyServer/
dotnet build
cd MapServer/
dotnet run 
cd ../BattleServer
dotnet run
```

Or just open the solution in visual studio and click "RUN"

## To run the client:

Requirements: Unity

Just open the project, select the single scene that exists and run it.

## Basic GDD

https://docs.google.com/document/d/1FoiBIhvpWwPVy5s_gLwaugZjPG2pnMK_K3bV8i--oiw/edit?usp=sharing
