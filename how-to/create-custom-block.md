# How-To: Create a Custom Block (Model + Definition)

End-to-end for a new buildable block, tying the model to the `.sbc` definition.

## 1. Model it (Blender + SEUT)

- Build in the **Main** collection, sized to the grid: **Large = 2.5m**, **Small =
  0.5m** per cube unit. Keep it inside the bounding box; set the pivot correctly.
- Add **LODs** (LOD1–3) for distance, **Build Stages** (BS1–3) for the welding
  animation, a low-poly **Collision** mesh, and place **Mountpoints**.
- Assign materials via the SEUT Shader Editor.
- **Export** → SEUT runs `mwmbuilder` → `.mwm` lands in your mod's `Models/`.

Deep detail: `../reference/modeling-reference.md`.

## 2. Define it (CubeBlocks.sbc)

Generate a valid skeleton:
```
python3 scripts/sbc_tool.py new-block --subtype MyBlock --name "My Block" --size 1x1x1 --cubesize Large
```
Then fill in:
- `<Id>` (TypeId + SubtypeId — SubtypeId must be unique).
- `<CubeSize>` Large/Small, `<Size>` in cubes.
- `<Model>` → relative path to your `.mwm`.
- `<Components>` → what it's built from (references Component SubtypeIds).
- `<MountPoints>` → where it attaches (matches your model's mount points).
- `<BuildProgressModels>` → the BS models for welding.
- Icon path, display name/description keys, build time, PCU, etc.

Field reference + the full TypeId↔xsi:type table + a minimal working example:
`../reference/cubeblocks-reference.md`.

## 3. Conveyors & interaction (if applicable)

Add conveyor ports and interaction points via **detector_ dummies** in the model
and `<ConveyorLines>` / mount data in the definition. See
`../reference/conveyors-and-interactions.md`.

## 4. Validate & test

```
python3 scripts/sbc_tool.py validate Data/CubeBlocks.sbc
```
Fix any errors, then load as a local mod in an offline world and place the block.

## Common pitfalls

- `.sbc` not lowercase → file silently skipped.
- Model scale wrong / outside bounding box → block clips or won't place.
- Bad collision (non-convex, too high-poly) → placement/physics break.
- SubtypeId typo or duplicate → block missing or overrides the wrong thing.
- Mount points in the XML not matching the model → can't attach to the grid.
