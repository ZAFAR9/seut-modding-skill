# Mod Folder Structure & Publishing Metadata

Source: sub-agent research + wiki.gg. Archived: 2026-07-17

## Folder layout (VRage loader requirements)

```
MyCustomMod/
├── Data/                              # all .sbc (lowercase!) definition files
│   ├── CubeBlocks.sbc                 # block stats, components, mountpoints
│   ├── TransparentMaterials.sbc       # glass/shield materials
│   ├── Blueprints.sbc                 # crafting recipes
│   └── ...                            # Components, PhysicalItems, etc.
├── Models/                            # compiled .mwm 3D assets
│   └── Cues/
│       ├── MyCustomBlock.mwm
│       ├── MyCustomBlock_Large.mwm
│       ├── MyCustomBlock_BS1.mwm      # build-stage models
│       └── MyCustomBlock_BS2.mwm
├── Textures/                          # DDS textures
│   ├── Icons/
│   │   └── MyCustomBlock_Icon.dds
│   └── Models/
│       ├── MyCustomBlock_cm.dds       # Color + Metalness
│       ├── MyCustomBlock_ng.dds       # Normal + Gloss
│       └── MyCustomBlock_add.dds      # AO/Emissive/Paint mask
├── metadata.mod                       # optional platform tags (PC/Xbox)
└── modinfo.sbmi                       # auto-managed workshop metadata
```

Key rules:
- The game loads **all** `.sbc` from `Data/` and every subfolder; filenames are
  arbitrary but the `.sbc` extension **must be lowercase**.
- Copy vanilla `.sbc` from the game's `Content\Data\` folder and edit; delete
  entries you don't want to override.

## Accessing Workshop mod files

Downloaded Workshop mods live under Steam's install path, keyed by the SE app
ID `244850`:

```
...\Steam\steamapps\workshop\content\244850\<workshopId>\
```

Each `<workshopId>` subfolder is one mod and contains its `Data/`, `Models/`,
`Textures/` folders (the structure above).

Your own local (non-Workshop) mods live at:

```
%AppData%\SpaceEngineers\Mods\
```

Notes:
- If a Workshop mod shows no loose `.sbc`/`.mwm`/`.dds`, it may be packed in a
  `.sbm` archive — unpack it (or reference the vanilla source) before editing.
- **Never edit Workshop files directly** — Steam overwrites them on update.
  Copy what you need into your own mod folder and work there.

## Editing a Workshop mod (copy → local → test → modify)

1. On the Workshop page → **Share** → copy the number at the end of the URL.
   That number is the `<workshopId>`.
2. Find the folder:
   `...\Steam\steamapps\workshop\content\244850\<workshopId>\`
3. In File Explorer, create your own **custom mods folder**, and inside it a
   **new subfolder** named for your mod.
4. **Copy everything** from the Workshop mod into your custom mod folder.
5. Place the custom mod at `%AppData%\SpaceEngineers\Mods\` so the game
   auto-detects it as a **local mod**.
6. Load it as a local mod in a **private/offline world** to test.
7. Equip it on the offline world, then **modify the XML** (`.sbc` files) and
   reload to see changes.

Reminders while editing:
- Keep the `.sbc` extension **lowercase** or the game silently skips the file.
- Copy-then-edit — don't touch the Workshop original.
- Remove definition entries you don't intend to override.

## modinfo.sbmi (publishing metadata)

Auto-created when you publish via the in-game dev overlay to Steam Workshop /
Mod.io. Ties the mod to its owner + workshop entry.

```xml
<?xml version="1.0" encoding="utf-8"?>
<MyObjectBuilder_ModInfo xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                         xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <SteamIDOwner>76561198000000000</SteamIDOwner>
  <WorkshopId>1234567890</WorkshopId>
</MyObjectBuilder_ModInfo>
```

- `<SteamIDOwner>` — Steam64 ID of the publisher; stops others hijacking your
  workshop item.
- `<WorkshopId>` — the assigned workshop ID. Change/delete it and re-publishing
  creates a brand-new workshop page instead of updating.
