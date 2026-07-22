# 📗 Reference

**The look-it-up encyclopedia.** These docs explain *how things work* — fields,
formats, rules, and definitions. Come here when a how-to guide mentions something and
you want the full detail.

← [Back to main README](../README.md)

---

## By topic

**Definitions & XML** (the `.sbc` files that describe your mod)
| Doc | What it covers |
|---|---|
| [sbc-xml-basics](sbc-xml-basics.md) | How `.sbc` works: the lowercase rule, XML structure, override behavior, TypeId vs xsi:type. **Read this early.** |
| [cubeblocks-reference](cubeblocks-reference.md) | Every CubeBlock field + the full TypeId ↔ xsi:type table, with a worked example. |
| [xml-and-scripting](xml-and-scripting.md) | Editing SBC XML by hand; the difference between Programmable-Block scripts and mod game-logic scripts. |

**Art & textures**
| Doc | What it covers |
|---|---|
| [materials-reference](materials-reference.md) | The CM / NG / ADD / alphamask packed channels, `texconv` commands, techniques, transparent materials. |
| [modeling-reference](modeling-reference.md) | SEUT collections, grid sizing, subparts, collision, export settings, common pitfalls. |

**Blocks in the world**
| Doc | What it covers |
|---|---|
| [conveyors-and-interactions](conveyors-and-interactions.md) | `detector_conveyor*` dummies, small vs large ports, interaction dummies. |
| [conveyor-performance-and-logistics](conveyor-performance-and-logistics.md) | Why conveyor networks cause sim-speed lag, what a mod can/can't do about the sealed solver, and script logistics patterns (`CanTransferItemTo` + `TransferItemTo`). |

**Tools & packaging**
| Doc | What it covers |
|---|---|
| [seut-reference](seut-reference.md) | SEUT install, panels, Shader Editor, export pipeline, error codes. |
| [mod-structure](mod-structure.md) | Mod folder layout, Workshop file access, publishing metadata. |

---

**Want steps instead of explanations?** Head to [how-to/](../how-to/).
