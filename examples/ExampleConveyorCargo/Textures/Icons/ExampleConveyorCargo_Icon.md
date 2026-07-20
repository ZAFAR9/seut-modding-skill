# Textures/Icons/ExampleConveyorCargo_Icon.dds — icon spec

The block's G-menu / toolbar icon, referenced by `<Icon>` in `../../Data/CubeBlocks.sbc`.

- **Format:** DDS, BC7 (or BC3/DXT5 for simple icons).
- **Size:** 128×128 or 256×256 (power of two), with **transparency (alpha)** so the
  icon reads on the dark toolbar.
- Export from your image editor and convert with texconv:
  `texconv icon.png -ft DDS -f BC7_UNORM_SRGB -sepalpha -sRGB -y -o Textures\Icons\`
- Keep the filename matching the path in the definition
  (`ExampleConveyorCargo_Icon.dds`).
