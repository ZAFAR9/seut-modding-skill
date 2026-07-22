# How to: Implement Game Logic and Session Components

← [Back to scripting index](README.md)

Modding in Space Engineers relies on two primary script entry points: **Game Logic Components** (`MyGameLogicComponent`) for entity-specific behaviors, and **Session Components** (`MySessionComponentBase`) for global, world-wide managers.

This guide details their lifecycles, configuration, performance optimization under the game's injected profiler, and presents a worked reference based on the repo's `examples/BlackHoleContainer/` script.

---

## 1. Game Logic Components (`MyGameLogicComponent`)

A game logic component attaches directly to an individual entity (e.g., a cargo container, reactor, or weapon block) and executes custom behavior specifically for that instance.

### Binding to an Entity Subtype
Use the `[MyEntityComponentDescriptor]` attribute to declare which block type and subtype your script should attach to:

```csharp
[MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "BlackHoleContainer")]
public class BlackHoleContainerLogic : MyGameLogicComponent
{
    // ...
}
```
* **Argument 1:** The `MyObjectBuilder_*` type of the target block (e.g., `MyObjectBuilder_CargoContainer`).
* **Argument 2:** `EntityUpdate` configuration behavior (typically set to `false` to handle registration manually in `Init`).
* **Argument 3:** String subtype ID(s). Matching this to your block's SBC `<SubtypeId>` isolates your script to that block variant, preventing it from attaching to vanilla blocks.

### The Lifecycle
1. **`Init(MyObjectBuilder_EntityBase objectBuilder)`**
   * Called when the entity is instantiated. 
   * **Rule of Thumb:** Keep this minimal. Physics, subparts, and other blocks on the grid are *not* guaranteed to be initialized or safe to query here. Use it primarily to enable the first-frame initialization callback:
     ```csharp
     public override void Init(MyObjectBuilder_EntityBase objectBuilder)
     {
         NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
     }
     ```
2. **`UpdateOnceBeforeFrame()`**
   * Runs exactly once, before the very first simulation tick.
   * Use this for safe, one-time setup: casting the entity, verifying block grids, checking for required inventories, and evaluating server vs. client execution.
3. **`NeedsUpdate` Callbacks**
   Depending on what flags you assign to the `NeedsUpdate` property, the engine will route execution to the following methods:
   * `MyEntityUpdateEnum.EACH_FRAME` (60 ticks/sec) -> Triggers `UpdateBeforeSimulation()` and `UpdateAfterSimulation()`.
   * `MyEntityUpdateEnum.EACH_10TH_FRAME` (6 ticks/sec) -> Triggers `UpdateBeforeSimulation10()` and `UpdateAfterSimulation10()`.
   * `MyEntityUpdateEnum.EACH_100TH_FRAME` (0.6 ticks/sec) -> Triggers `UpdateBeforeSimulation100()` and `UpdateAfterSimulation100()`.
4. **`Close()`**
   * Called when the block is ground down, deleted, or the world is unloaded.
   * **Mandatory Action:** Always unhook event handlers, close files, and nullify references here to avoid critical memory leaks.

---

## 2. Session Components (`MySessionComponentBase`)

While game logic runs per-block, a session component runs **once per world session**. It is used to coordinate global state, custom user interfaces, network message handling, or server configuration.

| Feature | Game Logic (`MyGameLogicComponent`) | Session Component (`MySessionComponentBase`) |
|---|---|---|
| **Scope** | Tied to a specific block/entity instance. | Global (one per world load). |
| **Instantiation** | Automatically created with the block. | Created by the game on session load. |
| **Key Entry Points** | `Init`, `UpdateOnceBeforeFrame`, `UpdateAfterSimulation` | `LoadData`, `BeforeStart`, `UpdateAfterSimulation`, `UnloadData` |
| **Typical Use** | Animating subparts, adjusting custom inventory volume, local terminal logic. | Server/Client packet routing, tracking global configs, registering terminal controls. |

---

## 3. Worked Reference: The Black Hole Container

The repo contains a complete example under `examples/BlackHoleContainer/Data/Scripts/BlackHoleContainer/BlackHoleContainerLogic.cs`. This block represents a hyper-capacity container with full conveyor support and custom subpart animations.

### High-Capacity Setup
Instead of hardcoding high volumes in SBC which can lead to display issues or broken calculations, the script overrides inventory behavior dynamically in `UpdateOnceBeforeFrame()`:

```csharp
private static void ConfigureMaxStorage(MyInventory inventory)
{
    // FixInventoryVolume raises the actual physics limit in cubic meters
    inventory.FixInventoryVolume(1000000000f); // 1e9 m^3 (~1e12 Litres)
    
    // Explicitly OR on transfer flags to keep standard conveyor flow active
    inventory.SetFlags(inventory.GetFlags() | MyInventoryFlags.CanReceive | MyInventoryFlags.CanSend);
    inventory.Refresh();
}
```

### Animating Subparts Safely (No Decompose)
To spin the accretion disk, the script targets a subpart and rotates it relative to its original rest pose. Because the matrix contains non-uniform scale and baked tilts, decomposing is risky (and blocked by the whitelist). Pre-multiplying is the safer, standard way:

```csharp
// Capture original pose ONCE
if (!_diskCaptured)
{
    _diskRest = disk.PositionComp.LocalMatrixRef;
    _diskCaptured = true;
}

// Compute new angle, build local rotation, and pre-multiply
Matrix spin = Matrix.CreateRotationY(_spinAngle);
Matrix m = spin * _diskRest; // Applies rotation in the subpart's local coordinate space

disk.PositionComp.SetLocalMatrix(ref m, null, true);
```

---

## 4. Performance & The Injected Profiler

Space Engineers injects profiling instrumentation into **every single custom mod method**, property getter/setter, anonymous lambda, and constructor. Because of this, bad coding habits inside per-frame updates (`EACH_FRAME`) can drag down simulation speed.

### Guarding the Hot Paths
* **Distance Culling:** Never run heavy calculation or client-side animation code if the block is far away:
  ```csharp
  var cam = MyAPIGateway.Session?.Camera;
  if (cam != null && Vector3D.DistanceSquared(cam.Position, _block.GetPosition()) > 500 * 500)
      return; // Skip visual logic beyond 500 meters
  ```
* **Dedicated Server Guarding:** Visuals, subpart animations, and emissive pulsing must never run on dedicated servers:
  ```csharp
  bool dedicated = MyAPIGateway.Utilities?.IsDedicated ?? true;
  _animate = EnableAnimation && !dedicated;
  ```
* **Throttling:** If you do not need 60Hz precision (e.g. searching for inventories, doing calculations, syncing terminal values), throttle execution to `MyEntityUpdateEnum.EACH_10TH_FRAME` or `EACH_100TH_FRAME`.
* **Auto-Properties:** The injected profiler bypasses standard auto-properties (`public bool Active { get; private set; }`). Use them instead of custom property definitions in hot paths.

---

## 5. Crash-Safety Verification

Per-frame callbacks run directly in the core engine loop. If an unhandled exception escape-routes, **Space Engineers will instantly crash to desktop.**

Always implement the [crash-safety-checklist.md](crash-safety-checklist.md) pattern:
```csharp
public override void UpdateAfterSimulation()
{
    if (!_animate) return;
    try
    {
        if (Entity == null || Entity.MarkedForClose || !Entity.InScene) return;
        
        // ... perform visuals or logic ...
    }
    catch (Exception e)
    {
        _animate = false; // Disable permanently
        NeedsUpdate &= ~MyEntityUpdateEnum.EACH_FRAME; // Stop ticking
        MyLog.Default.WriteLineAndConsole("Mod permanently disabled due to exception: " + e);
    }
}
```

---

## Gotchas & TL;DR

* **`Init()` is too early:** Grids, physics, camera structures, and subparts are often not ready in `Init()`. Always defer reference capture to `UpdateOnceBeforeFrame()`.
* **Zero Exception Tolerance:** One uncaught exception in `UpdateAfterSimulation` will terminate the game process. Always wrap per-frame bodies in `try/catch` and permanently self-disable on error.
* **Avoid Decompose:** Do not use `Matrix.Decompose` to isolate subpart rotations. Cache the original rest local matrix and pre-multiply instead.
* **Profiler Bloat:** Avoid complex custom getter/setter math or nested sub-calls in 60 FPS update loops.

---

**Source:**
* https://spaceengineers.wiki.gg/wiki/Modding/Reference/Programming/Injected_Profiler
* `.agents/skills/seut-modding/examples/BlackHoleContainer/Data/Scripts/BlackHoleContainer/BlackHoleContainerLogic.cs`
