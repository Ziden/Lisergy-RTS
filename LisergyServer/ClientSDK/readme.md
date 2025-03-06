# ClientSDK

## Overview

The ClientSDK is a .NET Standard library designed to facilitate communication between game clients and the server. It is built to be integrated into game engines such as Unity, providing a robust framework for handling game logic, server communication, and event management.

## Architecture

The ClientSDK is structured around several key modules, each responsible for a specific aspect of the game logic and server interaction. These modules are exposed through the `IServerModules` interface, which acts as the primary entry point for the SDK.

### IServerModules Interface

The `IServerModules` interface defines the core modules available in the SDK. Each module is responsible for a specific domain of the game logic:

- **IAccountModule**: Manages user accounts, including authentication and account settings.
- **IPlayerModule**: Handles player-specific data and actions.
- **GameViewModule**: Manages the game views, providing a way to interact with different game states and scenes.
- **IEntityModule**: Manages game entities, including creation, updates, and deletion.
- **IActionModule**: Handles actions performed by players or entities within the game.
- **IChatModule**: Manages in-game chat functionality, including sending and receiving messages.
- **IBattleModule**: Manages battle-related logic, including combat mechanics and battle state.
- **IBuildingModule**: Handles building-related logic, including construction, upgrades, and management.

## Integration with Unity

The ClientSDK is designed to be easily integrated into Unity projects. By importing the SDK as a .NET Standard DLL, developers can leverage the provided modules to handle game logic and server communication seamlessly.

## Client-Side Prediction

The ClientSDK includes mechanisms for client-side prediction to enhance the responsiveness and smoothness of the game experience. This involves predicting certain game states and actions on the client side before receiving confirmation from the server. Two key examples of this are movement prediction and fog of war management.

### Movement Prediction

The `PartyMovementListener` class demonstrates how the client predicts and animates the movement of party entities. This involves starting and stopping animations based on movement events.

#### Key Methods

- **OnMoveStart**: Triggered when a movement interpolation starts. It updates the party view to play the running animation and orient the units towards the destination.
  
  
```csharp
  private void OnMoveStart(MovementInterpolationStartEvent e)
  {
      if (e.Entity.EntityType == EntityType.Party)
      {
          var view = e.Entity.GetView<PartyView>();
          foreach (var unitView in view.UnitViews)
          {
              unitView.Animations.PlayAnimation(UnitAnimation.Running);
              unitView.GameObject.transform.LookAt(new Vector3(e.To.X, unitView.GameObject.transform.position.y, e.To.Y));
          }
      }
  }
  
```

- **OnMoveEnd**: Triggered when a movement interpolation ends. It updates the party view to play the idle animation if the movement is complete.
  
  
```csharp
  private void OnMoveEnd(MovementInterpolationEndEvent e)
  {
      if (e.LastStep && e.Entity.EntityType == EntityType.Party)
      {
          var view = e.Entity.GetView<PartyView>();
          foreach (var unitView in view.UnitViews)
          {
              unitView.Animations.PlayAnimation(UnitAnimation.Iddle);
          }
      }
  }
  
```

### Fog of War Management

The `FogOfWarListener` class demonstrates how the client manages the fog of war. This involves batching visibility changes to avoid redundant calculations and updating the fog state of tiles based on visibility events.

#### Key Methods

- **OnVisibilityChange**: Triggered when the visibility of a tile changes. It updates the fog state of the tile view based on whether the tile is visible or not.
  
  
```csharp
  private void OnVisibilityChange(TileVisibilityChangedEvent ev)
  {
      if (!ev.Explorer.OwnerID.IsMine()) return;

      var view = ev.Tile.Entity.GetView<TileView>();
      view.RunWhenRendered(() =>
      {
          var view = ev.Tile.Entity.GetView<TileView>();
          if (ev.Visible) view.SetFogState(FogState.EXPLORED);
          else view.SetFogState(FogState.UNEXPLORED);
      });
  }
  
```

- **OnPostRender**: Triggered after a tile is rendered. It logs the post-render event for debugging purposes.
  
  
```csharp
  private void OnPostRender(TilePostRenderedEvent e)
  {
      _client.Log.Debug("Post render " + e.View.Entity);
      var view = e.View;
      /.. after render ../
  }
  
```

- **CheckFogAround**: Checks the fog state of neighboring tiles and updates their fog state if necessary.
  
  
```csharp
  private void CheckFogAround(TileView thisView, Direction d)
  {
      var near = thisView.Tile.GetNeighbor(d);
      var view = _client.Modules.Views.GetOrCreateView(near.Entity);
      if (view.State == EntityViewState.NOT_RENDERED || !view.Entity.IsVisible())
      {
          view.SetFogState(FogState.UNEXPLORED);
      }
  }
  
```

### Summary

By implementing client-side prediction for movement and fog of war, the ClientSDK ensures a smoother and more responsive gameplay experience. These mechanisms allow the client to anticipate and display game states before receiving confirmation from the server, reducing perceived latency and improving overall player experience.
### Example Usage

The following example demonstrates how to use the ClientSDK within a Unity project. The `Main` class initializes the SDK, sets up necessary services, and handles game events.


```csharp
using ClientSDK;
using UnityEngine;

public class Main : MonoBehaviour, IEventListener
{
    public static readonly bool OFFLINE_MODE = true;

    private GameClient _client;
    private ClientNetwork _network;
    private GameStateMachine _stateMachine;
    private List<IEventListener> _listeners = new List<IEventListener>();
    private GameScheduler _scheduler;
    private StandaloneServer _server;

    void Awake()
    {
        _client = new GameClient();
        _client.ClientEvents.On<GameStartedEvent>(this, OnGameStarted);
        _network = _client.Network as ClientNetwork;
        SetupViews();
        ConfigureUnity();
        SetupServices();
        Serialization.LoadSerializers();

        if (OFFLINE_MODE)
        {
            _server = new StandaloneServer();
            _server.Multithreaded = false;
            _server.Start();
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        UnityServicesContainer.OnSceneLoaded();
        _stateMachine = new GameStateMachine(_client);
    }

    void Update()
    {
        _network?.Tick();
        if (!_server.Multithreaded)
        {
            _server?.SingleThreadTick();
        }
        _scheduler?.Tick(DateTime.UtcNow);
    }

    private void OnApplicationQuit()
    {
        _network?.Disconnect();
        _server?.Dispose();
    }

    private void OnGameStarted(GameStartedEvent ev)
    {
        _listeners.Add(new YourEventListener(_client));
        _scheduler = _client.Game.Scheduler as GameScheduler;
    }


    public void SetupViews()
    {
        _client.Modules.Views.CreatorFunction = e =>
        {
            return new UnityEntityView(e, _client);
        };
    }

    public void SetupServices()
    {
        UnityServicesContainer.Client = _client;
        UnityServicesContainer.Register<IInputService, InputService>(gameObject?.AddComponent<InputService>());
    }

    public static void ConfigureUnity()
    {
        Application.runInBackground = true;
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }
}

```

### Explanation

- **Initialization**: The `Main` class initializes the `GameClient` and sets up event listeners, views, and services.
- **Offline Mode**: If `OFFLINE_MODE` is enabled, a standalone server is started for local testing.
- **Event Handling**: The `OnGameStarted` method sets up various event listeners to handle game-specific events.
- **Service Registration**: The `SetupServices` method registers various services required by the game, such as input, UI, audio, and notifications.


## Network and Data Synchronization

The ClientSDK includes robust mechanisms for synchronizing data between the client and server. This ensures that the game state remains consistent across all clients and the server. The synchronization process involves delta compression, component synchronization, and efficient network communication.

### Delta Compression

Delta compression is a technique used to minimize the amount of data transmitted over the network by only sending the changes (deltas) rather than the entire state. This is particularly useful in reducing bandwidth usage and improving performance.

#### DeltaCompressionLogic

The `DeltaCompressionLogic` class is responsible for managing delta compression for game entities. It tracks changes to entity components and generates update packets containing only the modified components.

#### Key Methods

- **SendAllVisibleTiles**: Flags all tiles visible to the current entity (usually a player) and sets the exploration flag for these tiles.
  

```csharp
public void SendAllVisibleTiles()
{
    if (!CurrentEntity.Components.TryGet<PlayerVisibilityComponent>(out var visibilityData)) throw new Exception("Requires VisibilityData Component");

    foreach (var pos in visibilityData.VisibleTiles)
    {
        var tile = CurrentEntity.Game.World.GetTile(pos.X, pos.Y);
        tile.Logic.DeltaCompression.SetTileExplorationFlag(DeltaFlag.SELF_REVEALED);
    }
}

```

- **GetUpdatePacket**: Generates an update packet containing the modified components for a given receiver.
  

```csharp
public BasePacket GetUpdatePacket(GameId receiver, bool onlyDeltas = true)
{
    var packet = PacketPool.Get<EntityUpdatePacket>();
    packet.EntityId = CurrentEntity.EntityId;
    packet.OwnerId = CurrentEntity.OwnerID;
    packet.Type = CurrentEntity.EntityType;
    var deltas = CurrentEntity.Components.GetComponentDeltas(receiver, onlyDeltas);
    packet.SyncedComponents = deltas.updated.ToArray();
    packet.RemovedComponentIds = deltas.removed.Select(c => Serialization.GetTypeId(c)).ToArray();
    if (packet.SyncedComponents.Length == 0)
    {
        PacketPool.Return(packet);
        return null;
    }
    return packet;
}

```

- **SetTileExplorationFlag**: Sets the exploration flag for a tile and its associated entities.
  

```csharp
public void SetTileExplorationFlag(DeltaFlag flag)
{
    var tile = CurrentEntity;
    tile.Logic.DeltaCompression.SetFlag(flag);
    var onTile = CurrentEntity.Logic.Tile.GetEntitiesOnTile();
    foreach (var e in onTile)
    {
        e.Logic.DeltaCompression.SetFlag(flag);
    }
    tile.Logic.Tile.GetBuildingOnTile()?.Logic.DeltaCompression.SetFlag(flag);
}

```

- **SetFlag**: Sets a delta flag for the current entity and marks it as modified.
  

```csharp
public bool SetFlag(DeltaFlag flag)
{
    if (!Game.Network.DeltaCompression.Enabled) return false;

    if (!CurrentEntity.Components.TryGet<DeltaFlagsComponent>(out var deltas))
    {
        deltas = new DeltaFlagsComponent();
    }
    bool hasUpdated = deltas.SetFlag(flag);
    CurrentEntity.Save(deltas);
    if (hasUpdated)
    {
        Game.Network.DeltaCompression.AddModified(CurrentEntity);
    }
    return hasUpdated;
}

```

### Component Synchronization

Component synchronization ensures that the state of entity components is consistent between the client and server. The `ComponentSet` class manages the components of an entity and tracks changes to these components.

#### Key Methods

- **GetComponentDeltas**: Retrieves the modified components and removed components for synchronization.
  

```csharp
public (List<IComponent> updated, HashSet<Type> removed) GetComponentDeltas(GameId receiver = default, bool deltaCompression = true)
{
    _returnBuffer.Clear();
    var toSync = GetComponentsToSync(deltaCompression);
    foreach (var kp in toSync)
    {
        if (ShouldSync(kp, receiver)) _returnBuffer.Add(GetByType(kp));
    }
    return (_returnBuffer, GetRemoved());
}

```

- **Save**: Saves a component and marks it as modified for synchronization.
  

```csharp
public void Save<T>(in T c) where T : IComponent
{
    var t = c.GetType();
    GetComponents().TryGetValue(t, out var oldValue);
    GetComponents()[t] = c;
    if (t.IsValueType)
    {
        GetReadCopies()[t] = c;
    }
    else
    {
        GetReadCopies().Remove(t);
    }
    TrackSync(t);
    FlagCompnentHasDelta(t);
}

```

- **Remove**: Removes a component and marks it as removed for synchronization.
  

```csharp
public bool Remove<T>() where T : IComponent
{
    var t = typeof(T);
    if (GetComponents().TryGetValue(t, out var c))
    {
        if (c is IDisposable d) d.Dispose();
        if (GetComponents().Remove(t))
        {
            GetRemoved().Add(t);
            _entity.Logic.DeltaCompression.SetFlag(DeltaFlag.COMPONENTS);

            GetModified().Remove(t);
            _entity.Game.Log.Debug($"Removed {t.Name} from {_entity}");

            var ev = ClassPool<ComponentUpdateEvent<T>>.Get();
            ev.Entity = _entity;
            ev.Old = (T)c;
            ev.New = default;
            CallEvent(ev);
            ClassPool<ComponentUpdateEvent<T>>.Return(ev);

            return true;
        }
    }
    return false;
}

```

### Network Communication

The `IGameNetwork` interface defines the methods for sending and receiving packets over the network. The `DeltaCompression` property indicates whether delta compression is enabled.

#### Key Methods

- **SendToPlayer**: Sends a packet to a specific player.
  

```csharp
public void SendToPlayer<PacketType>(PacketType p, params GameId[] players) where PacketType : BasePacket;

```

- **SendToServer**: Sends a packet to the server.
  

```csharp
public void SendToServer(BasePacket p, ServerType server = ServerType.WORLD);

```

- **ReceiveInput**: Processes an input packet received from a player.
  

```csharp
public void ReceiveInput(GameId sender, BasePacket input);

```

### Summary

The ClientSDK's network and data synchronization mechanisms ensure that the game state remains consistent across all clients and the server. By leveraging delta compression, component synchronization, and efficient network communication, the SDK minimizes bandwidth usage and improves performance, providing a smooth and responsive gameplay experience.
## Conclusion

The ClientSDK provides a comprehensive framework for managing game logic and server communication in Unity projects. By leveraging the modular architecture, developers can easily integrate and extend the SDK to meet the specific needs of their game.