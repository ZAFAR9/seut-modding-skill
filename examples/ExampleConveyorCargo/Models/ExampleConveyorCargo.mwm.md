# Models/ExampleConveyorCargo.mwm — what to build in Blender/SEUT

`.mwm` is a **binary** exported by SEUT; it can't live in a text repo. Produce it
like this (full detail: `../../reference/modeling-reference.md`).

## In Blender (SEUT scene bound to SubtypeId `ExampleConveyorCargo`)

**Main collection** — the visible mesh:
- 1×1×1 **Large** grid → fit inside a **2.5 m** cube. Keep origin at the block center.
- One material slot named exactly **`ExampleCargo_Colorable`** (matches
  `material/ExampleConveyorCargo.xml`).

**Conveyor dummies** (this is what makes the ports — see
`../../reference/conveyors-and-interactions.md`):
- `detector_conveyor1` — on the **+X** face → **large** port.
- `detector_conveyor_small2` — on the **-X** face → **small** port.
- Align each dummy flush to its grid face so it connects to neighbors.
- Optional interaction: `detector_terminal` for the control panel.

**Collision collection** — one low-poly **convex** box (~the block bounds). Assign
the Collision collection to Main. Don't also link a `.hkt` (SEUT error E055).

**LOD collections** — `LOD1`/`LOD2`/`LOD3` (optional but recommended).

**Build-stage collections** — `BS1`/`BS2`/`BS3` → export to the
`ExampleConveyorCargo_BS1/2/3.mwm` referenced in `../Data/CubeBlocks.sbc`.

**Mount points** — paint with SEUT's Mountpoint tool to match the six
`<MountPoint>` faces in the definition.

## Export

SEUT **Export** panel → runs `mwmbuilder` → drops `ExampleConveyorCargo.mwm`
(+ the BS `.mwm`s) into this `Models/` folder. Verify with **F11 debug draw**
in-game that the two conveyor dummies show up on the correct faces.
