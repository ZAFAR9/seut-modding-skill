# Reference: PlanetGenerator Definition

← [Back to reference index](README.md)

Defines planets, moons, and terrain generation. Found in `PlanetGeneratorDefinitions.sbc`. Uses the `<Definition xsi:type="MyObjectBuilder_PlanetGeneratorDefinition">` attribute.

---

## Wrappers

- **Old Style** (EarthLike, Mars, Moon): Root `<Definitions>` containing `<Definition xsi:type="PlanetGeneratorDefinition">`.
- **New Style** (Triton, Pertam): Root `<Definitions>` containing `<PlanetGeneratorDefinitions>` containing `<PlanetGeneratorDefinition>`.
*(The chosen wrapper no longer matters as of SE v1.208).*

---

## Map Files
Planet maps are located in `Data\PlanetDataFiles\<FolderName>\*.png`. There are 6 face directions (front, back, left, right, down, up):
- **Base Name** (e.g., `front.png`): 16-bit grayscale heightmaps defining terrain elevation.
- **`_mat` suffix** (e.g., `front_mat.png`): Combined 8-bit RGB data channels:
  - **Red**: Mapped to `<CustomMaterialTable>` and `<ComplexMaterials>`.
  - **Green**: Mapped to `<EnvironmentItems>` biomes.
  - **Blue**: Mapped to `<OreMappings>`.
- **`_add` suffix**: Obsolete. Surface ore coloring is now handled via `<OreMappings>`'s `<TargetColor>`.

---

## Fast Testing & Planet Respawn
- **Realtime Testing**: Use the Render/Atmosphere tabs of the Developer Screen, or a Reload Definitions mod.
- **Saves override Definitions**: Gravity, atmosphere toggles, and atmospheric scale are saved directly in `SANDBOX_0_0_0_.sbs`. To see changes to these parameters in a pre-spawned world, you must manually edit the save file or respawn the planet.

---

## Terrain Heights & HillParams
`<HillParams Min="[ratio]" Max="[ratio]" />` defines the min/max terrain height ratios relative to the spawned planet radius:
- `LowestTerrain = PlanetRadius + (HillParams.Min * PlanetRadius)`
- `HighestTerrain = PlanetRadius + (HillParams.Max * PlanetRadius)`

---

## Key Definition Elements

### 1. Ore Mappings (`<OreMappings>`)
Pairs voxel materials to the blue channel of the `_mat` maps.
```xml
<OreMappings>
  <Ore Value="200" Type="Iron_02" Start="3" Depth="7" TargetColor="#9BFF00FF" />
</OreMappings>
```
- **`Value`**: Byte (0-255) corresponding to the pixel value in the blue channel of the `_mat` map.
- **`Type`**: SubtypeId of the `VoxelMaterialDefinition` to spawn.
- **`Start`**: Depth in meters underground where the deposit begins.
- **`Depth`**: Vertical thickness of the ore deposit in meters.
- **`TargetColor`**: Optional. Determines surface texturing above the deposit:
  - `#FFFFFF`: Increases texture saturation (+100%) and reduces value (-8%) to indicate ore.
  - *Any other color*: Desaturates texture (-90%) and reduces value (-8%).
  - *Omitted*: Surface texture is unaffected.

### 2. Biomes & Material Groups
Defines surface texturing by mapping the red channel of `_mat` maps.
- **`<CustomMaterialTable>`**: Simpler mapping structure. Matches a byte color value in the red channel to a series of materials.
- **`<ComplexMaterials>`**: Highly customizable material groupings. Contains `<MaterialGroup>` elements:
  - **`Value`**: Byte (0-255) color value in the red channel of the `_mat` map.
  - **`<Rule>`**: Child element governing material placement:
    - **`Material`**: SubtypeId of the `VoxelMaterialDefinition` to use.
    - **`MaxDepth`**: Maximum depth this material layer penetrates.
    - **`Slope`**: Min/Max range in degrees.
    - **`Height`**: Min/Max ratio relative to the planet radius.
    - **`Latitude` / `Longitude`**: Min/Max range in degrees for geographic constraints.
    - **`<Layers>`**: Child list containing nested `<Layer Material="[Type]" Depth="[m]" />` tags for layered stratigraphic geology.

### 3. Environment Items (`<EnvironmentItems>`)
Spawns trees, bushes, forageables, and voxel-based boulders.
- **`<Biomes>`**: List of green channel byte values (0-255) from the `_mat` map.
- **`<Materials>`**: List of required `VoxelMaterialDefinition` SubtypeIds. The surface voxel material at the designated biome must match one of these to allow spawning.
- **`<Items>`**: List of nested `<Item>` definitions:
  - **`Density`**: Spawning frequency. (Note: density is halved for destroyables and voxel maps).
  - **`TypeId`**: Must reference one of these specialized environment proxies:
    - `MyObjectBuilder_DestroyableItems`: Handled as destructible clutter with short draw distances.
    - `MyObjectBuilder_Trees`: Tall destructible trees with medium-to-long draw distances.
    - `MyObjectBuilder_Forageable`: Collectible items that can hold entity components.
    - `MyObjectBuilder_VoxelMapStorageDefinition`: Spawns physical voxel-based boulder meshes.

### 4. Atmosphere Configuration
Governs both physical mechanics and visual effects.

#### Atmospheric Mechanics (`<Atmosphere>`)
- **`<HasAtmosphere>`**: Boolean toggle.
- **`<Density>`**: Air density multiplier (0.0 to 1.0). Linearly falls off from `PlanetRadius` to the atmosphere ceiling defined by `<LimitAltitude>`. Affects thruster effectiveness, wind turbine output, and drag.
- **`<Breathable>`**: Boolean toggle.
- **`<OxygenDensity>`**: Ratio of breathable oxygen in the air.
- **`<LimitAltitude>`**: Height multiplier relative to planet radius establishing the atmospheric boundary (e.g. `LimitAltitude="2.0"` means atmosphere reaches 2x planet radius).

#### Atmospheric Visuals (`<AtmosphereSettings>`)
- **`<Intensity>`**: Atmospheric opacity and volumetric fog intensity.
- **`<Scale>`**: Thickness of the atmosphere visual dome (affects sky colors).
- **`<CloudLayers>`**: List of cloud spheres rotating independently.
- **`<HostileAtmosphereColorShift>`**: Minimum/Maximum RGB channels for shifting sky and volumetric colors on hostile planets.

---

## TL;DR
- Planet mapping uses 6-sided grayscale heightmaps for shape and 3-channel RGB `_mat` maps for material texturing (Red), environment spawns (Green), and ore generation (Blue).
- Ore deposits (`<OreMappings>`) define depth, thickness, and surface color indicators.
- Texturing is configured using `<ComplexMaterials>` with altitude, slope, latitude, and stratigraphic rules.
- Vegetation and boulders are populated via `<EnvironmentItems>` using green-channel biome masks.
- Atmosphere settings control both thruster/wind/life mechanics and visual sky/cloud rendering.

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/SBC/PlanetGenerator_Definition