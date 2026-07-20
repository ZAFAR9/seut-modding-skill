# How-To: Get Mount Points Right

**Mount points** define *where on each face a block can attach to a neighbor*. Get them
wrong and the block won't snap onto other blocks — or it snaps over empty air where your
model has no geometry.

← [Conveyors & mount points index](README.md) · [Main README](../../README.md)

**On this page:** [what they are](#what-mount-points-are) · [the cell math](#the-cell-math)
· [how large should they be](#how-large-should-a-mount-point-be) ·
[getting them perfect](#getting-mount-points-perfect) · [the SBC owner trap](#one-owner-only)

---

## What mount points are

Each `<MountPoint>` is a **rectangle on one face** of the block's bounding box, measured in
**grid cells**. It marks the area where SE will let another block (or conveyor) attach. A
face with **no** `<MountPoint>` line **cannot** attach to anything.

```xml
<MountPoints>
  <MountPoint Side="Top"    StartX="0" StartY="0" EndX="7" EndY="7" />
  <MountPoint Side="Bottom" StartX="0" StartY="0" EndX="7" EndY="7" />
  ...
</MountPoints>
```

---

## The cell math

- Coordinates are in **grid cells**, measured from a **corner** of that face.
- For a block of size **7×8×7**, each face is a grid whose size = the two axes that make up
  that face:
  - **Top / Bottom** faces = X × Z = **7 × 7** → valid values `0 → 7`.
  - **Front / Back / Left / Right** faces = (X or Z) × Y = **7 × 8** → values `0 → 7` on
    one axis, `0 → 8` on the other.
- `StartX/StartY` = bottom-left corner of the rectangle; `EndX/EndY` = top-right.

**Examples on a 7×7 face:**

| Coverage | XML |
|---|---|
| Full face | `Start 0,0 → End 7,7` |
| Centered 5×5 patch | `Start 1,1 → End 6,6` |
| Only the center cell | `Start 3,3 → End 4,4` |

---

## How large should a mount point be?

The right size is **wherever your model's solid surface actually meets the face** — not
necessarily the whole face.

- If the face is **flat and full** (a boxy block), use the **full face** (`0 → N`).
- If the model is **rounded** (sphere/cylinder body) so the corners are empty, use a
  **centered patch** that covers only the solid area, e.g. a **5×5** on a 7×7 face:
  ```xml
  <MountPoint Side="Top"    StartX="1" StartY="1" EndX="6" EndY="6" />
  <MountPoint Side="Bottom" StartX="1" StartY="1" EndX="6" EndY="6" />
  ```
- On tall side faces (7 wide × 8 tall), a centered patch might be `Start 1,1 → End 6,7`.

**Why this matters:** a full `0→7` mount over a rounded body lets players attach blocks to
**empty air** at the corners. Match the rectangle to the solid footprint.

> If a face carries a **conveyor port**, that face **must** have a mount point covering the
> port location, or the conveyor can't connect. See
> [conveyor-dummies.md](conveyor-dummies.md).

---

## Getting mount points perfect

Hand-math works, but the reliable, WYSIWYG way is **SEUT's built-in Mount Point tool**:

1. Select your **Main** collection / block in Blender.
2. SEUT panel ▸ **Mount Points** section ▸ **Toggle Mount Point Tools**. SEUT creates
   colored helper planes on each face of the bounding box.
3. Drag/scale the **green** planes so each covers exactly the cells where the block should
   attach — matching where your geometry is solid.
4. **Export.** SEUT writes the exact `<MountPoints>` XML into the `.sbc`, perfectly aligned
   to your bounding box. No manual coordinate math.

This is the recommended method precisely because it computes coordinates against the real
bounding box, so they can't drift out of sync with `<Size>`.

---

## One owner only

Don't hand-edit mount points in your `.sbc` **and** let SEUT generate them — on export
SEUT will **overwrite** that section. Pick one owner:

- **(a)** Do mount points in the **SEUT visual tool** and let SEUT own the `<MountPoints>`
  block, **or**
- **(b)** Hand-write them and **don't** let SEUT's auto-`.sbc` win (delete SEUT's
  auto-generated file — see
  [troubleshooting/export-errors.md](../troubleshooting/export-errors.md#seut-writes-its-own-sbc-duplicate-definition)).

Mixing both = they fight and your placement behaves unpredictably.

> Mount-point cells use the **same coordinate system** as `<Size>` and the SEUT bounding
> box — they must all agree. See
> [troubleshooting/size-and-placement.md](../troubleshooting/size-and-placement.md#the-three-that-must-agree).
