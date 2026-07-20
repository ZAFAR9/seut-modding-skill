# SEUT / Space Engineers Modding

Assist with Space Engineers modding: **SEUT** (the Blender addon), **SBC/XML**
definitions, **materials & voxel textures**, **modeling**, **conveyors &
interactions**, and **scripting**. Use this whenever Dp Peter is working on SE
models, blocks, materials, `.sbc`/XML, `.mwm` exports, mod scripts, or SEUT itself.

The knowledge base is split into two categories, mirroring the GitHub repo:

## 📘 how-to/ — step-by-step walkthroughs

Task-oriented guides for *doing* things:

- `how-to/setup-seut.md` — install SEUT, configure paths, scaffold a mod, test loop.
- `how-to/create-custom-block.md` — model → definition → validate → test, end to end.
- `how-to/create-custom-material.md` — the 4 packed maps, techniques, glass mats.
- `how-to/fix-voxel-textures.md` — the magenta/flat/black symptom→cause→fix map.
- `how-to/work-with-dds-textures.md` — open/inspect/combine DDS in GIMP, what AO is.
- `how-to/edit-workshop-mods.md` — copy Workshop mod → local → offline test → edit.

## 📗 reference/ — saved knowledge, commands, data, advanced

Encyclopedic reference to *look things up*:

- `reference/sbc-xml-basics.md` — how `.sbc` works: lowercase rule, XML, override
  behavior, wrapper structure, TypeId vs xsi:type, linking, non-moddable defs.
- `reference/cubeblocks-reference.md` — CubeBlock fields + full TypeId↔xsi:type
  table + minimal working block.
- `reference/materials-reference.md` — CM/NG/ADD/Alphamask channel packing,
  texconv commands, techniques, facing, wind, TransparentMaterials.
- `reference/modeling-reference.md` — SEUT collections (Main/LOD/BS/Collision/
  Mountpoints), grid sizing, subparts, collision rules, export, pitfalls.
- `reference/conveyors-and-interactions.md` — `detector_conveyor*` dummies, small
  vs large ports, in/out, interaction dummies, subparts, worked example.
- `reference/xml-and-scripting.md` — coding SBC XML properly + editing vanilla XML;
  PB scripts vs mod game-logic scripts, component skeletons, whitelist pitfalls.
- `reference/seut-reference.md` — SEUT install/requirements, panels, Shader Editor,
  export pipeline, error codes.
- `reference/mod-structure.md` — mod folder layout, Workshop file access,
  publishing metadata (`modinfo.sbmi`).

## 📦 examples/ — complete reference mod

- `examples/ExampleConveyorCargo/` — a full annotated skeleton mod: a 1×1×1
  large-grid cargo container with **two conveyor ports** (large + small via
  `detector_conveyor*` dummies), a **custom material**, a **game-logic C# script**
  bound by SubtypeId, `modinfo.sbmi`, and per-asset notes for the binary parts
  (`.mwm`/`.dds`). The `.sbc`, material `.xml`, and `modinfo` are validated by
  `sbc_tool.py`. Copy it as a starting point for new blocks.

## 🧠 advanced/ — real production mods (study & patterns)

Learnings extracted from complete shipping mods. See `advanced/README.md`.

- `advanced/aryx-mod/` — the **ARYX / AWE** WeaponCore weapons pack (82 weapons):
  vendored **text/definition files only** (`.cs`, `.sbc`), teardown in
  `advanced/aryx-mod/OVERVIEW.md`. Binaries (1161 .mwm / 195 .dds / 214 .wav) not in repo.
- `advanced/weaponcore-framework.md` — how WeaponCore (CoreParts) works: the
  `partial class Parts` pattern, three-file-per-weapon convention
  (`_Weapon`/`_AmmoTypes`/`_Animations`), `MasterConfig` registration, and how C#
  defs bind to `.sbc` blocks (fixed weapons use `ConveyorSorter` base type).
- `advanced/weaponcore-weapon-definition.md` — `WeaponDefinition` (Assignments/
  Targeting/HardPoint), turret vs fixed, field cheat-sheet, snippets.
- `advanced/weaponcore-ammo-and-armour.md` — `AmmoDef`, guided munitions,
  multi-stage torpedoes, EWAR, custom armour, snippets.
- `advanced/weaponcore-animations-effects.md` — AnimationDef/EventTriggers/RelMove,
  emissives, particle events, subpart naming, TransparentMaterials.

**Modding frameworks & tools** (vendored source, text files only):

- `advanced/mod-adjuster-framework.md` (+ `advanced/mod-adjuster/`) — **Mod Adjuster
  V2** (WS 3017795356): patch/override *existing* vanilla/other-mod definitions at
  runtime via `Data/ModAdjuster/` XML, no full SBC copies.
- `advanced/definition-extension-api.md` (+ `advanced/definition-extension-api/`) —
  **Definition Extension API** (WS 2756894170, Draygo): attach typed custom
  attributes (bool/decimal/string/color/vectors) to definitions and query them fast.
- `advanced/buildinfo.md` (+ `advanced/buildinfo/`) — **BuildInfo** (Digi): in-game
  block-info/overlay/ship-analysis toolkit; ComponentLib architecture; PublicAPI
  (mod-message channel 514062285) to suppress its info on your blocks; `/bi` commands.
- `advanced/animation-engine.md` (+ `advanced/animation-engine/`) — **Animation
  Engine** (WS 2880317963, Math0424): animate block subparts/emissives/particles via
  a custom `.bsl` script language bound by `@BlockId`.
- `advanced/mother-os.md` — **Mother OS** (WS 3411507973, Agentluke): in-game
  Programmable Block command framework (~80 commands + Custom Data routines,
  variables, hooks, intergrid). Reference only — the shipped script is obfuscated,
  so the code is not vendored; doc is built from the official Mother docs.

## 🛠 Tooling

`scripts/sbc_tool.py` (Python 3, stdlib only):

```
python3 scripts/sbc_tool.py validate <file.sbc|.xml>   # well-formed + SE checks
python3 scripts/sbc_tool.py inspect  <file.sbc>        # list TypeId/SubtypeId
python3 scripts/sbc_tool.py new-block --subtype ID [--name .. --size 1x1x1 --cubesize Large --typeid CubeBlock --xsitype .. --model ..]
python3 scripts/sbc_tool.py new-transparent --subtype ID
python3 scripts/sbc_tool.py new-material --name NAME [--tech DECAL_CUTOUT]
```

## How to help (standing rules)

- **Always** use this skill to inspect/validate/generate SBC/XML (`sbc_tool.py`).
- **Reading/tracking:** `inspect` to enumerate a mod's definitions; `grep` across
  `Data/` for a SubtypeId or texture path.
- **Writing:** generate a skeleton, edit, then `validate` before declaring done.
- **Texture issues:** verify **2048×2048** + **BC7 DDS** first, then channel
  packing; for voxels confirm the **ADD map red (AO) channel is white**.
- **Blender path errors:** prefer **relinking existing `.dds`** over hunting for
  missing source `.tif`.
- **Always** copy vanilla `.sbc` from the game folder rather than authoring from
  scratch, and keep the `.sbc` extension lowercase.

## Sources
Archived from spaceengineers.wiki.gg (Modding/Reference, SEUT pages) + verified
research, 2026-07-17 and 2026-07-20. See individual files for URLs.
