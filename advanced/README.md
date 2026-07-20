# ⚙️ Advanced — Real Shipping Mods, Taken Apart

**"Here's how real published mods actually do it."** This section holds complete,
real-world mods and modding frameworks, studied to extract reusable patterns, plus
the distilled write-ups.

← [Back to main README](../README.md)

> **Should you be here yet?** If you're still making your first block, start with
> [how-to/](../how-to/) instead. Come here once you want to build **weapons**,
> **animated parts**, or use a **framework** — or you're curious how a big mod is put
> together.

For every vendored codebase, only **text/source** files are kept (`.cs`, `.sbc`,
`.bsl`, `.resx`, `.xml`, `.txt`); binary assets (`.mwm`, `.dds`, `.wav`, `.png`,
`.jpg`) are excluded via per-folder `.gitignore`. These docs teach **patterns** and
cite the real files — open the cited file to see the full working example.

---

## 🔫 Weapons — WeaponCore / CoreParts

The standard framework for custom weapons. Don't reinvent turrets — define your gun against WeaponCore's API.

| Doc / folder | What it is |
|---|---|
| [aryx-mod/](aryx-mod/) → [OVERVIEW](aryx-mod/OVERVIEW.md) | The **ARYX / AWE** pack — 82 real WeaponCore weapons to learn from. |
| [weaponcore-framework](weaponcore-framework.md) | How WeaponCore works: the 3-files-per-weapon pattern, `MasterConfig` registration, how `.cs` defs bind to `.sbc` blocks. **Start here for weapons.** |
| [weaponcore-weapon-definition](weaponcore-weapon-definition.md) | `WeaponDefinition` (Assignments / Targeting / HardPoint) field cheat-sheet + snippets. |
| [weaponcore-ammo-and-armour](weaponcore-ammo-and-armour.md) | `AmmoDef`, guided munitions, multi-stage torpedoes, custom armour. |
| [weaponcore-animations-effects](weaponcore-animations-effects.md) | Weapon animation / emissive / particle patterns. |

## 🧩 Modding frameworks & tools

Reusable systems other mods build on top of.

| Doc / folder | What it does | Workshop |
|---|---|---|
| [mod-adjuster-framework](mod-adjuster-framework.md) ([src](mod-adjuster/)) | **Mod Adjuster V2** — patch/override *existing* vanilla or other-mod definitions at runtime via XML, instead of copying whole SBCs. | `3017795356` |
| [definition-extension-api](definition-extension-api.md) ([src](definition-extension-api/)) | **Definition Extension API** — attach typed custom attributes (bool/decimal/string/color/vectors) to definitions and read them back. | `2756894170` |
| [buildinfo](buildinfo.md) ([src](buildinfo/)) | **BuildInfo** (Digi) — the in-game block-info / ship-analysis / overlay toolkit, plus its modder-facing PublicAPI. | — |
| [animation-engine](animation-engine.md) ([src](animation-engine/)) | **Animation Engine** (Math0424) — animate block subparts/emissives/particles with a custom **`.bsl`** language bound by `@BlockId`. | `2880317963` |
| [mother-os](mother-os.md) | **Mother OS** (Agentluke) — an in-game **Programmable Block** command framework for driving a whole grid with text commands + Custom Data hooks/routines. *Reference only — script is obfuscated, not vendored.* | `3411507973` |
