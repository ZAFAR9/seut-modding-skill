<!-- Deep dive extracted from aryx-mod/ CoreParts files, verified against source. 2026-07-20. -->

# WeaponCore Weapon Definition Pattern (from ARYX/AWE)

This reference outlines the C# patterns and structural architecture used in
**WeaponCore (CoreParts)** weapon definitions, based on production configs from the
**Aryx Weapon Enterprises (AWE)** mod (`aryx-mod/`).

---

## 1. Overall C# structure

WeaponCore uses a **`partial class Parts`** architecture. Every weapon/ammo/anim is
a property on that shared class, spread across many files, compiled together.

Each weapon file opens with static usings from `Scripts.Structure`, which make
nested enums/structs (`Grids`, `Accurate`, `BlockWeapon`, …) directly usable:

```csharp
using static Scripts.Structure;
using static Scripts.Structure.WeaponDefinition;
using static Scripts.Structure.WeaponDefinition.ModelAssignmentsDef;
using static Scripts.Structure.WeaponDefinition.HardPointDef;
using static Scripts.Structure.WeaponDefinition.HardPointDef.Prediction;
using static Scripts.Structure.WeaponDefinition.TargetingDef.BlockTypes;
using static Scripts.Structure.WeaponDefinition.TargetingDef.Threat;
using static Scripts.Structure.WeaponDefinition.HardPointDef.HardwareDef;
using static Scripts.Structure.WeaponDefinition.HardPointDef.HardwareDef.HardwareType;

namespace Scripts
{
    partial class Parts
    {
        WeaponDefinition MyWeaponProperty => new WeaponDefinition
        {
            Assignments = new ModelAssignmentsDef { ... },
            Targeting   = new TargetingDef        { ... },
            HardPoint   = new HardPointDef         { ... },
            Ammos       = new [] { ... },
        };
    }
}
```

### Registration lifecycle (`MasterConfig.cs` + `PartCompile.cs`)

- `MasterConfig.cs` holds the `Parts()` constructor which calls
  `PartDefinitions(Weapon1, Weapon2, ...)` listing **every** weapon property.
- `PartCompile.cs` (framework-provided) stores those into a `ContainerDefinition`
  that WeaponCore reads at compile. **A weapon not listed in `PartDefinitions` does
  nothing** — this is the #1 "my weapon isn't showing up" cause.

---

## 2. Major WeaponDefinition sub-blocks

### Assignments (`ModelAssignmentsDef`)
Maps the logical weapon onto the model's subparts/dummies.
- **`MountPoints[].SubtypeId`** — links to the block's SubtypeId in `CubeBlocks.sbc`.
- **`SpinPartId`** — continuously-spinning subpart (gatling barrels).
- **`MuzzlePartId` / `AzimuthPartId` / `ElevationPartId`** — subparts controlling
  muzzle vector, horizontal rotation, vertical elevation. `"None"` for fixed guns.
- **`Muzzles[]`** — exact muzzle dummy names in the `.mwm` (e.g. `"muzzle_projectile_1"`).
- **`DurabilityMod`** (on MountPoint) — damage-taken multiplier (0.25f = 25%).

### Targeting (`TargetingDef`)
- **`Threats[]`** — what to auto-engage: `Grids`, `Projectiles`, `Characters`, `Meteors`.
- **`SubSystems[]`** — block-priority order: `Thrust, Utility, Offense, Power, Production, Any`.
- **`MaxTargetDistance` / `MinTargetDistance`** — engage range in meters (0 = unlimited).
  MinTargetDistance is reused on torpedoes to prevent self-damage.
- **`TopTargets` / `TopBlocks`** — randomize among N targets/blocks so weapons don't
  all hit the same block.

### HardPoint (`HardPointDef`)
The core physical/behavioral block:
- **`PartName`** — terminal display name.
- **`DeviateShotAngle`** — spread in degrees.
- **`AimingTolerance`** — alignment window (deg) before firing is allowed.
- **`AimLeadingPrediction`** — `Off | Basic | Accurate | Advanced`.
- **`Ui` (UiDef)** — which terminal sliders exist (RateOfFire, DamageModifier,
  ToggleGuidance, EnableOverload).
- **`Ai` (AiDef)** — `TrackTargets`, `TurretAttached`, `TurretController`,
  `PrimaryTracking`, `LockOnFocus`.
- **`HardWare` (HardwareDef)** — `RotateRate`/`ElevateRate` (rad/tick), azimuth/
  elevation limits, `InventorySize` (kL), `Type` (`BlockWeapon | HandWeapon | Phantom`),
  optional `CriticalReaction` (warhead behavior).
- **`Loading` (LoadingDef)** — `RateOfFire` (RPM; 3600 = continuous beam), `ReloadTime`,
  `DelayUntilFire` (charge/wind-up), `ShotsInBurst`/`DelayAfterBurst`, heat model
  (`HeatPerShot`, `MaxHeat`, `Cooldown`, `HeatSinkRate`, `DegradeRof`).
- **`Audio` (HardPointAudioDef)** — `PreFiringSound`, `FiringSound`,
  `FiringSoundPerShot`, `ReloadSound`, `FireSoundEndDelay`.
- **`Graphics` (HardPointParticleDef)** — muzzle-flash/other particle effects.
- **`Other` (OtherDef)** — restriction radius, energy priority, part caps.

### Linking ammo
```csharp
Ammos = new [] { AryxAutocannonAmmoWC },  // names of AmmoDef properties in *_AmmoTypes.cs
```
Because ammo is a property on the same `partial class Parts`, it's referenced
directly by name.

---

## 3. Turret vs fixed weapon

| Area | Turret | Fixed / static |
|---|---|---|
| Assignments | `AzimuthPartId`/`ElevationPartId` = real subparts | both = `"None"` |
| `Ai` | `TrackTargets/TurretAttached/TurretController = true` | all `false` |
| `HardWare` speeds | `RotateRate`/`ElevateRate` > 0 (rad/tick) | `0f` |
| `HardWare` arcs | real `Min/MaxAzimuth`, `Min/MaxElevation` | all `0` |

---

## 4. Field cheat-sheet

| Field | Location | Meaning |
|---|---|---|
| `SubtypeId` | Assignments.MountPoints | Links C# def ↔ SBC block |
| `DurabilityMod` | MountPoints | Damage-taken multiplier (0.25 = 25%) |
| `Threats` | Targeting | Target classes to auto-engage |
| `MaxTargetDistance` | Targeting | Max range (m), 0 = unlimited |
| `MinTargetDistance` | Targeting | Min range (m); prevents self-damage |
| `DeviateShotAngle` | HardPoint | Spread in degrees |
| `AimingTolerance` | HardPoint | Alignment window (deg) to allow firing |
| `AimLeadingPrediction` | HardPoint | Off/Basic/Accurate/Advanced |
| `DelayCeaseFire` | HardPoint | Ticks to hold lock after trigger release |
| `InventorySize` | HardWare | Ammo inventory (kL) |
| `RotateRate`/`ElevateRate` | HardWare | Turret speeds (rad/tick) |
| `RateOfFire` | Loading | RPM (3600 = continuous beam) |
| `ReloadTime` | Loading | Reload duration (ticks; 60 = 1s) |
| `DelayUntilFire` | Loading | Charge/wind-up delay (ticks) |
| `ShotsInBurst`/`DelayAfterBurst` | Loading | Burst count / cooldown |
| `HeatPerShot`/`MaxHeat`/`HeatSinkRate` | Loading | Overheat model |

---

## 5. Reusable snippets (from real files)

### Turret with thermal control — `AryxAutocannonChordTurret_Weapon.cs`
```csharp
WeaponDefinition AryxAutocannon => new WeaponDefinition {
    Assignments = new ModelAssignmentsDef {
        MountPoints = new[] {
            new MountPointDef {
                SubtypeId = "ARYX_ChordAutocannon",
                MuzzlePartId = "MissileTurretBarrels",
                AzimuthPartId = "MissileTurretBase1",
                ElevationPartId = "MissileTurretBarrels",
                DurabilityMod = 0.2f,
            },
        },
        Muzzles = new [] { "muzzle_projectile_1" },
    },
    HardPoint = new HardPointDef {
        PartName = "AC-54 Chord",
        AimLeadingPrediction = Advanced,
        Ai = new AiDef { TrackTargets = true, TurretAttached = true,
                         TurretController = true, PrimaryTracking = true },
        HardWare = new HardwareDef {
            RotateRate = 0.05f, ElevateRate = 0.05f,
            MinAzimuth = -180, MaxAzimuth = 180, MinElevation = -5, MaxElevation = 65,
            InventorySize = 0.150f, Type = BlockWeapon,
        },
        Loading = new LoadingDef {
            RateOfFire = 180, ReloadTime = 150,
            MaxHeat = 70000, Cooldown = .95f, HeatSinkRate = 9000,
        },
    }
};
```

### Fixed weapon with charge wind-up — `AryxRailgunAres_Weapon.cs`
```csharp
HardPoint = new HardPointDef {
    PartName = "Ares 1200mm Heavy Railgun",
    AimLeadingPrediction = Accurate,
    Ai = new AiDef { TrackTargets = false, TurretAttached = false,
                     TurretController = false, PrimaryTracking = false },
    HardWare = new HardwareDef { RotateRate = 0f, ElevateRate = 0f, InventorySize = 6f },
    Loading = new LoadingDef { RateOfFire = 30, ReloadTime = 600, DelayUntilFire = 390 },
    Audio = new HardPointAudioDef {
        PreFiringSound = "ArcWepShipARYXGauss_Windup",
        FiringSound = "ArcWepShipARYXGauss_Fire", FiringSoundPerShot = true,
    },
};
```

### Continuous beam / burst — `AryxCenturionIonCannon_Weapon.cs`
```csharp
Loading = new LoadingDef {
    RateOfFire = 3600,        // continuous
    ReloadTime = 1800, DelayUntilFire = 1,
    HeatPerShot = 100, MaxHeat = 24400, Cooldown = 0.5f, HeatSinkRate = 900,
    ShotsInBurst = 121, DelayAfterBurst = 1800, FireFull = true, GiveUpAfter = true,
},
Ammos = new[] { AryxLargeIonCannonBeam, AryxLargeIonCannonPulse,
                AryxLargeIonCannonEMP, AryxLargeIonCannonJumpDisruption },
```

### Guided torpedo launcher — `AryxHyperionTorpedo_Weapon.cs`
```csharp
Targeting = new TargetingDef {
    Threats = new[] { Grids },
    MaxTargetDistance = 12500, MinTargetDistance = 1000,  // safe-arm distance
},
HardPoint = new HardPointDef {
    PartName = "Hyperion Torpedo Launcher",
    Ui = new UiDef { ToggleGuidance = true },  // HUD guidance toggle
    Loading = new LoadingDef { RateOfFire = 30, ReloadTime = 1200, ShotsInBurst = 1 },
},
Ammos = new [] { AryxHyperion_ThaneAmmo, AryxHyperion_JavelinAmmo, AryxHyperion_BreacherAmmo },
```
