<!-- Extracted from advanced/animation-engine/ source (Workshop 2880317963, Math0424, Legacy API). Verified against files. 2026-07-20. -->

# Animation Engine (Math0424) — Block Animation Scripting

**Workshop:** `2880317963` · **Author:** Math0424 · **Path:** `Data/Scripts/Math0424/`
(this vendored copy is the **Legacy** API — `.../Math0424/Legacy/`).

Animate block **subparts, emissives, particles, and sounds** in Space Engineers
using a small custom scripting language (**`.bsl`**) bound to a block by
`@BlockId`, reacting to block events (create/working/cockpit/production/…). No C#
required from the mod author — you write `.bsl`, the engine compiles and runs it.

## 1. What it does

Each `.bsl` script targets one block SubtypeId and:
- maps model **subparts** to script handles (`using x as Subpart("Plate0")`),
- reacts to **events** (`action block(){ create(){…} working(){…} }`),
- drives movement (`rotate`, `spin`, `translate`, `scale`), emissive color, and
  particle/sound playback,
- can run **looping functions** each tick via `api.startLoop(...)`.

## 2. The `.bsl` language

**Directives** (top of file):
```
@BlockId "LargeBlockSmallAtmosphericThrust"   // matches a CubeBlock SubtypeId
@Version 2
@Author Math0424
```

**Subpart binding** — bind a model subpart (the `subpart_`-prefixed empty in the
`.mwm`) to a handle:
```
using inner_1 as Subpart("PlateSmall0")
```

**Variables & funcs** — `var x = 0`, arithmetic, and reusable `func`s:
```
func SetPosition() { value = Block.CurrentThrustPercent() ... }
```

**Event blocks** — `action <group>() { <event>() {…} }`. Groups/events seen in the
examples:
```
action Block()      { create() {}  built() {}  working() {}  notworking() {} }
action Cockpit()    { enter() {}  exit() {} }
action production() { startproducing() {}  stopproducing() {} }
```

**Library calls** — `Block.<fn>()`, `api.<fn>()`, plus emissive/emitter/grid libs
(see cheat-sheet). Movement calls are on subpart handles:
`handle.rotate(axis, degrees, ticks, Interp)`.

### Annotated real example — `AnimatedThruster.bsl`
```bsl
@BlockID "LargeBlockSmallAtmosphericThrust"
@Version 2

using inner_1 as Subpart("PlateSmall0")     // bind model subparts to handles
using outer_0 as Subpart("Plate0")
// ... 16 vane subparts total ...

var oldVal = 0
var value  = 0

func SetPosition() {                          // called every 20 ticks by the loop
    value  = Block.CurrentThrustPercent()     // 0..1 how hard the thruster fires
    var setVal = oldVal - value
    oldVal = value
    // rotate each vane around its axis, delta*9 degrees, over 20 ticks, linearly
    inner_1.rotate([-1, 0, 0], setVal * 9, 20, Linear)
    outer_0.rotate([-1, 0, 0], setVal * 9, 20, Linear)
    outer_1.rotate([-0.86, 0.5, 0], setVal * 9, 20, Linear)
    // ...
}

action block() {
    create() {                                // runs when the block spawns
        oldVal = Block.CurrentThrustPercent()
        api.startLoop("SetPosition", 20, -1)  // call SetPosition every 20 ticks, forever (-1)
    }
    built() { oldVal = Block.CurrentThrustPercent() }
}
```
Result: the thruster's iris vanes open/close proportionally to live thrust %.

## 3. Architecture (C#)

- **`AnimationEngine.cs`** — session entry; discovers `.bsl` files and compiles them.
- **Lexer/parser** (`Core/`) tokenize and compile `.bsl` into an executable script;
  `CoreScript.cs` is the per-block runtime instance.
- **`Core.cs`** binds a compiled script to a block via a game-logic component keyed
  on `@BlockId` == block SubtypeId, wires event hooks, and **ticks** active loops
  (`api.startLoop` registrations) and in-flight movements.
- **`Movements.cs` + `Lerp.cs`** implement the tween/interpolation math
  (`Linear` and other curves) for `rotate`/`translate`/`scale` over N ticks.
- **ScriptLibraries** expose the callable functions to `.bsl` (below).

## 4. Adding an animated block

1. Model the block with named **subparts** (`subpart_<name>` empties) in the `.mwm`.
2. Write a `.bsl` whose `@BlockId` == the block's **SubtypeId** in `CubeBlocks.sbc`.
3. `using <handle> as Subpart("<subpartName>")` for each animated part
   (name without the `subpart_` prefix).
4. Put the `.bsl` where Animation Engine scans (alongside the mod's scripts, per the
   engine's loader), and ensure the Animation Engine mod is a dependency.
5. Drive it from event blocks / a looped `func`.

## 5. Cheat-sheet

**Events:** `create`, `built`, `working`, `notworking`, `enter`/`exit` (cockpit),
`startproducing`/`stopproducing` (production).

**Subpart movement (BlockCore):** `rotate`, `rotatearound`, `spin`, `translate`,
`scale`, `resetpos`, `resetrot`, `setresetpos`, `reset`.

**Block state/control (BlockCore):** `currentthrustpercent`, `isworking`,
`isfunctional`, `isoccupied`, `pilot`, `canaccess`, `getgasfilledratio`,
`opendoor`/`closedoor`/`toggledoor`, `lockon`/`lockoff`/`togglelock`,
`poweron`/`poweroff`/`togglepower`, `isarmed`, `iscountingdown`, `detonationtime`,
`productionitemmodel`, `entityhashcode`.

**Emissive:** `setcolor`, `setsubpartcolor`, `setAllSubpartColors`, `brightness`,
`r`/`g`/`b`, `lerp`, `tocolor`, `subparttocolor`, `time`, `actualSubpartName`.

**Emitter:** `playparticle`, `stopparticle`, `playsound`, `stopsound`.

**GridCore:** `getspeed`, `getfuel`, `getnaturalgravity`, `getatmosphericdensity`,
`getplanetaltitude`, `getplanetgroundaltitude`, `getplanetmaxaltitude`,
`iscontrolled`, `isnpc`.

**api:** `startLoop("<func>", <everyTicks>, <count|-1 for infinite>)`, `log`, …

## 6. Reusable snippets

**Emissive pulse while working** (pattern):
```bsl
action block() {
    working()    { api.startLoop("Pulse", 5, -1) }
    notworking() { light.setcolor(0,0,0) }
}
func Pulse() { light.setcolor(0, Emissive.lerp(0.2, 1.0, Emissive.time()), 0) }
```

**Spin a subpart continuously while producing** (pattern):
```bsl
action production() {
    startproducing() { fan.spin([0,1,0], 360, 60, Linear) }  // 360°/60 ticks
    stopproducing()  { fan.resetrot() }
}
```

> Note: this is the **Legacy** API path. A V3 rewrite (typed `let x: float`,
> `struct`s) is sketched in the vendored readme `.txt` but the shipping runtime here
> is V2 as shown above.
