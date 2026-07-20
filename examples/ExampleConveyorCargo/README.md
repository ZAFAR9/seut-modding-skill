# Example Mod: ExampleConveyorCargo

A complete, minimal, **copy-from** reference mod — a 1×1×1 large-grid cargo
container with two conveyor ports (one large, one small), a custom material, and a
game-logic script. Every file is annotated so you can see exactly how the pieces
connect.

> This is a **reference/skeleton**, not a drop-in playable mod: the binary assets
> (`.mwm` model, `.dds` textures, `.hkt` collision) can't live in a text repo. The
> `.md` notes next to each placeholder tell you what to produce in Blender/GIMP and
> where it goes. Everything text-based (the `.sbc`, material `.xml`, `.cs`,
> `modinfo`) is real and validated.

## How the pieces connect

```
CubeBlocks.sbc  ──<Model>──►  Models/ExampleConveyorCargo.mwm  (built by SEUT)
      │                              │
      │                              ├─ material "ExampleCargo_Colorable"
      │                              │     └─► ExampleConveyorCargo.xml (texture paths)
      │                              │            └─► Textures/…_cm/_ng/_add.dds
      │                              └─ detector_conveyor* dummies  ─► conveyor ports
      │
      └──<SubtypeId> "ExampleConveyorCargo"
             └─► Data/Scripts/…  [MyEntityComponentDescriptor(..., "ExampleConveyorCargo")]
                    └─► game-logic script attaches to this block
```

## Folder layout

```
ExampleConveyorCargo/
├─ modinfo.sbmi                         # publishing metadata
├─ Data/
│  └─ CubeBlocks.sbc                    # the block definition (REAL, validated)
├─ Data/Scripts/ExampleConveyorCargo/
│  └─ ExampleCargoLogic.cs              # game-logic component (REAL)
├─ Models/
│  ├─ ExampleConveyorCargo.mwm.md       # what to export from Blender/SEUT
│  └─ material/ExampleConveyorCargo.xml # model material XML (REAL, validated)
└─ Textures/
   ├─ Models/ExampleCargo_Colorable_*.md  # what textures to make + texconv cmds
   └─ Icons/ExampleConveyorCargo_Icon.md  # icon spec
```

## Build order

1. **Model** it in Blender (see `Models/ExampleConveyorCargo.mwm.md`) with two
   `detector_conveyor*` dummies + collision + mount points → SEUT export → `.mwm`.
2. **Textures** (see `Textures/Models/…md`) → convert to BC7 DDS with texconv.
3. **Definition** — `Data/CubeBlocks.sbc` is ready; adjust as needed.
4. **Script** — `Data/Scripts/…/ExampleCargoLogic.cs` attaches by SubtypeId.
5. **Validate:** `python3 ../../scripts/sbc_tool.py validate Data/CubeBlocks.sbc`
6. **Test** as a local mod in an offline world (see `../../how-to/edit-workshop-mods.md`).
