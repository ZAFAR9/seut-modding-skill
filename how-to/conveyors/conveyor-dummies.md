# How-To: Add Conveyor Ports (Dummies) So the Connector Is Recognized

**Symptom:** your block exports fine and places, but the **conveyor port isn't recognized**
— items won't flow, and nothing snaps to the port. Almost always this means the block has
conveyor *geometry* but no conveyor *dummy* — or the dummy exists but **never survives the
SEUT export**.

← [Conveyors & mount points index](README.md) · [Main README](../../README.md)

**On this page:** [why it isn't recognized](#why-a-connector-isnt-recognized) ·
[⭐ the reliable method: steal a working dummy](#-the-reliable-method-borrow-a-working-dummy-from-a-vanilla-import) ·
[the named dummies](#the-named-dummy-empties) ·
[manual placement](#manual-alternative-place-a-dummy-from-scratch) · [orientation](#orientation-matters) ·
[parenting](#parenting) · [why dummies dont export](#why-dummies-dont-carry-over-on-export) ·
[checklist](#connector-not-recognized-checklist)

---

## Why a connector isn't recognized

In Space Engineers, conveyor ports are **not** detected from your mesh. A mesh you *named*
"Conveyor" is still just geometry. SE detects ports from **named Empty objects (dummies)**
placed on the block's faces. No dummy → no port → no connection, period.

For a conveyor connection to work, **three things must all line up**:

1. A **named conveyor dummy** on the face (this is what's usually missing).
2. The dummy **oriented** so its forward axis points **out** of the block face.
3. A **mount point** on that same face covering the port (see
   [mount-points.md](mount-points.md)).

And critically: the dummy has to **actually export into the `.mwm`**. Manually-created
Plain Axes empties frequently get **stripped by SEUT/MwmBuilder** even when they're named
right and parented (see [why dummies don't carry over](#why-dummies-dont-carry-over-on-export)).
That failure is what the method below sidesteps entirely.

---

## ⭐ The reliable method: borrow a working dummy from a vanilla import

This is the method that **actually works every time**, because you start from a dummy the
game already accepts — correct type, correct scale, correct internal properties — instead of
hand-building one and hoping SEUT keeps it.

**The idea:** import a vanilla block that already has the conveyor port you want, **tear its
detector dummy off** the imported model, reposition it onto *your* block, parent it, delete
the vanilla mesh. You keep a genuine, export-proven dummy.

### Steps

1. **Import a vanilla block that has the port you need.**
   Use SEUT's FBX import (Space Engineers ▸ Import, or **File ▸ Import ▸ FBX** pointed at a
   vanilla block from the Mod SDK / game files). A **CargoContainer** (large cargo) is the
   easiest source — it has clean large `detector_conveyor_*` dummies. For a small port,
   import a block that has `detector_conveyorsmall_*`.

2. **Find the detector dummy in the Outliner.**
   In the imported hierarchy, locate the empty named `detector_conveyor_1` (or
   `detector_conveyorsmall_1`). This is a real, game-valid dummy — right empty type, right
   size, right orientation baked in.

3. **Tear it off the vanilla model.**
   Select the dummy and **clear its parent but keep transform**:
   **Alt + P ▸ Clear Parent and Keep Transformation**. Now it's a free-standing empty that
   still carries all its correct dummy properties.

4. **Reposition it onto your block's port face.**
   Move/snap it to where your port belongs. The clean way: select the target spot on your
   mesh, **Shift + S ▸ Cursor to Selected**, then select the dummy and
   **Shift + S ▸ Selection to Cursor**. Nudge as needed so it sits on the face.
   Keep its **orientation** so forward (**−Z**) still points **out** of your face
   (a top port faces up, a bottom port faces down).

5. **Parent it to YOUR model.**
   Select the dummy, then **Shift-select your main block object last** (active), and
   **Ctrl + P ▸ Object (Keep Transform)**. Now it belongs to your block.

6. **Put it in your Main SEUT collection.**
   In the Outliner, drag the dummy **into the same `Main` collection** as your block mesh
   (parenting alone isn't enough — SEUT exports per-collection).

7. **Delete the leftover vanilla mesh.**
   Remove the imported cargo-container geometry (and any of its other empties you don't
   need). Keep only the dummy (or dummies) you tore off.

8. **Duplicate for more ports.**
   Need a second port? **Shift + D** the working dummy, rename the copy
   `detector_conveyor_2`, snap it to the next face, re-orient outward. Duplicating a
   known-good dummy keeps all the properties intact.

9. **Export and verify.**
   Re-export. Because these are real dummies, they carry into the `.mwm`. In SEUT's
   **Debug Dummies** display they should now be visible on your model — and in-game the port
   connects.

> **Why this beats making them from scratch:** a vanilla-imported dummy comes with the exact
> empty type, non-zero scale, and properties SEUT/MwmBuilder expect, so it isn't silently
> stripped on export. You're copying a proven part, not gambling on manual setup.

---

## The named dummy empties

However you create them, the names must be **exact**:

| Dummy name | Port type |
|---|---|
| `detector_conveyor_1`, `detector_conveyor_2`, … | **Large** conveyor port (accepts big items) |
| `detector_conveyorsmall_1`, `detector_conveyorsmall_2`, … | **Small** conveyor port |

Number them sequentially per type. Place each dummy **on the face** where the port should
be, at the spot where items enter/exit.

**Example** (a container with top + bottom center ports): one `detector_conveyor_1` at the
top-center of the block, one `detector_conveyor_2` at the bottom-center — both centered on
X/Z, sitting on the respective faces.

---

## Manual alternative: place a dummy from scratch

If you can't import a vanilla block, you can build one by hand — but expect the
[export-stripping gotcha](#why-dummies-dont-carry-over-on-export).

> **Axis note:** glTF/`.glb` is **Y-up**, but Blender is **Z-up**. A port that reads as
> "top/bottom on Y" in a `.glb` is **top/bottom on Z** inside Blender. So a vertical
> container's ports sit on the **+Z (top)** and **−Z (bottom)** faces in Blender.

1. **Add the empty.** Object Mode ▸ **Add ▸ Empty ▸ Cube** (not Plain Axes — see gotcha).
2. **Rename it exactly.** In the Outliner, double-click and type `detector_conveyor_1` —
   lowercase, exact spelling. A typo = no port.
3. **Give it a real size.** In **Object Data Properties**, set **Display As = Cube** (or
   Arrows) and a **non-zero radius/scale** (~0.5–1). Zero-size empties get dropped on export.
4. **Snap it onto the port spot.** Select the mesh/marker where the port belongs,
   **Shift + S ▸ Cursor to Selected**, then select the empty and
   **Shift + S ▸ Selection to Cursor**.
5. **Orient it** so its forward (**−Z**) arrow points **out** of the face (see below).
6. **Repeat** for each port (`detector_conveyor_2`, …).
7. **Parent them** to the main model and **put them in the Main collection**.
8. **Export**, watch for `I007` + `I008` and no `E016`, and check Debug Dummies.

---

## Orientation matters

The dummy's **local forward (−Z) axis must point OUT** through the face. If it's rotated so
forward points *into* the block, SE sees the port facing inward and **won't connect**.

- **See the axis:** set the empty's **Display As = Arrows** — the Z arrow's **tip is +Z**, so
  **−Z (forward)** is the *opposite* direction. Forward must point out of the face.
- **Check by rotation (N-panel ▸ Item ▸ Rotation):** with rotation `0,0,0`, forward (−Z)
  points **straight down** (world −Z). So a **bottom** port is correct at `0,0,0`; a **top**
  port needs **X = 180°** to flip forward to point up.
- Scale the empty to roughly cover the port opening so it reads cleanly.

---

## Parenting

The dummy must **export with the model**, so **parent it to your main block object**:

1. Select the dummy, then **Shift-select the main model object last** (so it's active).
2. **Ctrl + P** ▸ **Object (Keep Transform)**.

Unparented dummies may not export, and loose top-level objects can also trip **E016** on
export — see
[troubleshooting/export-errors.md](../troubleshooting/export-errors.md#e016--more-than-one-unparented-top-level-object).

---

## Why dummies don't carry over on export

If you have the dummies parented and named right but they **still don't appear in-game** (and
SEUT's **Debug Dummies** display shows nothing on the model), it's almost always one of these
— all of which the [vanilla-import method](#-the-reliable-method-borrow-a-working-dummy-from-a-vanilla-import)
avoids:

1. **Wrong collection.** SEUT exports **per collection** and only writes empties that live in
   the **`Main` collection** (the one named after your scene). A dummy that's *parented* to
   the mesh but sitting in the Scene root, a **BS**, **LOD**, or **collision** collection is
   **silently dropped**. Parenting ≠ collection membership — it must be *in* the Main
   collection. This is the #1 cause.
2. **Zero-size / Plain Axes empty.** A freshly-added **Plain Axes** empty at zero scale can
   export as a null-size dummy the game ignores. Set **Display As = Cube/Arrows** and give it
   a real scale.
3. **Name not exact.** Must be lowercase `detector_conveyor_1` / `detector_conveyorsmall_1`.
   `Conveyor LG`, `Detector_Conveyor_1`, spaces, capitals → ignored.
4. **Not parented / loose top-level object.** Unparented empties may not export and can also
   trip `E016`.

**Debug Dummies showing nothing = same root cause as dead ports in-game.** SEUT's overlay
only draws dummies it recognizes in the active Main collection; if they're invisible there,
they're absent from the `.mwm` too.

---

## "Connector not recognized" checklist

Run through this in order:

- [ ] **Named dummy empty** on the face — `detector_conveyor_N` (large) or
      `detector_conveyorsmall_N` (small). ← most common miss
- [ ] Dummy is a **Cube/Arrows empty with non-zero scale** (not zero-size Plain Axes).
- [ ] Dummy **forward axis (−Z) points OUT** of the block face.
- [ ] Dummy **parented** to the main model **and** in the **Main SEUT collection**.
- [ ] Dummy is **visible in SEUT's Debug Dummies** display (if not, it won't export).
- [ ] A **mount point** exists on that face, covering the port location.
- [ ] After re-export, the block's `.mwm` was rebuilt (`I007` + `I008` in the console —
      see [reading a clean log](../troubleshooting/export-errors.md#reading-a-successful-export-log)).
- [ ] In-game: place the block, run a conveyor to the port face, and confirm items flow.

> **Still nothing after all that?** Use the
> [⭐ vanilla-import method](#-the-reliable-method-borrow-a-working-dummy-from-a-vanilla-import) —
> starting from a game-proven dummy removes every failure mode above at once.

> Is this meant to be conveyor-connected at all? If it's a standalone storage block with no
> item flow, you can skip dummies — but then it also can't pull/push through conveyors.
