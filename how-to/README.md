# 📘 How-To Guides

**Step-by-step walkthroughs for *doing* things.** Each guide takes you from start to
finish. If you're new, follow them roughly in this order.

← [Back to main README](../README.md)

---

## Suggested path for beginners

1. **[setup-seut](setup-seut.md)** — install Blender + SEUT, set your folders, learn the test loop. *Do this first.*
2. **[create-custom-block](create-custom-block.md)** — make your first block and see it in-game.
3. **[create-custom-material](create-custom-material.md)** — give it a custom look.

Then use the rest as you hit specific needs.

---

## All guides

| Guide | Use it when… |
|---|---|
| [setup-seut](setup-seut.md) | You're setting up tools for the first time, or paths are broken. |
| [create-custom-block](create-custom-block.md) | You want the full model → definition → validate → test loop. |
| [create-custom-material](create-custom-material.md) | You need a custom texture/material (the 4 packed maps, glass, etc.). |
| [create-animated-subpart](create-animated-subpart.md) | You want a **spinning/moving subpart** (accretion disk, dish, doors) — scene-based subpart setup + the *export each scene individually* trick. |
| [fix-voxel-textures](fix-voxel-textures.md) | Textures render **magenta, black, or flat** and you need the cause + fix. |
| [work-with-dds-textures](work-with-dds-textures.md) | You need to open, inspect, or combine `.dds` files (in GIMP). |
| [blender-seut-concepts](blender-seut-concepts.md) | You want to **understand LODs, Build Stages, collision, mount points, bounding box, mirroring, icon render, import & export**. |
| [install-and-publish-checklist](install-and-publish-checklist.md) | You're ready to **load the mod locally**, test it, and check what's needed before publishing. |
| [edit-workshop-mods](edit-workshop-mods.md) | You want to copy a Steam Workshop mod locally and edit it. |

---

## 🧯 Troubleshooting subpages

When something breaks, these focused pages go straight to cause + fix:

| Folder | Covers |
|---|---|
| **[troubleshooting/](troubleshooting/README.md)** | SEUT export errors (**E016**, **W012**, duplicate `.sbc`, reading a clean log) and **size/placement** bugs (block acts 1×1, floats/clips, multi-cell alignment, measuring a model). |
| **[conveyors/](conveyors/README.md)** | **Mount points** (sizing, cell math, SEUT visual tool) and **conveyor dummies** (`detector_conveyor_N` — why a connector "isn't recognized"). |

---

**Looking to *understand* how something works rather than do a task?**
Head to [reference/](../reference/) instead.

## Scripting (C#)

- [scripting/mod-script-setup.md](scripting/mod-script-setup.md) — where scripts live (`\Data\Scripts\<ModName>\`), one-folder-per-assembly rule, `.cs`-only compile, MDK2, `Ingame` namespace alias gotcha, ModAPI doc sites. **Start here.**
- [scripting/game-logic-components.md](scripting/game-logic-components.md) — `MyGameLogicComponent` lifecycle + `MySessionComponentBase`, `NeedsUpdate` flags, injected profiler; worked from the BlackHoleContainer script.
- [scripting/the-whitelist.md](scripting/the-whitelist.md) — the modding whitelist, the MDK2 Roslyn analyzer, blocked APIs (incl. the `Matrix.Decompose` trap + fix), finding allowed alternatives.
- [scripting/save-and-sync.md](scripting/save-and-sync.md) — persisting mod data + server-authoritative multiplayer sync.
- [scripting/finding-things-in-the-world.md](scripting/finding-things-in-the-world.md) — querying entities in an area + reading decompiled game source.
- [scripting/crash-safety-checklist.md](scripting/crash-safety-checklist.md) — **mandatory** crash-safety pre-flight before shipping any game-logic/session script (try/catch per-frame, first-frame guards, API verification, master switch, cull+client-only, validate+test).
