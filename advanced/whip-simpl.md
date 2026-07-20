<!-- Whip's SIMPL (Ship Integrity Monitoring Program Lite), Workshop 2344510837, by Whiplash141. Readable single-file PB script, vendored at advanced/whip-simpl/SIMPL.cs. 2026-07-20. -->

# Whip's SIMPL — Ship Integrity Monitoring Program (Lite)

**Workshop `2344510837`** · **Author:** Whiplash141 · **Version studied:** 1.14.0 (2024/10/23)
**Source:** [`whip-simpl/SIMPL.cs`](whip-simpl/SIMPL.cs) (single file, ~3150 lines, readable & heavily commented)

> This is an **in-game Programmable Block script** (not a mod). Unlike Mother OS, the
> source is clean and readable — it's a great study piece for **LCD rendering** and,
> especially, **robust Custom Data configuration parsing**.

## What it does

SIMPL draws a **live block-integrity map of your ship** onto LCD screens: a top-down
(and multi-view) render of every block, colored by damage state, so you can see at a
glance what's broken and where. All behavior is driven by the PB's **Custom Data** —
"DO NOT EDIT VARIABLES IN THE SCRIPT / USE THE CUSTOM DATA!" as the header says.

## Why it's worth studying

1. **The gold-standard Custom Data config pattern.** SIMPL uses `MyIni` wrapped in a
   reusable `ConfigSection` base class, then one subclass per settings group:
   - `GeneralSection` → `"SIMPL - General Config"`
   - `ColorSection` → `"SIMPL - Colors"` (comment documents the `R, G, B, A` format)
   - `DisplaySection` / `CompatDisplaySection` → per-screen display config
   - `LegendSection`, `MultiscreenSection`, `LegendItemSection`
   Templated section names (`"SIMPL - Display Config{0} - View {1}"`) let one screen
   host **multiple views**, and `_ini.GetSections(...)` enumerates user-defined legend
   categories. If you ever need to give *your* script configurable settings, copy this
   structure — it round-trips cleanly, writes back defaults + comments, and stays
   backward-compatible (note the `Compat` section for old configs).
2. **Ship-analysis + LCD drawing.** It walks the grid's blocks, computes integrity,
   and renders sprites to text surfaces via the `IMyTextSurface` sprite API — a
   complete example of building a graphical readout on an LCD.
3. **Multi-screen output.** One PB can drive several screens, each with its own view,
   using the `"...- Screen {0}"` / "Number of views for screen {0}" keys.

## Reusable takeaways

- **Config:** wrap `MyIni` in a `ConfigSection` base with `Read(MyIni)` / `Write(MyIni)`;
  one subclass per `[Section]`; use templated section names for repeatable groups.
- **Setup on a grid:** put SIMPL on a PB, point it at your cockpit/LCDs, and configure
  colors/views in Custom Data. `Echo(...)` output is the PB's status/diagnostics.
- **Pattern to lift:** the block-iteration + integrity calc + sprite draw loop is a
  clean template for any "status display" script.

> Attribution: SIMPL © Whiplash141, vendored for study only. See repo [LICENSE](../LICENSE).
