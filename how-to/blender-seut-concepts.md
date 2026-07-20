# How-To: Understand the SEUT Building Blocks (LODs, BS, Collision, Mount Points & more)

New to SEUT and drowning in unfamiliar terms? This guide explains every core concept
you meet when modeling a block in Blender — **what it is, why it matters, and how to
do it in SEUT** — then how to import and export cleanly.

Jump to what you need:

- [What are LODs?](#what-are-lods)
- [What are Build Stages (BS1–3)?](#what-are-build-stages-bs13)
- [What is Collision?](#what-is-collision)
- [What are Mount Points?](#what-are-mount-points)
- [What is Icon Render Mode?](#what-is-icon-render-mode)
- [⚠️ Icon Render broken on Blender 5.x? (manual icon workaround)](#-icon-render-broken-on-blender-5x-manual-icon-workaround)
- [What is the Bounding Box?](#what-is-the-bounding-box)
- [What is Mirroring Mode?](#what-is-mirroring-mode)
- [How to properly import](#how-to-properly-import)
- [How to properly export](#how-to-properly-export)

> These are the *concepts*. For the terse field/rule lookup, see
> [`../reference/modeling-reference.md`](../reference/modeling-reference.md) and
> [`../reference/seut-reference.md`](../reference/seut-reference.md).

---

## What are LODs?

**LOD = Level of Detail** — simpler, lower-poly copies of your model that the game
swaps in as the block gets farther from the camera.

- **Why:** performance. A distant block doesn't need full detail, so the game shows
  `LOD1` → `LOD2` → `LOD3` (progressively cruder) to save framerate. Up close it uses
  your full **Main** model.
- **Do you need them?** **No — LODs are optional.** Your block works with just Main.
  Add them only if you'll place many copies and notice frame drops.
- **How in SEUT:** put each lower-detail mesh in its own **LOD1 / LOD2 / LOD3**
  collection. SEUT exports them with automatic distance thresholds.
- **Rule of thumb:** LOD1 ≈ 50% of Main's triangles, LOD2 ≈ 25%, LOD3 a boxy
  silhouette. Bake/simplify — don't just re-use Main.

## What are Build Stages (BS1–3)?

**Build Stages** are the partially-built versions of your block shown *while you weld
it up* (the "under construction" look before it's finished).

- **Why:** SE animates construction — BS1 (just started) → BS2 → BS3 (almost done) →
  Main (complete). Without them the block pops from nothing to finished.
- **How in SEUT:** model each stage in the **BS1 / BS2 / BS3** collections. They're
  referenced in the definition via `<BuildProgressModels>`.
- **Tip:** you already have BS1–3 for your black-hole container — good. Order them
  least→most complete; the last one should look nearly like Main.

## What is Collision?

**Collision** is the invisible simplified shape the physics engine uses for the block
— what you bump into, land on, and shoot.

- **Why:** the render mesh is too complex/expensive for physics. SE uses a separate
  low-poly **convex** collision mesh instead.
- **How in SEUT:** put your collision mesh in the **Collision** collection and assign
  that collection to **Main** (or a BSx). Keep it low-poly and convex; split into
  multiple convex chunks if the shape is concave.
- **Get it wrong and:** you can't place the block, you fall through it, or physics
  behaves strangely.
- ⚠️ Use **either** a linked collision file (`.hkt`) **or** objects in the Collision
  collection — **not both** at once.

## What are Mount Points?

**Mount points** are the faces where a block is allowed to attach to the grid and to
neighbouring blocks.

- **Why:** they tell the game "this side can connect here." No mount points = a block
  that won't stick to anything.
- **How in SEUT:** define them in the **Mountpoints** collection (SEUT has a mount-point
  tool to place/size them per face). They must match the `<MountPoints>` entries in
  your `CubeBlocks.sbc`.
- **Tip:** for a 1×1×1 cargo container you'll typically want mount points on the faces
  you intend to place against — commonly all sides for a freely-placeable block.
- Detail: [`../reference/cubeblocks-reference.md`](../reference/cubeblocks-reference.md).

## What is Icon Render Mode?

**Icon Render Mode** is a SEUT feature that renders the toolbar/inventory **icon** for
your block straight from the model in Blender — so you don't have to make the icon by
hand.

- **Why:** every block needs an `<Icon>` (the little picture in the build menu). Icon
  Render Mode gives you a clean, consistent one from the actual model.
- **How in SEUT:** enable **Icon Render** mode in the SEUT panel; it sets up a camera/
  lighting rig aimed at your block. Frame it, render, and SEUT outputs a `.dds`/`.png`
  icon you point `<Icon>` at in the definition.
- **Tip:** render the icon *after* the model and materials look right, so the icon
  matches the final block.
- ⚠️ **On Blender 5.x, SEUT's Icon Render is broken** (crashes with
  `'Scene' object has no attribute 'node_tree'`) — see the
  [workaround below](#-icon-render-broken-on-blender-5x-manual-icon-workaround).

## ⚠️ Icon Render broken on Blender 5.x? (manual icon workaround)

**Symptom:** SEUT's Icon Render toggle does nothing / produces an **empty (transparent)
icon**, and the Blender **System Console** shows:

```
AttributeError: 'Scene' object has no attribute 'node_tree'
  File "...\space-engineers-utilities\seut_icon_render.py", line 87, in setup_icon_render
    tree = scene.node_tree
SEUT Info: Icon successfully saved to '...\Icons\Scene.dds'. (I018)   ← but it's blank
```

**Cause:** SEUT's icon renderer builds a compositor rig via `scene.node_tree`. In
**Blender 5.x that attribute was removed** (the compositor was reworked), so SEUT
crashes *before* the rig is built and saves an empty frame. Normal `F12` render works
because it never touches that code.

**Real fix:** run SEUT on **Blender 4.x (4.2 LTS / 4.3)** — SEUT's supported version.
You can keep 5.x installed alongside it and do SE work in the 4.x build.

**Workaround (stay on Blender 5.x):** skip Icon Render and make the 128×128 icon by
hand. Two ways — pick whichever you prefer.

### A. Render the icon in Blender (works on any version)

1. **Output Properties** → set **Resolution X = 128, Y = 128** (power-of-two;
   256×256 also fine for crisper icons).
2. **Render Properties** → enable **Film → Transparent** (icons need a see-through
   background).
3. Frame the block with your normal camera, then **F12** to render.
4. In the render window: **Image → Save As** → **PNG** (keeps the alpha).

### B1. Convert the PNG → DDS with `texconv` (command line)

`texconv.exe` is a free Microsoft tool (from the **DirectXTex** GitHub releases). It
doesn't "install" — it's a single `.exe` you drop somewhere (e.g. `C:\GAMMA\`). Open a
terminal **in the folder with your PNG** (type `cmd` in Explorer's address bar), then:

```
texconv -f BC7_UNORM -m 0 -srgb -y -o Textures\GUI\Icons MyBlock.png
```

- `-f BC7_UNORM` → BC7, SE's icon/texture format
- `-m 0` → full mipmap chain
- `-srgb` → icons are colour data, treat as sRGB
- `-y` → overwrite · `-o <dir>` → output folder

If `texconv.exe` isn't on your PATH, call it by full path:
`C:\GAMMA\texconv.exe -f BC7_UNORM -m 0 -srgb -y -o Textures\GUI\Icons MyBlock.png`

### B2. Convert the PNG → DDS in GIMP (no download)

Already have GIMP from texture work? Skip texconv entirely:

1. Open the PNG in GIMP · **Image → Scale Image** → **128 × 128** if needed.
2. **File → Export As** → filename ending in **`.dds`** (e.g. `MyBlock.dds`).
3. In the DDS dialog: **Compression = BC7**, tick **Generate mipmaps** → Export.

### Point the block at your icon

In `CubeBlocks.sbc` (path relative to the mod root):

```xml
<Icon>Textures\GUI\Icons\MyBlock.dds</Icon>
```

> Same texture standard as everything else: **DDS / BC7 / mipmaps**, power-of-two
> size. See [work-with-dds-textures](work-with-dds-textures.md).

## What is the Bounding Box?

The **bounding box** is the cube-grid volume your block is *allowed* to occupy —
its `<Size>` in cubes × the cube size.

- **Cube size:** **Large grid = 2.5 m** per cube, **Small grid = 0.5 m** per cube. A
  1×1×1 large block = a 2.5 m cube.
- **Why it matters:** anything sticking outside the box clips into neighbours or stops
  the block from being placed. Model *inside* the box.
- **How in SEUT:** SEUT can show the bounding box for your chosen size so you can check
  the fit. Set the block's size (and the matching `<Size>` in the `.sbc`) and keep
  geometry within it. Also set the **origin/pivot** correctly so the block seats right.

## What is Mirroring Mode?

**Mirroring Mode** lets SE place a correctly *mirrored* version of your block when the
player builds with symmetry (the mirror planes in Creative), or for inherently
left/right-handed blocks.

- **Why:** without mirror data, asymmetric blocks (ramps, corner pieces, cockpits)
  place wrong or not at all when mirrored.
- **How in SEUT:** use SEUT's **Mirroring** mode to define how the block maps across
  each axis. For a symmetric block (like a plain cube container) this barely matters;
  it's important for asymmetric shapes.
- **Tip:** a symmetrical black-hole cube needs little/no mirroring setup — revisit this
  only if you make handed variants.

---

## How to properly import

Getting geometry *into* your SEUT block scene cleanly:

1. **Start from a SEUT block scene**, don't import into a random file — use SEUT's
   scene/collection setup so Main/LOD/BS/Collision/Mountpoints exist.
2. **Import your mesh** (`File → Import`, e.g. FBX/OBJ) **into the correct collection**
   — the main render mesh goes in **Main**.
3. **Fix scale & orientation immediately:** SE is metre-scale (Large cube = 2.5 m).
   Apply transforms (`Ctrl+A` → All Transforms) so scale = 1.0 and rotation = 0, and
   confirm the model sits inside the bounding box.
4. **Set the origin/pivot** where SEUT expects, so the block seats on the grid.
5. **Assign SE materials** via the **SEUT Shader Editor** (don't rely on generic
   Blender materials — SE needs its own material setup and DDS textures).
6. **Re-parent empties/dummies** (conveyor/interaction) under the right object and put
   them in place (see [conveyors](../reference/conveyors-and-interactions.md)).

> Magenta faces after import = missing texture paths, not a modeling bug. Fix via
> `File → External Data`, and see [fix-voxel-textures](fix-voxel-textures.md).

## How to properly export

Turning your Blender scene into a usable in-game `.mwm`:

1. **Set the block basics** in the SEUT Main Panel: link the **Scene → SubtypeId**,
   set the **grid size** (Large/Small) and the block **Size** in cubes.
2. **Check each collection is populated** as intended: **Main** (required), **LOD1–3**
   (optional), **BS1–3** (if you want build stages), **Collision** (strongly
   recommended), **Mountpoints**.
3. **Verify materials** are SE materials and textures are **2048×2048, DDS/BC7 with
   mipmaps** — the standing texture rule for all model assets.
4. **Export** from the SEUT panel. SEUT writes an FBX and runs **mwmbuilder**, which
   produces the **`.mwm`** in your mod's **`Models/`** folder.
5. **Render the icon** (Icon Render Mode) if you haven't, and point `<Icon>` at it.
6. **Wire it to the definition:** set `<Model>` (and `<BuildProgressModels>`,
   `<Size>`, `<MountPoints>`) in `CubeBlocks.sbc`, then validate:
   ```
   python3 scripts/sbc_tool.py validate Data/CubeBlocks.sbc
   ```
7. **Test** as a local mod in an offline world; place the block and check placement,
   collision, build stages, and the icon.

> Export detail & error codes: [`../reference/seut-reference.md`](../reference/seut-reference.md).
> Export gotchas checklist: [`../reference/modeling-reference.md`](../reference/modeling-reference.md).

---

## Quick reference: the SEUT collections

| Collection | Holds | Required? |
|---|---|---|
| **Main** | the full-detail render mesh | ✅ yes |
| **LOD1 / LOD2 / LOD3** | lower-detail meshes for distance | optional |
| **BS1 / BS2 / BS3** | build-stage (under-construction) models | optional but nice |
| **Collision** | low-poly convex physics shape | strongly recommended |
| **Mountpoints** | where the block attaches to the grid | ✅ effectively yes |
