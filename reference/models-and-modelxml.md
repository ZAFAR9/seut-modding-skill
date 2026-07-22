# Reference: Models and ModelXML

← [Back to reference index](README.md)

An overview of the Space Engineers proprietary `.mwm` model asset pipeline, layout requirements, LOD structures, dummy helper nodes, subpart mechanics, and the ModelXML (`.xml`) sidecar definition format.

---

## The `.mwm` Model Pipeline
Space Engineers uses a proprietary `.mwm` (Miner Wars Model) file format for its 3D assets. The `.mwm` file is not a simple zip archive, but a compiled, binary-optimized package that combines:
- **`.fbx`**: Visual geometry, UV mapping, material slot names, and animation data.
- **`.hkt`**: Physical collision mesh data (Havok physics).
- **`.xml`**: Sidecar config linking visual materials to texture paths, level of detail (LOD) thresholds, and model parameters.

### Folder Layout Requirements
For custom models to load in-game, they **must** be stored within a `Models` folder in your mod root (e.g., `MyMod/Models/Cubes/...`). 
- **The Cutoff Rule**: The game engine parses the asset's full path and locates the mod root by looking for the leftmost, case-insensitive occurrence of the string `"models"`. If this folder is missing or named incorrectly, custom texture mapping and LOD references will fail.

---

## Core Model Features

### 1. Level of Detail (LODs)
LODs improve rendering performance by swapping complex meshes with simpler ones as the camera recedes.
- **Mesh Naming**: The primary detailed mesh is **LOD0**. Simpler, performance-friendly meshes are defined as **LOD1**, **LOD2**, etc.
- **No Logic inside LODs**: LODs (LOD1+) should **never** contain dummies or collision geometry. They are swapped purely at the graphics rendering level.
- **RAM Optimization**: The game stores the vertex/triangle data of **LOD1** (or LOD0 if no LODs exist) in system RAM for CPU-side raycasting (e.g., bullet impact decal placement, terminal interaction checks, welder/grinder aim checks). Having a low-poly LOD1 reduces the CPU memory and performance cost of these checks.
- **Graphics Scaling**: LOD transition distances (in meters) are dynamically scaled in-game based on the screen's vertical resolution, player field of view (FOV), and the user's "Model Quality" setting.

### 2. Dummies (Helper Nodes)
Dummies are empty nodes (empties in Blender) defined in the model to specify positions, orientations, and scales for logic systems.
- Dummies must be placed on **LOD0**; game systems cannot easily access dummies on subsequent LOD levels.
- Typical use cases:
  - **Conveyor Ports**: `detector_conveyor_line...`
  - **Interactive Area**: `subpart_...` or interaction use-objects.
  - **Upgrade Modules**: upgrade slot ports.

### 3. Subparts
Subparts are child meshes within a block (e.g., turret heads, piston pistons, door panels) that rotate, translate, or animate relative to the parent block.
- They are separate `.mwm` files linked through custom dummy nodes named `subpart_<SubpartName>` in the parent's model.
- **Collisions**: By default, subparts do **not** support physics collisions. However, specific parent block types (such as `Door` or `Piston`) explicitly manage subpart collision meshes, requiring them to have `.hkt` data to function.

### 4. Collisions
Collision bounds are defined in a `.hkt` (Havok physics) file packaged within the `.mwm` file. You can easily export Havok collisions using **SEUT** in Blender.

### 5. Animations
Animations can be defined within the `.fbx` and packed into the `.mwm` file. Note that animations are only loaded once per model, so they should be managed carefully.

---

## ModelXML Definition (`.xml`)
The sidecar `.xml` file configures the `mwmbuilder` packaging pipeline. **SEUT** generates this automatically, but understanding its structure is crucial for manual compilation or debugging.

```xml
<Model Name="MyCustomBlock">
  <!-- General Parameters -->
  <Parameter Name="RescaleFactor">0.01</Parameter>
  <Parameter Name="RotationX">0</Parameter>
  
  <!-- Material Texture Mapping -->
  <Material Name="MyFbxMaterial">
    <Parameter Name="Technique">MESH</Parameter>
    <Parameter Name="ColorMetalTexture">Textures\Models\Cubes\MyBlock_cm.dds</Parameter>
    <Parameter Name="NormalGlossTexture">Textures\Models\Cubes\MyBlock_ng.dds</Parameter>
    <Parameter Name="AddMapsTexture">Textures\Models\Cubes\MyBlock_add.dds</Parameter>
  </Material>

  <!-- Level of Detail Links -->
  <LOD Distance="15" Name="Models\Cubes\MyBlock_LOD1.mwm" />
  <LOD Distance="50" Name="Models\Cubes\MyBlock_LOD2.mwm" />
</Model>
```

### Key Elements & Parameters

#### `<Parameter>` (Global Model Parameters)
Multiple parameter tags can configure the mesh's physical characteristics or scale:
- `RescaleFactor` *(float, default: 0.01)*: Scales each vertex uniformly (e.g., converting Blender centimeters/meters to game units).
- `RotationX`, `RotationY`, `RotationZ` *(float, default: 0)*: Rotates the model in degrees around the specified axis.
- `PatternScale` *(float, default: 1)*: Scale of the armor pattern used by the deformable armor system.
- `SimpleModel` *(bool, default: false)*: If true, discards normals/UVs and saves vertex position data only.

#### `<Material>` (Material Configurations)
Assigns shader techniques and textures to specific material names defined inside the `.fbx`.
- **`Technique`**: Configures the shader pass. Available values:
  - `MESH` *(default)*: Standard solid opaque blocks.
  - `ALPHA_MASKED`: Alpha-tested transparency (e.g., vents, grates).
  - `DECAL`: Bullet hole or damage overlays.
  - `GLASS`: Transparent double-sided glass panels.
  - `HOLOGRAM`: Blue terminal projection lines.
- **Texture Paths**: Specify paths relative to the mod root (e.g., `Textures\Models\Cubes\Custom_cm.dds`):
  - `ColorMetalTexture` (`_cm`): Albedo color in RGB, metalness in the Alpha channel.
  - `NormalGlossTexture` (`_ng`): Normal map in RGB, glossiness (roughness inverse) in the Alpha channel.
  - `AddMapsTexture` (`_add`): AO in Red, Emissive mask in Green, Paint mask in Blue.
  - `AlphamaskTexture`: Used by transparency techniques like `ALPHA_MASKED`.
- **`Facing`**: Used for billboard-style rendering (`None`, `Vertical`, `Full`, `Impostor`).
- **`WindScale` & `WindFrequency`**: Adjusts sway animations for foliage materials.

#### `<MaterialRef>`
Points to a library of pre-defined materials (e.g., `<MaterialRef Name="PaintedMetal_Colorable"/>`).

#### `<LOD>`
Points to a simpler `.mwm` file to swap in at the specified distance.
- `<LOD Distance="30" Name="Models\Cubes\MyModel_LOD1.mwm" />`

---

## TL;DR
- In-game model files must live in a folder containing the case-insensitive string `"models"` to resolve texture paths.
- The `.mwm` bundles visual mesh, collision, animations, and metadata.
- **LOD1** is saved in RAM for physical checks (bullet decals, terminal interactions). Keep it simple!
- ModelXML maps FBX material slots to game DDS textures and sets shader techniques (e.g., `MESH`, `ALPHA_MASKED`).
- Subparts are separate models attached via parent dummies; they don't support collision unless explicitly supported by parent logic (e.g. doors).

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/Models, https://spaceengineers.wiki.gg/wiki/Modding/Reference/Models/ModelXML
