# Reference: Conveyors & Interaction Points

Source: spaceengineers.wiki.gg/wiki/Modding/Reference/Conveyor_System + verified
search. Archived: 2026-07-20.

Conveyor ports and interaction points are defined by **named dummies (empties)**
placed in the block's model, exported through SEUT. The `.sbc` definition rarely
declares ports directly — the **model dummies drive it**.

> **Just trying to get a port working?** Use the task guides instead:
> [how-to/conveyors/conveyor-dummies.md](../how-to/conveyors/conveyor-dummies.md) (naming,
> orientation, parenting, "connector not recognized" checklist) and
> [how-to/conveyors/mount-points.md](../how-to/conveyors/mount-points.md) (sizing them).

## Defining conveyor ports (detector dummies)

The block model needs **`detector_conveyor*`** dummies to connect to the conveyor
network. The `*` is a set of **suffixes**, added **in this order** when combined:

| Suffix | Meaning |
|---|---|
| `line` | makes it a conveyor **line** (pass-through) rather than an endpoint |
| `small` | **small port** — only items small enough (per the item's `<Size>`) pass. Without it, the port is a **large port**. |
| `in` / `out` | restricts flow direction (input-only / output-only) |

Examples of valid dummy names:
- `detector_conveyor` — a standard large port.
- `detector_conveyor_small` — small port.
- `detector_conveyorline` — a conveyor line segment.
- `detector_conveyor_small_in` — small, input only.
- `detector_conveyor_large_out` — large, output only.

Numbering: append an index for multiple ports, e.g. `detector_conveyor1`,
`detector_conveyor2`.

**Tip:** In-game, open the **debug draw menu (F11)** to visualize model dummies and
confirm ports line up with the grid.

## Small vs large ports

- **Large port:** transfers any item (components, ingots, ore, tools, large items).
- **Small port:** only items whose `<Size>` is small enough (ores, ingots,
  ammo, small components). This mirrors the small-tube vs large-conveyor system
  in-game.
- A port's size comes purely from the **presence/absence of the `_small`
  suffix** on its dummy.

## Placement rules

- The dummy's **position + orientation** must sit on the block face where the
  conveyor should attach, aligned to the grid so it meets the neighboring block's
  port. Misaligned dummies = ports that won't connect.
- The dummy's local scale/orientation defines the port's footprint and the
  direction items flow.

## Interaction / "use" dummies

Other `detector_` dummies define where the player interacts with a block and where
subsystems attach. Common ones:

| Dummy | Purpose |
|---|---|
| `detector_terminal` | opens the terminal / control panel |
| `detector_textpanel` | LCD/text-panel interaction surface |
| `detector_ownercontrol` | ownership / access control point |
| `detector_inventory` | inventory access point |
| `detector_conveyor*` | conveyor ports (above) |

These are empties in the model; SEUT exports them and the engine reads them at
load. Naming is exact and case-sensitive-ish — follow the wiki spelling.

## Subparts (animated parts)

Moving parts (rotor heads, doors, pistons, custom animated bits) use the
**`subpart_<name>`** empty/dummy convention. The subpart is its own model linked
at that empty; the engine animates it (rotation/translation) relative to the
parent. See `modeling-reference.md`.

## Worked example — a block with two conveyor ports

In the **model**, add two dummies:
- `detector_conveyor1` on the +X face (large port)
- `detector_conveyor_small2` on the -X face (small port)

The `.sbc` block definition references the model; the ports come from the dummies:

```xml
<Definition xsi:type="MyObjectBuilder_CargoContainerDefinition">
  <Id>
    <TypeId>CargoContainer</TypeId>
    <SubtypeId>MyCustomCargo</SubtypeId>
  </Id>
  <CubeSize>Large</CubeSize>
  <Size x="1" y="1" z="1" />
  <Model>Models\MyCustomCargo.mwm</Model>
  <!-- conveyor ports come from detector_conveyor* dummies in the .mwm -->
  <InventorySize>
    <X>2</X><Y>2</Y><Z>2</Z>
  </InventorySize>
</Definition>
```

## Common pitfalls

- **Ports don't connect:** dummy not aligned to the grid face / wrong orientation.
- **Wrong items blocked/allowed:** missing or extra `_small` suffix.
- **No terminal:** missing `detector_terminal` dummy.
- **Nothing appears:** dummy misspelled (must start `detector_conveyor…`).
- Use **F11 debug draw** in-game to see the actual dummy positions.
