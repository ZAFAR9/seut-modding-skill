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
- `how-to/blender-seut-concepts.md` — explains LODs, Build Stages (BS), collision,
  mount points, icon render mode, bounding box, mirroring mode, + how to import/export.
- `how-to/install-and-publish-checklist.md` — load a mod locally (%AppData% Mods),
  F11 test, OneDrive gotcha, and what a complete mod needs before publishing.
- `how-to/troubleshooting/` — focused fix pages: `dead-ports-case-study.md` (places &
  mounts fine but conveyor ports dead — full triage + the vanilla-dummy fix),
  `export-errors.md` (E016 unparented,
  W012 DLC material, SEUT's duplicate auto-`.sbc`, reading a clean I007/I008 log) and
  `size-and-placement.md` (block acts 1×1, off-origin clipping, `<Size>`↔bounding
  box↔mesh-origin must agree, measuring a `.glb`).
- `how-to/conveyors/` — `mount-points.md` (sizing, `0→N` cell math, SEUT visual tool)
  and `conveyor-dummies.md` (`detector_conveyor_N` naming/orientation/parenting — the
  "connector not recognized" checklist).

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
- `reference/conveyor-performance-and-logistics.md` — why conveyor networks lag
  (graph size × re-solve frequency), the sealed engine solver, and script logistics
  patterns: `CanTransferItemTo` (checks + runs the solver) vs `TransferItemTo`
  (teleports, no solver), and the cached-connectivity teleport pattern.
- `reference/models-and-modelxml.md` — `.mwm` pipeline, LODs, dummies, subparts, ModelXML sidecar.
- `reference/physics.md` — Havok collision layers/filtering, collision shapes, the 32×32 matrix.
- `reference/world-and-session-settings.md` — Checkpoint/WorldConfiguration + moddable SessionSettings.
- `reference/sbc/` — definition-type references beyond CubeBlocks (planetgenerator, spawngroup;
  more folding in from the wiki crawl). Sourced from the SE wiki Modding/Reference, 2026-07-22.
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

- `examples/BlackHoleContainer/` — a single **infinite-storage** cargo block:
  validated `CargoContainerDefinition` `.sbc` (mount points on all faces, huge
  `<InventorySize>` fallback, BS1–3) + `BlackHoleContainerLogic.cs`, a
  `MyGameLogicComponent` (exact-subtype bind) that `FixInventoryVolume(1e9)`, clears
  the item constraint, and removes the count cap. The reference for the game-logic
  half SEUT can't produce. Model `.mwm` not included — drop your SEUT export in.

## 🧠 advanced/ — real production mods (study & patterns)

Learnings extracted from complete shipping mods. See `advanced/README.md`.

- `advanced/custom-terminal-detailinfo.md` — print custom text in a block's terminal
  DetailInfo panel via `AppendingCustomInfo`; engine `L` bar isn't moddable; SE terminal
  font can't render `∞` (shows `?`) — use words. Used by the BlackHoleContainer example.
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
- `advanced/whip-simpl.md` (+ `advanced/whip-simpl/SIMPL.cs`) — **Whip's SIMPL**
  (WS 2344510837, Whiplash141): in-game PB script rendering a live ship-integrity map
  on LCDs. Readable single file — the reference for robust Custom Data config parsing
  (`MyIni` + `ConfigSection` base, one subclass per `[Section]`).
- `advanced/elidanghud.md` (+ `advanced/elidanghud/`) — **EliDangHUD** (WS 3252520404):
  Elite-Dangerous-style cockpit HUD mod. Text source only. The reference for drawing a
  custom HUD from a mod: `MySessionComponentBase` draws billboards each tick using
  cached `MyStringId`s that must match `<TransparentMaterial>`s in
  `TransparentMaterials_ED.sbc`; `CustomCockpitLogic` adds cockpit terminal toggles.

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

### ⚡ Hard-won lessons (apply BEFORE guessing)

- **⚠️ NEVER ship a game-logic C# script without crash-safety guards.** One unhandled
  exception in a per-frame callback (`UpdateAfterSimulation`, `UpdateBeforeSimulation`,
  `UpdateAfterSimulation10/100`) **crashes SE to desktop instantly** — it is NOT a catchable
  log error like a compile fault. Mandatory pre-flight for ANY script: (1) wrap every
  per-frame/event body (`Update*`, `AppendingCustomInfo`, `Close`, handlers) in **try/catch**
  that logs via `MyLog.Default.WriteLineAndConsole` and **self-disables** (clear the
  `EACH_FRAME` flag / set a disabled bool) — never rethrow; (2) guard first-frame races
  (`Entity != null && !Entity.MarkedForClose`, `Entity.InScene`, `Entity.Render != null`,
  `CubeGrid?.Physics != null` to skip projections, null-check `Session`/`Camera`); (3) verify
  every API against vendored `advanced/` source — don't invent signatures; (4) give visual /
  experimental features a **master `const bool` switch** so they can ship OFF; (5)
  **distance-cull + client-only** (`!IsDedicated`) any per-frame visual work; (6) `sbc_tool.py
  validate` before repackaging. Full checklist: `how-to/scripting/crash-safety-checklist.md`.

- **VRageMath method "doesn't exist"? It's the whitelist hiding it.** First try the
  `D`/double variant (`MatrixD`/`Vector3D`/`QuaternionD`). But some methods are blocked
  in BOTH — e.g. **`Matrix.Decompose` AND `MatrixD.Decompose` are both blocked.** For
  those, redesign to avoid the call: to spin/rotate a subpart, cache its local matrix
  once and pre-multiply a rotation each frame (`Matrix m = Matrix.CreateRotationY(a) *
  restMatrix;`) — `Create*` builders + matrix multiply ARE whitelisted, and this keeps
  the baked tilt/scale/pos. See `how-to/scripting/crash-safety-checklist.md`.

- **Animated subparts (spinning disk, dish, doors) are SCENE-based, and the export trick
  is EXPORT EACH SCENE INDIVIDUALLY.** The subpart's real mesh lives in its own Scene
  (Type=`Subpart`); the Main scene holds only an **empty** named `subpart_<Name>`,
  parented to the body, in the Main collection, with its **Subpart Scene** dropdown
  pointing at the subpart scene. Runtime key strips the prefix (`TryGetValue("<Name>")`).
  ⭐ **"Export All" repeatedly failed to produce the linked `subpart_*.mwm`. The fix that
  worked: switch to the Subpart scene → Export Current Scene, then switch to the Main
  scene → Export Current Scene.** Verify with `strings -n 6 Main.mwm | grep -i subpart`
  (must show the link) and confirm `subpart_<Name>.mwm` exists WITH the prefix. Traps we
  hit, each with its own signature: **W005** = empty not parented to body; file exports as
  `<Name>.mwm` with **no** `subpart_` prefix + no link in main = mesh left in the Main
  scene / Subpart scene empty; **E002/E016** = Subpart scene's Main collection empty or
  unticked; **"mesh keeps getting deleted on export"** = the disk existed only as the
  preview instance SEUT injects into the Main scene (export removes that temp preview) —
  the real geometry MUST live in the Subpart scene, never rely on the preview. Full guide:
  `how-to/create-animated-subpart.md`.

- **Custom material exports PITCH BLACK? Textures weren't baked into the `.mwm`.** The
  material name is in the model but its CM/NG/ADD texture paths are **empty** (verify:
  `strings -n 6 X.mwm | grep ColorMetalTexture` — your custom mat's `_cm.dds` line is absent).
  Putting the DDS in `Textures\Models\Cubes\` does nothing if the model doesn't reference it,
  and you can't safely patch the `.mwm` binary (shifts offsets → corrupts). Fix at export:
  point slots at the in-mod DDS, assign via the **SEUT node group** (not a loose Image Texture
  node), Technique=MESH, then **Convert Textures ▸ Export Materials BEFORE Export All** so SEUT
  bakes the paths. See `how-to/troubleshooting/black-untextured-material.md`.

- **Conveyor dummies won't export? Don't hand-build them.** The reliable fix is to
  **import a vanilla CargoContainer in SEUT, tear off its `detector_conveyor_1` empty**
  (Alt+P ▸ Clear Parent Keep Transform), reposition + reparent onto the block, **drag it
  into the Main collection**, delete the vanilla mesh, Shift+D for more ports. A vanilla
  dummy carries the exact type/scale/props MwmBuilder needs, so it isn't stripped.
- **The #1 reason dummies vanish = wrong collection.** SEUT exports **per-collection** and
  only writes empties in the **Main** collection. Parenting is NOT the same as being in the
  Main collection. Zero-size Plain Axes empties also get dropped — use Cube/Arrows + real
  scale. **If SEUT's Debug Dummies overlay shows nothing, the ports will be dead in-game** —
  same root cause. See `how-to/troubleshooting/dead-ports-case-study.md`.
- **Conveyor dummies misaligned with neighbours? It's the dummy ORIGIN, not the mesh.** A
  port must sit exactly on the cell-face plane: **`± (cells × 1.25) m`** from block centre
  (Large grid; `× 0.25` Small). 7-cell block → **±8.75 m**, other two axes at 0. Set it via
  Blender N-panel ▸ Location — don't eyeball (8.53 vs 8.75 = off-grid). If the mesh centre is
  offset, recentre to 0,0,0 first. See `how-to/conveyors/conveyor-dummies.md#exact-position`.
- **A block that places flat and mounts correctly does NOT have a size/octant problem.**
  Don't chase `<Size>` or positive-octant alignment when ports are the real issue. Re-measure
  the *current* `.glb` (cells = dim ÷ 2.5 Large / ÷ 0.5 Small) before touching `<Size>` — a
  stale export is a classic false lead.
- **SE's terminal DetailInfo font has no `∞` glyph** (U+221E → renders as `?`). Use words
  ("Infinite") / ASCII in `AppendingCustomInfo`. ∞ works on LCD text panels (fuller font),
  just not the terminal DetailInfo box. The engine inventory `L` fill bar is **not moddable**
  — you can only add a line beside it. See `advanced/custom-terminal-detailinfo.md`.
- **Conveyor lag is the engine's sealed pathfinder — you can't mod the solver.** Lag =
  graph size × re-solve frequency. Advice "fewer inventories / fewer 6-way junctions" maps to:
  fewer endpoints → fewer queries; smaller graph → each query cheaper. A mod CANNOT inject or
  cache a route or throttle re-solves (`MyGridConveyorSystem` is sealed C++; BuildInfo only
  copies its constants). A mod CAN: detect endpoints (`GetConveyorEndpointBlock`), check
  reachability (`CanTransferItemTo` — but this RUNS the solver), and teleport items
  (`TransferItemTo` — no solver). Best offload = **cache `CanTransferItemTo` rarely (startup +
  block add/remove), teleport on a slow tick often**. A huge central container is real
  consolidation ONLY when it replaces scattered storage. See
  `reference/conveyor-performance-and-logistics.md`.
- **SEUT is a Blender 4.x addon.** Blender 5.x breaks Icon Render (`scene.node_tree` removed
  → blank icon) and install (`PATH_SUPPORTS_BLEND_RELATIVE` enum). Use Blender 4.2 LTS / 4.3.
  After a failed install, **delete the whole `space-engineers-utilities` addon folder** before
  reinstalling — a half-registered copy throws `already registered` / `cannot import name`.
- **GitHub push (Git Data API):** blobs → tree → commit → PATCH ref. Sleep **0.35–0.7s**
  between blob uploads; **re-read the main HEAD SHA immediately before the PATCH** and
  re-commit onto it if it moved (avoids 422). For 500+ files, run it in a **tmux** session
  (bash tool times out at 300s).

## Sources
Archived from spaceengineers.wiki.gg (Modding/Reference, SEUT pages) + verified
research, 2026-07-17 and 2026-07-20. See individual files for URLs.
