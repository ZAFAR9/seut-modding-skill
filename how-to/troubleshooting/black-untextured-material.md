# Case Study: Custom material renders PITCH BLACK in-game (textures not baked into the .mwm)

**Symptom:** A custom material (e.g. `EmissiveAccretion`) previews correctly in
Blender's SEUT material panel, the DDS files are placed in the mod's
`Textures\Models\Cubes\` folder, but in-game the surface is **pure black** — no
color, no glow, nothing.

## Root cause

SE does **not** read the Blender node tree. It reads texture paths from the
material data **baked into the `.mwm`** at export. If SEUT exported the material
with **empty texture paths**, SE has nothing to load → black.

**How to confirm (read the .mwm binary):**
```bash
strings -n 6 MyBlock.mwm | grep -iE "ColorMetalTexture|NormalGlossTexture|AddMapsTexture"
```
Every working material shows a real path, e.g.
`ColorMetalTexture ... Textures\Models\Cubes\BlackMetal_cm.dds`.
If your custom material's name is present elsewhere in the file but there is **no
`..._cm.dds` path line for it**, the textures were NOT baked → this bug.

In the raw bytes the broken material looks like:
`\x11EmissiveAccretion \x00\x00\x00\x00 \x00\x00\x00\x00 \x04MESH` — the name, then
**zero-length** texture-path strings, then the technique. Empty = black.

## Why it happens

- The material's texture slots in Blender pointed at a **temp/Downloads path**, or
  the material was treated as an **external/library** reference, not a SEUT
  local/internal material — so SEUT wrote only the name, not the paths.
- A **plain model export** doesn't always re-bake material texture paths.

## The fix (at export time — you cannot safely patch the .mwm binary)

Patching paths into the `.mwm` after the fact shifts internal offsets (BVH /
sections) and corrupts the file. Fix it in Blender instead:

1. Put the DDS inside the mod at `Textures\Models\Cubes\<Mat>_cm.dds` (+ `_ng`,
   `_add`). Point the material's CM/NG/ADD slots at **those** files (not a
   Downloads/temp path).
2. Assign via the **SEUT Shader Node Group** (not a loose Image Texture node).
3. Set **Technique = MESH**.
4. In the SEUT **Export** panel: click **Convert Textures**, then **Export
   Materials**, *before* **Export All**. This forces SEUT to write the texture
   paths into the material baked into the `.mwm`.
5. Re-export, then re-verify with the `strings` command above — the
   `..._cm.dds` path must now be present.


## ⭐ Decisive test: read the exported MaterialsLib XML

SEUT writes a material-library XML next to the export (e.g. `SAVEME BLACK HOLE.xml`),
`<MaterialsLib>` with one `<Material>` block per material **SEUT actually
exported**. This is the single fastest diagnosis:

```
<MaterialsLib Name="...">
  <Material Name="PaintedMetal_Yellow"> ... has ColorMetalTexture ... </Material>
  <!-- EmissiveAccretion is ABSENT -->
</MaterialsLib>
```

**If your custom material is NOT listed in this XML at all**, SEUT is not treating
it as an exportable custom material — so the `.mwm` gets the material *name* with
empty textures → pitch black. (Vanilla materials can be absent too and still work,
because SE already ships them; your custom one MUST be present.)

## The actual root cause (confirmed over multiple re-exports)

The material was **not set up as a SEUT Custom/Local material**. Assigning a DDS in
a plain Blender Image Texture node, or leaving the material on a preset/library
type, makes SEUT skip it — it never enters the MaterialsLib XML no matter how many
times you re-export or click Export Materials.

**The fix that actually registers it** (do this exactly):
1. Select the object. Open the **SEUT** tab (not just Blender's Material
   Properties).
2. Set the material's **SEUT preset to `Custom`** so the SEUT shader node group
   appears.
3. In that **SEUT node group**, fill **CM Color** with `<Mat>_cm.dds`, **ADD
   Color** with `<Mat>_add.dds` (emissive in the green channel), NG if you have it.
4. Technique = **MESH**.
5. Export. **Confirm** the material now appears in the exported `<MaterialsLib>`
   XML with a `ColorMetalTexture` line. If it's still absent, the preset is still
   not `Custom` — that is the blocker, not the textures.

**Subpart note:** a subpart mesh (`subpart_<name>`) exported inside the main
`.mwm` still needs its material set up the same way. Being a subpart does not
exempt it — its custom material must still be a SEUT Custom material to get
textures baked.


## ⭐⭐ THE root cause (from the official SEUT tutorial + Shader Editor ref)

If your material is **missing entirely from the exported MaterialsLib XML** (only
some other material shows up), it is almost always ONE of these two — confirmed
against the official wiki:

1. **The material is LINKED, not LOCAL.** Official Shader Editor ref: *"If the
   options in this section are greyed out, that is because the currently active
   material is linked into the BLEND-file. Unless the material is made local, it
   cannot be edited directly."* A linked/appended material CANNOT be exported by
   SEUT → it never enters the MaterialsLib. **Fix: Material dropdown ▸ Make Local.**
2. **Textures are not under a `\Textures\` path.** Official tutorial: textures
   must be placed in `[SEUT Assets]\Textures\Custom\<name>\` (or any dir whose
   path contains `\Textures\`) *"so that SEUT can convert the path to a relative
   one."* Textures elsewhere → SEUT drops the material silently.

Also: link textures as **TIF** in the Shader Editor (Blender can't read DDS BC7);
SEUT converts TIF→DDS on export. Then click **Create SEUT Material** to build the
node tree (greyed out = not local yet).

## Emissive specifics

- Material name must start with `Emissive` for SE to treat it as self-lit, and
  the `_add` map's **green channel** is the emissive mask (red channel = AO, keep
  white or it renders dark).
- A game-logic script driving `SetEmissiveParts(name,color,brightness)` rides on
  top of this — but it still needs the base texture baked in, or the surface is
  black regardless.
