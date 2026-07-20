# Example: Black Hole Container (infinite storage)

A worked example of a **single cargo block with effectively-infinite storage and no
stack limits** — the "black hole" container. It's the reference for the one thing SEUT
*can't* give you: the **game-logic C# script** that lifts the vanilla inventory caps.

> The **model** side (Main / collision / BS1–3 / mount points → `.mwm`) is made in
> Blender+SEUT. This folder is the **definition + script** side. Drop your exported
> `.mwm` files into `Models\` and you have a complete mod.

## What's here

```
BlackHoleContainer/
├── Data/
│   ├── CubeBlocks.sbc                         ← the block definition (validated)
│   └── Scripts/BlackHoleContainer/
│       └── BlackHoleContainerLogic.cs         ← the "infinite" game-logic script
├── modinfo.sbmi                               ← publishing metadata
└── Models/                                    ← YOUR exported .mwm go here (not included)
```

## How the two halves connect

| Half | Made by | Produces |
|---|---|---|
| **Model** | Blender + **SEUT Export** | `Models\BlackHoleContainer.mwm` (+ `_BS1/2/3`) |
| **Definition + logic** | this folder (hand-authored) | `CubeBlocks.sbc` + the `.cs` script |

SEUT does **not** create/update your `.sbc` — its Export only builds the `.mwm`. The
`.sbc` is a plain text file; that's what this example provides.

## The definition (`CubeBlocks.sbc`)

- `xsi:type="MyObjectBuilder_CargoContainerDefinition"`, TypeId `CargoContainer`,
  **SubtypeId `BlackHoleContainer`** — this SubtypeId must match your Blender scene
  name / `.mwm` name **and** the `[...]` in the script attribute.
- `<Model>` / `<BuildProgressModels>` point at your SEUT exports.
- Mount points on **all six faces** (freely placeable cube).
- A large `<InventorySize>` (1000³) as a **fallback** — if the script fails to attach
  for any reason, the block still has huge (if finite) storage.
- Icon at `Textures\GUI\Icons\BlackHoleContainer.dds` (make it with SEUT Icon Render,
  or the manual workaround — see
  [how-to/blender-seut-concepts](../../how-to/blender-seut-concepts.md#-icon-render-broken-on-blender-5x-manual-icon-workaround)).

## The script (`BlackHoleContainerLogic.cs`)

A `MyGameLogicComponent` bound by the attribute:

```csharp
[MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "BlackHoleContainer")]
```

- The `false, "BlackHoleContainer"` means **exact-subtype match** — it attaches ONLY to
  your block, never to vanilla cargo containers.
- On the first frame it grabs the block's `MyInventory` and:
  1. `FixInventoryVolume(1e9)` — gigantic max volume (finite, to avoid UI NaN).
  2. `SetFlags(CanReceive | CanSend)` — normal I/O, no count cap.
  3. `Constraint = null` — accept **any** item type.
  4. `Refresh()` — push the new limits to the UI.

Why "1e9 m³" and not literally infinite: `float.MaxValue` can produce NaN/overflow in
the cargo-fill bar. A billion cubic metres is unfillable in practice.

## Install & test

1. Export your model in Blender → SEUT, put the `.mwm`(s) in `Models\`.
2. Copy this whole folder into `%AppData%\SpaceEngineers\Mods\BlackHoleContainer\`.
3. Enable it as a **local mod** in an offline world.
4. Place the block, open its inventory — capacity should read effectively unlimited,
   and stacks shouldn't split.

## ⚠️ Caveats (read before shipping)

- **Whitelist:** SE game-logic scripts run under a modding whitelist. If a member used
  here (e.g. an inventory method) is blocked in your game version, the script will fail
  to compile — check the **F11 / script errors** and adjust. The definition's big
  `<InventorySize>` keeps the block usable even then.
- **Multiplayer/perf:** truly enormous inventories can strain sync and the UI on
  servers. For MP, consider a smaller (but still large) cap.
- **"Infinite stacking":** vanilla stacks are effectively unbounded already for most
  items once the volume cap is removed; if you want *component* items to ignore their
  per-stack behaviour, that's governed by item defs, not the container.
