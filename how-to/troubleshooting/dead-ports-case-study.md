# Case Study: "It places fine but the conveyor ports are dead"

A real debugging walkthrough for a custom **large-grid cargo container** whose block
placed perfectly, sat on the grid correctly, mounted to other blocks — but **would not
accept items through its conveyor ports**. This is the single most confusing SEUT symptom
because *almost everything looks right*.

← [Troubleshooting index](README.md) · [Main README](../../README.md)

**What we ruled out, in order, and what actually fixed it.**

---

## The symptom

- Block places, snaps to the grid, sits flat on the ground — **model is fine**.
- Mount points work — it attaches to other blocks.
- Bounding box / size correct.
- **But:** nothing flows through the conveyor ports; connected blocks aren't recognized.

When the visible geometry is right, the instinct is to keep tweaking the `.sbc`. That's the
trap. Work the list below top-to-bottom.

---

## Step 1 — Don't trust the model size, measure it

We first assumed a sizing/placement bug. Measuring the exported `.glb` settled it:

- Parse the mesh bounds; **cells = dimension ÷ 2.5** (Large grid) or **÷ 0.5** (Small grid).
- Our model measured **17.42 × 18.45 × 17.42 m → ~7 × 7 × 7 cells**. So `<Size x="7" y="7"
  z="7"/>` was correct — an earlier "make it 7×8×7" guess (from a *stale* `.glb`) was wrong.

**Lesson:** re-measure the *current* export before changing `<Size>`. See
[size-and-placement.md](size-and-placement.md).

> Positive-octant alignment (mesh corner at origin, nothing negative) matters for
> **clipping/floating**, but a model that already places flat and mounts correctly is NOT
> suffering from an octant problem — don't chase it. Ours placed fine, so origin wasn't the
> bug even though the raw `.glb` looked centered.

---

## Step 2 — Is it the geometry or a real dummy?

A mesh **named** "Conveyor" is still just geometry. SE detects ports **only** from named
empty dummies (`detector_conveyor_N` / `detector_conveyorsmall_N`). Our `.glb` had meshes
called `Conveyor LG` / `Conveyor LG.001` — **geometry, not dummies**. No dummy → no port.

**Lesson:** confirm you have actual **detector empties**, not just port-shaped mesh. See
[conveyors/conveyor-dummies.md](../conveyors/conveyor-dummies.md).

---

## Step 3 — The dummies exist but don't show in Debug Dummies

Next: the modeler *had* added `detector_conveyor` empties and parented them — yet SEUT's
**Debug Dummies** overlay showed **nothing** on the model, and in-game the ports were still
dead. That combination is diagnostic: **the dummies aren't exporting.**

Causes (any one is enough):

1. **Wrong collection.** SEUT exports **per collection** and only writes empties in the
   **`Main`** collection. Parented-but-in-Scene-root / BS / LOD / collision → **silently
   dropped**. Parenting ≠ collection membership.
2. **Zero-size / Plain Axes empty** → exported as a null dummy the game ignores. Needs
   **Display As = Cube/Arrows** + non-zero scale.
3. **Name not exact** (lowercase `detector_conveyor_1`).
4. **Not parented** to the model.

**Debug Dummies showing nothing == the ports will be dead in-game.** Same root cause.

---

## Step 4 — The fix that actually worked: borrow a proven dummy

Hand-built empties kept getting stripped. The reliable fix was to **steal a working dummy
from a vanilla import** instead of creating one:

1. **Import a vanilla CargoContainer** via SEUT (FBX).
2. **Tear its `detector_conveyor_1` empty off** the model — **Alt + P ▸ Clear Parent and
   Keep Transformation**.
3. **Reposition** it onto your block's port face (Shift + S cursor snapping), keeping
   forward **−Z** pointing **out**.
4. **Parent it to your model** (Ctrl + P ▸ Object) **and drag it into the Main collection**.
5. **Delete the leftover vanilla mesh.**
6. **Duplicate** (Shift + D) for the second port; rename `detector_conveyor_2`; re-orient.
7. **Re-export** — now real dummies land in the `.mwm`, appear in Debug Dummies, and the
   ports connect in-game.

**Why it works:** a vanilla-imported dummy already has the exact empty type, non-zero
scale, and internal properties SEUT/MwmBuilder expect, so it isn't stripped. You copy a
game-proven part instead of gambling on manual setup. Full write-up:
[conveyor-dummies.md → the reliable method](../conveyors/conveyor-dummies.md#-the-reliable-method-borrow-a-working-dummy-from-a-vanilla-import).

---

## Quick triage table

| You observe | Most likely cause | Go to |
|---|---|---|
| Block acts 1×1 / wrong footprint | `<Size>` ≠ mesh cells | [size-and-placement.md](size-and-placement.md) |
| Places over air / floats / clips | mesh off the positive octant | [size-and-placement.md](size-and-placement.md) |
| Ports dead, only port-shaped **mesh** exists | no detector dummy | [conveyor-dummies.md](../conveyors/conveyor-dummies.md) |
| Dummies added but **Debug Dummies shows none** | wrong collection / zero-size / bad name | [conveyor-dummies.md → why dummies don't export](../conveyors/conveyor-dummies.md#why-dummies-dont-carry-over-on-export) |
| Tried everything, still dead | hand-built dummy stripped | [borrow a vanilla dummy](../conveyors/conveyor-dummies.md#-the-reliable-method-borrow-a-working-dummy-from-a-vanilla-import) |

---

## See also

- [conveyors/conveyor-dummies.md](../conveyors/conveyor-dummies.md) — the dummy method in full.
- [conveyors/mount-points.md](../conveyors/mount-points.md) — mount patches on the port face.
- [export-errors.md](export-errors.md) — reading a clean export log, `E016`, the duplicate-`.sbc` trap.
- [size-and-placement.md](size-and-placement.md) — `<Size>` ↔ bounding box ↔ origin rule.
