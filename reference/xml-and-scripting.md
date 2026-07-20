# Reference: XML Coding & Scripting for SE Mods

Source: spaceengineers.wiki.gg Modding reference + verified search. Archived:
2026-07-20.

---

# Part A — Coding in XML (SBC files)

**SBC files ARE XML.** They're the data-definition layer of every mod.

## Anatomy of an .sbc file

```xml
<?xml version="1.0" encoding="utf-8"?>
<Definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
             xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <CubeBlocks>                          <!-- definition-group wrapper -->
    <Definition xsi:type="MyObjectBuilder_CubeBlockDefinition">
      <Id>
        <TypeId>CubeBlock</TypeId>       <!-- the class of thing -->
        <SubtypeId>MyBlock</SubtypeId>   <!-- unique instance name -->
      </Id>
      <!-- ...fields... -->
    </Definition>
  </CubeBlocks>
</Definitions>
```

Layers:
1. **XML declaration** — `<?xml version="1.0" encoding="utf-8"?>` (UTF-8, **no BOM**).
2. **`<Definitions>` root** — MUST carry the two namespaces `xmlns:xsi` and
   `xmlns:xsd`.
3. **Definition-group wrapper** — `<CubeBlocks>`, `<Components>`, `<Blueprints>`,
   `<PhysicalItems>`, `<TransparentMaterials>`, etc.
4. **Individual definitions** — each with an `<Id>`.

## Id: TypeId + SubtypeId

- **TypeId** = the category/class (e.g. `CubeBlock`, `Component`, `Ore`).
- **SubtypeId** = the unique name of this specific thing. **Casing matters** and it
  must be unique within its type.
- References between definitions are **by SubtypeId string**.

## xsi:type polymorphism

Many definitions need an **`xsi:type`** on the `<Definition>` element to tell the
game which concrete class to deserialize into, e.g.
`xsi:type="MyObjectBuilder_CargoContainerDefinition"`. A plain `CubeBlock` uses
`MyObjectBuilder_CubeBlockDefinition`; specialized blocks (thrusters, cockpits,
cargo, reactors…) each have their own. Wrong/missing `xsi:type` → the definition
fails to load or loses its specialized fields. The full mapping is in
`cubeblocks-reference.md`.

## Editing existing (vanilla) XML — the right way

1. **Copy the vanilla `.sbc`** from `...\SpaceEngineers\Content\Data\` — don't
   author from scratch.
2. **Keep only the entries you override.** Delete the rest so you don't
   accidentally clobber unrelated definitions.
3. **`.sbc` extension must be lowercase** or the game silently skips the file.
4. Preserve the exact `<Id>` (TypeId + SubtypeId) of the vanilla thing you're
   overriding — that's how the game knows to replace it.

### Merge / override behavior

- Matching an existing **TypeId + SubtypeId** → your definition **replaces** the
  vanilla one (full replace of that definition).
- A **new SubtypeId** → **adds** a new thing alongside vanilla.
- Some list-type data is additive; when unsure, test in-game with logging on.

## Referencing between definitions

- A **block** references **Component** SubtypeIds in `<Components>` (what it's
  built from).
- A **Component** references a **PhysicalItem**.
- A **Blueprint** references its result item(s)/component(s) by SubtypeId.
- **Models & icons** are referenced by **relative path** from the mod root
  (`Models\Foo.mwm`, `Textures\Icons\Foo.dds`).

## Common XML mistakes → fixes

| Mistake | Symptom | Fix |
|---|---|---|
| Missing closing tag / malformed | mod won't load | validate (below) |
| Wrong SubtypeId **casing** | reference not found | match exact casing |
| **UTF-8 BOM** in file | parse error | save UTF-8 **without** BOM |
| Missing/wrong **xsi:type** | fields ignored / load fail | add correct type |
| `.sbc` **uppercase** extension | file skipped silently | rename lowercase |
| Editing Workshop original | overwritten on update | copy-then-edit |

## Tooling & validation

- Editor: **VS Code** or **Notepad++** with XML support (bracket matching, schema).
- **This skill's tool:**
  ```
  python3 scripts/sbc_tool.py validate <file.sbc|.xml>   # well-formed + SE checks
  python3 scripts/sbc_tool.py inspect  <file.sbc>        # list TypeId/SubtypeId
  ```
- Sanity-check by loading the mod in-game with **logging enabled** and reading the
  log for definition errors.

---

# Part B — Scripting for mods

SE has **two distinct** C# scripting types. Don't confuse them.

## 1. In-game Programmable Block (PB) scripts

- Written by players inside the **Programmable Block** terminal, C#, heavily
  **sandboxed** (whitelist).
- Structure: a `Program` class with a **constructor**, optional **`Save()`**, and a
  **`Main(string argument, UpdateType updateSource)`** entry point.
- Access the grid through **`IMyGridTerminalSystem`** (`GridTerminalSystem`),
  `Me`, `Runtime`, etc.
- Lives in the world/blueprint, not in a mod's file tree.

## 2. Mod scripts / plugins (game-logic)

- C# `.cs` files shipped **inside the mod** at:
  ```
  <Mod>\Data\Scripts\...\*.cs
  ```
- **Compiled at world load.** Errors there can silently disable the mod — check
  the log.
- Use the **modding API** and run under the **whitelist/sandbox** (many .NET APIs
  are blocked; violations fail compilation).
- Core base types:
  - **`MySessionComponentBase`** — session-wide logic (init, update loop).
  - **`MyGameLogicComponent`** — per-entity/block behavior, attached via attribute.

### How a script attaches to a block

A game-logic component is bound to a block by attribute, matching the block's
`TypeId`/`SubtypeId` from `CubeBlocks.sbc`:

```csharp
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.Game;

namespace MyMod
{
    [MyEntityComponentDescriptor(
        typeof(MyObjectBuilder_CargoContainer),   // block base type
        useEntityUpdate: false,
        "MyCustomCargo")]                          // SubtypeId(s) it applies to
    public class MyBlockLogic : MyGameLogicComponent
    {
        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            // per-frame block logic here
        }
    }
}
```

So: **XML defines the block; the script extends its behavior.** The
`[MyEntityComponentDescriptor]` SubtypeId(s) tie the C# to the SBC definition.

### Session component skeleton

```csharp
using VRage.Game.Components;

namespace MyMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class MyModSession : MySessionComponentBase
    {
        public override void UpdateBeforeSimulation() { /* world logic */ }
    }
}
```

## Scripting pitfalls

- **Whitelist violations** — using a blocked API stops compilation; the mod's
  scripts silently don't run. Check the log.
- **Server safety** — guard client-only calls; run authoritative logic on the
  server to stay multiplayer-safe.
- **Silent failures** — a single script error can disable all scripts in the mod;
  always test with logging.
- **Don't rely on PB-script patterns in mod scripts** — different APIs and lifecycle.

> Note: exact API type names/attributes can shift across SE versions. Verify against
> the current modding API and the game log when in doubt.
