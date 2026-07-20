# seut-modding

A Space Engineers modding knowledge base + tooling: **SEUT** (Blender addon),
**SBC/XML** definitions, **materials & voxel textures**, **modeling**, **conveyors
& interactions**, and **scripting**.

The repo is split into two categories:

## 📘 How-To — step-by-step walkthroughs (`how-to/`)

Task-oriented guides for getting something done:

| Guide | What it covers |
|---|---|
| [`setup-seut.md`](how-to/setup-seut.md) | Install SEUT, configure paths, scaffold a mod, test loop |
| [`create-custom-block.md`](how-to/create-custom-block.md) | Model → definition → validate → test, end to end |
| [`create-custom-material.md`](how-to/create-custom-material.md) | The 4 packed maps, techniques, glass materials |
| [`fix-voxel-textures.md`](how-to/fix-voxel-textures.md) | Magenta / flat / black symptom → cause → fix |
| [`work-with-dds-textures.md`](how-to/work-with-dds-textures.md) | Open/inspect/combine DDS in GIMP, what AO is |
| [`edit-workshop-mods.md`](how-to/edit-workshop-mods.md) | Copy Workshop mod → local → offline test → edit |

## 📗 Saved Knowledge, Commands, Data, Advanced (`reference/`)

Encyclopedic reference to look things up:

| Doc | What it covers |
|---|---|
| [`sbc-xml-basics.md`](reference/sbc-xml-basics.md) | How `.sbc` works: lowercase rule, XML, override behavior, TypeId vs xsi:type |
| [`cubeblocks-reference.md`](reference/cubeblocks-reference.md) | CubeBlock fields + full TypeId↔xsi:type table + example |
| [`materials-reference.md`](reference/materials-reference.md) | CM/NG/ADD/Alphamask channels, texconv commands, techniques, transparent materials |
| [`modeling-reference.md`](reference/modeling-reference.md) | SEUT collections, grid sizing, subparts, collision, export, pitfalls |
| [`conveyors-and-interactions.md`](reference/conveyors-and-interactions.md) | `detector_conveyor*` dummies, small/large ports, interaction dummies |
| [`xml-and-scripting.md`](reference/xml-and-scripting.md) | Coding & editing SBC XML; PB scripts vs mod game-logic scripts |
| [`seut-reference.md`](reference/seut-reference.md) | SEUT install, panels, Shader Editor, export pipeline, error codes |
| [`mod-structure.md`](reference/mod-structure.md) | Mod folder layout, Workshop file access, publishing metadata |

## 📦 Complete example mod (`examples/`)

[`examples/ExampleConveyorCargo/`](examples/ExampleConveyorCargo/) — a full,
annotated reference mod you can copy from: a large-grid cargo container with two
conveyor ports (large + small), a custom material, a game-logic C# script bound by
SubtypeId, publishing metadata, and notes describing the binary assets to export.
The text definitions are validated by `sbc_tool.py`.

## 🧠 Advanced — real production mods (`advanced/`)

Patterns extracted from complete shipping mods:

- [`advanced/aryx-mod/`](advanced/aryx-mod/) — the **ARYX / AWE** WeaponCore weapons
  pack (82 weapons). Text/definition files only; teardown in
  [`OVERVIEW.md`](advanced/aryx-mod/OVERVIEW.md).
- [`weaponcore-framework.md`](advanced/weaponcore-framework.md) — how WeaponCore /
  CoreParts works end to end.
- [`weaponcore-weapon-definition.md`](advanced/weaponcore-weapon-definition.md),
  [`weaponcore-ammo-and-armour.md`](advanced/weaponcore-ammo-and-armour.md),
  [`weaponcore-animations-effects.md`](advanced/weaponcore-animations-effects.md) —
  deep dives with field cheat-sheets and reusable snippets.

**Modding frameworks & tools:**

- [`mod-adjuster-framework.md`](advanced/mod-adjuster-framework.md) — **Mod Adjuster
  V2**: runtime definition patching via XML.
- [`definition-extension-api.md`](advanced/definition-extension-api.md) —
  **Definition Extension API**: typed custom attributes on definitions.
- [`buildinfo.md`](advanced/buildinfo.md) — **BuildInfo** (Digi): in-game info
  toolkit + modder PublicAPI.
- [`animation-engine.md`](advanced/animation-engine.md) — **Animation Engine**
  (Math0424): `.bsl` block-animation scripting.
- [`mother-os.md`](advanced/mother-os.md) — **Mother OS** (Agentluke): in-game PB
  command framework (commands + Custom Data hooks/routines). Reference only.

## 🛠 Tooling

`scripts/sbc_tool.py` — validate / inspect / generate SBC + material XML
(Python 3, stdlib only):

```bash
python3 scripts/sbc_tool.py validate <file.sbc|.xml>
python3 scripts/sbc_tool.py inspect  <file.sbc>
python3 scripts/sbc_tool.py new-block --subtype MyBlock --name "My Block" --size 1x1x1
python3 scripts/sbc_tool.py new-transparent --subtype MyGlass
python3 scripts/sbc_tool.py new-material --name MyMat --tech DECAL_CUTOUT
```

Sources archived 2026-07-17 and 2026-07-20 from spaceengineers.wiki.gg + verified research.
