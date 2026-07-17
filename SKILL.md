# SEUT / Space Engineers Modding

Assist with Space Engineers modding: **SEUT** (the Blender addon), **SBC/XML**
definitions, **materials/textures**, and mod project structure. Use this
whenever Dp Peter is working on Space Engineers models, blocks, materials,
`.sbc` files, `.mwm` exports, or SEUT itself.

## What this skill knows (knowledge base)

Read the relevant file(s) before answering detailed questions:

- `knowledge/sbc-xml-basics.md` — how `.sbc` files work: lowercase rule, XML
  format, copy-from-game workflow, override/additive/merged behavior, wrapper
  structure, TypeId vs xsi:type, how definitions link, non-moddable defs.
- `knowledge/cubeblocks-reference.md` — CubeBlock definition fields + the full
  TypeId ↔ xsi:type block table + a minimal working block example.
- `knowledge/materials-reference.md` — the Materials reference: CM/NG/ADD/
  Alphamask RGBA channel packing, texconv commands, techniques, facing, wind,
  and TransparentMaterials.
- `knowledge/seut-reference.md` — SEUT install/requirements, collection
  structure (Main/LOD/BS/Collision/Mountpoints), Main Panel + Shader Editor,
  material workflow, export pipeline, collision rules, error codes.
- `knowledge/mod-structure.md` — mod folder layout + `modinfo.sbmi` publishing
  metadata.

## Tooling

`scripts/sbc_tool.py` (Python 3, stdlib only) — read/write/validate helper:

```
python3 scripts/sbc_tool.py validate <file.sbc|.xml>   # well-formed + SE checks
python3 scripts/sbc_tool.py inspect  <file.sbc>        # list TypeId/SubtypeId
python3 scripts/sbc_tool.py new-block --subtype ID [--name .. --size 1x1x1 --cubesize Large --typeid CubeBlock --xsitype .. --model ..]
python3 scripts/sbc_tool.py new-transparent --subtype ID
python3 scripts/sbc_tool.py new-material --name NAME [--tech DECAL_CUTOUT]
```

- `validate` catches malformed XML, missing `<Id>`/`<SubtypeId>`, wrong root,
  and unknown material techniques.
- Generators emit properly-namespaced, valid skeletons to build from.

## How to help

- **Reading/tracking:** use `inspect` to enumerate a mod's definitions; use
  `grep` across `Data/` for a SubtypeId or texture path.
- **Writing:** generate a skeleton with `new-block` / `new-transparent` /
  `new-material`, then edit, then `validate` before declaring done.
- **Materials:** channel packing lives in `materials-reference.md` — always
  confirm ADD-map channels against the official wiki mapping (R=AO, G=Emissive).
- **Always** copy vanilla `.sbc` from the game folder rather than authoring
  from scratch, and keep the `.sbc` extension lowercase.

## Sources
All content archived 2026-07-17 from spaceengineers.wiki.gg (Modding/Reference,
SEUT pages) + verified deep research. See individual knowledge files for URLs.
