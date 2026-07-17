# SBC / XML Basics — Space Engineers Modding

Source: https://spaceengineers.wiki.gg/wiki/Modding/Reference/SBC (+ /SBC/CubeBlocks)
Archived: 2026-07-17

`.sbc` = "Sandbox Companion" definition files. They are plain **XML**. All game
content (blocks, items, materials, blueprints, etc.) is declared through them.

## Things you MUST know about .sbc files

1. **The `.sbc` extension must be lowercase.** The game will silently NOT load
   a file named `.SBC`, `.Sbc`, etc.
2. **Contents are XML.** Standard XML rules apply (well-formed tags, escaping,
   encoding declaration).
3. **Copy from the game folder, don't write from scratch.** Find the vanilla
   `.sbc` that already contains the type you want, copy it into your mod's
   `Data/` folder, then edit it. Remove any blocks you don't intend to override.
4. **The game loads ALL `.sbc` files** from your mod's `Data/` folder and any
   subfolders. File names don't matter to the loader (except the lowercase
   `.sbc` extension) — the `<TypeId>`/`<SubtypeId>` inside is what identifies a
   definition.
5. **`.sbcB5` / `.sbsB5` files are binary caches** — auto-generated, not edited
   by hand.

## Override vs. Additive vs. Merged behavior

How the game reconciles your definition with the vanilla one of the same Id:

- **Full override** (default for almost everything): your definition replaces
  the vanilla one entirely.
- **Merged:** `Environment Definition`.
- **Additive** (yours is added alongside vanilla): `BlockCategories`,
  `BlueprintClassEntries`, `MaterialProperties Definition`,
  `AnimationController Definition`.
- **No override** (cannot replace vanilla, only add new): `Decal Definition`,
  `EmissiveColor Definition`, `EmissiveColorStatePreset Definition`.

## Wrapper structure

Every `.sbc` file uses this outer shape:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
             xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <CubeBlocks>
    <!-- one or more <Definition> entries here -->
  </CubeBlocks>
</Definitions>
```

A single block entry:

```xml
<Definition xsi:type="...">
  <Id>
    <TypeId>...</TypeId>
    <SubtypeId>...</SubtypeId>
  </Id>
  <!-- various elements -->
</Definition>
```

## TypeId vs. xsi:type — a critical distinction

- **`<TypeId>`** identifies the engine class the definition instantiates
  (e.g. `CubeBlock`, `OxygenGenerator`, `Thrust`).
- **`xsi:type`** on the `<Definition>` tag specifies the *definition schema*
  used to parse it (e.g. `MyObjectBuilder_WindTurbineDefinition`).

Rules:
- If a block uses the plain **CubeBlock Definition**, it needs **no** `xsi:type`
  because the containing list (`<CubeBlocks>`) already implies that type.
- `xsi:type` is only required to specify a type that *inherits* the CubeBlock
  definition.
- If a definition's wiki page doesn't exist, you can derive the `xsi:type`
  from the page name: remove spaces and prefix with `MyObjectBuilder_`.
  Example: "ExhaustBlock Definition" → `MyObjectBuilder_ExhaustBlockDefinition`.

## How definitions link together (quick reference)

- **Blocks:** `CubeBlock` is essential → `Components` (what builds it) →
  `BlueprintClassEntries` (adds to assemblers) → `BlockCategories` (Build Menu
  grouping) → `BlockVariantGroups` (scrollable variant icon).
- **Weapons:** `CubeBlock` → `WeaponDefinition` → `AmmoMagazine` → `Ammo`.
- **Production:** `CubeBlock` → `BlueprintClass` → `BlueprintClassEntries` →
  `Blueprints`.
- **Inventory/components:** `EntityContainers` links a block to an
  `EntityComponent`; `EntityComponents` defines the component data.
- **Physical materials:** `<PhysicalMaterial>Subtype</PhysicalMaterial>` →
  `PhysicalMaterials.sbc` → `MaterialProperties.sbc` (interaction effects).
- **Hand weapons/tools:** `HandItems`, `PhysicalItems`, `AnimationControllers`,
  `Blueprints`.
- **Random loot:** `ContainerTypes` + `<ContainerType>` on a prefab's container.
- **NPC ships:** see Encounters / SpawnGroups.

## Definitions NOT designed to be modded

`AssetModifiers.sbc` & `ArmorModifiers.sbc`, `ControllerSchemes.sbc`,
`MainMenuInventoryScenes.sbc`, `WheelModels.sbc`, `Game\DLCs.sbc`,
`RadialMenu.sbc`. Also: `Screens\*.gsc` (loaded only from the game folder by
exact name), `Scenarios.sbx` (only that exact name from the game folder;
workaround exists), `Tutorials.sbx` (not used).

## Other files that end in .sbc but aren't definitions

- `Checkpoint` — world's `sandbox.sbc`
- `WorldConfiguration` — world's `sandbox_config.sbc`
- `SessionSettings` — settings in both `sandbox_config.sbc` and `sandbox.sbc`
- `ShipBlueprint` — player blueprint format
- `MyConfigDedicated` — `SpaceEngineers-Dedicated.cfg` on dedicated servers

## Data types seen in definition pages

`Single` (float), `Nullable<>` (optional value), `String`, plus vectors.
Some vectors are inline attributes (`<Size x="1" y="1" z="1" />`) and others are
nested elements depending on the definition schema.
