# How-To: Create a Custom Material

Making your own model material (not voxel) from textures to in-game.
Source: official SEUT tutorial + Shader Editor reference (spaceengineers.wiki.gg).

## The 4 textures (packed maps)

A material is up to four DDS textures, each packing data per channel (black=0%,
white=100%). Save as **DDS / BC7**, dimensions powers of two.

| Map | RGB | Alpha |
|---|---|---|
| **`_cm`** Color/Metal | base color | metalness |
| **`_ng`** Normal/Gloss | normal map | gloss |
| **`_add`** Additive | R=AO, G=emissive, B=unused | paintability |
| **`_alphamask`** | alpha mask (white=opaque) | unused |

## ⚠️ Two gotchas that cause "material won't export / pitch black in-game"

These are the #1 and #2 reasons a custom material previews fine in Blender but
**never appears in the exported MaterialsLib XML** (so it renders black in-game):

1. **The material must be LOCAL, not linked.** In the Shader Editor SEUT panel, if
   the SEUT options are **greyed out**, the material is *linked* into the .blend
   (appended/library) and SEUT **cannot edit or export it**. Fix: make the material
   **Local** (Material dropdown ▸ the linked/user icon ▸ *Make Local*). Only a local
   material gets written to the MaterialsLib XML.
2. **Textures must sit in a path containing `\Textures\`.** SEUT only exports the
   material if it can build a **relative** texture path — which requires the source
   image to live under a folder whose path includes `\Textures\`, ideally
   `[SEUT Assets]\Textures\Custom\<yourname>\`. Textures in Downloads/Desktop/temp
   → SEUT silently drops the material.

**Decisive test after export:** open the SEUT-generated `<MaterialsLib>` XML. Your
material MUST appear as a `<Material Name="...">` block with a `ColorMetalTexture`
line. If it's absent, one of the two gotchas above applies — re-check before
touching anything else.

## Steps (official SEUT workflow)

1. **Author textures** at power-of-two size (2048×2048 typical). Set the `_add`
   **red (AO) channel white** if you have no AO, or the material renders flat/dark.
2. **Make Blender-readable copies as TIF.** Blender cannot read `DDS BC7`, so for
   *linking in the Shader Editor* you use `TIF` (display only — SE still uses the
   DDS). SEUT's Texture Conversion panel has a **TIF** preset for this.
3. **Place textures under a `\Textures\` path** — `[SEUT Assets]\Textures\Custom\
   <yourname>\` (see gotcha #2). This lets SEUT build the relative mod path.
4. **Create the SEUT material:** in the Shader Editor (press `N`), click
   **`Create SEUT Material`** to build the node tree. If greyed out → make the
   material Local first (gotcha #1).
5. **Link textures to their image nodes** in that SEUT node tree — CM, NG, ADD
   (delete any leftover default nodes). SE ignores the raw Blender node tree; it
   only reads the SEUT texture paths + parameters and writes them into the model's
   material XML on export.
6. **Pick a Technique:** `MESH` (standard), `DECAL`/`DECAL_CUTOUT` (overlays),
   `ALPHA_MASKED`, `FOLIAGE`, or `GLASS`/`HOLO`/`SHIELD` (transparent — name must
   match a `TransparentMaterials.sbc` SubtypeId).
7. **Export** — SEUT converts the TIF→DDS, writes the MaterialsLib XML, and bakes
   the material (with texture paths) into the `.mwm`.
8. **Verify** with the decisive test above + `strings -n 6 X.mwm | grep
   ColorMetalTexture` — your material's `_cm.dds` path must be present.

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
4. Material is **Local** (SEUT options not greyed out).
5. Textures were under a `\Textures\` path at export time.
6. Material **appears in the exported MaterialsLib XML** with a texture path.

## See also

- `troubleshooting/black-untextured-material.md` — full case study of a material
  that exports with empty texture paths / is missing from the MaterialsLib.
- `../reference/materials-reference.md` — texconv per-channel commands, XML fields.
