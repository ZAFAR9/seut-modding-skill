# Crash-safety checklist for Space Engineers C# scripts

> **Why this page exists:** a game-logic script with an **unguarded per-frame
> callback** crashed a user's game **instantly on block spawn**. One unhandled
> exception in `UpdateAfterSimulation` (or any per-frame/event method) does **not**
> produce a catchable "script error" — it **crashes Space Engineers to desktop**.
> This checklist is **mandatory** before shipping any `MyGameLogicComponent` /
> `MySessionComponentBase` code. Run every item, every time.

---

## The golden rule

**A mod feature failing should mean "the feature doesn't work" — NEVER "the game
crashes."** Everything below exists to guarantee that.

---

## 1. Wrap every per-frame / event body in try/catch

These callbacks run inside the engine's frame loop. If they throw and nothing
catches it, SE dies. Wrap **all** of them:

- `UpdateAfterSimulation`, `UpdateBeforeSimulation`
- `UpdateAfterSimulation10`, `UpdateAfterSimulation100`
- `UpdateOnceBeforeFrame`
- `AppendingCustomInfo` and any other event handler
- `Close`

On catch: **log once and self-disable** — never rethrow.

```csharp
public override void UpdateAfterSimulation()
{
    if (!_enabled) return;
    try
    {
        // ... per-frame work ...
    }
    catch (Exception e)
    {
        _enabled = false;                              // stop trying
        NeedsUpdate &= ~MyEntityUpdateEnum.EACH_FRAME; // unhook the callback
        MyLog.Default.WriteLineAndConsole("MyMod disabled after error: " + e);
    }
}
```

A `catch { }` around a UI callback like `AppendingCustomInfo` is also fine — a
StringBuilder line is never worth a crash.

## 2. Guard first-frame races

On the very first frames after spawn, the render entity / subparts / session may
not be ready. Check before you touch them:

```csharp
if (Entity == null || Entity.MarkedForClose) return;
if (Entity.Render == null || !Entity.InScene)  return;      // render not ready
if (block?.CubeGrid?.Physics == null)          return;      // projection/ghost
var cam = MyAPIGateway.Session?.Camera;                      // may be null
if (cam == null) return;
```

For subparts, never assume one exists:

```csharp
MyEntitySubpart part;
if (myEntity.Subparts != null &&
    myEntity.Subparts.TryGetValue("MyPart", out part) && part != null)
{ /* safe */ }
```

## 3. Verify every API against real source — don't invent signatures

Before using an API, confirm it in the vendored source under `advanced/`
(BuildInfo, Animation Engine, EliDangHUD, ARYX, etc.). Confirmed-good examples:

| API | Verified signature | Source |
|-----|--------------------|--------|
| Emissive glow | `entity.SetEmissiveParts(string id, Color c, float brightness)` | animation-engine `Emissive.cs` |
| Emissive on subparts | `entity.SetEmissivePartsForSubparts(id, c, brightness)` | animation-engine |
| Get one subpart | `entity.GetSubpart(string name)` → `MyEntitySubpart` | animation-engine |
| Enumerate subparts | `entity.Subparts.TryGetValue(name, out sub)` | buildinfo `Weapon_GatlingTurret.cs` |
| Rotate a subpart | `sub.PositionComp.SetLocalMatrix(ref m, null, true)` | animation-engine `SubpartCore.cs` |
| Per-frame on game-logic | `override void UpdateAfterSimulation()` (valid on `MyGameLogicComponent`) | buildinfo `DebugEvents.cs` |

`grep -rniE 'TheApiName' advanced/` before you write the call.

## 4. Give experimental / visual features a master switch

Any glow / animation / cosmetic feature that depends on model content the user
may not have built yet gets a compile-time off switch, so it can ship disabled:

```csharp
private const bool EnableAnimation = true; // flip to false to ship pure-logic
```

## 5. Optimize: distance-cull + client-only for visual work

Per-frame visuals should never run on a dedicated server or for blocks nobody is
looking at:

```csharp
_animate = EnableAnimation && !MyAPIGateway.Utilities.IsDedicated; // client only
// ...and each frame:
if (Vector3D.DistanceSquared(cam.Position, block.GetPosition()) > 500 * 500) return;
```

Also throttle non-visual logic to `UpdateAfterSimulation10`/`100` when per-frame
precision isn't needed.

## 6. Validate the definition + repackage

Before handing over the mod:

```bash
python3 scripts/sbc_tool.py validate Data/CubeBlocks.sbc
```

Then repackage and deliver.

## 7. Be honest about the compile caveat

We can't compile against a live SE build here, so always tell the user to **F11
twice** in-game to check for script errors and to test the feature. The guards
above mean the **worst realistic case is "the feature is silently off," never a
crash** — that's the whole point.

---

## Copy-paste skeleton

```csharp
[MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "MySubtype")]
public class MyLogic : MyGameLogicComponent
{
    private const bool EnableExtras = true;
    private bool _enabled;
    private IMyCubeBlock _block;

    public override void Init(MyObjectBuilder_EntityBase ob)
        => NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;

    public override void UpdateOnceBeforeFrame()
    {
        try
        {
            _block = Entity as IMyCubeBlock;
            if (_block?.CubeGrid?.Physics == null) return; // skip ghosts
            // ... one-time setup ...
            _enabled = EnableExtras && !MyAPIGateway.Utilities.IsDedicated;
            if (_enabled) NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
        }
        catch (Exception e) { _enabled = false; MyLog.Default.WriteLineAndConsole("Init: " + e); }
    }

    public override void UpdateAfterSimulation()
    {
        if (!_enabled) return;
        try
        {
            if (Entity == null || Entity.MarkedForClose || !Entity.InScene) return;
            // ... per-frame work ...
        }
        catch (Exception e)
        {
            _enabled = false;
            NeedsUpdate &= ~MyEntityUpdateEnum.EACH_FRAME;
            MyLog.Default.WriteLineAndConsole("Update: " + e);
        }
    }
}
```

---

## See also
- `advanced/custom-terminal-detailinfo.md` — DetailInfo pattern (also needs guarded `Close`)
- `advanced/animation-engine.md` — verified subpart/emissive animation APIs
- `how-to/troubleshooting/dead-ports-case-study.md` — debugging methodology


## Whitelist gotchas — decomposing a matrix (NEITHER Decompose is allowed)

The mod whitelist is picky about VRageMath. A blocked API **stops compilation**
with *"'Matrix' does not contain a definition for 'Decompose'"* — even though the
method exists. It's the whitelist hiding it, not a missing using/reference.

**Known trap: BOTH `Matrix.Decompose` (float) AND `MatrixD.Decompose` (double) are
blocked.** Do not reach for the double variant here — it fails the same way. The
robust fix is to **not decompose at all.**

To animate a subpart (spin/rotate) while preserving its baked tilt, flatten-scale
and position: **cache its original local matrix once, then pre-multiply a pure
rotation each frame.** `Matrix.CreateRotationX/Y/Z` and matrix `*` ARE whitelisted:

```csharp
// once:
_diskRest = subpart.PositionComp.LocalMatrixRef;   // holds tilt+scale+pos
if (!_diskRest.IsValid() || _diskRest.IsNan()) { /* disable safely */ }

// each frame — spin in the subpart's OWN local space, keep everything else:
Matrix spin = Matrix.CreateRotationY(angle);
Matrix m = spin * _diskRest;                       // local-space pre-multiply
subpart.PositionComp.SetLocalMatrix(ref m, null, true);
```

Because `_diskRest` already carries the non-uniform scale and tilt, you never need
to separate them out — pre-multiplying a rotation spins the part in its own plane
and leaves scale/tilt/position intact. Confirmed against the SE Matrix API ref.

General rule when a VRageMath method "doesn't exist" at compile: first try the
`D`/double variant, but if that's *also* blocked (as with Decompose), redesign to
avoid it — compose matrices from `Create*` builders + multiply instead.

