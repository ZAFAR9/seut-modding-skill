# Reference: Custom Modeling (Blender + SEUT)

Source: spaceengineers.wiki.gg SEUT pages (Main Panel, Collisions, MountPoints,
Installation) + verified search. Archived: 2026-07-20.

SEUT is a Blender **4.0+** addon that exports models into SE's **MWM** file format
and generates the material `.xml`. This is the **model/block** pipeline (not voxels,
not scripting).

## Pipeline overview

```
Blender model  →  SEUT collections  →  FBX  →  mwmbuilder  →  .mwm  →  CubeBlocks.sbc <Model>
```

SEUT ships **material libraries** with most vanilla materials so you can preview
them in Blender, and it sources textures from your configured SE install path
(set in Addon Preferences).

## SEUT scene & collections

The **Main Panel** ties a Blender **Scene** to a block **SubtypeId** and manages
collections. A block scene uses these collection types:

| Collection | Holds |
|---|---|
| **Main** | the primary render mesh |
| **LOD1 / LOD2 / LOD3** | progressively lower-detail meshes for distance |
| **BS1 / BS2 / BS3** | build-stage models (shown while welding up the block) |
| **Collision** | Havok collision objects; assigned to Main or a BSx collection |
| **Mountpoints** | mount-point definition helpers |

Collision-specific rule (SEUT error **E055**): you may use **either** an external
linked collision file (`.hkt`) **or** objects in the Collision collection — **not
both** at once.

## Grid sizing & pivot

- **Large grid** cube = **2.5 m**; **Small grid** cube = **0.5 m**.
- The model must fit within the block's bounding box (its `<Size>` in cubes ×
  cube size). Overhang clips or blocks placement.
- Set the **origin/pivot** per SEUT's expectation so the block sits correctly on
  the grid.

## LODs

Lower-detail meshes swap in at distance to save performance. Put each in its LODx
collection; SEUT exports them with distance thresholds. High-poly blocks without
LODs hurt framerate on big builds.

## Build stages (BS)

The partial models shown as a player welds the block from a frame to complete.
Place each stage in BS1/BS2/BS3; they're referenced via `<BuildProgressModels>` in
the definition.

## Collision meshes

- Keep them **low-poly and convex** (Havok). Concave/complex collision breaks
  placement and physics.
- Assign the Collision collection to the Main (or a BSx) collection.
- Bad or missing collision = block you can't place, or you fall through it.

## Mount points

- Define **where the block attaches** to neighbors. SEUT's Mountpoint tool lets you
  paint mount faces; they export to the `<MountPoints>` XML.
- Mount points have optional rules for what can attach (see
  `cubeblocks-reference.md`). No minimum surface area is required, but each mount
  point can carry attach rules.

## Empties / dummies

- **`subpart_<name>`** — animated/movable sub-models (rotor heads, doors, pistons).
  Linked at the empty; the engine animates relative to the parent.
- **`detector_*`** — conveyor ports and interaction points (see
  `conveyors-and-interactions.md`).
- Empties export with the model; their transform defines placement/orientation.

## Export

Use the SEUT **Export** panel. SEUT writes FBX + material `.xml`, then runs
**mwmbuilder** to produce the `.mwm` in your mod's `Models/` folder. Reference that
path from `<Model>` in the block definition.

## Common pitfalls

- **Scale wrong** — model not matching 2.5m/0.5m grid → clipping / won't place.
- **Non-manifold / bad geometry** — export or render artifacts.
- **Too many triangles, no LODs** — performance tanks.
- **Missing/complex collision** — placement & physics break (or E055 if you have
  both `.hkt` and collision objects).
- **Wrong pivot/origin** — block offset on the grid.
- **Materials magenta** — missing/unlinked textures (see
  `../how-to/fix-voxel-textures.md`).
