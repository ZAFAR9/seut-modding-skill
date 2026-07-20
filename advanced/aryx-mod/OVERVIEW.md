# ARYX / AWE Mod — Teardown & Index

**Source:** "ARYX CUSTOM.zip" (customizable ARYX build), ~476 MB zipped / ~1.2 GB
unpacked. Studied 2026-07-20.

**What it is:** *Aryx Weapons Enterprise (AWE)* — a large **WeaponCore-based**
weapons pack for Space Engineers. **82 weapons** registered, spanning autocannons,
railguns, coilguns, gauss cannons, shock cannons, lasers/beams, plasma, flak,
missiles/torpedoes, flare launchers, radar, drones, and siege/artillery pieces.

> Only text/definition files are vendored in this repo. Binary assets (1161 `.mwm`,
> 195 `.dds`, 214 `.wav`) stay in the original zip.

## By the numbers

| Thing | Count |
|---|---|
| Weapons registered (`MasterConfig.cs`) | 82 |
| `*_Weapon.cs` (weapon definitions) | 77 |
| `*_AmmoTypes.cs` (ammo/projectile defs) | 62 |
| `*_Animations.cs` (visual/anim defs) | 36 |
| `.sbc` definition files | 17 (in `Data/`) + `trail.sbc` |
| CubeBlock definitions | 96 |

## Folder layout (vendored text files)

```
aryx-mod/
├─ OVERVIEW.md                      (this file)
├─ Data/
│  ├─ Aryx_AWE_CubeBlocks.sbc       96 blocks (the physical blocks)
│  ├─ Aryx_AWE_Weapons.sbc          vanilla-style <Weapon> defs (mag/sound bridge)
│  ├─ Aryx_AWE_Ammos.sbc            21 <Ammo> defs (game-side ammo)
│  ├─ Aryx_AWE_AmmoMagazines.sbc    magazines (what the block consumes)
│  ├─ Aryx_AWE_Components.sbc       custom build components (e.g. MilitaryPlateAWE)
│  ├─ Aryx_AWE_Blueprints.sbc       + BlueprintClasses / research / categories
│  ├─ Aryx_AWE_TransparentMaterials.sbc  beam/glow materials
│  ├─ Aryx_AWE_Audio.sbc            weapon sound cues
│  ├─ Aryx_AWE_Weapon_Particles.sbc 5.5 MB of particle effect defs
│  └─ Localisation/ (MyTexts.resx, .sbl)
└─ Data/Scripts/CoreParts/
   ├─ MasterConfig.cs               registers all 82 weapons via PartDefinitions(...)
   ├─ AWE_GlobalConfig.cs           global tunables (damage scalar, tracer variation)
   ├─ AryxArmourTypes.cs            custom armour definitions
   ├─ AryxRadar.cs / AryxRadarAmmo.cs  radar subsystem
   ├─ <Weapon>_Weapon.cs            one per weapon: the WeaponDefinition
   ├─ <Weapon>_AmmoTypes.cs         one per weapon: AmmoDef(s)
   ├─ <Weapon>_Animations.cs        one per weapon: AnimationDef(s)
   └─ script/ (PartCompile.cs, Slave.cs, Structure.cs)  the WeaponCore API surface
```

## The three-file-per-weapon convention

Almost every weapon is defined by a trio sharing a prefix:

```
AryxRailgunAres_Weapon.cs      → the WeaponDefinition (mounts, targeting, hardpoint, links Ammo by name)
AryxRailgunAres_Animations.cs  → AnimationDef (firing anim, emissive glow, muzzle particles)
AryxRailgunAres_AmmoTypes.cs   → AmmoDef (trajectory, damage, tracers)   [shared across a family sometimes]
```

`script/Structure.cs` (66 KB) is the **WeaponCore API definition** — every struct
the `.cs` files use (`WeaponDefinition`, `TargetingDef`, `HardPointDef`,
`AmmoDef`, `AnimationDef`, `ArmorDefinition`, …). Treat it as the schema.

## How .cs definitions bind to .sbc blocks (the key link)

1. **Block** in `Aryx_AWE_CubeBlocks.sbc` — e.g. SubtypeId `ARYXGaussCannon`.
   Notably WeaponCore fixed weapons use base type **`ConveyorSorter`** (52 of the
   96 blocks!), turrets use `LargeMissileTurret`/`LargeGatlingTurret`, small-grid
   use `SmallMissileLauncher`/`SmallGatlingGun`.
2. **WeaponDefinition** (`*_Weapon.cs`) — its `Assignments.MountPoints[].SubtypeId`
   matches the block SubtypeId, wiring the WeaponCore behavior onto that block.
3. **Muzzle/aim dummies** — `Muzzles`, `AzimuthPartId`, `ElevationPartId` reference
   named dummies in the block's `.mwm` model.
4. **Ammo** — the HardPoint's `Ammos` list references AmmoDef names from
   `*_AmmoTypes.cs`.
5. **Animations** — `AnimationDef` attaches to the same weapon by name and drives
   emissives/particles on the model's subparts/dummies.

## Block base-type histogram (WeaponCore idiom)

| Base TypeId | Count | Meaning |
|---|---|---|
| `ConveyorSorter` | 52 | fixed/static WeaponCore weapons (sorter block reused as a gun) |
| `LargeMissileTurret` | 33 | large-grid turrets |
| `SmallMissileLauncher` | 5 | small-grid launchers |
| `LargeGatlingTurret` | 3 | large-grid gatling-base turrets |
| `SmallGatlingGun` | 2 | small-grid fixed |

## Where to learn each pattern

- Weapon definition internals → `../weaponcore-weapon-definition.md`
- Ammo & armour → `../weaponcore-ammo-and-armour.md`
- Animations & effects → `../weaponcore-animations-effects.md`
- Framework overview → `../weaponcore-framework.md`
