# Codename Lisergy

[![LisergyPipeline](https://github.com/Ziden/Lisergy/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Ziden/Lisergy/actions/workflows/dotnet.yml)

## Overview

The Lisergy Project is an ambitious MMO RTS game framework built with a modular and data-driven architecture. It leverages .NET technologies to provide a robust and scalable solution for both client and server-side game logic. The project is divided into several key components, each with its own dedicated functionality and purpose.

## Project Structure

The Lisergy Project is organized into three main components:

1. **BaseServer**: A versatile server framework designed to run game servers, supporting both offline and online modes. [BaseServer README](LisergyServer/Core/readme.md)
2. **Lisergy Game Engine**: An innovative ECSL (Entity-Component-System-Logic) game engine tailored for an exploration-oriented MMO RTS. [Lisergy Game Engine README](LisergyServer/Game/readme.md)
3. **ClientSDK**: A .NET Standard library designed to facilitate communication between game clients and the server, with seamless integration into game engines like Unity. [ClientSDK README](LisergyServer/ClientSDK/readme.md)

## BaseServer

### Overview

BaseServer is a versatile server framework built in .NET Standard 2.1, designed to run a game server on the client itself, enabling offline mode gameplay. It supports both single-threaded and multi-threaded server configurations and is divided into several specialized servers: Chat, Account, Battle, and World servers.

### Features

- **Offline Mode**: Run the game server on the client for offline gameplay.
- **Multi-threading Support**: Choose between single-threaded and multi-threaded server configurations.
- **Modular Design**: Separate servers for chat, account management, battles, and world interactions.

### Tech Stack

- **.NET Standard 2.1**: Ensures compatibility across different .NET implementations.
- **C#**: The primary programming language used for development.
- **JSON**: Used for configuration files.
- **Multi-threading**: Utilizes .NET's threading capabilities to support concurrent server operations.
- **Telepathy**: A simple, message-based networking library used for communication between the server and clients.

For more detailed information, refer to the [BaseServer README](LisergyServer/Core/readme.md).

## Lisergy Game Engine

### Overview

The Lisergy Game Engine is an innovative entity-component-system-ish (ECS) game engine built with C# 8.0 targeting .NET Standard 2.1. It is designed for an exploration-oriented MMO RTS and implements a unique variation of the ECS pattern called ECSL (Entity-Component-System-Logic).

### Core Concepts

- **Entities**: Game objects that act as containers for components and have a unique identity.
- **Components**: Pure data structures defining entity properties (e.g., `BuildingComponent`, `CargoComponent`).
- **Systems**: Process entities with specific components and implement game rules.
- **Logic Classes**: Stateless behavior implementations that operate on entity data.

### Implemented Systems

The engine features numerous specialized systems supporting a rich MMO RTS experience, including:

- **Building System**: Manages in-game structures.
- **Battle Group System**: Handles combat mechanics.
- **Dungeon System**: Manages procedural dungeon entities.
- **Delta Compression System**: Optimizes network traffic.
- **Entity Vision System**: Implements Fog of War mechanics.
- **Movement System**: Manages entity movement.
- **Party System**: Manages groups of entities.
- **Tile System**: Manages the infinite chunk-based map.
- **Map System**: Coordinates entity positioning.
- **Player System**: Manages player entities.
- **Harvesting System**: Implements resource gathering mechanics.
- **Resource System**: Manages game resources.
- **Cargo System**: Implements inventory management.

### System Integration and Task Scheduler

Instead of a traditional update loop, Lisergy uses a task-based scheduler, enabling better server scalability at a slight cost to real-time precision.

### Client-Server Architecture

The engine supports a robust networked multiplayer implementation with TCP-based networking, delta compression, and client prediction.

For more detailed information, refer to the [Lisergy Game Engine README](LisergyServer/Game/readme.md).

## ClientSDK

### Overview

The ClientSDK is a .NET Standard library designed to facilitate communication between game clients and the server. It is built to be integrated into game engines such as Unity, providing a robust framework for handling game logic, server communication, and event management.

### Architecture

The ClientSDK is structured around several key modules, each responsible for a specific aspect of the game logic and server interaction. These modules are exposed through the `IServerModules` interface, which acts as the primary entry point for the SDK.

### Client-Side Prediction

The ClientSDK includes mechanisms for client-side prediction to enhance the responsiveness and smoothness of the game experience. This involves predicting certain game states and actions on the client side before receiving confirmation from the server.

### Network and Data Synchronization

The ClientSDK includes robust mechanisms for synchronizing data between the client and server. This ensures that the game state remains consistent across all clients and the server. The synchronization process involves delta compression, component synchronization, and efficient network communication.

For more detailed information, refer to the [ClientSDK README](LisergyServer/ClientSDK/readme.md).

## Conclusion

The Lisergy Project provides a comprehensive framework for developing an MMO RTS game with a focus on modularity, scalability, and performance. By leveraging the BaseServer, Lisergy Game Engine, and ClientSDK, developers can create a seamless and engaging game experience for players. The project's architecture emphasizes a data-driven approach, ensuring flexibility and ease of maintenance.

## Prorotype Design

https://docs.google.com/document/d/1FoiBIhvpWwPVy5s_gLwaugZjPG2pnMK_K3bV8i--oiw/edit?usp=sharing

## High Level Design

https://docs.google.com/document/d/1f66aY_0vgrSU00gnmlunzPuO68NLbRfhTXuf-0_Lbdk/edit?usp=sharing
