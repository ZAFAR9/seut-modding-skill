# Reference: SpawnGroup Definition

← [Back to reference index](README.md)

Defines encounter spawns, which can compose multiple ships (prefabs) and voxel maps. Found in `SpawnGroups.sbc`. Uses the `<Definition xsi:type="MyObjectBuilder_SpawnGroupDefinition">` attribute.

---

## Shared Elements

- **`<EnableNpcResources>`**: Boolean (default: `true`). If true, tags spawned grids as NPC-spawned, making them claimable by players.
- **`<Frequency>`**: Float (default: `1`). Relative spawn weight used by the engine for cargo ships, random encounters, planetary/global encounters, and pirate antennas. Uses a weighted random selection. Value of `0` is invalid.
- **`<Factions>`**: List of Faction SubtypeIds. Uses the first in-world faction matching the Tag in the definition. If declared and no faction matches, the spawngroup fails to spawn.
- **`<FactionTypes>`**: List of FactionType SubtypeIds (default: `Pirate`). Used if `<Factions>` is empty. Chooses a random in-world faction of the listed type.
  *Note: If both `<Factions>` and `<FactionTypes>` are empty, the spawned encounter will belong to Nobody.*
- **`<ContractTriggerSettings>`**: Configures triggers that grant a grid-delivery contract (Grid Hauling) to players.
  - `<SpawnWeight>`: Float (0.0 to 1.0). Chance to generate the trigger.
  - `<TriggerRadius>`: Float (default: `500`). Radius in meters of the spherical trigger around the targeted grid's pivot.
  - `<ContractSubtypeId>`: SubtypeId of the `ContractTypeGridHaulingDefinition`.

---

## Encounter Type Flags

- **`<IsEncounter>`**: Boolean (default: `false`). Marks for use in Random Space Encounters.
- **`<IsCargoShip>`**: Boolean (default: `false`). Marks for use in Cargo Ships.
- **`<IsGlobalEncounter>`**: Boolean (default: `false`). Marks for use in Global Space Encounters (such as Economy trade stations).
- **`<IsPlanetaryEncounter>`**: Boolean (default: `false`). Marks for use in Planetary Encounters. Needs `<PlanetaryInstallationSettings>` to function.

---

## Planetary Installation Settings (`<PlanetaryInstallationSettings>`)

Requires `<IsPlanetaryEncounter>` to be true:
- **`<AlignToSurface>`**: Boolean (default: `false`). If true, aligns spawned grids with the local average surface terrain. If false, aligns with gravity vector.
- **`<RandomizedOrientation>`**: Randomizes grid orientation (Yaw, Pitch, Roll in radians) relative to surface alignment:
  ```xml
  <RandomizedOrientation YawLimitMin="-3.14" YawLimitMax="3.14" PitchLimitMin="-0.1" PitchLimitMax="0.1" RollLimitMin="-0.1" RollLimitMax="0.1" />
  ```
- **`<Planets>`**: List of partial planet display names where this encounter can spawn (e.g., `<PlanetName>Europa</PlanetName>`). Empty matches all.
- **`<VoxelMaterials>`**: List of voxel material types (using `<MaterialTypeName>`, not `<SubtypeId>`) allowed for spawning on.

---

## Spawnables

### 1. Prefabs (`<Prefabs>`)
List of grids to spawn. Requires at least one `<Prefab>` block.
- **`SubtypeId`** (attribute): SubtypeId of a `PrefabDefinition`.
- **`<Speed>`**: Float (default: `10`). Forward speed in m/s the grid spawns with.
- **`<Position>`**: Vector3 offset (X, Y, Z in meters) from the spawn origin.
- **`<BeaconText>`**: String. Renames all beacon blocks on the grid to this.
- **`<GpsGrid>`**: Boolean (default: `false`). If true, shows a GPS marker on this grid to all players (global encounters only).
- **`<Behaviour>`**: SubtypeId of a `DroneBehaviorDefinition` for Remote Control blocks (Random/Global encounters and Pirate Antennas only).
- **`<BehaviourActivationDistance>`**: Float (default: `1000`). Distance in meters around the Remote Control block to trigger the behavior.

```xml
<Prefabs>
  <Prefab SubtypeId="LargeNPC_Carrier">
    <Speed>12.5</Speed>
    <Position>
      <X>0.0</X>
      <Y>100.0</Y>
      <Z>0.0</Z>
    </Position>
    <BeaconText>Pirate Raiding Party</BeaconText>
    <Behaviour>DefensiveDroneBehavior</Behaviour>
    <BehaviourActivationDistance>2000</BehaviourActivationDistance>
  </Prefab>
</Prefabs>
```

### 2. Voxels (`<Voxels>`)
Spawns voxelmap asteroid prefabs (Random/Global encounters only).
- **`StorageName`** (attribute): SubtypeId of a `VoxelMapStorageDefinition` (or `.vx2` file name in the game's VoxelMaps folder if definition doesn't exist).
- **`<Offset>`**: Vector3 offset in meters from spawn position.
- **`<CenterOffset>`**: Boolean (default: `false`). Enables the use of `<Orientation>`.
- **`<Orientation>`**: Quaternion rotational offset for the voxelmap.

---

## TL;DR
- SpawnGroups define NPC encounters consisting of grids (prefabs) and/or asteroid voxels.
- Weighted random spawning is determined by the `<Frequency>` element.
- Ownership is mapped using `<Factions>` or `<FactionTypes>`.
- Behavior scripts can be attached to Remote Control blocks with `<Behaviour>`.
- Encounter types are split into space-based Cargo Ships/Random/Global Encounters and surface-based Planetary Encounters.

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/SBC/SpawnGroup_Definition