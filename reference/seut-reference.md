# Space Engineers Utilities (SEUT) Reference

Sources: wiki.gg SEUT pages (main, Main Panel, Shader Editor, Material tutorial)
+ sub-agent deep research. Archived: 2026-07-17

SEUT is a Blender **4.0+** addon by **enenra** that exports models to the SE
**MWM** format and provides tooling to build them. GitHub:
github.com/enenra/space-engineers-utilities

## Features (summary)

- **Blender:** collection-based organization, Simple Navigation, multi-scene per
  BLEND, grid-scale presets, update notifications, QuickTools, robust error
  handling + log export.
- **Modes:** Bounding Box, Mirroring, Mountpoint, Icon Render.
- **Import:** SE FBX with auto-materials, structure conversion from old 2.7x
  plugin, material-library import.
- **Materials:** displays vanilla materials, Material Libraries via Asset
  Browser, create your own materials/libraries, TransparentMaterials support,
  DDS texture conversion.
- **Empties:** subparts instanced into scenes, managed via lists.
- **Export:** LOD distances, simultaneous large+small grid, direct MWM export,
  CubeBlocks SBC generation, full character model/pose/animation support.

## Installation & requirements

1. **Blender 4.0+**.
2. **Space Engineers ModSDK** — Steam Library → Tools → install.
3. **SEUT addon** — install the `.zip` via Edit → Preferences → Add-ons → Install.
4. **Havok Content Tools 2013 (x64)** — required for `.hkt` collision.
5. **Microsoft .NET Framework 3.5 SP1** and **Visual C++ Redistributable 2012 (x86)**.

Configure in SEUT Addon Preferences:
- **MWM Builder path:** `...\SpaceEngineersModSDK\Tools\MwmBuilder\MwmBuilder.exe`
- **Havok Standalone Filter Manager:** `...\Havok Content Tools\hctFilterManager.exe`
  (ensure `hctFilterManager.dll` is present too).

Current downloads (as of archive): SEUT 1.2.1 (Blender 4.5.0); dev 1.2.2-alpha.2 (5.1+).

> ⚠️ **Blender 5.x Icon Render bug:** on Blender 5.x, SEUT's **Icon Render** crashes
> with `'Scene' object has no attribute 'node_tree'` (5.x removed `scene.node_tree`),
> saving a **blank/transparent** icon. Use **Blender 4.2 LTS / 4.3** for icon
> rendering, or make the icon manually — see
> [how-to/blender-seut-concepts → Icon Render broken on Blender 5.x](../how-to/blender-seut-concepts.md#-icon-render-broken-on-blender-5x-manual-icon-workaround).

## Scene & collection structure

SEUT uses Blender **collections** to recognize assets:

```
[Scene Collection]
└── Main                (final built model)
    ├── LOD1 / LOD2 / LOD3   (levels of detail, gapless indexing)
    ├── BS1 / BS2 / BS3      (build stages; BS1 = earliest frame)
    │   └── BS_LOD1 ...      (LODs per build stage)
    ├── Collision           (convex collision meshes)
    └── Mountpoints         (visualized attach boundaries)
```

- Create with **Recreate Collections** / **Create Collection** in the Main Panel.
- Indexing must be gapless (no LOD2 without LOD1) — else error **E006**.

## SEUT Main Panel (Viewport N-panel → SEUT tab)

- **Scene / Type:** scene type = Main / Subpart / Character / Character Animation
  / Planet Editor. Only Main scenes write an SBC.
- **SubtypeId:** unique model identifier; written to SBC + filename; must be
  unique per BLEND.
- **Grid Scale:** aligns Blender grid to large/small SE grid (set to half size);
  affects SBC output and bounding box scale.
- **Update/Unlink Subparts, Paint Color, UV Grid Overlay, SEUT Notifications
  (turns red on error), Simple Navigation** (auto-hide inactive collections).
- **Quick Tools** (enable in Addon Prefs): Add Bevels, Face Orientation, Origin
  to Geometry, Mirror And Apply, Flip Normals, Recalculate Outside, Apply
  Transformations (collision rigid bodies), Apply 'Construction', Cut and
  Solidify (build stages), Remove Bevels, LOD Distance View.
- **Bounding Box:** X/Y/Z size (written to SBC) + color. The entire collision
  model must sit inside the bounding box or the block reverts to cube collision.
- **Mirroring Mode:** sets up in-game block mirroring (relies on materials for
  display); writes to SBC.
- **Mountpoint Mode:** define attach areas per Side; Enabled / Default /
  Pressurized / Mask / Add Area; compiled into SBC coordinates on export.
- **Icon Render Mode:** render vanilla-style block icons (Zoom, Location,
  Rotation, Color Overlay, Resolution, File Format, Output Path).
- **Export:** Export All / Current Scene, Export Materials, Delete Temporary
  Files, Convert Textures; toggles SBC / Large / Small; Mod + Export Folder.
- **Import:** Import FBX, Complete Import, Better FBX, Fix Scratched Materials,
  Remap Materials, Convert to New Structure, Make Blender/SE compatible.

## SEUT Shader Editor panel (Shader Editor N-panel)

- **SEUT Material:** shows active material name + preview (greyed out if the
  material is linked/not local).
- **Technique / Facing:** dropdowns (see materials-reference.md).
- **Create Material:** new SE-ready material on selected object.
- **Import Materials:** import a Material Library XML.
- **Texture Conversion:** convert to/from DDS. Presets: Icon, Color Metal,
  Add Maps, Normal Gloss, Alphamask, TIF, Custom. File/Folder, Input, Output,
  Options (Custom only), Convert Texture(s).

## Material creation workflow

1. **Setup textures:** create → export to DDS → place in mod's
   `Textures\Models\...`.
2. **Setup material:** Create Blender Material (SEUT) → link texture files →
   define additional parameters → set image options.
3. On export, SEUT checks the mod folder for textures; if TIFs are newer than
   DDS (or DDS missing), it converts TIF→DDS into the right folder. Mark
   vanilla-only materials as "vanilla" on creation.
4. Reused materials → set up a **Material Library (MatLib)**.

## Export pipeline (under the hood)

```
Blender mesh + materials
        │  (SEUT Export)
   ┌────┴─────────┬──────────────┐
 .FBX (geo)   .XML (material/LOD)  .HKT (Havok collision)
   └────┬─────────┴──────────────┘
        │ MwmBuilder.exe
    .MWM  (final SE model in mod folder)
```

## Collision rules (Havok)

- Collision meshes **must be convex**; concave shapes get shrink-wrapped and
  break. Build concave shapes from multiple convex pieces.
- Can link an external `.hkt` instead of meshes — but having both = **E055**.
- Use QuickTools → **Apply Transformations** for collision rigid bodies.

## Troubleshooting

- Check the **SEUT Notifications** screen first (filter by type; errors link to
  solutions). No relevant entry → **Export Logs** and inspect.
- `Traceback` in logs or unknown issue → search GitHub issues; open a new one
  with full logs if needed.

Common errors:
| Code | Cause | Fix |
|---|---|---|
| E006 | LOD/BS collections out of order or gap in indexing | make indexes sequential from 1 |
| E037 | `hctFilterManager.dll/.exe` not found | reinstall HCT; fix path in Addon Prefs |
| E055 | both collision meshes AND an external `.hkt` defined | use one or the other |
| E053 | shader node tree edited beyond supported params | keep to SEUT-supported inputs |

## Credits
enenra (main dev), Stollie (character/export/MWM), Harag (original SE Blender
plugin), Wizard Lizard (SE Texture Converter), Kamikaze (remapMaterials), +many.
