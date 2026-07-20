<!-- Extracted from advanced/definition-extension-api/ source (Workshop 2756894170, Draygo), verified against files. 2026-07-20. -->

# Definition Extension API — Custom Typed Attributes on Definitions

**Workshop:** `2756894170` · **Author:** Draygo · **Namespace:** `Draygo.BlockExtensionsAPI`

Lets modders attach **strongly-typed custom attributes** (bool, long, double,
string, color, vectors) to game definitions (CubeBlocks, Components, PhysicalItems,
PlanetGenerators, generic Definitions) via clean XML, and read them back at runtime
with **allocation-free** lookups. It replaces brittle description-tag / CustomData
string-parsing hacks. It also supports dynamic `MyGameLogicComponent` attachment
and multiplayer-synced TSS data.

## Why

- **Data separated from description** — structured XML, not parsed text.
- **Strong typing** — parsed straight into C# types.
- **Fast** — parsed once at `LoadData` into dictionaries keyed by precomputed
  `MyStringId`; runtime `TryGet…` is allocation-free.
- **Bonus features** — dynamic multi-game-logic attachment (bypasses the engine's
  one-game-logic limit), and TSS data sync across client/server.

## Architecture / data flow

```
Data/DefinitionExtensions.txt           → lists your XML files (e.g. MyExtensions.xml)
        │
        ▼
DefinitionExtensionsAPICore : MySessionComponentBase
  1. loops all active mods
  2. loads XML, pre-strips xsi:type (avoids serializer crash)
  3. deserializes into attribute objects
  4. imports into DataRoot / PropertiesGroup dictionaries
        │  publishes API via mod message  MODID 2756894170
        ▼
DefinitionExtensionsAPI  (copied into the consuming mod)
  → receives delegate dictionary → allocation-free TryGet queries
```

### Attribute hierarchy
All attributes extend `BaseAttribute` (just an XML `Name`):
```csharp
public class BaseAttribute { [XmlAttribute] public string Name; }
public class ColorAttribute : BaseAttribute { [XmlAttribute] public byte R,G,B,A; }
public class Vector3DAttribute : BaseAttribute { [XmlAttribute] public double X,Y,Z; }
// + Boolean/Long/Decimal/String/Vector2I/Vector2D/Vector3I
```

### In-memory storage
`DataRoot.Groups` = `Dictionary<MyDefinitionId, Dictionary<MyStringId, PropertiesGroup>>`.
`PropertiesGroup` holds per-type dicts (`Text`/`Integer`/`Boolean`/`Decimal`/`Color`/
`Vector*`), all keyed by `MyStringId`.

### XML pre-strip (key trick)
`DefinitionExtensionsAPICore.cs` rewrites `<Definition xsi:type="…">` → `<Definition>`
line-by-line before `SerializeFromXML`, so vanilla SBC schemas don't crash the
custom deserializer.

## Usage

**`Data/DefinitionExtensions.txt`**
```
MyBlockExtensions.xml
```

**`Data/MyBlockExtensions.xml`** — attach grouped attributes to a block:
```xml
<Definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <CubeBlocks>
    <Definition xsi:type="MyObjectBuilder_CubeBlockDefinition">
      <Id><TypeId>OxygenGenerator</TypeId><SubtypeId>HeavyIndustrialO2Gen</SubtypeId></Id>
      <ModExtensions>
        <Group Name="ShieldSettings">
          <Boolean Name="ShieldEnabled" Value="true" />
          <Decimal Name="ShieldCapacity" Value="15000.50" />
          <Integer Name="RechargeRate" Value="350" />
          <Color   Name="ImpactShieldColor" R="0" G="192" B="255" A="180" />
        </Group>
        <Group Name="VisualEffects">
          <Vector3D Name="EmitterOffset" X="0.5" Y="1.2" Z="-0.8" />
          <Text     Name="EffectName" Value="ArcWelderSparks" />
        </Group>
      </ModExtensions>
    </Definition>
  </CubeBlocks>
</Definitions>
```
`Group` gives you namespaces so multiple mod systems don't collide.

**Consuming mod (C#)** — copy `DefinitionExtensionsAPI.cs` in, init early, cache
`MyStringId`s, query:
```csharp
private DefinitionExtensionsAPI _api;
static readonly MyStringId GrpShield = MyStringId.GetOrCompute("ShieldSettings");
static readonly MyStringId PropCap   = MyStringId.GetOrCompute("ShieldCapacity");

public override void LoadData() { _api = new DefinitionExtensionsAPI(OnReady); }
protected override void UnloadData() { _api?.UnloadData(); _api = null; }

void ReadSettings(MyDefinitionId def) {
    if (_api == null || !_api.DefinitionIdExists(def)) return;
    float cap;
    if (_api.TryGetFloat(def, GrpShield, PropCap, out cap)) { /* use cap */ }
}
```

## Attribute cheat-sheet

| XML element | Class | Core type | Retrieval | Format |
|---|---|---|---|---|
| `<Text>` | StringAttribute | string | `TryGetText`/`TryGetString` | `Value="…"` |
| `<Integer>` | LongAttribute | long | `TryGetInt`/long | `Value="123"` |
| `<Boolean>` | BooleanAttribute | bool | `TryGetBool` | `Value="true"` |
| `<Decimal>` | DecimalAttribute | double | `TryGetFloat`/`TryGetDouble` | `Value="1.5"` |
| `<Color>` | ColorAttribute | Color | `TryGetColor` | `R= G= B= A=` |
| `<Vector2I>` | Vector2IAttribute | Vector2I | `TryGetVector2I` | `X= Y=` (int) |
| `<Vector2D>` | Vector2DAttribute | Vector2D | `TryGetVector2D` | `X= Y=` (dbl) |
| `<Vector3I>` | Vector3IAttribute | Vector3I | `TryGetVector3I` | `X= Y= Z=` (int) |
| `<Vector3D>` | Vector3DAttribute | Vector3D | `TryGetVector3D` | `X= Y= Z=` (dbl) |

## Notes / gotchas

- Manifest is fixed: **`Data/DefinitionExtensions.txt`** lists the XML files.
- Consuming mods must **copy `DefinitionExtensionsAPI.cs`** and init in `LoadData`,
  then wait for the ready callback before querying.
- Cache `MyStringId.GetOrCompute(...)` at static/field scope — don't recompute in
  tick loops (that reintroduces allocations the API is designed to avoid).
- Core API returns `MyTuple<bool,T>` internally; guards check def→group→property
  existence before returning `true`.
