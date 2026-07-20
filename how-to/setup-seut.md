# How-To: Set Up SEUT + a Mod Project

Getting from zero to a working Blender→Space Engineers pipeline.

## What SEUT is (and isn't)

**SEUT = Space Engineers Utilities**, a Blender add-on (Blender 4.5+ era) that
turns Blender models into SE-ready `.mwm` block models and generates the material
`.xml`. It handles **models/blocks** — **not** voxel materials (those are DDS +
`VoxelMaterials.sbc` only) and not scripting.

## Install / requirements

1. Install a compatible **Blender** version (check the SEUT release notes for the
   exact supported range).
2. Download SEUT from the official source (github.com/enenra/space-engineers-utilities).
3. In Blender: **Edit → Preferences → Add-ons → Install…** → pick the SEUT zip →
   enable it.
4. In SEUT preferences set your paths:
   - **Space Engineers install / Content** folder (for `mwmbuilder`, vanilla
     assets, and the material libraries).
   - **mod output** folder.
5. SEUT ships **material libraries** with most vanilla materials so you can preview
   them in Blender.

## Create a mod project in SEUT

1. Use SEUT's **Create Mod / project** workflow to scaffold the standard folder
   layout (see `../reference/mod-structure.md`).
2. SEUT sets up the **collections** it needs: Main, LODs, Build Stages, Collision,
   Mountpoints (see `../reference/modeling-reference.md`).
3. Model in the Main collection, add LOD/BS/Collision as needed, assign materials
   via the SEUT Shader Editor, place mount points with the Mountpoint tool.
4. **Export** via the SEUT panel → SEUT runs `mwmbuilder` → produces `.mwm` in your
   mod's `Models/` folder.
5. Reference that `.mwm` from your block in `CubeBlocks.sbc`.

## Test loop

- Put the mod under `%AppData%\SpaceEngineers\Mods\` (local mod).
- Load an offline/creative world, enable the mod, place the block.
- Iterate: edit `.sbc`/model → re-export → reload world.

## When something breaks

- **Magenta in Blender** → missing/unlinked texture. See
  `fix-voxel-textures.md` (same root cause applies to model materials).
- **Export errors** → check collection naming and collision; see
  `../reference/modeling-reference.md` error section.
- **Block missing in-game** → `.sbc` not lowercase, bad SubtypeId, or model path
  wrong. Validate with `python3 scripts/sbc_tool.py validate <file>`.
