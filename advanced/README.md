# ⚙️ Advanced — Real Shipping Mods & Frameworks, Explained

**"Here's how real published mods actually do it."**

This is the deep end of the repo. Every item here is a **complete, real-world Space
Engineers mod or framework**, studied to pull out reusable patterns — with a plain-
English write-up (`.md`) *and*, in most cases, the actual **source code** (text files
only) so you can read the real thing.

← [Back to main README](../README.md)

> **Should you be here yet?** If you're still making your first block, start in
> [how-to/](../how-to/). Come here once you want to build **weapons**, **animated
> parts**, **patch other mods**, or just understand how a big mod is put together.

---

## 🧭 Which one do I need?

| I want to… | Go to |
|---|---|
| 🔫 Make a **custom weapon / turret / ammo** | [WeaponCore](#-weaponcore--the-weapons-framework) |
| ✏️ **Change an existing** vanilla/other-mod block without copying it | [Mod Adjuster](#-mod-adjuster) |
| 🏷️ Attach **custom data/settings** to definitions and read them in code | [Definition Extension API](#-definition-extension-api) |
| 🎞️ **Animate** block parts (moving barrels, folding gear, emissives) | [Animation Engine](#-animation-engine) |
| 📊 Understand the **block-info HUD** or hook into it from my mod | [BuildInfo](#-buildinfo) |
| 🎛️ Control a **whole grid with text commands** (in-game, no mod) | [Mother OS](#-mother-os) |
| 🩺 Show a **live ship-integrity / status map** on LCDs (in-game) | [Whip's SIMPL](#-whips-simpl) |
| 🛸 Build a **custom cockpit HUD / on-screen overlay** from a mod | [EliDangHUD](#-elidanghud) |
| 🖥️ Show a **custom text line in a block's terminal** (DetailInfo) | [Custom Terminal Readouts](#-custom-terminal-readouts-detailinfo) |

---

## 📁 How this folder is organized

```
advanced/
├── README.md                     ← you are here (the hub)
│
├── aryx-mod/                     ← real 82-weapon WeaponCore pack (source)
│   ├── OVERVIEW.md               ← teardown of the whole pack
│   └── Data/, Models/ …          ← the actual mod files (text only)
├── weaponcore-framework.md       ┐
├── weaponcore-weapon-definition.md│  the 4 WeaponCore
├── weaponcore-ammo-and-armour.md  │  deep-dive docs
├── weaponcore-animations-effects.md┘
│
├── mod-adjuster/                 ← Mod Adjuster V2 source
├── mod-adjuster-framework.md     ← its write-up
├── definition-extension-api/     ← Definition Extension API source
├── definition-extension-api.md   ← its write-up
├── animation-engine/             ← Animation Engine source (+ .bsl examples)
├── animation-engine.md           ← its write-up
├── buildinfo/                    ← BuildInfo source
├── buildinfo.md                  ← its write-up
├── mother-os.md                  ← Mother OS write-up (no source — see note)
├── whip-simpl/                   ← Whip's SIMPL PB script (readable source)
├── whip-simpl.md                 ← its write-up
├── elidanghud/                   ← EliDangHUD HUD-mod source (text only)
└── elidanghud.md                 ← its write-up
```

**About the source folders:** only **text/source** files are kept (`.cs`, `.sbc`,
`.bsl`, `.resx`, `.xml`, `.txt`). Binary assets (`.mwm`, `.dds`, `.wav`, `.png`) are
excluded via a per-folder `.gitignore` — you're here to read code and definitions,
not download gigabytes of models. Each `.md` teaches the **patterns** and cites the
real files; open the cited file to see the full working example.

---

## 🔫 WeaponCore — the weapons framework

The standard framework for custom weapons in SE. Don't reinvent turrets — you define
your gun *against* WeaponCore's API and it handles tracking, firing, projectiles.

**Read in this order:**

1. **[weaponcore-framework.md](weaponcore-framework.md)** — *Start here.* What
   WeaponCore (CoreParts) is, the **3-files-per-weapon** pattern (`_Weapon` /
   `_AmmoTypes` / `_Animations`), `MasterConfig` registration (the #1 "why won't my
   weapon load" gotcha), and how `.cs` definitions bind to `.sbc` blocks.
2. **[weaponcore-weapon-definition.md](weaponcore-weapon-definition.md)** — the big
   one: `WeaponDefinition` (Assignments / Targeting / HardPoint) with a full field
   cheat-sheet and copy-paste snippets. Turret vs. fixed weapon setup.
3. **[weaponcore-ammo-and-armour.md](weaponcore-ammo-and-armour.md)** — `AmmoDef`:
   damage, projectiles, guided/homing munitions, multi-stage torpedoes, custom armour.
4. **[weaponcore-animations-effects.md](weaponcore-animations-effects.md)** —
   recoil/spin animations, muzzle flashes, emissives, beam/particle effects.

**Source to study:** **[aryx-mod/](aryx-mod/)** — the real **ARYX / AWE (Aryx Weapons
Enterprise)** pack, **82 weapons**. Read **[aryx-mod/OVERVIEW.md](aryx-mod/OVERVIEW.md)**
first for the full teardown, then dig into its `Data/` weapon definitions.

---

## ✏️ Mod Adjuster

**Workshop `3017795356`.** Patch or override **existing** vanilla / other-mod
definitions at runtime via small XML files — instead of copying whole `.sbc` files
(which causes conflicts). Change a vanilla block's stats without "owning" it.

- **Write-up:** [mod-adjuster-framework.md](mod-adjuster-framework.md)
- **Source:** [mod-adjuster/](mod-adjuster/) — the `Data/Scripts/Adjuster/` tree has a
  handler per definition type you can patch: `Blocks/` (CubeBlock, FunctionalBlocks,
  Warhead), plus `Ammo`, `Blueprints`, `Character`, `Containers`, `Planet`,
  `SpawnGroups`, `Voxels`, `Weapons`, and more — a good map of *what* is patchable.

---

## 🏷️ Definition Extension API

**Workshop `2756894170` (Draygo).** Attach strongly-typed **custom attributes**
(bool / decimal / long / string / color / Vector2/3) to any game definition, then read
them back in your own script. Great for data-driving mod behavior from `.sbc`/XML.

- **Write-up:** [definition-extension-api.md](definition-extension-api.md)
- **Source:** [definition-extension-api/](definition-extension-api/) — see
  `Attributes/` for each supported type, and `DefinitionExtensionsAPI.cs` (the file you
  copy into your own mod to consume it).

---

## 🎞️ Animation Engine

**Workshop `2880317963` (Math0424).** Animate block **subparts, emissives, particles,
and sounds** using a custom **`.bsl`** scripting language, bound to a block by
`@BlockId` = its SubtypeId. No C# needed for the animation itself.

- **Write-up:** [animation-engine.md](animation-engine.md)
- **Source:** [animation-engine/](animation-engine/) — note it ships **two APIs**:
  `Legacy/` (the stable V1/V2 language, what most guides use) and `New/` (V3, newer).
  **Best starting point: the `.bsl` example scripts** in
  `Legacy/ExampleTestScripts/` (e.g. `AnimatedThruster.bsl`, `ActionStatements.bsl`)
  and `New/Tests/` — real, readable animation scripts.

---

## 📊 BuildInfo

**By Digi.** The huge in-game **block-info / ship-analysis / overlay** toolkit —
the extra block details, mount-point overlays, and `/bi` chat commands many players
run. Included both as a reference for a large, well-structured mod *and* for its
**modder-facing PublicAPI** you can send messages to.

- **Write-up:** [buildinfo.md](buildinfo.md)
- **Source:** [buildinfo/](buildinfo/) — a big, real codebase. Highlights to browse:
  `Data/Scripts/BuildInfo/Features/` (each feature is its own file — HUD stats,
  overlays, chat commands under `ChatCommands/`, per-block `LiveData/`), and
  `InterModAPI.cs` / `API Information.txt` for hooking in from your own mod.

---

## 🎛️ Mother OS

**Workshop `3411507973` (Agentluke).** An **in-game Programmable Block** command
framework: drive a whole grid with text commands (`door/open`, `light/color #airlock
red`) plus Custom Data **hooks** (event automation) and **routines**. Intergrid
control too (`@Mothership door/open Hangar`).

- **Write-up:** [mother-os.md](mother-os.md) — commands cheat-sheet + Custom Data
  hooks/variables/routines, with ready recipes.
- **Source:** *not included.* Unlike the others, Mother OS is an in-game PB **script,
  not a mod**, and its shipped code is minified/obfuscated (single-letter/Unicode
  names) — there's nothing to learn from it. The write-up is built from the
  [official Mother docs](https://lukejamesmorrison.github.io/mother-docs/) instead.

---

## 🩺 Whip's SIMPL

**Workshop `2344510837` (Whiplash141).** An **in-game Programmable Block** script that
renders a **live block-integrity map** of your ship onto LCDs — see what's damaged and
where, across multiple screens/views. All config lives in the PB's Custom Data.

- **Write-up:** [whip-simpl.md](whip-simpl.md)
- **Source:** [whip-simpl/SIMPL.cs](whip-simpl/SIMPL.cs) — single readable ~3150-line
  file. **The reason to read it:** the *gold-standard* Custom Data config pattern
  (`MyIni` + a reusable `ConfigSection` base, one subclass per `[Section]`, templated
  section names for repeatable groups) — copy this for any configurable script. Also a
  clean example of block-iteration + LCD sprite drawing.

---

## 🛸 EliDangHUD

**Workshop `3252520404`.** An *Elite Dangerous*-style **holographic cockpit HUD** mod:
radar, targeting reticles, compass, velocity vectors, planet info, animated
dust/visor. The reference for **drawing a custom HUD / world billboards from a mod**.

- **Write-up:** [elidanghud.md](elidanghud.md)
- **Source:** [elidanghud/](elidanghud/) — text only (67 `.dds`, 29 `.wav`, `bin/obj`
  excluded). Read `CircleRenderer.cs` (the whole HUD, a per-tick
  `MySessionComponentBase`), `CustomCockpitLogic.cs` (adds cockpit terminal toggles via
  a `MyGameLogicComponent`), and `TransparentMaterials_ED.sbc` (**defines every HUD
  material the code draws** — the binding between cached `MyStringId`s and the `.dds`
  art). Key lesson: cache `MyStringId` material ids → define matching transparent
  materials → draw billboards each frame.

---

## 📜 Attribution

The source in these folders belongs to its respective authors (ARYX/AWE, Mod Adjuster,
Definition Extension API by Draygo, BuildInfo by Digi, Animation Engine by Math0424,
Mother OS by Agentluke, SIMPL by Whiplash141, EliDangHUD) under their own licenses,
and is vendored here **for study only**. See the repo [LICENSE](../LICENSE).

---

## 🖥️ Custom Terminal Readouts (DetailInfo)

**Doc:** [custom-terminal-detailinfo.md](custom-terminal-detailinfo.md)

How to print your own live text into a block's terminal info panel (the DetailInfo box on
the right of the control screen) from a `MyGameLogicComponent`, via
`AppendingCustomInfo` + `RefreshCustomInfo()`. Covers what you **can't** change (the
engine's `L` fill bar isn't moddable), the always-unhook-in-`Close()` rule, and the
**font gotcha**: SE's terminal font has no `∞` glyph (renders as `?`) — use words/ASCII.
Reach for this whenever a block needs a status/capacity/label line. The
[Black Hole Container example](../examples/BlackHoleContainer/README.md) uses it.
