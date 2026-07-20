<!-- Deep dive extracted from aryx-mod/ *_AmmoTypes.cs + AryxArmourTypes.cs, verified against source. 2026-07-20. -->

# WeaponCore Ammo & Armour Pattern (from ARYX/AWE)

Munitions and custom armour in WeaponCore (CoreParts), from the ARYX/AWE mod
(`aryx-mod/`).

---

## 1. AmmoDef structure & weapon binding

Ammo is a `private AmmoDef` property on `partial class Parts`. A weapon's
`Ammos = new[]{ ... }` array references it by C# name.

- **`AmmoMagazine`** — SubtypeId of the physical `AmmoMagazine` SBC item consumed,
  or `"Energy"` for energy weapons.
- **`AmmoRound`** — display name in the terminal (players pick ammo per turret).
- **`HybridRound`** — true = consumes both a magazine and grid power.

```csharp
// AryxLaserArgus_Weapon.cs
Ammos = new[] { AryxArgusLaserAmmo },

// AryxLaserArgus_AmmoTypes.cs
private AmmoDef AryxArgusLaserAmmo => new AmmoDef {
    AmmoMagazine = "Energy",
    AmmoRound = "Argus Laser",
    BaseDamage = (float)(75f * AWEGlobalDamageScalar),   // note: global scalar reuse
};
```

---

## 2. Key ammo sub-blocks

**Trajectory (`TrajectoryDef`)** — `Guidance` (`None`, `Smart`, `Remote`,
`TravelTo`, `DetectSmart`, `DetectFixed`…), `MaxTrajectory` (range / beam length),
`DesiredSpeed` (m/s; keep ≤ ~5100 to avoid voxel phasing; 0 for beams),
`AccelPerSec`, `MaxLifeTime`.

**Shape / physical (`ShapeDef`, `ObjectsHitDef`)** — `Shape` (`LineShape` for
fast rounds, `SphereShape`), `Diameter`, `Mass` (recoil/impact), `Health`
(0 = can't be shot down), `ObjectsHit.MaxObjectsHit` (penetration; 0 = infinite),
`CountBlocks`.

**Damage & blast (`DamageScaleDef`, `AreaOfDamageDef`)** — `BaseDamage` (1 steel
plate ≈ 100 HP), `AreaOfDamage.ByBlockHit` / `.EndOfLife` (`Radius`, `Depth`,
`Damage`, `Falloff`, `Shape`). Falloff options: `NoFalloff`, `Linear`, `Curve`,
`InvCurve`, `Squeeze`, `Pooled`.

**Beams (`BeamDef`)** — `Enable=true` = continuous hitscan laser; `VirtualBeams`
merges visuals into one logical beam for performance.

**AmmoGraphics (`GraphicDef`)** — `ModelName` (missile `.mwm`),
`Lines.Tracer` (`Length`, `Width`, `Color`, `Textures`), `.Segmentation` for
pulsing beams.

**AmmoAudio** — `TravelSound`, `HitSound`, `ShieldHitSound`, `VoxelHitSound`.

**Pattern / fragmentation** — `Fragment` spawns sub-projectiles on death
(`AmmoRound`, `Fragments`, `Degrees`) → used for shotguns, flak, and multi-stage
missiles. `Pattern` cycles through several AmmoDefs in sequence.

**EWAR (`EwarDef`)** — `Enable=true` **replaces** kinetic/AoE damage with an
electronic effect. `Type`: `Emp`, `EnergySink`, `Offense`, `Nav`, `Dot`,
`AntiSmart`, `JumpNull`, `Tractor`/`Pull`/`Push`/`Anchor`. Plus `Strength`,
`Radius`, `Duration` (ticks).

---

## 3. Guided munitions (missiles / torpedoes)

`Trajectory.Guidance = Smart` + `Trajectory.Smarts`:
- **`AccelPerSec`** — booster acceleration (launch slow, speed up).
- **`MaxLateralThrust`** — turn-rate limiter (low = sluggish/heavy torpedo).
- **`Aggressiveness`** — tracking responsiveness.
- **`TrackingDelay`** — delay tracking so it clears the firing ship.

**Multi-stage separation pattern** (realistic launch): Stage-1 booster ammo has a
short `MaxLifeTime` + high gravity to drop clear → on death its `Fragment` spawns
`1` Stage-2 cruise ammo which sets `Guidance=Smart` and accelerates to cruise.

---

## 4. Custom armour (`AryxArmourTypes.cs`)

Maps block SubtypeIds to an `ArmorDefinition`, overriding vanilla armor math:
```csharp
ArmorDefinition AryxHeavierTurretArmour => new ArmorDefinition {
    SubtypeIds = new[]{ "ARYXTyphoonCannon", "ARYXGaussTurret", /* ... */ },
    EnergeticResistance = 1.5f,  // damage divided by this for Energy rounds
    KineticResistance   = 1.5f,  // 1.0 = baseline
    Kind = Heavy,                // matches AmmoDef.DamageScales.Armor.{Light/Heavy/NonArmor}
};
```
Damage flow on hit: block SubtypeId → find `ArmorDefinition` → apply `Kind`
multiplier from the ammo's `Armor` scale → then divide by Kinetic/Energetic
resistance based on the ammo's `DamageType.Base`.

---

## 5. Field cheat-sheet

| Field | Block | Meaning |
|---|---|---|
| `AmmoMagazine` | root | Magazine SubtypeId or `"Energy"` |
| `AmmoRound` | root | Terminal display name |
| `HybridRound` | root | Consumes magazine + power |
| `BaseDamage` | root | Direct block damage (plate ≈ 100 HP) |
| `Beams.Enable` | Beams | Continuous hitscan laser |
| `Guidance` | Trajectory | `None`/`Smart`/`Remote`/… |
| `DesiredSpeed` | Trajectory | Cruise m/s (≤~5100) |
| `MaxTrajectory` | Trajectory | Range / beam length (m) |
| `AccelPerSec` | Trajectory | Accel for slow-launch rockets |
| `MaxObjectsHit` | ObjectsHit | Penetration (0 = infinite) |
| `Fragment.AmmoRound`/`Fragments` | Fragment | Sub-munition / next stage |
| `Ewar.Enable`/`Type` | Ewar | Electronic warfare (disables normal dmg) |
| `EnergeticResistance`/`KineticResistance` | ArmorDef | Custom armor dividers |

---

## Snippets (from real files)

### Piercing kinetic railgun — `AryxRailgunLight_AmmoTypes.cs`
```csharp
private AmmoDef AryxPDRailgunAmmo => new AmmoDef {
    AmmoMagazine = "AryxSmallRailgunMagDef", AmmoRound = "AryxPDRailgunAmmo",
    HybridRound = true, EnergyCost = 0.05f,
    BaseDamage = (float)(15500 * AWEGlobalDamageScalar),
    Mass = 10f, Health = 0, BackKickForce = 500f,
    Shape = new ShapeDef { Shape = LineShape, Diameter = 4 },
    ObjectsHit = new ObjectsHitDef { MaxObjectsHit = 0, CountBlocks = false },  // infinite pierce
    Trajectory = new TrajectoryDef { Guidance = None, DesiredSpeed = 4999, MaxTrajectory = 10000f },
};
```

### Multi-stage guided torpedo (cruise) — `AryxTorpedo_AmmoTypes.cs`
```csharp
private AmmoDef AryxTorpFullSpeed => new AmmoDef {
    AmmoMagazine = "Energy", AmmoRound = "Devastator Torpedo",
    Mass = 3000, Health = 15,   // shootable by PD
    AreaOfDamage = new AreaOfDamageDef { EndOfLife = new EndOfLifeDef {
        Enable = true, Radius = 3f, Damage = 5000f, Depth = 10f, Falloff = Linear, Shape = Round } },
    Trajectory = new TrajectoryDef {
        Guidance = Smart, AccelPerSec = 300f, DesiredSpeed = 300, MaxTrajectory = 10000,
        Smarts = new SmartsDef { Aggressiveness = 0.75f, MaxLateralThrust = 0.1f } },
};
```

### Anti-shield beam — `AryxLaserArgus_AmmoTypes.cs`
```csharp
private AmmoDef AryxArgusLaserAmmo => new AmmoDef {
    AmmoMagazine = "Energy", AmmoRound = "Argus Laser", BaseDamage = (float)(75f * AWEGlobalDamageScalar),
    Shape = new ShapeDef { Shape = LineShape, Diameter = 1 },
    DamageScales = new DamageScaleDef {
        Shields = new ShieldDef { Modifier = 5f },   // 500% vs shields
        DamageType = new DamageTypes { Base = Energy, Shield = Energy } },
    // Beams.Enable = true elsewhere for continuous beam
};
```
