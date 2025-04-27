# Lisergy Client

## Main Entrypoint

The code main entrypoint is `Main.cs`
All listeners, services, and views are setup from this file.

## Structure

The client uses the Lisergy SDK to handle communication with the game logic.

+-------+                        +---------+                                               +-----------+                                           +-------------+      
| Logic |                        | Server  |                                               | ClientSDK |                                           | UnityClient |      
+-------+                        +---------+                                               +-----------+                                           +-------------+      
    |                                 |                                                          |                                                        |             
    | Game state has been update      |                                                          |                                                        |             
    |-------------------------------->|                                                          |                                                        |             
    |                                 |                                                          |                                                        |             
    |                                 | Send update packets according to delta compression       |                                                        |             
    |                                 |--------------------------------------------------------->|                                                        |             
    |                                 |                                                          |                                                        |             
    |                                 |                                                          | Fire events and create EntityViews for entities        |             
    |                                 |                                                          |------------------------------------------------------->|             
    |                                 |                                                          |                                                        |             
    |                                 |                                                          |                                                        | Update View 
    |                                 |                                                          |                                                        |------------ 
    |                                 |                                                          |                                                        |           | 
    |                                 |                                                          |                                                        |<----------- 
    |                                 |                                                          |                                                        |             
    |                                 |                                                          |                                  Action request packet |             
    |                                 |                                                          |<-------------------------------------------------------|             
    |                                 |                                                          |                                                        |             
    |                                 |                                             Authenticate |                                                        |             
    |                                 |<---------------------------------------------------------|                                                        |             
    |                                 |                                                          |                                                        |             
    |      Update state if applicable |                                                          |                                                        |             
    |<--------------------------------|                                                          |                                                        |             
    |                                 |                                                          |                                                        |            

## Listeners

#### Component Updates

All entities have components. One of the best ways to keep track of what's happening with an Entity is to add a component update hook to it using the client SDK:

```
private void Register() 
{
    GameClient.Modules.Components.OnComponentUpdate<MapPlacement>(OnUpdateComponent);
}


private void OnUpdateComponent(IEntity entity, MapPlacement oldValue, MapPlacement newValue) 
{
    ... do something ..
}
```

You can also register for when a component is being removed or added.

#### SDK Events

Another way to hook to the game events is hooking to the SDK events. Those events are ready to be consumed by the game client such as EntityViewCreated etc.

#### Game Logic Events

You can also hook to specific game logic events that are fired from inside the game logic.
Bear in mind that on the client, only systems that have `[SyncedSystem]` attribute will be registered on client.

#### Game Logic Packets

As the lowest possible level, you can also listen for game packets even tho it's better to avoid it and let the SDK handle it.