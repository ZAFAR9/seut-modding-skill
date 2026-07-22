# Reference: World and Session Settings

← [Back to reference index](README.md)

How Space Engineers structures game saves, the division of responsibility between the configuration and runtime checkpoint files, and the key customizable fields within SessionSettings that dictate gameplay rules, multipliers, and limits.

---

## Anatomy of a Space Engineers Save Directory
A standard save folder (located in `%AppData%\SpaceEngineers\Saves\<SteamID>\<WorldName>\`) contains several key files. For modders, the two most important are:

1. **`sandbox_config.sbc` (WorldConfiguration)**:
   - Holds static world properties: the world's name, the active mod list (`<Mods>`), and the gameplay rules (`<Settings xsi:type="MyObjectBuilder_SessionSettings">`).
   - **Priority**: This file is prioritized by the game client when loading a world from the menu. Changes made here will override duplicate keys in `sandbox.sbc` on load.
2. **`sandbox.sbc` (Checkpoint)**:
   - Contains the live, serializable checkpoint/state of the game world (e.g., entity positions, players, factions, voxel changes, GPS coordinates, etc.).
   - It also contains duplicate `<Settings>` and `<Mods>` blocks, which are overwritten by `sandbox_config.sbc` on load, then serialized back to both files upon saving.

---

## SBC File Definitions

### 1. `sandbox_config.sbc` (WorldConfiguration)
This file defines metadata for world selection menus and initial rules.

```xml
<MyObjectBuilder_WorldConfiguration>
  <Settings xsi:type="MyObjectBuilder_SessionSettings">
    <!-- Detailed SessionSettings go here -->
  </Settings>
  <Mods>
    <ModItem FriendlyName="Custom Mod Name">
      <Name>12345678.sbm</Name>
      <PublishedFileId>12345678</PublishedFileId>
      <PublishedServiceName>Steam</PublishedServiceName> <!-- Steam or mod.io -->
      <IsDependency>false</IsDependency>
    </ModItem>
  </Mods>
  <SessionName>My Custom Survival World</SessionName>
</MyObjectBuilder_WorldConfiguration>
```

- **`<Mods>`**: A list of `<ModItem>` nodes. If a mod was downloaded as a dependency of another mod, `<IsDependency>` will be set to `true`.
- **`<SessionName>`**: The world's display name shown in the main menu and server lists.

### 2. `sandbox.sbc` (Checkpoint)
This file stores the active session data. Key elements of the `MyObjectBuilder_Checkpoint` include:

| Element | Type | Default | Description |
|---|---|---|---|
| `<ElapsedGameTime>` | `Int64` | `0` | Total running time of the world in ticks (10 million ticks per second). |
| `<CustomLoadingScreenImage>` | `String` | `null` | Path to a custom loading screen image (supports relative paths like `..\..\..\workshop\content\244850\<ModId>\Textures\Loading.jpg`). |
| `<CustomLoadingScreenText>` | `String` | `null` | Overrides the center text on the loading screen. |
| `<CustomSkybox>` | `String` | `null` | SubtypeId of the skybox/environment definition to force-load. |
| `<Factions>` | `List` | `null` | List of factions, relationships, and active members. |
| `<Gps>` | `List` | `null` | List of players' saved GPS coordinates. |
| `<ScriptManagerData>` | `Complex` | `null` | Stores internal visual scripting variables and state. |

---

## SessionSettings Reference (`MyObjectBuilder_SessionSettings`)
These settings are parsed into `<Settings>` in both files and represent the active rules of the world.

### Block Limits & PCU
| Field | Type | Default | Description / Value Options |
|---|---|---|---|
| `<BlockLimitsEnabled>` | `Enum` | `NONE` | Defines limit modes: `NONE` (limits ignored), `GLOBALLY` (shared), `PER_FACTION`, `PER_PLAYER`. Console worlds revert to `GLOBALLY` if set to `NONE`. |
| `<LimitBlocksBy>` | `Enum` | `BlockPairName` | Dictates grouping for limits: `BlockPairName` (matching large/small grids) or `Tag` (from `BlockTags.sbc`). |
| `<MaxGridSize>` | `Int32` | `50000` | Max blocks allowed in a single grid. Ignored if `BlockLimitsEnabled` is `NONE`. |
| `<MaxBlocksPerPlayer>` | `Int32` | `100000` | Max total blocks a player can build. Ignored if `BlockLimitsEnabled` is `NONE`. |
| `<TotalPCU>` | `Int32` | `600000` | Max PCU limit based on `BlockLimitsEnabled` (global, faction-divided, or player-divided). |

### Gameplay & Speed Multipliers
Multipliers adjust basic player/production speeds. All numbers are floats.
| Field | Default | Description |
|---|---|---|
| `<InventorySizeMultiplier>` | `3.0` | Multiplies character inventory volume (range: 1.0 to 100.0). |
| `<BlocksInventorySizeMultiplier>` | `1.0` | Multiplies cargo block volumes. **Note:** Divided mass makes cargo heavier/lighter relative to thrust requirements. |
| `<WelderSpeedMultiplier>` | `2.0` | Speeds up hand/ship welders. |
| `<GrinderSpeedMultiplier>` | `2.0` | Speeds up hand/ship grinders. |
| `<RefinerySpeedMultiplier>` | `1.0` | Speeds up ore processing. |
| `<AssemblerSpeedMultiplier>` | `1.0` | Speeds up component crafting. |
| `<AssemblerEfficiencyMultiplier>` | `1.0` | Reduces raw material costs for component crafting. |

### Environment, Spawning & Physics
| Field | Type | Default | Description |
|---|---|---|---|
| `<DestructibleBlocks>` | `Boolean` | `true` | Allows block damage and destruction. |
| `<EnableVoxelDestruction>` | `Boolean` | `true` | Allows planet/asteroid deformation and destruction. |
| `<EnableOxygen>` | `Boolean` | `true` | Enables suit/room oxygen consumption. |
| `<EnableOxygenPressurization>` | `Boolean` | `true` | If true, air tightness is checked and pressurized rooms can form. |
| `<EnableSunRotation>` | `Boolean` | `true` | Enables day/night cycles. Interval set by `<SunRotationIntervalMinutes>`. |
| `<MaxFloatingObjects>` | `Int32` | `56` | Cap on floating ore/item drops before garbage collection triggers. |
| `<AdjustableMaxVehicleSpeed>` | `Boolean` | `true` | Dictates if the suspension wheel terminal displays a max speed slider. |

### NPCs, Economy & Spawns
| Field | Type | Default | Description |
|---|---|---|---|
| `<EnableEconomy>` | `Boolean` | `false` | Enables trade stations, contracts, factions, and currency (SC). |
| `<ReputationDecayRate>` | `Float` | `0.0f` | Rate at which player reputation decays back to 0 (neutral) with NPC factions. |
| `<CargoShipsEnabled>` | `Boolean` | `true` | Toggles random vanilla cargo ship spawns. |
| `<EnableEncounters>` | `Boolean` | `true` | Enables deep-space exploration encounters (dungeons/ships). |
| `<EnableDrones>` | `Boolean` | `true` | Toggles automatic hostile pirate drone spawns. |
| `<EnablePlanetaryEncounters>` | `Boolean` | `true` | Enables encounter bases to spawn on planetary surfaces. |

### Scripting & Experimental Settings
These elements are highly relevant to modders and script authors:
| Field | Type | Default | Description |
|---|---|---|---|
| `<ExperimentalMode>` | `Boolean` | `false` | Must be `true` to enable scripting, supergridding, and high-limit settings. |
| `<EnableIngameScripts>` | `Boolean` | `true` | Allows Programmable Blocks (PB) to execute in-game C# scripts. Requires `ExperimentalMode = true`. |
| `<StationVoxelSupport>` | `Boolean` | `false` | If true, grids embedded in voxels become static; dynamic grids are not automatically converted. |
| `<EnableShareInertiaTensor>` | `Boolean` | `true` | Exposes "Share Inertia Tensor" checkbox on rotors/pistons to help reduce physics wobble (clang). |
| `<EnableSubgridDamage>` | `Boolean` | `false` | Toggles physical collision damage between a grid and its subgrids. |

---

## TL;DR
- `sandbox_config.sbc` is the high-priority world configuration (settings, active mods, world name).
- `sandbox.sbc` is the runtime checkpoint (live entity state, positions, factions, etc.); it duplicates settings but gets overwritten on load by `sandbox_config.sbc`.
- Modders can point `<CustomLoadingScreenImage>` to relative workshop mod paths using `..\..\..\workshop\content\244850\<ModId>\`.
- Programmable blocks require `<EnableIngameScripts>` and `<ExperimentalMode>` to be set to `true`.

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/Saves/SessionSettings, https://spaceengineers.wiki.gg/wiki/Modding/Reference/Saves/WorldConfiguration, https://spaceengineers.wiki.gg/wiki/Modding/Reference/Saves/Checkpoint
