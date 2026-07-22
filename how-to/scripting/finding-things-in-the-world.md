# How to: Find Objects in the World and Explore Game Code

← [Back to scripting index](README.md)

Developing complex Space Engineers mods often requires searching for physical objects in the game world (e.g., grids, characters, or planets inside an area of effect). When the API documentation is thin or outdated, you must also know how to decompile and search the game's source code to discover undocumented features.

This guide covers efficient entity querying using spatial pruning structures and step-by-step instructions for exploring the game's codebase.

---

## 1. Finding Entities in a Volume

When you need to find entities inside a specific boundary (like a sphere around a custom block), querying `MyAPIGateway.Entities` is **not recommended** because it allocates a new list on every call, creating heavy garbage collection overhead.

### The Standard: `MyGamePruningStructure`
`MyGamePruningStructure` manages spatial hashing. It allows you to pass in a **pre-allocated, reusable list** to store results, avoiding garbage generation in high-frequency update loops.

#### Rule: Always Target "TopMost" Entities
Use the methods containing `TopMost` (e.g., `GetTopMostEntities`). "TopMost" entities are non-parented, root entities (such as grids, unseated characters, planets, and voxel maps). 
* Avoid `GetAllEntities()`—it performs slow, redundant hierarchical queries to find sub-objects (like individual armor blocks or tree models) that are typically unnecessary.

### Geometric Query Example (Sphere Query)
```csharp
// Pre-allocate the list at the class level to reuse across frames
private readonly List<MyEntity> _queriedEntities = new List<MyEntity>();

public void FindGridsInArea(Vector3D centerPosition, double radius)
{
    var sphere = new BoundingSphereD(centerPosition, radius);

    _queriedEntities.Clear(); // 1. Always clear before querying
    MyGamePruningStructure.Instance.GetTopMostEntities(ref sphere, _queriedEntities);

    // 2. Iterate through the results
    foreach (var entity in _queriedEntities)
    {
        var grid = entity as IMyCubeGrid;
        if (grid != null && grid.Physics != null)
        {
            // Found a valid physics grid in range
        }
    }

    _queriedEntities.Clear(); // 3. Clear again to free references
}
```

### Custom Lookup: `MyDynamicAABBTreeD`
If you need to query boundaries not aligned with standard bounding boxes (e.g., the complex, custom-sized gravity fields of planets), you can maintain your own spatial tree:

```csharp
// Create a custom tree structure
MyDynamicAABBTreeD CustomFieldTree = new MyDynamicAABBTreeD();

// To add an object (returns an ID required for moving or removing it)
int proxyId = CustomFieldTree.AddProxy(ref boundingBox, customObject, 0);

// If the object moves, update its position
CustomFieldTree.MoveProxy(proxyId, ref newBoundingBox, Vector3.Zero);

// Remove the object when deleted
CustomFieldTree.RemoveProxy(proxyId);
```

---

## 2. Exploring the Game Code (Decompiling)

Because the Space Engineers API is large and lacks exhaustive documentation, you will regularly need to read the game's actual implementation files to understand how a method works. 

The game code is **not obfuscated**, making decompiling highly practical.

### Setup Your Decompiler
1. Download a decompiler like **ILSpy** (simple, fast) or **dnSpy** (supports live debugging and advanced search).
2. Open your decompiler, click `File -> Open`, and navigate to your Space Engineers installation folder (typically `C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64\`).
3. Select and open all `.dll` files in this folder.
4. *(Optional)* Remove any `.XmlSerializers.dll` files from your list to reduce search noise.

### Finding Undocumented Classes
Do not try to navigate namespaces manually. Instead, use the global search tool (`Edit -> Search Assemblies` in dnSpy):
* **Class Names:** Search for a partial name. If you are coding custom safe zones, search "safezone". You will find `MySafeZone` (the barrier itself) and `MySafeZoneBlock` (the physical generator).
* **Verify Whitelist Access:** Once you find a class, cross-reference it with your MDK2 IDE template. If the IDE shows errors, the type is hidden by the whitelist.

### Right-Click -> Analyze
When you find a method or property but do not know how to invoke or assign it:
1. **Right-click** on the method name inside the decompiler.
2. Select **Analyze**.
3. Expand the **Used By** / **Called By** tree. This shows you every vanilla block and manager that calls that method, giving you immediate, working copy-paste code patterns.

### Key Prefix Conventions
To navigate classes easily, remember these naming prefixes used by Keen Software House:

| Class Prefix | What it represents | Example |
|---|---|---|
| **`MyObjectBuilder_*****`** | Raw serialized data structure (matches XML SBC nodes, network packets, or save file nodes). | `MyObjectBuilder_Thrust` |
| **`*****Definition`** | Live in-memory configuration. The engine deserializes an ObjectBuilder SBC, converts it to a Definition, and stores it in the `MyDefinitionManager` for fast lookup. | `MyThrustDefinition` |
| **`My*****`** | The actual, active physical/logic class in the game world. | `MyThrust` |

Double-check your custom math or object querying steps against the [crash-safety-checklist.md](crash-safety-checklist.md) before deployment.

---

## Gotchas & TL;DR

* **GC Allocation Bloat:** Never use `MyAPIGateway.Entities.GetEntities()` in update loops. It instantiates fresh lists on every call. Use `MyGamePruningStructure` instead.
* **TopMost Only:** Sub-objects like blocks inside a grid are not returned by `GetTopMostEntities()`. You must cast to `IMyCubeGrid` and then query the grid for specific blocks.
* **No Interface Analysis:** Analyzing an interface method (e.g., `IMyCubeBlock.Update`) inside a decompiler will not show direct calls to classes that implement it. You must analyze the interface's actual implementation classes.
* **No Enum Usages:** You cannot analyze enum values directly. To see where they are used, export the decompiled DLL as C# files (`File -> Export to project`) and perform a "Find in Files" search.

---

**Source:**
* https://spaceengineers.wiki.gg/wiki/Modding/Reference/Programming/Find_objects_in_an_area
* https://spaceengineers.wiki.gg/wiki/Modding/Tutorials/Programming/Exploring_the_game_code
