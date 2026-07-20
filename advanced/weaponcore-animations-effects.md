<!-- Deep dive extracted from aryx-mod/ *_Animations.cs + TransparentMaterials.sbc, verified against source. 2026-07-20. -->

# WeaponCore Animations & Visual Effects Pattern (from ARYX/AWE)

How model animations and visual effects are driven in WeaponCore, as implemented
in the ARYX/AWE mod (`aryx-mod/`).

---

## 1. Animations file structure

`*_Animations.cs` files in `Data/Scripts/CoreParts/` add an `AnimationDef` property
to `partial class Parts`:

```csharp
using System.Collections.Generic;
using static Scripts.Structure.WeaponDefinition;
using static Scripts.Structure.WeaponDefinition.AnimationDef;
using static Scripts.Structure.WeaponDefinition.AnimationDef.PartAnimationSetDef.EventTriggers;
using static Scripts.Structure.WeaponDefinition.AnimationDef.RelMove.MoveType;
using static Scripts.Structure.WeaponDefinition.AnimationDef.RelMove;

namespace Scripts
{
    partial class Parts
    {
        private AnimationDef AryxExampleAnims => new AnimationDef
        {
            AnimationSets = new[] { /* PartAnimationSetDef ... */ }
        };
    }
}
```

**Attach to a weapon** in its `*_Weapon.cs`:
```csharp
Animations = AryxLongbowAnims,   // the AnimationDef property name
```

---

## 2. Key concepts

### EventTriggers (weapon-state hooks)
`PreFire` (wind-up/charge), `Firing`, `StopFiring` (spin-down/return), `Reloading`,
plus others. Animations and particles are keyed to these.

### AnimationSets & subparts
`AnimationSets` is an array of `PartAnimationSetDef`, each controlling one model
subpart:
```csharp
new PartAnimationSetDef {
    SubpartId = Names("barrel_right"),   // binds to model subpart_barrel_right
    BarrelId  = "muzzle_projectile_2",   // limit to this muzzle ("Any" = all)
    Loop        = Events(Firing),        // loop firing anim
    TriggerOnce = Events(PreFire, Firing, StopFiring),
    EventMoveSets = new Dictionary<EventTriggers, RelMove[]> { ... },
}
```

### Emissive color driving
`Emissives` cycle color/intensity on a named material mesh (heat/charge glow).
Often left to WeaponCore's built-in heat visuals, but can be scripted:
```csharp
Emissive("PowerUp",
   Colors: new[]{ Color(0,.051f,.051f,.05f), Color(0,1,1,1) },
   IntensityFrom:0, IntensityTo:1, CycleEmissiveParts:false,
   LeavePreviousOn:true, EmissivePartNames: new[]{ "Emissive3" })
```

### Particle events
`EventParticles` place SE particles on dummies/muzzles per trigger:
```csharp
EventParticles = new Dictionary<EventTriggers, EventParticle[]>
{
   [PreFire] = new[]{ new EventParticle {
       EmptyNames  = Names("muzzle_projectile_1"),
       MuzzleNames = Names("muzzle_projectile_1"),
       Particle = new ParticleDef {
           Name = "Aryx_Railgun_windup_effect",  // Particle SubtypeId from the SBC
           Extras = new ParticleOptionDef { Loop=true, MaxDistance=1000, Scale=1 }
       }
   }},
}
```

### Barrel / RelMove animations
Movement is arrays of `RelMove` steps: translation (`LinearPoints`), rotation
(`Rotation`), a speed curve (`MovementType`), and duration (`TicksToMove`, 60=1s).

---

## 3. Connecting model dummies to animation defs

1. **`SubpartId`** — the model subpart is named `subpart_<name>`; in code the prefix
   is stripped: `Names("barrel_left")` ↔ `subpart_barrel_left`.
2. **`BarrelId`** — restricts an animation to a specific muzzle dummy; `"Any"` =
   all barrels.
3. **`CenterEmpty`** — a dummy used as the rotation pivot (e.g. gatling spin axis);
   `""` = rotate around the subpart's own origin.

---

## 4. Emissive materials & TransparentMaterials

Beams/tracers/flares use `<TransparentMaterials>` defs
(`Aryx_AWE_TransparentMaterials.sbc`):
```xml
<TransparentMaterial>
  <Id><TypeId>TransparentMaterialDefinition</TypeId>
      <SubtypeId>AryxPulseLaserEffect</SubtypeId></Id>
  <CanBeAffectedByOtherLights>false</CanBeAffectedByOtherLights>  <!-- fully self-lit / glows -->
  <Texture>Textures\Particles\laser_diffuse_1.dds</Texture>
  <UVSize><X>1</X><Y>1</Y></UVSize>
</TransparentMaterial>
```
- `CanBeAffectedByOtherLights=false` is what makes it glow in the dark (renders
  emissive, unaffected by scene lighting).
- AWE frame-animates bolts via indexed materials (`AryxReaperBolt1`..`5`).
- Laser beams = a tileable diffuse texture on a WeaponCore-stretched polygon.

---

## 5. Field cheat-sheet

| Field | Type | Meaning |
|---|---|---|
| `SubpartId` | `string[]` (Names) | Target subparts (strips `subpart_`) |
| `BarrelId` | `string` | Muzzle that triggers the anim ("Any"=all) |
| `TicksToMove` | int | Phase duration (60 = 1s) |
| `MovementType` | MoveType | `Linear`, `ExpoDecay`, `ExpoGrowth`, `Delay`, `Show`, `Hide` |
| `LinearPoints` | XYZ[] | Translation over time (Transformation(x,y,z)) |
| `Rotation` | XYZ | Rotation in degrees (pitch,yaw,roll) |
| `CenterEmpty` | string | Pivot dummy for rotation ("" = own origin) |

---

## Snippets (from real files)

### Gatling spin (up/loop/down) — `AryxGatlingVulcan_Animations.cs`
```csharp
Loop = Events(Firing),
TriggerOnce = Events(PreFire, Firing, StopFiring),
EventMoveSets = new Dictionary<EventTriggers, RelMove[]> {
  [PreFire]   = new[]{ new RelMove{ TicksToMove=3, MovementType=ExpoGrowth, Rotation=Transformation(0,0,60f) } },
  [Firing]    = new[]{ new RelMove{ TicksToMove=2, MovementType=Linear,     Rotation=Transformation(0,0,30f) } },
  [StopFiring]= new[]{ new RelMove{ TicksToMove=3, MovementType=ExpoDecay,  Rotation=Transformation(0,0,60f) } },
}
```

### Recoil + recovery — `AryxBattleCyclone_Animations.cs`
```csharp
[Firing] = new[]{
  new RelMove{ TicksToMove=5,  MovementType=ExpoDecay, LinearPoints=new[]{ Transformation(0,0,0.25f) } }, // kick back
  new RelMove{ TicksToMove=5,  MovementType=Delay,     LinearPoints=new[]{ Transformation(0,0,0f) } },     // hold
  new RelMove{ TicksToMove=70, MovementType=Linear,    LinearPoints=new[]{ Transformation(0,0,-0.25f) } }, // return
}
```

### Charge particle on PreFire — `AryxRailgunAres_Animations.cs`
```csharp
[PreFire] = new[]{ new EventParticle {
    EmptyNames=Names("muzzle_projectile_1"), MuzzleNames=Names("muzzle_projectile_1"),
    Particle = new ParticleDef { Name="Aryx_Gauss_Windup_Effect",
        Extras = new ParticleOptionDef { Loop=true, MaxDistance=1000, MaxDuration=120, Scale=1 } } } }
```
