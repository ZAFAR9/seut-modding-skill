# How-To: Work With DDS Textures (Open, Inspect, Combine)

Practical GIMP/software workflow for SE's packed `.dds` textures.

## Software that reads DDS

- **GIMP 2.10+** (free) — DDS support **built in**, reads/writes BC7. Best free pick.
- **Paint.NET** (free) — DDS via plugin; light and quick for view/tweak.
- **Photoshop** + **Intel Texture Works** plugin — what most SE modders use for
  serious BC7 work.
- **XnView MP** (free) — fast DDS *viewer* (no editing).
- **texconv / texdiag** (free, Microsoft) — CLI; texconv is the converter used
  throughout this skill's `reference/materials-reference.md`.
- **NVIDIA Texture Tools Exporter** (free) — DDS export with proper compression.

## Opening a DDS properly in GIMP

1. **File → Open** → pick the `.dds`.
2. In the **Load DDS** dialog:
   - **Load mipmaps** — leave **unchecked** for editing (you only want full-res base).
   - Leave YCoCg/decode options at default.
3. OK.

**Don't panic at the colors — packed data looks "wrong":**
- `_cm` opens tinted (RGB = color, **Alpha = metalness**).
- `_ng` opens **lavender/periwinkle** — that's the **normal map** (correct!),
  Alpha = gloss.
- `_add` — **Red = AO**, Green = emissive.

To view/edit individual channels: **Windows → Dockable Dialogs → Channels**.

## Exporting back to DDS (GIMP)

**File → Export As** → filename ending `.dds` → in **Export DDS**:
- **Compression:** **BC7** (to match SE standard).
- **Mipmaps:** "Generate mipmaps" for model/voxel textures.

(For exact game-correct output, prefer the `texconv` per-channel commands in the
materials reference — GIMP's generic BC7 is fine for previews and simple maps.)

## Combining the 3 maps to preview "the real texture"

The maps hold different data; you can't just stack them. A true combined look
needs a **3D renderer** (Blender Rendered viewport or in-game) because metalness,
gloss and normals are **live lighting effects** a 2D editor can't compute.

**~80% approximation (color + baked shadow) in GIMP:**
1. Open `_cm`. **Layer → Transparency → Remove Alpha Channel** (drops metalness so
   it won't interfere). This is your albedo.
2. Open `_add` as a new image. **Colors → Components → Decompose** → RGBA. The
   **red** layer is AO. Copy it.
3. Back on the `_cm`, **Edit → Paste As → New Layer**.
4. Set that AO layer's **Mode → Multiply** in the Layers panel.
5. **Image → Flatten Image.** Export as PNG.

If Multiply changes nothing, the AO channel is all white (no occlusion) — valid,
just means no baked shadow.

## What is AO?

**Ambient Occlusion** — a grayscale map of where a surface naturally traps shadow
(cracks, corners). **White = fully lit, black = recessed/shadowed.** Multiplying it
over color gives a grounded, dimensional look. In SE it lives in the **red channel
of the `_add` map**; if there's no AO, that channel should be **white**.

## Upload limit note

Chat upload cap is ~5MB — a 2048 BC7 DDS may exceed it. To have the agent inspect
a texture: scale a copy to 512×512 and export PNG, or zip the DDS. For just fixing
magenta you don't need to upload at all — relink in Blender and use Rendered view.
