# Lisergy

The infinite chunk based map loaded from tiled will be serve for this exploration oriented mmo rts, where players will kill, harvest and craft to gain power and items. 
The idea is to enable a simple mmo rts engine.

# Game Engine

Basic toolkit for the game to implement its logic.

# Gameplay Features Roadmap:

- ~~Harvesting~~ (Done)
- ~~Cargo~~ (Done)
- ~~Battles~~ (Done)
- ~~Dungeons~~ (Done)
- ~~Fog of War~~ (Done)
- Building (WIP)
- Monsters
- Crafting
- Raiding (PvP)
- Recruiting
- Harvesting Buildings
- Workers
- Battle AI Crystals
- Guilds
- Guild Wars

## Running:

You will require:

- [Unity3d](https://unity3d.com/pt/get-unity/download) 
- [.NET Core 6 SDK](https://www.microsoft.com/net/download)
- [Microsoft .NET Framework 4.8](https://www.microsoft.com/pt-br/download/details.aspx?id=21)
- [Redis Database](https://redis.io/download) (Optional)

To run the client, simply run the Unity project.

To run the server, simply run the "StandaloneServer" project on your favorite .net IDE.
If there's any errors when running the server to open the port, ensure the port is opened in your firewall settings.

You can use any login/password as that account will be created if those credentials are not existant.

### Namings

- Game, or Game.dll refers to the dll that contains the game logic that's shared in client and server.
- GameData refers to the data that populates the game (aka game configs)
- Server refers to the backend that runs the game world in an authoritative manner
- Client refers to Unity3D, who runs the game and renders its views
- Client SDK refers to the SDK Unity imports to integrate with the game which pre-handles most of the logical connection to the game.

### Core Structure

Game logic uses a odd implementation of ECS we abbreviated ECSL, which is a modification of ECS introducing the Logic layer.

The game pieces are divided in 4 main parts:

- Entities: 
     The component holder.
     Identified pieces of data that can have components attached to them.
     Can be superclassed for hard-typed creation with specific components as a form of typing the entity.

- Components: 
     Components are little pieces of data that can be added to entities. When a component is added to an entity, now its system starts picking events that component is interested in to 
     alter that entity's behaviour.

- Systems: 
     Systems are event listeners for entities that have specific components. Whenever something specific a component is interested in happens, the system will pick it up and perform
     entity updates. To read entity data or perform the updates on entities, systems will call that entity logic to do so.

- Logic:
     Instead of systems updating the entity, the code that actually performs the updates or reads data is isolated in another layer - the logic.
     The logic is stateless in itself, that means any updates are done in the entity components where the logic itself will never hold any state
     the logic objects are shared among entities.

- Entity Views:
     Every entity has a corresponding view. The view is how this entity is perceived outside the logic world, that means, in the rendering world.
     The view is who handles animations, mesh etc.

### Tasks & Scheduler

On top of running a standard ticking update mechanic under the hood, all operations on server are based towards a single task scheduler.
Movement, battles, or anything that's required to happen is a task scheduled to happen in the future as opposed to the common update loop that would iterate over
entities. 
This is a tradeoff chosen for enabling scalability options on the server while loosing a bit the realtime precision aspect of the game.

### Events

Events are the baseline of how systems communicate with themselves. They are in-memory, pooled, fire-and-forget classes.

### Services

Systems listen for game events. 
Services are a level above, they listen for packets, so only required for systems that contains specific networking communication.
Services will alter the game state by using logic the same way systems do.

### Networking

Client and server communicate trought TCP packets, either by specific packets being sent or, more commonly, updates from server driven from updates to entities which are tracked with delta compression then sent.
Every packet is represented by a class that will extend `IServerPacket` if is sent from server to clients, or `IClientPacket` if its sent from client to server.

### Delta Compression & Components

Whenever an entity component get's updated, the server will track that update happened - however it will not do anything at that point.
After the server finishes processing the current tick, it will get all modifications done to all entities and send the necessary packets to the necessary viewers.

### Persistence

The game can serialize its entities and its entire world very fast. Basic storage should be handled with flat files and/or Redis.

