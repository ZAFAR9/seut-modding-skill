# How-To: Edit a Workshop Mod (Copy → Local → Test → Modify)

The safe workflow for taking an existing Workshop mod and making your own edited
version, without Steam overwriting your work.

## Where mod files live

**Downloaded Workshop mods** (keyed by SE app ID `244850`):
```
...\Steam\steamapps\workshop\content\244850\<workshopId>\
```
Each `<workshopId>` subfolder is one mod with its `Data/`, `Models/`, `Textures/`.

**Your own local mods** (game auto-detects these in the mod list):
```
%AppData%\SpaceEngineers\Mods\
```

## The workflow

1. On the Workshop page → **Share** → copy the number at the end of the URL. That
   number is the `<workshopId>`.
2. Open the folder:
   `...\Steam\steamapps\workshop\content\244850\<workshopId>\`
3. In File Explorer, create your own **custom mods folder**, and inside it a
   **new subfolder** named for your mod.
4. **Copy everything** from the Workshop mod into your custom mod folder.
5. Place the custom mod under `%AppData%\SpaceEngineers\Mods\` so it appears as a
   **local mod**.
6. Load it as a local mod in a **private/offline world** to test.
7. Equip it on the offline world, then **modify the XML** (`.sbc` files) and reload
   to see changes.

## Rules while editing

- **Never edit the Workshop original** — Steam overwrites it on update. Always
  copy-then-edit.
- Keep the **`.sbc` extension lowercase** or the game silently skips the file.
- Remove definition entries you don't intend to override.
- If a Workshop mod shows no loose `.sbc`/`.mwm`/`.dds`, it may be packed in a
  `.sbm` archive — unpack it (or reference the vanilla source) first.

## Packed vs loose

Vanilla source to copy from lives in the game install:
```
...\SpaceEngineers\Content\Data\
```
Copying vanilla `.sbc` and trimming to just your overrides is the recommended way
to start any definition edit.
