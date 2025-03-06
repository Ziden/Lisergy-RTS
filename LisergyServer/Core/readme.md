# BaseServer

## Overview
BaseServer is a versatile server framework built in .NET Standard 2.1, designed to run a game server on the client itself, enabling offline mode gameplay. It supports both single-threaded and multi-threaded server configurations, and is divided into several specialized servers: Chat, Account, Battle, and World servers.

## Features
- **Offline Mode**: Run the game server on the client for offline gameplay.
- **Multi-threading Support**: Choose between single-threaded and multi-threaded server configurations.
- **Modular Design**: Separate servers for chat, account management, battles, and world interactions.

## Server Types
1. **Chat Server**: Manages in-game chat functionalities.
2. **Account Server**: Handles user account management and authentication.
3. **Battle Server**: Manages in-game battles and combat mechanics.
4. **World Server**: Oversees the game world, including NPCs, environment, and world events.

## Tech Stack
- **.NET Standard 2.1**: Ensures compatibility across different .NET implementations.
- **C#**: The primary programming language used for development.
- **JSON**: Used for configuration files.
- **Multi-threading**: Utilizes .NET's threading capabilities to support concurrent server operations.
- **Telepathy**: A simple, message-based networking library used for communication between the server and clients.

## Code Architecture
- **Modular Design**: The codebase is divided into distinct modules, each responsible for a specific server type (Chat, Account, Battle, World).
- **Dependency Injection**: Utilizes dependency injection to manage dependencies and improve testability.
- **Configuration Management**: Uses `appsettings.json` for configuration settings, allowing easy customization and environment-specific configurations.
- **Logging**: Implements logging to track server activities and errors, aiding in debugging and monitoring.
- **Event-Driven**: The server operations are event-driven, allowing for responsive and scalable interactions.

## Telepathy Library
BaseServer uses the Telepathy library for networking. Telepathy is a simple, message-based networking library that provides reliable TCP communication. It is designed to be easy to use and integrate, making it a great choice for game server development.

### Key Features of Telepathy
- **Reliable TCP Communication**: Ensures that messages are delivered in order and without loss.
- **Simple API**: Easy to use and integrate into existing projects.
- **Cross-Platform**: Works on multiple platforms, including Windows, macOS, and Linux.