# Lisergy Game Engine

An innovative entity-component-system-ish (ECS) game engine built with C# 8.0 targeting .NET Standard 2.1, designed for an exploration-oriented MMO RTS.

## Architecture Overview

Lisergy implements a unique variation of the ECS pattern called ECSL (Entity-Component-System-Logic) with a clear separation of concerns:

- **Entities**: Game objects that act as containers for components and have a unique identity
- **Components**: Pure data structures defining entity properties (e.g., `BuildingComponent`, `CargoComponent`)
- **Systems**: Process entities with specific components and implement game rules
- **Logic Classes**: Stateless behavior implementations that operate on entity data

### Core Concepts

The engine organizes game elements through:

- **GameSystems**: Central registry managing all systems
- **EventBus**: Pub/sub mechanism for cross-domain communication
- **EntityLogic**: Access layer to all entity-specific logic
- **Entity Views**: Visual representations of entities in the rendering world

## Implemented Systems

The engine features numerous specialized systems supporting a rich MMO RTS experience:

### Building System 
- Handles creation and management of in-game structures
- Supports instant building and multi-stage construction processes
- Manages building specifications and placement on the tile-based world

### Battle Group System 
- Manages groups of units for combat
- Handles unit recruitment, battle initialization, and combat resolution
- Processes group-related events and battle outcomes

### Dungeon System
- Manages procedural dungeon entities and their behavior
- Populates dungeons from specifications 
- Responds to group death events to clean up defeated dungeons

### Delta Compression System
- Optimizes network traffic by tracking entity state changes
- Sends only modified components to reduce bandwidth usage
- Core to the game's networking architecture

### Entity Vision System 
- Implements Fog of War mechanics
- Manages visibility calculations based on entity line of sight
- Updates discovery state as entities explore the world

### Movement System 
- Handles entity movement through the infinite chunk-based world
- Manages movement paths, courses, and waypoints
- Interacts with the map system for position updates

### Party System
- Manages groups of entities that act as a cohesive unit
- Facilitates unit management within parties
- Responds to group-related events

### Tile System 
- Manages the infinite chunk-based map loaded from Tiled
- Tracks entity and building placement
- Provides spatial queries for the game world

### Map System 
- Coordinates positioning of entities on the game map
- Handles chunk loading and unloading
- Synchronizes position data between server and client

### Player System 
- Manages player entities and their state
- Tracks discovered areas through fog of war
- Handles player-specific actions and recruiting

### Harvesting System
- Implements resource gathering mechanics
- Tracks harvesting progress and resource collection
- Handles harvesting interruptions

### Resource System
- Manages resources throughout the game world
- Sets up resource points on tiles based on specifications
- Critical for the game's economy

### Cargo System 
- Implements inventory management for entities
- Processes harvested resources
- Manages cargo capacity and contents

## System Integration and Task Scheduler

Instead of a traditional update loop, Lisergy uses a task-based scheduler:

- Operations are scheduled tasks rather than immediate updates
- Enables better server scalability at a slight cost to realtime precision
- Movement, battles, and other time-sensitive operations are managed as future tasks

## Client-Server Architecture

The engine supports a robust networked multiplayer implementation:

- TCP-based networking with packet classes
- Delta compression for efficient state synchronization
- Components marked for network synchronization
- Client prediction and server reconciliation

## Usage Example


```csharp
// Creating a new game instance
var game = new LisergyGame(gameSpecs, logger);

// Building a structure at a specific tile
var player = game.Players[playerId];
var targetTile = game.World.GetTile(location);
var building = player.Entity.Logic.Building.InstantBuild(buildingSpecId, targetTile);

// Moving a party to a location
var party = player.EntityLogic.GetParties()[0];
var path = game.Logic.Systems.Map.FindPath(party.GetTile(), destinationTile);
party.Logic.Movement.TryStartMovement(path, CourseIntent.Move);

// Harvesting resources
var resourceTile = game.World.GetTile(resourceLocation);
if (party.Logic.Harvesting.GetPossibleHarvest(resourceTile) != null)
{
    party.Logic.Harvesting.StartHarvesting(resourceTile);
}

```

The Lisergy engine combines performance optimization techniques like unsafe code and memory management with a flexible architecture designed for an ambitious MMO RTS experience.