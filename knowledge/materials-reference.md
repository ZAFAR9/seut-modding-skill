# Materials Reference (Space Engineers)

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/Materials
Archived: 2026-07-17

Materials are the color/surface displayed on models. Made of up to four textures
plus additional parameters. `mwmbuilder` packs a `.fbx` with an `.xml` (and
optionally a `.hkt`). The `.xml` links textures to material names because the
`.fbx` is not searched for textures. SEUT generates this `.xml` from the Shader
Editor data.

## Material definition XML (the material part of the model .xml)

```xml
<Material Name="Atlas3_Colorable">
  <Parameter Name="Technique">DECAL_CUTOUT</Parameter>
  <Parameter Name="ColorMetalTexture">Textures\Models\Cubes\Atlas3_Colorable_cm.dds</Parameter>
  <Parameter Name="NormalGlossTexture">Textures\Models\Cubes\Atlas3_Colorable_ng.dds</Parameter>
  <Parameter Name="AddMapsTexture">Textures\Models\Cubes\Atlas3_Colorable_add.dds</Parameter>
  <Parameter Name="AlphamaskTexture">Textures\Models\Cubes\Atlas3_alphamask.dds</Parameter>
</Material>
```

**Blender note:** SE does NOT read the Blender node tree. It pulls all material
info from the definition above. Only texture paths + SEUT Shader Node Editor
parameters survive export.

## Material Name matters

- It's the link between mesh and textures.
- Used by the Armor Skins system to swap textures (except GLASS/HOLO/SHIELD).
- With GLASS/HOLO/SHIELD, the name references a `SubtypeId` of a
  TransparentMaterial definition.
- Can be referenced by emissive presets, LCD screens, interior lights, etc.

## Textures — RGBA channel packing

Textures store different data per channel. Black = 0% of that channel's value,
white = 100%. Save as **DDS**, prefer **BC7** compression. Dimensions must be
powers of 2 (4, 8, 16 … 4096); width and height need not match (e.g. 512x16 ok).

### CM — Color / Metalness
`texconv [in] -ft DDS -f BC7_UNORM_SRGB -sepalpha -sRGB -y -o [out]`
- **RGB:** base material color.
- **Alpha:** metalness (affects light reflection).

### NG — Normal / Gloss
`texconv [in] -ft DDS -f BC7_UNORM -sepalpha -y -o [out]`
- **RGB:** normal map (fake 3D relief).
- **Alpha:** gloss (with metalness, controls reflections).

### ADD — Additive
`texconv [in] -ft DDS -f BC7_UNORM_SRGB -if POINT_DITHER_DIFFUSION -sepalpha -sRGB -y -o [out]`
- **Red:** ambient occlusion. If none, make it fully **white**.
- **Green:** emissiveness (glow; color comes from CM at that pixel).
- **Blue:** unused (legacy terrain data).
- **Alpha:** paintability (white = strongly paintable).

> Note: channel meaning for ADD is documented two ways across sources. The
> wiki Materials page states R=AO, G=Emissive, B=unused, A=Paintability. Some
> SEUT-side summaries swap R/G. When in doubt, follow the official wiki mapping
> above (R=AO white-if-none, G=Emissive).

### ALPHAMASK — Alpha Masked
`texconv [in] -ft DDS -f BC7_UNORM -if POINT_DITHER_DIFFUSION -y -o [out]`
- **RGB:** alpha mask — white = opaque, black = transparent.
- **Alpha:** unused.

## Additional parameters (set via SEUT Shader Node Editor)

### Technique (render mode)
- `MESH` — standard.
- `DECAL` — looks like part of the model behind it.
- `DECAL_NOPREMULT` — higher transparency accuracy, same style as DECAL.
- `DECAL_CUTOUT` — cuts into the model behind it.
- `ALPHA_MASKED` — uses an alphamask texture.
- `FOLIAGE` — half-transparent (leaves); shadows respect texture transparency.
- `GLASS` — name must match a TransparentMaterial SubtypeId.
- `HOLO` — emissive GLASS; name matches a TransparentMaterial SubtypeId.
- `SHIELD` — emissive, animated, distorted GLASS; name matches a
  TransparentMaterial SubtypeId. Dims GLASS/HOLO seen through it. May crash on
  some block types — test thoroughly.

### Facing (rotation toward camera)
- `None` (default) — disabled.
- `Vertical` — rotates around model up-axis.
- `Full` — rotates on all axes to always face camera.
- `Imposter` — special texture layout switching perspectives (needs research).
- Shadows don't reflect rotated mesh properly.

### Wind Scale / Wind Frequency
Mainly for tree/bush materials (works on blocks too, any technique).
Scale = displacement amount during wind animation; Frequency = animation speed.

## Transparent Materials (TransparentMaterials.sbc)

When technique is GLASS/HOLO/SHIELD, the material ignores model textures and its
name references a `SubtypeId` in `TransparentMaterials.sbc`, which holds the
texture paths and other parameters.

- **CA — Color / Alpha:** RGB = material color, Alpha = alpha (transparency).
- **NG — Normal / Gloss:** RGB = normal map, Alpha = gloss.

Example definition:

```xml
<?xml version="1.0"?>
<Definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
             xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TransparentMaterials>
    <TransparentMaterial>
      <Id>
        <TypeId>TransparentMaterialDefinition</TypeId>
        <SubtypeId>MyCustomGlassMaterial</SubtypeId>
      </Id>
      <Texture>Textures\Models\MyCustomGlass_cm.dds</Texture>
      <GlossTexture>Textures\Models\MyCustomGlass_ng.dds</GlossTexture>
      <Gloss>0.4</Gloss>
      <Reflectivity>0.6</Reflectivity>
      <AlphaMisting>true</AlphaMisting>
      <LightMultiplier>1.5</LightMultiplier>
      <GlossTextureAdd>0.1</GlossTextureAdd>
    </TransparentMaterial>
  </TransparentMaterials>
</Definitions>
```

- `<Gloss>` — base gloss for environment reflections.
- `<Reflectivity>` — reflection intensity scale.
- `<AlphaMisting>` — fades transparent edges to prevent harsh clipping.
- `<LightMultiplier>` — amplifies light through/from the material.

## Particle & Voxel materials
Handled by their own systems (particle materials via ParticleEffects; voxel
materials via VoxelMaterials.sbc). The ADD-map examples show how AO/emissive
affect voxels (white voxels with red add-map = standard, yellow = emissive).
