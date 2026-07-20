# How-To: Add Conveyor Ports (Dummies) So the Connector Is Recognized

**Symptom:** your block exports fine and places, but the **conveyor port isn't recognized**
— items won't flow, and nothing snaps to the port. Almost always this means the block has
conveyor *geometry* but no conveyor *dummy*.

← [Conveyors & mount points index](README.md) · [Main README](../../README.md)

**On this page:** [why it isn't recognized](#why-a-connector-isnt-recognized) ·
[the named dummies](#the-named-dummy-empties) · [orientation](#orientation-matters) ·
[parenting](#parenting) · [checklist](#connector-not-recognized-checklist)

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

---

## The named dummy empties

Add **Empty objects** (Add ▸ Empty ▸ Plain Axes / Cube) and name them exactly:

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

## Orientation matters

The dummy's **local forward (−Z) axis must point OUT** through the face. If it's rotated so
forward points *into* the block, SE sees the port facing inward and **won't connect**.

- Add the empty, then **rotate it** so its forward arrow points outward from the face
  (e.g. straight up for a top port, straight down for a bottom port).
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

## "Connector not recognized" checklist

Run through this in order:

- [ ] **Named dummy empty** on the face — `detector_conveyor_N` (large) or
      `detector_conveyorsmall_N` (small). ← most common miss
- [ ] Dummy **forward axis points OUT** of the block face.
- [ ] Dummy **parented** to the main model so it exports.
- [ ] A **mount point** exists on that face, covering the port location.
- [ ] After re-export, the block's `.mwm` was rebuilt (`I007` + `I008` in the console —
      see [reading a clean log](../troubleshooting/export-errors.md#reading-a-successful-export-log)).
- [ ] In-game: place the block, run a conveyor to the port face, and confirm items flow.

> Is this meant to be conveyor-connected at all? If it's a standalone storage block with no
> item flow, you can skip dummies — but then it also can't pull/push through conveyors.
