<!-- EliDangHUD (Elite-Dangerous-style HUD), Workshop 3252520404. C# mod, source vendored at advanced/elidanghud/ (text only; 67 .dds, 29 .wav, .png, bin/obj excluded). 2026-07-20. -->

# EliDangHUD — Elite-Dangerous-Style Holographic HUD

**Workshop `3252520404`** · A cockpit HUD mod that overlays an *Elite Dangerous*-style
holographic interface — radar, targeting reticles, compass, velocity vectors, planet
info, toolbar, and animated dust/visor effects — while you pilot.

**Source:** [`elidanghud/`](elidanghud/) — text only. The mod's 67 `.dds` HUD textures,
29 `.wav` UI sounds, thumbnail, and `bin/obj` build output are excluded via `.gitignore`.

## Files (what to read)

| File | What it is |
|---|---|
| [`CircleRenderer.cs`](elidanghud/Data/Scripts/EliDangHUD/EliDangHUD/CircleRenderer.cs) | **The whole HUD** (~4900 lines). A `MySessionComponentBase` that draws everything each frame. **Start here.** |
| [`CustomCockpitLogic.cs`](elidanghud/Data/Scripts/EliDangHUD/EliDangHUD/CustomCockpitLogic.cs) | A `MyGameLogicComponent` on cockpits — adds the terminal controls/toggles that turn the HUD on per-cockpit. |
| [`GridHelper.cs`](elidanghud/Data/Scripts/EliDangHUD/EliDangHUD/GridHelper.cs) | Helpers: grid/target scanning, math utilities used by the renderer. |
| [`ED_extras.cs`](elidanghud/Data/Scripts/EliDangHUD/EliDangHUD/ED_extras.cs) | Small extras/stub. |
| [`Data/TransparentMaterials_ED.sbc`](elidanghud/Data/TransparentMaterials_ED.sbc) | **Defines every HUD material** (`ED_Border`, `ED_Compass`, `ED_Targetting`, `ED_Circle*`, `ED_visor`, dust, etc.) that the code draws. |
| [`Data/Audio_ED.sbc`](elidanghud/Data/Audio_ED.sbc) | Defines the UI sound cues (beeps, zoom, target lock/lost). |

## How it works — the pattern that matters

This mod is the reference for **drawing a custom HUD / 3D-world billboards from a mod**
(no LCD, no model — it draws directly into the view each frame).

1. **Session component drives rendering.** `CircleRenderer` is decorated
   `[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation | MyUpdateOrder.AfterSimulation)]`
   and runs every tick, drawing while the player is in a cockpit.
2. **Everything is a billboard.** HUD elements are drawn with the transparent-geometry
   / billboard API using `MyStringId` material handles cached up front, e.g.:
   ```csharp
   private MyStringId MaterialCompass  = MyStringId.GetOrCompute("ED_Compass");
   private MyStringId MaterialCross    = MyStringId.GetOrCompute("ED_Targetting");
   private MyStringId MaterialLockOn   = MyStringId.GetOrCompute("ED_LockOn");
   // ...drawn via MyTransparentGeometry.AddBillboard*/AddPointBillboard with a BlendTypeEnum
   ```
   **Each of those names must exist as a `<TransparentMaterial>` in
   `TransparentMaterials_ED.sbc`** pointing at a `.dds` — that's the binding between
   the C# draw calls and the art. This is the key lesson: *cache `MyStringId` material
   ids, define matching transparent materials, then draw billboards.*
3. **Helper data classes** (`RadarPing`, `PlanetInfo`, `VelocityLine`, `RadarAnimation`,
   `DustParticle`) hold per-frame HUD state — a clean way to structure a complex overlay.
4. **Per-cockpit toggle** via `CustomCockpitLogic` (`[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Cockpit), false)]`)
   adds terminal controls in `UpdateOnceBeforeFrame`, guarded by a static
   `_controlsCreated` flag so controls are registered exactly once.

## Reusable takeaways

- **Want a custom HUD/overlay mod?** Copy this shape: a `MySessionComponentBase` that
  draws billboards each tick + a `TransparentMaterials.sbc` defining your art + cached
  `MyStringId`s.
- **Add terminal controls to a vanilla block** (like a cockpit) with a
  `MyGameLogicComponent` + the static "created once" guard — a common, easy-to-get-wrong
  pattern shown cleanly here.
- **Transparent materials** are how you get glow/see-through HUD art; note the
  `BlendTypeEnum` used per element (additive for glows).

> Attribution: EliDangHUD © its author, vendored for study only. See repo [LICENSE](../LICENSE).
