# Mod Folder Structure & Publishing Metadata

Source: sub-agent research + wiki.gg. Archived: 2026-07-17

## Folder layout (VRage loader requirements)

```
MyCustomMod/                           # ← MOD ROOT (goes in %AppData%\SpaceEngineers\Mods\)
├── Data/                              # all .sbc (lowercase!) definition files
│   ├── CubeBlocks.sbc                 # block stats, components, mountpoints
│   ├── TransparentMaterials.sbc       # glass/shield materials
│   ├── Blueprints.sbc                 # crafting recipes
│   ├── Scripts/                       # C# game-logic (MyGameLogicComponent, etc.)
│   │   └── MyMod/
│   │       └── MyLogic.cs
│   └── ...                            # Components, PhysicalItems, etc.
├── Models/                            # compiled .mwm 3D assets
│   └── Cubes/
│       ├── large/                     # large-grid models
│       │   ├── MyBlock.mwm
│       │   ├── MyBlock_BS1.mwm        # build-stage models
│       │   ├── MyBlock_BS2.mwm
│       │   └── MyBlock_BS3.mwm
│       └── small/                     # small-grid models (if any)
├── Textures/                          # DDS textures
│   ├── GUI/
│   │   └── Icons/
│   │       └── MyBlock.dds            # block icon (128×128 BC7)
│   └── Models/
│       └── Cubes/                     # ⭐ CUSTOM BLOCK/MATERIAL TEXTURES GO HERE
│           ├── MyMaterial_cm.dds      # Color + Metalness
│           ├── MyMaterial_ng.dds      # Normal + Gloss
│           └── MyMaterial_add.dds     # AO(R) / Emissive(G) / Paint mask
├── metadata.mod                       # optional platform tags (PC/Xbox)
├── modinfo.sbmi                       # auto-managed workshop metadata
└── thumb.png                          # workshop thumbnail (~640×480)
```

Key rules:
- The game loads **all** `.sbc` from `Data/` and every subfolder; filenames are
  arbitrary but the `.sbc` extension **must be lowercase**.
- Copy vanilla `.sbc` from the game's `Content\Data\` folder and edit; delete
  entries you don't want to override.
- Custom C# scripts live under `Data/Scripts/<AnyFolder>/` and are compiled by
  the game on load (check F11×2 for script errors).

## ⭐ Where custom textures go — and why a material renders PITCH BLACK

This is the #1 cause of a custom material that **looks right in Blender but is
pure black in-game.**

**The rule:** when SEUT exports, it bakes a **texture path** into the model's
material (inside the `.mwm`). In-game SE looks for that DDS file **relative to
the mod root**. If the file isn't at that exact path inside your mod → the
material renders **black** (no texture found). Vanilla materials "just work"
because SE already ships their DDS; your custom ones only exist if you include
them.

- Custom model/material textures go in **`Textures\Models\Cubes\`** — the same
  path vanilla materials use (e.g. `Textures\Models\Cubes\BlackMetal_cm.dds`).
- The DDS filename + folder must **match the path SEUT wrote into the `.mwm`**.
  Read the baked path by running `strings` on the `.mwm` and looking for
  `ColorMetalTexture`, `NormalGlossTexture`, `AddMapsTexture` lines.
- **Watch for absolute paths.** If SEUT baked `C:\Users\...\MyMat_cm.dds`
  instead of `Textures\Models\Cubes\MyMat_cm.dds`, it will be black for everyone
  (and break the moment the file moves). Fix: set SEUT's project/mod folder so it
  writes **mod-relative** paths, then re-export.
- Assign textures in Blender through the **SEUT shader node group**, not a loose
  Image Texture node — SE ignores the raw Blender node tree and only reads the
  SEUT paths. A material set up with a plain Image Texture node exports with
  **no texture path at all** → black. See `../how-to/create-custom-material.md`.

**Texture standards (all model + icon DDS):** 2048×2048 (icons 128×128),
power-of-two, **BC7** compression, **mipmaps generated**. In the `_add` map keep
the **red channel (AO) white** or the surface renders dark/flat; the green
channel is the emissive mask.

**Diagnose black textures fast:**
```bash
# What texture paths did SEUT actually bake into the model?
strings -n 6 MyBlock.mwm | grep -iE "ColorMetalTexture|NormalGlossTexture|AddMapsTexture"
```
If your custom material's name/path is **missing** from that list, the material
was exported with no texture → set it up via the SEUT node group and re-export.
If the path is present but points somewhere the DDS isn't, move the DDS to match.

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
