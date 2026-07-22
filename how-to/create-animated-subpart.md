# How-To: Create an Animated Subpart (spinning/moving part)

← [Back to how-to index](README.md)

A **subpart** is a separate model piece the game can move independently of the main
block — a spinning accretion disk, a rotating radar dish, opening doors, etc. In
SEUT, subparts are **scene-based**: each subpart is its own Blender *Scene* that
exports to its own `subpart_<Name>.mwm` file, linked to the main model by an empty.

This guide is the distilled result of a long real-world debugging session (the
"BlackHoleContainer accretion disk"). Every step here exists because skipping it
caused a concrete, confusing failure. **Read the whole thing before exporting.**

---

## The mental model (get this right and everything else follows)

You need **two scenes** that hold **different things**:

```
Scene: BlackHoleContainer   (Type = Main)
  └─ main body mesh
       └─ subpart_AccretionDisk        ← an EMPTY (Arrows), NOT the disk mesh
            (Object Data Properties → Subpart Scene → "AccretionDisk")

Scene: AccretionDisk        (Type = Subpart)
  └─ Main (AccretionDisk) collection
       └─ the disk mesh   ← the ACTUAL geometry lives here, and ONLY here
```

- The **Main scene** holds the block + an **empty** named `subpart_AccretionDisk`.
  The empty's *Subpart Scene* dropdown points at the subpart scene.
- The **Subpart scene** (Type = `Subpart`) holds the **real disk mesh** in its Main
  collection. Nothing else — no empty, no build stages, no LODs needed.

That's it. The empty is the *anchor/link*; the subpart scene is the *geometry*.

---

## Steps

### 1. Create the subpart scene
- New scene, name it plainly, e.g. **`AccretionDisk`** (no `subpart_` prefix on the scene).
- In the SEUT panel set **Type = `Subpart`**.
- Click **Recreate Collections** so it has a `Main (AccretionDisk)` collection.

### 2. Put the mesh in the subpart scene
- Add/import the disk mesh **into the `Main (AccretionDisk)` collection** of that scene.
- Assign its material (e.g. `EmissiveAccretion`) via the **SEUT node group** (see
  [create-custom-material](create-custom-material.md) — loose Image Texture nodes get ignored).
- ⚠️ The mesh must be a **real object in this scene**, not a cross-scene instance/preview.
  If the only copy you have is the preview SEUT shows in the Main scene, it is **not**
  real geometry and will be deleted on export (see failure #4 below).

### 3. Add the linking empty in the Main scene
- Switch to your **Main** scene.
- Add an **Empty** (Display As: Arrows/Cube — *not* zero-size Plain Axes), place it at the
  subpart's pivot, name it exactly **`subpart_AccretionDisk`**.
- **Parent it to the main body mesh** (`Ctrl+P → Object (Keep Transform)`) — this clears
  the **W005** warning ("empty has no parent object").
- **Drag it into the Main collection** (parenting ≠ being in the collection).

### 4. Link the empty to the subpart scene
- Select the empty → **Object Data Properties** → **Subpart Scene** → choose `AccretionDisk`.
- This is the single field that ties them together. If it says *None*, nothing works.

### 5. ⭐ EXPORT EACH SCENE INDIVIDUALLY — this is the trick
**Do NOT rely on "Export All Scenes."** In practice that path repeatedly failed to
produce the linked `subpart_*.mwm`. The reliable method:

1. Switch to the **`AccretionDisk` (Subpart)** scene → click **Export Current Scene**.
   → produces `subpart_AccretionDisk.mwm`.
2. Switch to the **`BlackHoleContainer` (Main)** scene → click **Export Current Scene**.
   → produces the main `.mwm` with the subpart **link baked in**.

Exporting each scene on its own, while that scene is active, is what finally generated
both files correctly and with the proper `subpart_` prefix.

---

## Verify the export (no game launch needed)

```bash
# Both mwm files should exist:
ls Models/Cubes/large/ | grep -i mwm
#   BlackHoleContainer.mwm
#   subpart_AccretionDisk.mwm      ← MUST have the subpart_ prefix

# The MAIN model must reference the subpart:
strings -n 6 BlackHoleContainer.mwm | grep -i subpart
#   → should show subpart_AccretionDisk  (if blank, the link didn't bake)

# The subpart model must carry its material:
strings -n 6 subpart_AccretionDisk.mwm | grep -i EmissiveAccretion
```

- File named `AccretionDisk.mwm` **without** the `subpart_` prefix → SEUT didn't treat it
  as a linked subpart (empty/link/scene-type problem — recheck steps 1, 4).
- `grep subpart` on the main model returns **nothing** → the empty wasn't in the Main
  collection or the Subpart Scene link was unset.

Runtime lookup key = the name **with `subpart_` stripped**, i.e. `"AccretionDisk"`
(confirmed vs vendored BuildInfo `part.Source.Substring("subpart_".Length)`), so a
script does `Entity.Subparts.TryGetValue("AccretionDisk", out disk)`.

---

## The failures we actually hit (diagnostic table)

| Symptom | Root cause | Fix |
|---|---|---|
| **W005** "empty has no parent object" | subpart empty not parented to the main mesh | `Ctrl+P → Object (Keep Transform)` |
| Exports as `AccretionDisk.mwm`, **no** `subpart_` prefix, and `grep subpart` on main model is empty | disk mesh + empty both sitting in the **Main** scene; Subpart scene empty | move the mesh into the **Subpart** scene; keep only the empty in Main, linked via Subpart Scene |
| **E002** "Collection 'Main (AccretionDisk)' not found, excluded or empty" + **E016** "Scene could not be exported" | the Subpart scene's Main collection has **no geometry** (or is unticked in the outliner) | put the mesh in that collection / **Recreate Collections** / tick its checkbox |
| **"The mesh in the subpart scene keeps getting deleted when I export"** | the disk existed only as the **preview instance** SEUT injects into the Main scene when you link a subpart empty. Export unlinks/removes that temporary preview → your only copy vanishes (and the unused material gets purged on save) | the real mesh must live **in the Subpart scene**. Leave the Main-scene preview alone — never treat it as your working copy |
| Both files export but disk doesn't spin | script looked up wrong key, or `subpart_*.mwm` absent so `Subparts` dict is empty | verify both `.mwm` exist + `TryGetValue("AccretionDisk")` (prefix stripped) |

---

## Spinning it in script (recap)

Both `Matrix.Decompose` **and** `MatrixD.Decompose` are blocked by the whitelist. To
spin a subpart while preserving its baked tilt + non-uniform scale, cache the rest
matrix once and pre-multiply a pure rotation each frame:

```csharp
// once, after the subpart resolves:
_diskRest = disk.PositionComp.LocalMatrixRef;   // carries tilt+scale+pos

// each frame (inside try/catch, client-only, guarded):
_spinAngle += 0.02f;
Matrix m = Matrix.CreateRotationY(_spinAngle) * _diskRest;
disk.PositionComp.SetLocalMatrix(ref m, null, true);
```

`Create*` builders + matrix multiply ARE whitelisted. See
[scripting/crash-safety-checklist.md](scripting/crash-safety-checklist.md) for the
mandatory try/catch + first-frame guards before shipping.

---

## TL;DR
1. Subpart scene (Type=Subpart) holds the **mesh**; Main scene holds the **empty**.
2. Empty is named `subpart_X`, parented to the body, in the Main collection, and its
   **Subpart Scene** points at the subpart scene.
3. **Export each scene individually** (Export Current Scene) — not Export All.
4. Verify: `subpart_X.mwm` exists (with prefix) **and** `grep subpart` on the main model shows the link.
