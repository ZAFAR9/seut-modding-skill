# Advanced — Real Production Mods (Study & Reference)

This section holds **complete, real-world mods** studied to extract reusable
patterns, plus the distilled learnings. Unlike `../how-to/` (tutorials) and
`../reference/` (encyclopedia), this is **"here's how a shipping mod actually does
it."**

## Contents

- **`aryx-mod/`** — the **ARYX / AWE (Aryx Weapons Enterprise)** mod: a large
  **WeaponCore (CoreParts)** weapons pack. Only the **text/definition** files are
  vendored here (`.cs`, `.sbc`, `.sbl`, `.resx`); the ~1.2 GB of binary assets
  (1161 `.mwm` models, 195 `.dds` textures, 214 `.wav` sounds) are **not** in the
  repo. See `aryx-mod/OVERVIEW.md` for the full teardown.

- **`weaponcore-framework.md`** — distilled reference: what WeaponCore is, the
  three-file-per-weapon pattern (`_Weapon` / `_AmmoTypes` / `_Animations`), how
  scripts register via `MasterConfig`, and how `.cs` definitions bind to `.sbc`
  blocks.

- **`weaponcore-weapon-definition.md`** — deep dive on `WeaponDefinition`
  (Assignments / Targeting / HardPoint), field cheat-sheet, reusable snippets.

- **`weaponcore-ammo-and-armour.md`** — `AmmoDef` structure, guided munitions,
  custom armour types, field cheat-sheet, snippets.

- **`weaponcore-animations-effects.md`** — animation/emissive/particle patterns
  and how model dummies drive them.

> These docs teach **patterns**, citing the real files in `aryx-mod/`. To see a
> full working example, open the cited file.
