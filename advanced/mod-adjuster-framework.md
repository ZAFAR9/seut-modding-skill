<!-- Extracted from advanced/mod-adjuster/ source (Workshop 3017795356), verified against files. 2026-07-20. -->

# Mod Adjuster V2 — Runtime Definition Patching

**Workshop:** `3017795356`  ·  **Namespace:** `ModAdjusterV2`

## What it is / why use it

Mod Adjuster lets you **tweak or override fields on *existing* definitions**
(vanilla or from other mods) at load time, **without copying the whole `.sbc`**.
You ship a small XML that lists only the definitions + fields you want to change;
Mod Adjuster finds the live definition in `MyDefinitionManager` and applies your
values on top.

Why it matters:
- **Compatibility / load order** — you don't fork someone's SBC, so their updates
  still flow through; you only override the specific fields you name.
- **Small, focused patches** — a balance pass on 20 blocks is one short file, not
  20 full block copies.
- **Survives updates** — vanilla rebalances don't get clobbered by a stale copy.

## How it loads (the real mechanism)

From `Session/ModAdjuster.cs` (`MySessionComponentBase`):

1. On init it scans **every active mod** for a manifest file
   `Data/ModAdjuster/ModAdjusterFiles.txt` (constant `PATH`).
2. That `.txt` lists one config filename per line. Each is read from
   `Data\ModAdjuster\<name>`.
3. Each config is read line-by-line; any line containing `xsi:type=` has
   `MyObjectBuilder_` stripped (so you can paste OB type names directly), then it's
   deserialized: `MyAPIGateway.Utilities.SerializeFromXML<Definitions.Definitions>(data)`.
4. `ImportDefinitions(...)` looks each entry up via `MyDefinitionManager.Static`
   (`GetDefinition`, `GetBlueprintDefinition`, …) and applies the overrides onto the
   already-loaded definition.

So the flow for a modder is:
```
Data/ModAdjuster/ModAdjusterFiles.txt   → lists your config files
Data/ModAdjuster/MyChanges.xml          → <Definitions> with only the fields to change
```

## What it can patch

From `Definitions/Base.cs` (`class Definitions`, the XML root), the supported
arrays are:

| XML element | Targets |
|---|---|
| `<Definition>` (GenericDefinitions) | any generic definition by Id |
| `CubeBlocks` → `<Definition>` | `CubeBlockDefinition` (blocks, functional blocks, warhead) |
| `Ammos` → `<Ammo>` | ammo definitions |
| `Weapons` → `<Weapon>` | weapon definitions |
| `BlockVariantGroups`, `CategoryClasses` | variant groups, GUI categories |
| `Blueprints` / `BlueprintClasses` / `BlueprintClassEntries` | crafting |
| `Characters`, `HandItems` | character + hand items |
| `PhysicalItems`, `AmmoMagazines`, `Components` | items |
| `ContainerTypes`, `DropContainers` | loot/containers |
| `Prefabs`, `RespawnShips`, `SpawnGroups` | spawns |
| `PlanetGeneratorDefinitions` | planets |
| `VoxelMaterials` | voxel materials |

(See the `Definitions/Blocks/`, `Ammo.cs`, `Particles.cs`, `Planet.cs`, etc. classes
for the per-type overridable fields.)

## Example config (grounded in the real classes)

`Data/ModAdjuster/ModAdjusterFiles.txt`:
```
Balance.xml
```

`Data/ModAdjuster/Balance.xml` — nerf a block's build time and an ammo's damage:
```xml
<?xml version="1.0"?>
<Definitions>
  <CubeBlocks>
    <Definition xsi:type="MyObjectBuilder_CubeBlockDefinition">
      <Id><TypeId>Reactor</TypeId><SubtypeId>LargeBlockLargeGenerator</SubtypeId></Id>
      <BuildTimeSeconds>120</BuildTimeSeconds>
    </Definition>
  </CubeBlocks>
  <Ammos>
    <Ammo xsi:type="MyObjectBuilder_ProjectileAmmoDefinition">
      <Id><TypeId>AmmoDefinition</TypeId><SubtypeId>NATO_25x184mm</SubtypeId></Id>
      <ProjectileMassDamage>50</ProjectileMassDamage>
    </Ammo>
  </Ammos>
</Definitions>
```
Only the fields you include are changed; everything else stays as the base
definition. (The `MyObjectBuilder_` prefix on `xsi:type` is auto-stripped, so it's
optional.)

## Cheat-sheet / gotchas

- Manifest path is fixed: **`Data/ModAdjuster/ModAdjusterFiles.txt`**, and configs
  live in **`Data/ModAdjuster/`**. Wrong path = silently skipped (check the log).
- It **overrides existing** definitions by `Id`; it is not for creating brand-new
  blocks (use a normal SBC for that).
- It logs to local storage (`Session/Logs.cs`) — check the log if a patch didn't
  apply (missing file / definition-not-found messages).
- Component integrity ratios get re-adjusted when you modify a component
  (`AdjustRatiosForModifiedComponent`).
- Because it edits the live definition, **load order matters** only in that the
  target must exist; it reads all active mods' manifests.
