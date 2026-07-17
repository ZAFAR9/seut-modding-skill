#!/usr/bin/env python3
"""SEUT / Space Engineers modding helper.

Read, validate, and generate SBC / material XML files.

Usage:
  sbc_tool.py validate <file.sbc|file.xml>     Check well-formedness + SE structure
  sbc_tool.py inspect  <file.sbc>              List definitions (TypeId/SubtypeId)
  sbc_tool.py new-block --subtype ID [opts]    Print a CubeBlock .sbc skeleton
  sbc_tool.py new-transparent --subtype ID     Print a TransparentMaterial .sbc
  sbc_tool.py new-material --name N [--tech T]  Print a model-.xml <Material> block

Options for new-block:
  --subtype ID         (required) SubtypeId
  --typeid T           default: CubeBlock
  --xsitype X          default: MyObjectBuilder_CubeBlockDefinition
  --name "Display"     display name
  --size LxWxH         default: 1x1x1
  --cubesize S         Large|Small (default Large)
  --model PATH         .mwm path
"""
import sys
import argparse
import xml.etree.ElementTree as ET

TECHNIQUES = {"MESH","DECAL","DECAL_NOPREMULT","DECAL_CUTOUT","ALPHA_MASKED",
              "FOLIAGE","GLASS","HOLO","SHIELD"}


def validate(path):
    try:
        tree = ET.parse(path)
    except ET.ParseError as e:
        print(f"INVALID XML: {e}")
        return 1
    root = tree.getroot()
    issues, notes = [], []
    if root.tag not in ("Definitions", "MyObjectBuilder_ModInfo"):
        if root.tag == "Material":
            notes.append("Standalone <Material> block (model .xml fragment).")
        else:
            issues.append(f"Root <{root.tag}> is not <Definitions>.")
    if path.lower().endswith(".sbc") and not path.split("/")[-1].endswith(".sbc"):
        issues.append("File extension must be lowercase '.sbc'.")
    # walk definitions
    for d in root.iter("Definition"):
        idn = d.find("Id")
        if idn is None:
            issues.append("A <Definition> has no <Id>.")
            continue
        sub = idn.findtext("SubtypeId")
        if sub is None:
            issues.append("A <Definition> <Id> has no <SubtypeId>.")
    for m in root.iter("Material"):
        tech = None
        for p in m.findall("Parameter"):
            if p.get("Name") == "Technique":
                tech = (p.text or "").strip()
        if tech and tech not in TECHNIQUES:
            issues.append(f"Material '{m.get('Name')}' has unknown Technique '{tech}'.")
    if issues:
        print("VALIDATION FAILED:")
        for i in issues:
            print("  - " + i)
    else:
        print("OK: well-formed and structurally valid.")
    for n in notes:
        print("  note: " + n)
    return 1 if issues else 0


def inspect(path):
    tree = ET.parse(path)
    root = tree.getroot()
    count = 0
    for d in root.iter("Definition"):
        count += 1
        idn = d.find("Id")
        tid = idn.findtext("TypeId") if idn is not None else "?"
        sid = idn.findtext("SubtypeId") if idn is not None else "?"
        xsi = d.get("{http://www.w3.org/2001/XMLSchema-instance}type") or "(implicit)"
        print(f"[{count}] {tid} / {sid}   xsi:type={xsi}")
    for t in root.iter("TransparentMaterial"):
        idn = t.find("Id")
        sid = idn.findtext("SubtypeId") if idn is not None else "?"
        print(f"[TM] TransparentMaterial / {sid}")
    if count == 0:
        print("No <Definition> elements found.")
    return 0


HEADER = ('<?xml version="1.0" encoding="utf-8"?>\n'
          '<Definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
          'xmlns:xsd="http://www.w3.org/2001/XMLSchema">')


def new_block(a):
    sx, sy, sz = (a.size.split("x") + ["1", "1", "1"])[:3]
    name = a.name or a.subtype
    model = a.model or f"Models\\Cues\\{a.subtype}.mwm"
    print(f"""{HEADER}
  <CubeBlocks>
    <Definition xsi:type="{a.xsitype}">
      <Id>
        <TypeId>{a.typeid}</TypeId>
        <SubtypeId>{a.subtype}</SubtypeId>
      </Id>
      <DisplayName>{name}</DisplayName>
      <Description>{name} block.</Description>
      <Icon>Textures\\Icons\\{a.subtype}_Icon.dds</Icon>
      <CubeSize>{a.cubesize}</CubeSize>
      <BlockTopology>TriangleMesh</BlockTopology>
      <Size x="{sx}" y="{sy}" z="{sz}" />
      <ModelOffset x="0" y="0" z="0" />
      <Model>{model}</Model>
      <Components>
        <Component Subtype="SteelPlate" Count="10" />
      </Components>
      <CriticalComponent Subtype="SteelPlate" Index="0" />
      <MountPoints>
        <MountPoint Side="Back" StartX="0" StartY="0" EndX="{sx}" EndY="{sy}" />
      </MountPoints>
      <BlockPairName>{a.subtype}</BlockPairName>
      <BuildTimeSeconds>10</BuildTimeSeconds>
      <PCU>1</PCU>
    </Definition>
  </CubeBlocks>
</Definitions>""")
    return 0


def new_transparent(a):
    print(f"""{HEADER}
  <TransparentMaterials>
    <TransparentMaterial>
      <Id>
        <TypeId>TransparentMaterialDefinition</TypeId>
        <SubtypeId>{a.subtype}</SubtypeId>
      </Id>
      <Texture>Textures\\Models\\{a.subtype}_cm.dds</Texture>
      <GlossTexture>Textures\\Models\\{a.subtype}_ng.dds</GlossTexture>
      <Gloss>0.4</Gloss>
      <Reflectivity>0.6</Reflectivity>
      <AlphaMisting>true</AlphaMisting>
      <LightMultiplier>1.0</LightMultiplier>
    </TransparentMaterial>
  </TransparentMaterials>
</Definitions>""")
    return 0


def new_material(a):
    tech = a.tech.upper()
    if tech not in TECHNIQUES:
        print(f"# warning: unknown technique '{tech}'", file=sys.stderr)
    base = a.name
    print(f"""<Material Name="{base}">
  <Parameter Name="Technique">{tech}</Parameter>
  <Parameter Name="ColorMetalTexture">Textures\\Models\\{base}_cm.dds</Parameter>
  <Parameter Name="NormalGlossTexture">Textures\\Models\\{base}_ng.dds</Parameter>
  <Parameter Name="AddMapsTexture">Textures\\Models\\{base}_add.dds</Parameter>
</Material>""")
    return 0


def main():
    p = argparse.ArgumentParser(description="SEUT / SE modding helper")
    sub = p.add_subparsers(dest="cmd", required=True)

    v = sub.add_parser("validate"); v.add_argument("file")
    i = sub.add_parser("inspect");  i.add_argument("file")

    b = sub.add_parser("new-block")
    b.add_argument("--subtype", required=True)
    b.add_argument("--typeid", default="CubeBlock")
    b.add_argument("--xsitype", default="MyObjectBuilder_CubeBlockDefinition")
    b.add_argument("--name", default="")
    b.add_argument("--size", default="1x1x1")
    b.add_argument("--cubesize", default="Large")
    b.add_argument("--model", default="")

    t = sub.add_parser("new-transparent")
    t.add_argument("--subtype", required=True)

    m = sub.add_parser("new-material")
    m.add_argument("--name", required=True)
    m.add_argument("--tech", default="MESH")

    a = p.parse_args()
    if a.cmd == "validate":       return validate(a.file)
    if a.cmd == "inspect":        return inspect(a.file)
    if a.cmd == "new-block":      return new_block(a)
    if a.cmd == "new-transparent":return new_transparent(a)
    if a.cmd == "new-material":   return new_material(a)


if __name__ == "__main__":
    sys.exit(main())
