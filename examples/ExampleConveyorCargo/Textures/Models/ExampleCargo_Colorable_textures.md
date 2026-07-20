# Textures/Models/ExampleCargo_Colorable_*.dds — what to make

Three packed DDS maps for the material `ExampleCargo_Colorable`. All **2048×2048**,
**BC7**, power-of-two (standing rule: verify resolution + BC7 before channel work).
Full channel docs: `../../../reference/materials-reference.md`.

| File | RGB | Alpha | texconv |
|---|---|---|---|
| `ExampleCargo_Colorable_cm.dds` | base color | metalness | `texconv in.png -ft DDS -f BC7_UNORM_SRGB -sepalpha -sRGB -y -o out\` |
| `ExampleCargo_Colorable_ng.dds` | normal map | gloss | `texconv in.png -ft DDS -f BC7_UNORM -sepalpha -y -o out\` |
| `ExampleCargo_Colorable_add.dds` | R=AO, G=emissive, B=unused | paintability | `texconv in.png -ft DDS -f BC7_UNORM_SRGB -if POINT_DITHER_DIFFUSION -sepalpha -sRGB -y -o out\` |

## Critical checks

- **ADD map red (AO) channel = white** if you have no AO, or the block renders
  flat/dark (standing rule).
- Get the **sRGB flags right** per row above — CM/ADD use `-sRGB`, NG does **not**.
  Wrong sRGB = washed-out or overly dark result.
- These are model textures (block), so the magenta/relink guidance in
  `../../../how-to/fix-voxel-textures.md` and `work-with-dds-textures.md` applies
  the same way.
