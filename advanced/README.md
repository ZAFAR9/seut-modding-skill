# Advanced — Real Production Mods & Frameworks (Study & Reference)

This section holds **complete, real-world mods and modding frameworks** studied to
extract reusable patterns, plus the distilled learnings. Unlike `../how-to/`
(tutorials) and `../reference/` (encyclopedia), this is **"here's how shipping
mods/frameworks actually do it."**

For every vendored codebase, only **text/source** files are kept (`.cs`, `.sbc`,
`.bsl`, `.resx`, `.xml`, `.txt`); binary assets (`.mwm`, `.dds`, `.wav`, `.png`,
`.jpg`) are excluded via per-folder `.gitignore`.

## Weapons — WeaponCore / CoreParts

- **`aryx-mod/`** — the **ARYX / AWE (Aryx Weapons Enterprise)** mod: a large
  **WeaponCore (CoreParts)** weapons pack (82 weapons). See
  `aryx-mod/OVERVIEW.md` for the full teardown.
- **`weaponcore-framework.md`** — what WeaponCore is, the three-file-per-weapon
  pattern (`_Weapon` / `_AmmoTypes` / `_Animations`), `MasterConfig` registration,
  and how `.cs` defs bind to `.sbc` blocks.
- **`weaponcore-weapon-definition.md`** — `WeaponDefinition` (Assignments /
  Targeting / HardPoint), field cheat-sheet, snippets.
- **`weaponcore-ammo-and-armour.md`** — `AmmoDef`, guided munitions, custom armour.
- **`weaponcore-animations-effects.md`** — animation/emissive/particle patterns.

## Modding frameworks & tools

- **`mod-adjuster/`** (Workshop `3017795356`) — **Mod Adjuster V2**: patch/override
  *existing* vanilla or other-mod definitions at runtime via XML, instead of copying
  whole SBCs. Reference: `mod-adjuster-framework.md`.
- **`definition-extension-api/`** (Workshop `2756894170`) — **Definition Extension
  API**: attach strongly-typed custom attributes (bool/decimal/string/color/vectors)
  to game definitions and read them back. Reference:
  `definition-extension-api.md`.
- **`buildinfo/`** — **BuildInfo** (Digi): the big in-game block-info / ship-analysis
  / overlay / chat-command toolkit, and its modder-facing **PublicAPI**. Reference:
  `buildinfo.md`.
- **`animation-engine/`** (Workshop `2880317963`) — **Animation Engine** (Math0424):
  animate block subparts/emissives/particles with a custom **`.bsl`** scripting
  language bound to blocks by `@BlockId`. Reference: `animation-engine.md`.

> These docs teach **patterns**, citing the real files in each folder. Open the
> cited file to see the full working example.
