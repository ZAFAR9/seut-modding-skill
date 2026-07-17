# seut-modding

A Space Engineers modding assistant skill: **SEUT** (Blender addon), **SBC/XML**
definitions, **materials/textures**, and mod project structure.

## Contents

- `SKILL.md` — skill manifest / how to use it.
- `knowledge/` — archived reference from spaceengineers.wiki.gg:
  - `sbc-xml-basics.md` — how `.sbc` files work (lowercase rule, XML, overrides, TypeId vs xsi:type).
  - `cubeblocks-reference.md` — CubeBlock fields + full TypeId↔definition table + example.
  - `materials-reference.md` — CM/NG/ADD/Alphamask channels, techniques, transparent materials.
  - `seut-reference.md` — SEUT install, collections, panels, export pipeline, error codes.
  - `mod-structure.md` — mod folder layout, accessing/editing Workshop mods, publishing metadata.
- `scripts/sbc_tool.py` — validate / inspect / generate SBC + material XML (Python 3, stdlib only).

## Tool usage

```bash
python3 scripts/sbc_tool.py validate <file.sbc|.xml>
python3 scripts/sbc_tool.py inspect  <file.sbc>
python3 scripts/sbc_tool.py new-block --subtype MyBlock --name "My Block" --size 1x1x1
python3 scripts/sbc_tool.py new-transparent --subtype MyGlass
python3 scripts/sbc_tool.py new-material --name MyMat --tech DECAL_CUTOUT
```

Sources archived 2026-07-17 from spaceengineers.wiki.gg.
