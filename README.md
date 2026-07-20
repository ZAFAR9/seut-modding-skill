# 🚀 Space Engineers Modding Guide

**Everything you need to make mods for Space Engineers — explained from scratch.**

This repo teaches you how to build custom **blocks, materials, weapons, and scripts**
for Space Engineers using **Blender + SEUT**. Whether you've never opened a `.sbc`
file or you're tearing apart a WeaponCore pack, start below and you'll find your way.

---

## 🧭 Start here — what do you want to do?

Pick the row that matches you. Click the link. That's it.

| I want to… | Go to |
|---|---|
| 🟢 **Set up my tools** (Blender, SEUT, folders) for the first time | [how-to/setup-seut](how-to/setup-seut.md) |
| 🟢 **Make my first custom block** (model → in-game) | [how-to/create-custom-block](how-to/create-custom-block.md) |
| 🎨 **Make a custom material / texture** | [how-to/create-custom-material](how-to/create-custom-material.md) |
| 🐛 **Fix magenta / black / flat textures** in Blender | [how-to/fix-voxel-textures](how-to/fix-voxel-textures.md) |
| 🖼️ **Open or edit `.dds` texture files** (in GIMP) | [how-to/work-with-dds-textures](how-to/work-with-dds-textures.md) |
| ✏️ **Edit a mod I downloaded from the Workshop** | [how-to/edit-workshop-mods](how-to/edit-workshop-mods.md) |
| 📖 **Look up how something works** (fields, formats, rules) | [reference/](reference/) |
| 📦 **Copy a complete working mod** to learn from | [examples/ExampleConveyorCargo](examples/ExampleConveyorCargo/) |
| ⚙️ **Study real shipping mods** (weapons, frameworks) | [advanced/](advanced/) |
| 🛠️ **Validate or generate `.sbc` files** from the command line | [Tooling](#-tooling-sbc_toolpy) |

> **New to all of this?** Do these three in order:
> **1.** [setup-seut](how-to/setup-seut.md) → **2.** [create-custom-block](how-to/create-custom-block.md) → **3.** skim [reference/sbc-xml-basics](reference/sbc-xml-basics.md).
> You'll have a block in-game and understand *why* it works.

---

## 🗂️ What's in this repo

```
seut-modding/
├── how-to/      ← step-by-step walkthroughs. START HERE.  (do a thing)
├── reference/   ← look-it-up encyclopedia.               (understand a thing)
├── examples/    ← a full, working mod to copy from.
├── advanced/    ← teardowns of real published mods + frameworks.
└── scripts/     ← sbc_tool.py: validate / inspect / generate .sbc files.
```

Two folders do most of the work, and they answer different questions:

- **`how-to/`** = *"How do I get X done?"* — ordered steps, start to finish.
- **`reference/`** = *"What does this field/format/rule actually mean?"* — look it up.

---

## 🆕 New here? A 60-second glossary

The jargon that trips everyone up at first:

| Term | Plain meaning |
|---|---|
| **SE** | Space Engineers, the game. |
| **SEUT** | *Space Engineers Utilities* — the **Blender add-on** that exports your 3D models into the game's format. |
| **`.sbc`** | An **XML text file** that *defines* a block/item to the game (name, size, stats). This is "the mod's data." |
| **`.mwm`** | The game's **compiled 3D model** format. SEUT turns your Blender model into this. |
| **block** | Anything you place on a grid — armor, thruster, weapon, container. Each has a `.sbc` definition + a `.mwm` model. |
| **voxel** | The **terrain / asteroid / planet surface** material — handled differently from block textures (see [fix-voxel-textures](how-to/fix-voxel-textures.md)). |
| **`.dds`** | The game's **compressed texture** format. Blocks use 4 packed maps: `_cm`, `_ng`, `_add`, alphamask. |
| **SubtypeId** | The **unique name** that ties a `.sbc` definition to its model, and that everything else references. |
| **Workshop mod** | A mod published on Steam Workshop; you can copy one locally to study or edit. |

---

## 📘 how-to/ — walkthroughs (do a thing)

| Guide | What it covers |
|---|---|
| [setup-seut](how-to/setup-seut.md) | Install SEUT, configure paths, scaffold a mod, test loop |
| [create-custom-block](how-to/create-custom-block.md) | Model → definition → validate → test, end to end |
| [create-custom-material](how-to/create-custom-material.md) | The 4 packed maps, techniques, glass materials |
| [fix-voxel-textures](how-to/fix-voxel-textures.md) | Magenta / flat / black symptom → cause → fix |
| [work-with-dds-textures](how-to/work-with-dds-textures.md) | Open / inspect / combine DDS in GIMP, what AO is |
| [edit-workshop-mods](how-to/edit-workshop-mods.md) | Copy Workshop mod → local → offline test → edit |

## 📗 reference/ — encyclopedia (understand a thing)

| Doc | What it covers |
|---|---|
| [sbc-xml-basics](reference/sbc-xml-basics.md) | How `.sbc` works: lowercase rule, XML, override behavior, TypeId vs xsi:type |
| [cubeblocks-reference](reference/cubeblocks-reference.md) | CubeBlock fields + full TypeId↔xsi:type table + example |
| [materials-reference](reference/materials-reference.md) | CM/NG/ADD/Alphamask channels, texconv commands, transparent materials |
| [modeling-reference](reference/modeling-reference.md) | SEUT collections, grid sizing, subparts, collision, export, pitfalls |
| [conveyors-and-interactions](reference/conveyors-and-interactions.md) | `detector_conveyor*` dummies, small/large ports, interaction dummies |
| [xml-and-scripting](reference/xml-and-scripting.md) | Editing SBC XML; PB scripts vs mod game-logic scripts |
| [seut-reference](reference/seut-reference.md) | SEUT install, panels, Shader Editor, export pipeline, error codes |
| [mod-structure](reference/mod-structure.md) | Mod folder layout, Workshop file access, publishing metadata |

## 📦 examples/ — a full working mod

[**ExampleConveyorCargo**](examples/ExampleConveyorCargo/) — a complete, annotated
reference mod you can copy: a large-grid cargo container with two conveyor ports
(large + small), a custom material, a game-logic C# script bound by SubtypeId, and
publishing metadata. The text definitions all pass `sbc_tool.py validate`.

## ⚙️ advanced/ — real shipping mods, taken apart

Patterns pulled from complete published mods and frameworks. Full index in
[advanced/README](advanced/README.md). Highlights:

- **[ARYX / AWE](advanced/aryx-mod/)** — a 82-weapon **WeaponCore** pack, plus 4
  deep-dive docs on building WeaponCore weapons, ammo, and animations.
- **[Mod Adjuster](advanced/mod-adjuster-framework.md)** — patch existing definitions at runtime.
- **[Definition Extension API](advanced/definition-extension-api.md)** — add custom attributes to definitions.
- **[BuildInfo](advanced/buildinfo.md)** — the in-game block-info HUD + its modder API.
- **[Animation Engine](advanced/animation-engine.md)** — animate block parts with a `.bsl` script.
- **[Mother OS](advanced/mother-os.md)** — an in-game command framework for whole grids.

---

## 🛠️ Tooling (`sbc_tool.py`)

A dependency-free Python 3 helper to validate, inspect, and generate `.sbc` + material XML:

```bash
python3 scripts/sbc_tool.py validate <file.sbc|.xml>          # is my XML well-formed & valid?
python3 scripts/sbc_tool.py inspect  <file.sbc>               # list what's defined inside
python3 scripts/sbc_tool.py new-block --subtype MyBlock --name "My Block" --size 1x1x1
python3 scripts/sbc_tool.py new-transparent --subtype MyGlass
python3 scripts/sbc_tool.py new-material --name MyMat --tech DECAL_CUTOUT
```

---

## ❓ FAQ / common first problems

- **"My block/material shows up magenta (pink) in Blender."** → Missing texture paths.
  Full fix: [fix-voxel-textures](how-to/fix-voxel-textures.md).
- **"My mod won't load in-game."** → Almost always a `.sbc` typo or a SubtypeId that
  doesn't match. Run `sbc_tool.py validate`, then check [sbc-xml-basics](reference/sbc-xml-basics.md).
- **"What's the difference between a script mod and a Programmable Block script?"** →
  [xml-and-scripting](reference/xml-and-scripting.md) explains both.
- **"Textures look wrong / washed out."** → Check resolution (2048×2048) and format
  (DDS/BC7) in [materials-reference](reference/materials-reference.md).

---

*Knowledge archived and verified 2026 from spaceengineers.wiki.gg + teardowns of real
published mods. Tooling is stdlib-only Python 3. Contributions & corrections welcome.*
