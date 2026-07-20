# How-To: Fix SEUT Export Errors & Warnings

The SEUT export runs through Blender's **System Console** (Window ▸ Toggle System
Console). Every line is tagged: `I###` = info, `W###` = warning, `E###` = error. This
page covers the ones you'll actually hit.

← [Troubleshooting index](README.md) · [Main README](../../README.md)

**On this page:** [E016 unparented](#e016--more-than-one-unparented-top-level-object) ·
[W012 DLC material](#w012--dlc-material-warning) ·
[duplicate SBC](#seut-writes-its-own-sbc-duplicate-definition) ·
[reading a clean log](#reading-a-successful-export-log)

---

## `E016` — "more than one unparented top-level object"

**Full message:** `SEUT Error: Scene '<Name>' could not be exported. (E016)`, usually just
below a line complaining about more than one unparented top-level object in a collection.

**What it means:** SEUT requires each exported collection (Main, BS1–3, collision) to
contain **exactly ONE unparented "root" object**. Everything else must be **parented
under that root**. If two or more meshes sit loose at the top level with no parent, SEUT
doesn't know which one is *the* block model and refuses to export.

**Why it happens:** you modeled the block from several separate pieces (shell, inner
geometry, detail bits) and never parented them together, so they're all top-level roots.

### Fix — parent everything under one root

1. In the **Outliner**, look at the collection. Objects with **no parent** are the ones
   not indented under anything.
2. Pick the object that should be the **root** (usually the main body/shell).
3. Select all the *other* pieces **first**, then **Shift-select the root LAST** so the
   root is the active (bright) object.
4. Press **Ctrl + P** ▸ **Object (Keep Transform)**.
5. The Outliner should now show **one** top-level object with the rest nested under it.

**Alternative — join them:** if the pieces don't need to stay separate, select all ▸
**Ctrl + J** to merge into a single mesh. (Only if they share the same material setup and
you don't need them as distinct parts.)

Repeat for **each** collection (Main, BS1, BS2, BS3, collision) that has multiple loose
meshes — every collection needs its single root.

> Tip: if E016 persists, scroll up in the console for the line directly above it — it
> names the offending collection.

---

## `W012` — DLC material warning

**Full message:** `SEUT Warning: Material '<Name>' is a DLC material. Keen requires any
model using it to be DLC-locked. (W012)`

**What it means:** a face on your model uses a **paid-DLC material** (e.g.
`Leather_Dark`, decorative-pack materials). Two consequences:

- Players **without that DLC** see it as a placeholder/error.
- Keen's rules say the **entire block must be flagged DLC-locked** if you keep it.

**Fix (recommended):** swap the DLC material for a **free** equivalent (e.g. plain
`Leather`, or any non-DLC SE material) on the affected face, then re-export. The warning
disappears and the block is usable by everyone.

**If you *want* it DLC-locked:** leave the material and add the DLC lock to the block
definition instead. Otherwise, always clear W012 before publishing.

> It's only a **warning**, not an error — the export still completes. But shipping with it
> means non-DLC players get a broken-looking block.

---

## SEUT writes its OWN `.sbc` — duplicate definition

**The line to watch for:**
`SEUT Info: Entry for '<Name>' has been created / updated in file
'…\Data\CubeBlocks\<Name>.sbc'. (I015)` — and sometimes a second `…_1.sbc (I004)`.

**What's happening:** on export, SEUT **auto-generates a bare-bones `.sbc`** into
`Data\CubeBlocks\` containing only what it can read from the scene (model paths, size,
mount points). It does **not** include hand-authored things like a large
`<InventorySize>`, custom components, or a link to your game-logic script.

**Why it's a problem:** if you *also* have your own hand-written `.sbc` (with the real
definition), you now have **two definitions for the same block** — they conflict and the
mod may fail to load or load the wrong one.

### Fix — keep exactly one definition

- Decide which `.sbc` is the **source of truth** (normally your hand-authored one with the
  full definition + script link).
- **Delete SEUT's auto-generated file(s)** in `Data\CubeBlocks\` (`<Name>.sbc`,
  `<Name>_1.sbc`).
- Make sure your kept `.sbc`'s `<Model>` path matches **exactly** where SEUT exported the
  `.mwm` (e.g. `Models\Cubes\large\<Name>.mwm`).

> Don't hand-edit mount points in your `.sbc` *and* let SEUT own them — pick one owner for
> that section or they'll overwrite each other. See
> [conveyors/mount-points.md](../conveyors/mount-points.md).

---

## Reading a successful export log

A **clean, finished** export ends with these two lines:

```
SEUT Info: FBX and XML files of scene '<Name>' have been compiled to MWM. (I007)
SEUT Info: 1 of 1 scenes successfully exported. (I008)
```

- **`I007`** = the FBX + XML were handed to `mwmbuilder` and your `.mwm` was built. ✅
- **`I008`** = the scene count that succeeded. `1 of 1` (or `N of N`) = everything worked;
  `0 of 1` = it failed (look upward for the `E###`).

You'll also see each collection reported (`Exporting Collection 'Main …'`, `BS1`, `BS2`,
`BS3`) with its own `FBX export starting…` / `export finished` pair — that's normal.

**Rule of thumb:** if you see `I007` + `I008 = all`, and **no `E###`**, the model side is
done. Warnings (`W###`) don't block the build but should be cleared before publishing.
