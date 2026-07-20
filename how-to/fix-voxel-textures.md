# How-To: Fix Voxel Textures (Magenta / Flat / Black)

The single most common SEUT/Blender texture problem and how to resolve it end to end.

## Symptom → cause map

| What you see | Real cause |
|---|---|
| **Magenta / hot-pink faces** in Blender | Texture file **missing / unlinked** (Blender's "no image" fallback) |
| **Lavender/periwinkle** flat image in GIMP | This is **correct** — it's a normal map (`_ng`), not a problem |
| **Flat / washed-out / dark** in-game | Wrong format, wrong sRGB flag, or **ADD map red (AO) channel is black** |
| **Black** voxels in-game | Missing ADD map or AO=black |
| **Crash** loading a planet | More than **8 custom `ComplexMaterials`** loaded at once |

## Key mental model

**Voxel materials are NOT a Blender/SEUT export.** SEUT is for *models* (blocks).
Voxels never go through the `.mwm` pipeline and SE **ignores the Blender node
tree** for them. The game only reads:
- the `.dds` texture files, and
- the parameters in `VoxelMaterials.sbc`.

So a voxel fix is **always** in the textures + sbc — never in the Blender shader.

## The magenta case (missing linked textures)

Blender shows magenta when a material points at an image path that doesn't exist.
A linked material library (e.g. a `Voxels.blend`) stores **material node setups +
cached thumbnails inside the .blend**, but the **full-res textures are referenced
by absolute path** and are NOT packed in. If that path/folder is missing, the
Asset Browser thumbnails still render (they're cached) but applying the material
falls back to magenta.

Confirm which files are missing:
1. **File → External Data → Report Missing Files** — lists every unresolved path.
2. Read the paths. If they point at `.tif` source files you don't have but you DO
   have the `.dds`, you don't need the TIFs — the game only uses DDS.

Fix, in priority order (per standing instruction — **prefer relinking existing
.dds over hunting for missing .tif**):
1. **Relink to your existing `.dds`:** in the Shader Editor, click each Image
   Texture node → open the image dropdown → browse to your `.dds`. Blender reads
   DDS fine for display; magenta resolves immediately.
2. Or **File → External Data → Find Missing Files** → point at the folder holding
   your textures; Blender auto-relinks by filename.
3. Only if you genuinely need editable source and have neither DDS nor TIF do you
   re-source the texture pack.

## The in-game "flat/black" case

Verify **in this order** (standing rule):
1. **Resolution 2048×2048** (power of two).
2. **Format BC7 DDS.**
3. **Channel packing** — especially the **ADD map red channel = white** when the
   material has no AO. A black/missing AO renders voxels flat and dark.

Reconvert with the correct per-channel `texconv` commands (see
`../reference/materials-reference.md`). Using the wrong sRGB flag (e.g. CM without
`-sRGB`) causes the washed-out look.

## The crash case

Split custom voxel materials so a single planet `.sbc` never loads more than
**8 custom `ComplexMaterials`** simultaneously.
