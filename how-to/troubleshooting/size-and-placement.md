# How-To: Fix Block Size & Placement (1×1 bug, clipping, multi-cell blocks)

Two of the most common "my block is broken in-game" problems come from the **block size
and the model origin not agreeing**. This page explains both and the one rule that fixes
them.

← [Troubleshooting index](README.md) · [Main README](../../README.md)

**On this page:** [Size vs model](#the-game-uses-size-not-your-model) ·
[off-origin clipping](#off-origin-model--clipping) ·
[the three that must agree](#the-three-that-must-agree) ·
[measuring a model](#measuring-a-model-from-its-glb)

---

## The game uses `<Size>`, NOT your model

**Symptom:** your model is clearly big (say 7×7 cells) in Blender, but in-game it behaves
like a **1×1×1 block** — tiny collision box, snaps to one cell.

**Cause:** a block's grid footprint comes from the **`<Size>` in the `.sbc`**, not from the
mesh. If your definition says:

```xml
<Size x="1" y="1" z="1" />
```

…the game treats it as 1×1×1 no matter how large the mesh is.

**Fix:** set `<Size>` to the block's real cell span:

```xml
<Size x="7" y="8" z="7" />   <!-- example: 7 wide, 8 tall, 7 deep -->
```

Grid cell = **2.5 m on Large grid**, **0.5 m on Small grid**. So cells = model_dimension ÷
cell_size (see [measuring a model](#measuring-a-model-from-its-glb)).

---

## Off-origin model → clipping

**Symptom:** the block **floats off-center** or **clips through the ground/map**, appearing
away from the cell where you actually placed it.

**Cause:** the mesh **origin doesn't match the size box**. SE anchors a block from a
**corner cell** and expects the geometry to sit in the **positive octant** from the SEUT
bounding-box origin — i.e. from `(0,0,0)` toward positive X/Y/Z. If you modeled the block
**centered on the world origin** (geometry running negative→positive, e.g. −8.75 to
+8.75), the game anchors the corner cell and the mesh hangs half-off into space.

This is *the* classic "glitching away from the center block" behavior on multi-cell blocks.

**Fix (in Blender / SEUT):**

1. Set the SEUT **Bounding Box** (SEUT Main panel ▸ Bounding Box X/Y/Z) to match your
   intended `<Size>` (e.g. `7,7,7` or `7,8,7`).
2. **Move the whole model** so it sits **inside** that box, origin at one corner, filling
   toward **positive X/Y/Z** — not centered on origin, not below it.
3. Re-export the `.mwm`.

A Large 7-cell axis = `7 × 2.5 = 17.5 m`, so a 7×8×7 block should occupy the box from
`(0,0,0)` to `(17.5, 20.0, 17.5)` in SEUT space.

---

## The three that must agree

For a multi-cell block to place cleanly, **three things must line up**:

| # | Thing | Where | Must equal |
|---|---|---|---|
| 1 | **`<Size>`** | `.sbc` definition | the block's cell span |
| 2 | **SEUT Bounding Box** | Blender, SEUT Main panel | the same cell span |
| 3 | **Mesh position/origin** | Blender geometry | inside the box, positive octant |

If any one disagrees, you get the 1×1 bug, clipping, or mis-aligned mount points. Change
`<Size>`? Update the bounding box and re-check the mesh position too.

> Mount points are measured in these same cells — see
> [conveyors/mount-points.md](../conveyors/mount-points.md).

---

## Measuring a model from its `.glb`

If you're unsure of the real footprint, export the model as **`.glb`** from Blender and
measure it. The bounding box tells you the exact dimensions and whether it's off-origin.

**Manual math:** cells = dimension (m) ÷ cell size (2.5 Large, 0.5 Small), rounded to the
nearest whole cell.

**Worked example** (a real "black hole container" `.glb`):

- Overall bbox dims ≈ `X 17.42 m, Y 19.63 m, Z 17.42 m`
- On Large grid: X = 17.42 ÷ 2.5 ≈ **7**, Z ≈ **7**, Y = 19.63 ÷ 2.5 ≈ **8**
- So `<Size x="7" y="8" z="7" />` (tall column), **not** 7×7×7.
- Its min corner was `Y ≈ −14.4` (well below origin) → confirmed the **off-origin
  clipping** cause; the mesh needed to be moved into the positive octant.

You can read a `.glb` bounding box with any glTF tool, or from Blender itself: select all,
**N** panel ▸ Item ▸ Dimensions, and check the object's location relative to world origin.

> A `.glb` also lets you confirm your named conveyor **dummies** and their positions before
> export — see [conveyors/conveyor-dummies.md](../conveyors/conveyor-dummies.md).
