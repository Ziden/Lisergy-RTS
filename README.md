# Codename Lisergy

[![LisergyPipeline](https://github.com/Ziden/Lisergy/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Ziden/Lisergy/actions/workflows/dotnet.yml)

The main idea of the game is a easily moddable mmo strategy game that runs on a persistent world, where it really feels more of a strategy game rather then a puzzle game, where positioning, predicting and reacting to situations are the key features of the gameplay.

With turn based battles simillar to classical RPG's together with some twists where player can also setup the IA of their units via basic programming commands simillar to the Gambit system from FF12, combat should have its own little mix of games like civilization, age of empires and final fantasy.


Planned Features:
- Player built structures
- Player recruited units
- Battles
- IA Battles
- Player Customizable IA for battles
- Missions
- Resource gathering
- Open Trading
- Crafting 
- Dungeons (Sequential Battles)

## To run the server:

Requirements: .net core 2.1, docker

#### Running the Server

Currently the MapServer and BattleServer are bundled into a standalone server for convenience.

```
cd LisergyServer/
dotnet build
cd ../MapServer/
dotnet build
dotnet run 
```

Or just open the solution in visual studio and click "RUN" on the MapServer solution.

## To run the client:

Requirements: Unity

Just open the project, select the single scene that exists and run it.

## Basic GDD

https://docs.google.com/document/d/1FoiBIhvpWwPVy5s_gLwaugZjPG2pnMK_K3bV8i--oiw/edit?usp=sharing
