# How to: Save and Synchronize Mod Data

← [Back to scripting index](README.md)

Persisting block states and synchronizing them across clients in multiplayer is essential for terminal controls, custom grid logic, and server configurations. Space Engineers provides multiple mechanisms for data storage and network replication.

This guide covers which storage systems to use, how to implement them, and how to maintain server-authoritative multiplayer synchronization.

---

## 1. Choosing the Right Mechanism

Select your persistence and sync tools based on your target scope:

| Storage Type | Good For | Persists? | Syncs? | Scope |
|---|---|---|---|---|
| **`CustomData`** | Player-configurable INI settings on blocks. | Yes (SBC) | Yes | Per-block. |
| **`MyModStorageComponent`** | Custom mod state (complex types, structures). | Yes (SBC) | Yes (on stream) | Per-entity (block, grid, voxel). |
| **`MySync` Fields** | Real-time state syncing (switches, sliders, colors). | No | Yes (instant) | Per-block. |
| **Packet Sending** | Event-driven data, large packets, custom UI events. | No | Yes | Global / Mod-wide. |
| **Sandbox Variables** | Global mod configs, server parameters. | Yes (SBC) | Yes (on join) | Global / World. |
| **Storage Folders** | Log files, client-local layouts, massive caches. | Yes (Files) | No | Mod-wide or Local PC. |

---

## 2. Block Storage: `MyModStorageComponent`

This is the standard way to persist arbitrary data tied to a specific block (or other entity). It stores key-value pairs where the key is a unique GUID and the value is a serialized string.

### Step 1: Claim Your GUID in SBC
Before your script can write to a storage component, you **must register your GUID** via a `.sbc` definition inside your mod's `Data/` folder. Unregistered GUID data is automatically discarded by the game on world save.

Create a file named `Data/ModStorageComponents.sbc`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <EntityComponents>
    <EntityComponent xsi:type="MyObjectBuilder_ModStorageComponentDefinition">
      <Id>
        <TypeId>ModStorageComponent</TypeId>
        <SubtypeId>YourUniqueModName</SubtypeId>
      </Id>
      <RegisteredStorageGuids>
        <guid>11111111-2222-3333-4444-555555555555</guid>
      </RegisteredStorageGuids>
    </EntityComponent>
  </EntityComponents>
</Definitions>
```

### Step 2: Accessing and Writing to Storage
In your C# block logic, check for the storage component's existence and populate your custom GUID key with a serialized string (e.g., JSON, INI, or Base64 binary):

```csharp
public static readonly Guid StorageGuid = new Guid("11111111-2222-3333-4444-555555555555");

public void SaveBlockSettings(string settingsString)
{
    if (Entity.Storage == null)
    {
        Entity.Storage = new MyModStorageComponent();
    }
    
    Entity.Storage[StorageGuid] = settingsString;
}

public string LoadBlockSettings()
{
    if (Entity.Storage != null && Entity.Storage.ContainsKey(StorageGuid))
    {
        return Entity.Storage[StorageGuid];
    }
    return null;
}
```

---

## 3. Real-Time Sync: `MySync`

For high-frequency terminal properties (like a slider position or toggled checkbox), use the game's built-in `MySync` wrappers. This replicates primitive values across the network with minimal overhead.

### Rules for `MySync`
* **Blittable types only:** Supported types include `bool`, `int`, `float`, `double`, `enum`, `Color`, `Vector3`, and `MyFixedPoint`. Complex structs, lists, and `string` are **blocked**.
* **Limit:** There is a total limit of 64 `MySync` properties per block instance (shared with the block's vanilla features).
* **Interface Required:** Your containing class **must** implement `IMyEventProxy`.

### Implementation Example
```csharp
public class MyInteractiveBlock : MyGameLogicComponent, IMyEventProxy
{
    // The engine automatically registers these fields via reflection
    public MySync<float, SyncDirection.BothWays> CustomSlider;
    public MySync<bool, SyncDirection.FromServer> IsServerRunning;

    public override void Init(MyObjectBuilder_EntityBase ob)
    {
        NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        
        // Listen for value changes
        CustomSlider.ValueChanged += OnSliderChanged;
    }

    private void OnSliderChanged(MySync<float, SyncDirection.BothWays> syncField)
    {
        float newValue = syncField.Value;
        // Apply slider value locally...
    }

    public void UpdateSlider(float newValue)
    {
        // Set the property. BothWays means client change syncs to server,
        // and server automatically relays it to all other clients.
        CustomSlider.Value = newValue;
    }
}
```
* **Gotcha (SetLocalValue):** When initializing a block's properties from loaded settings (e.g. during `Init` or `UpdateOnceBeforeFrame`), use `CustomSlider.SetLocalValue(loadedValue)`. Using `.Value` on startup triggers unnecessary, wasteful network packets.

---

## 4. Custom Packets & Messaging

When you need to sync non-primitive data types (such as custom structural layouts, logs, or command requests), use network packet routing.

### Network Rules
1. **Never trust the client:** Clients can modify mod files. Always perform strict validation on the server before acting on a client-received packet.
2. **Channel collision:** Message channels are shared game-wide. Choose a unique, non-trivial `ushort` channel ID to avoid colliding with other mods.

### Packet Implementation Pattern
Implement network messaging globally, preferably inside a dedicated session component:

```csharp
[MySessionComponentBase(MyUpdateOrder.NoUpdate)]
public class NetworkManagerSession : MySessionComponentBase
{
    public const ushort NetworkChannel = 54891; // Choose a unique, large number

    public override void LoadData()
    {
        // Register the network message handler
        MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(NetworkChannel, OnPacketReceived);
    }

    protected override void UnloadData()
    {
        // Unregister to prevent memory leaks
        MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(NetworkChannel, OnPacketReceived);
    }

    private void OnPacketReceived(ushort channel, byte[] data, ulong senderSteamId, bool isSenderServer)
    {
        try
        {
            // Deserialize packet data...
            if (MyAPIGateway.Session.IsServer)
            {
                // Validate payload and perform actions on the server
            }
            else
            {
                // Apply visual updates on the client
            }
        }
        catch (Exception e)
        {
            MyLog.Default.WriteLineAndConsole($"Error processing packet on channel {channel}: " + e);
        }
    }

    public static void SendPayloadToServer(byte[] payload)
    {
        MyAPIGateway.Multiplayer.SendMessageToServer(NetworkChannel, payload);
    }
}
```

---

## 5. Global World Data: `Sandbox.sbc`

If you are persisting server configs or global parameters that are not bound to an individual block, use Sandbox Variables. The game automatically saves these in the world's master `Sandbox.sbc` file and syncs them to joining clients:

```csharp
// Store value (in-memory; persists on next world save)
MyAPIGateway.Utilities.SetVariable("MyMod_ServerLimit", 50);

// Retrieve value
int maxLimit;
if (MyAPIGateway.Utilities.GetVariable("MyMod_ServerLimit", out maxLimit))
{
    // ... use limit ...
}
```

Make sure any script operations interacting with these persistent APIs are guarded under the [crash-safety-checklist.md](crash-safety-checklist.md).

---

## Gotchas & TL;DR

* **Unregistered GUIDs Wipe:** If you write to `Entity.Storage[MyGuid]` without registering that GUID in a `<RegisteredStorageGuids>` block in an SBC file, the game silently deletes that data on world save.
* **`MySync` restricts types:** You cannot use `MySync` for `string` or `byte[]`. Use primitives or enums only.
* **Verify `SetLocalValue`:** On block load, always initialize `MySync` fields with `.SetLocalValue()` to prevent massive startup network packets.
* **Server Authority:** Never execute client requests directly. Receive the request via a packet, validate boundaries on the server, and then alter state.

---

**Source:**
* https://spaceengineers.wiki.gg/wiki/Modding/Reference/Programming/Save_and_Sync
