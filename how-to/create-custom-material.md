# How-To: Create a Custom Material

Making your own model material (not voxel) from textures to in-game.

## The 4 textures (packed maps)

A material is up to four DDS textures, each packing data per channel (black=0%,
white=100%). Save as **DDS / BC7**, dimensions powers of two.

| Map | RGB | Alpha |
|---|---|---|
| **`_cm`** Color/Metal | base color | metalness |
| **`_ng`** Normal/Gloss | normal map | gloss |
| **`_add`** Additive | R=AO, G=emissive, B=unused | paintability |
| **`_alphamask`** | alpha mask (white=opaque) | unused |

## Steps

1. **Author textures** at power-of-two size (2048×2048 typical). Set the `_add`
   **red (AO) channel white** if you have no AO, or the material renders flat/dark.
2. **Convert to DDS** with the correct per-channel `texconv` command — the sRGB
   flags differ per map. See `../reference/materials-reference.md` for the exact
   commands (getting sRGB wrong = washed-out result).
3. **Assign in Blender** via the SEUT Shader Node Editor. SE ignores the raw
   Blender node tree — it only reads texture paths + SEUT Shader parameters, and
   writes them into the model's material `.xml` on export.
4. **Pick a Technique:** `MESH` (standard), `DECAL`/`DECAL_CUTOUT` (overlays),
   `ALPHA_MASKED`, `FOLIAGE`, or `GLASS`/`HOLO`/`SHIELD` (transparent — name must
   match a `TransparentMaterials.sbc` SubtypeId).
5. **Export** — SEUT bakes the material into the model `.xml` alongside the `.mwm`.

## Material name matters

The **Material Name** is the link between mesh and textures. It's also used by the
Armor Skins system, emissive presets, LCDs, and lights. For GLASS/HOLO/SHIELD the
name instead references a `SubtypeId` in `TransparentMaterials.sbc`.

## Transparent (glass) materials

Generate a skeleton:
```
python3 scripts/sbc_tool.py new-transparent --subtype MyCustomGlass
```
Then set `<Texture>` (CA: RGB=color, A=alpha), `<GlossTexture>` (NG), `<Gloss>`,
`<Reflectivity>`, `<AlphaMisting>`, `<LightMultiplier>`. Full field docs in the
materials reference.

## Verify before shipping (standing checklist)

1. Resolution 2048×2048.
2. Format BC7 DDS.
3. Channel packing correct — **ADD red = white** when no AO.
