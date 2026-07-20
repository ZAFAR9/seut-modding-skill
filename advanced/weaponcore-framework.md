# WeaponCore (CoreParts) Framework — Overview

Distilled from the ARYX/AWE mod (`aryx-mod/`). WeaponCore (a.k.a. **CoreParts**) is
the de-facto community weapons framework for Space Engineers. Modders ship weapon
behavior as **C# definition files** that the WeaponCore mod compiles at load — you
don't write your own game-logic component, you fill in WeaponCore's data structs.

## The moving parts

| Piece | File(s) | Role |
|---|---|---|
| **API schema** | `script/Structure.cs` | Defines every struct you fill in (`WeaponDefinition`, `TargetingDef`, `HardPointDef`, `AmmoDef`, `AnimationDef`, `ArmorDefinition`…). Your definition files `using static Scripts.Structure...`. |
| **Compiler glue** | `script/PartCompile.cs`, `script/Slave.cs` | WeaponCore-provided; wires your `Parts` class into the framework. Don't edit. |
| **Registry** | `MasterConfig.cs` | `PartDefinitions(...)` lists every weapon to load. **A weapon not listed here does nothing.** |
| **Global tunables** | `AWE_GlobalConfig.cs` | Mod-wide knobs (e.g. `AWEGlobalDamageScalar`, base damage, tracer variation). Referenced by the ammo/weapon defs. |
| **Per-weapon defs** | `<Name>_Weapon.cs`, `<Name>_AmmoTypes.cs`, `<Name>_Animations.cs` | The actual weapon. |
| **Blocks** | `Data/*.sbc` | The physical CubeBlocks, magazines, components, sounds, particles. |

## C# definition file anatomy

Every definition file is a **partial class `Parts`** in `namespace Scripts`, adding
one property that returns a filled struct:

```csharp
using static Scripts.Structure;
using static Scripts.Structure.WeaponDefinition;
// ... more static usings for nested enums/structs ...

namespace Scripts
{
    partial class Parts   // partial: every file adds to the same Parts class
    {
        WeaponDefinition AryxGaussCannon => new WeaponDefinition {
            Assignments = new ModelAssignmentsDef { ... },
            Targeting   = new TargetingDef        { ... },
            HardPoint   = new HardPointDef         { ... },
            // Ammos, Animations referenced by name
        };
    }
}
```

- The **property name** (`AryxGaussCannon`) is the handle used in
  `MasterConfig.cs` → `PartDefinitions(AryxGaussCannon, ...)`.
- `partial class Parts` is why 200+ files coexist: each contributes members to one
  class the framework instantiates.
- Heavy use of `using static` lets the files write `Accurate`, `Grids`,
  `BlockWeapon` etc. without full qualification.

## Load / register a weapon (the one rule people forget)

Add the property name to `PartDefinitions(...)` in `MasterConfig.cs`. Example
(82 entries in ARYX):

```csharp
PartDefinitions(
    AryxAtlasTurret,
    AryxGaussCannon,
    AryxRailgun,
    // ...
);
```

## Binding a WeaponDefinition to a block

```
CubeBlocks.sbc  <SubtypeId>ARYXGaussCannon</SubtypeId>
        ▲
        │  Assignments.MountPoints[].SubtypeId = "ARYXGaussCannon"
<Name>_Weapon.cs  WeaponDefinition
        │  HardPoint.Ammos → names in <Name>_AmmoTypes.cs
        │  Muzzles / Azimuth / Elevation PartId → dummies in the block's .mwm
        ▼
<Name>_Animations.cs  AnimationDef attaches by weapon name → drives emissive/particle on model subparts
```

Fixed WeaponCore weapons commonly use the **`ConveyorSorter`** base block type
(so they inventory-feed ammo and sit as a functional block); turrets use
`LargeMissileTurret` / `LargeGatlingTurret`. See `aryx-mod/OVERVIEW.md` histogram.

## Vanilla `<Weapon>`/`<Ammo>` SBC vs WeaponCore C#

The mod also ships `Aryx_AWE_Weapons.sbc` and `Aryx_AWE_Ammos.sbc` with vanilla
`<Weapon>`/`<Ammo>` definitions. These provide the **game-side** magazine/sound
bridge and fallbacks; WeaponCore's C# defs are what actually drive behavior for
CoreParts weapons. When inspecting, our tool now enumerates both styles:

```
python3 ../scripts/sbc_tool.py inspect aryx-mod/Data/Aryx_AWE_Weapons.sbc
python3 ../scripts/sbc_tool.py inspect aryx-mod/Data/Aryx_AWE_Ammos.sbc
```

## Customizing safely (matches the "customizable ARYX" intent)

- **Damage/tracers globally:** edit `AWE_GlobalConfig.cs` scalars.
- **One weapon's feel:** edit its `HardPoint.Loading` (RateOfFire, ReloadTime,
  heat) in `<Name>_Weapon.cs`.
- **One weapon's punch/range:** edit its `<Name>_AmmoTypes.cs` (BaseDamage,
  MaxTrajectory, DesiredSpeed, AreaEffect).
- **Add a weapon:** copy a trio, rename the property + files, add the block to
  `CubeBlocks.sbc`, and register the property in `MasterConfig.cs`.
- **Always** keep `.sbc` lowercase and validate with `sbc_tool.py` before testing.

See the deep dives: `weaponcore-weapon-definition.md`,
`weaponcore-ammo-and-armour.md`, `weaponcore-animations-effects.md`.
