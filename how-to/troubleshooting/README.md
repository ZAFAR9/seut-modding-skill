# 🧯 Troubleshooting: Export & Placement

**When SEUT won't export, or your block behaves wrong in-game.** Each page below is a
single problem with its cause and fix. Jump straight to your symptom.

← [Back to How-To index](../README.md) · [Main README](../../README.md)

---

## 🧭 Find your symptom

| What you're seeing | Go to |
|---|---|
| Export fails with **`E016 Scene could not be exported`** | [export-errors → E016](export-errors.md#e016--more-than-one-unparented-top-level-object) |
| Export warns **`W012 … is a DLC material`** | [export-errors → W012](export-errors.md#w012--dlc-material-warning) |
| You have **two `.sbc` files fighting** / block defined twice | [export-errors → duplicate SBC](export-errors.md#seut-writes-its-own-sbc-duplicate-definition) |
| How do I know the export **actually worked**? | [export-errors → reading the log](export-errors.md#reading-a-successful-export-log) |
| Block is huge in Blender but acts **1×1** in-game | [size-and-placement → Size vs model](size-and-placement.md#the-game-uses-size-not-your-model) |
| Block **floats / clips through the map** away from where you placed it | [size-and-placement → off-origin clipping](size-and-placement.md#off-origin-model--clipping) |
| Multi-cell block won't line up right | [size-and-placement → the three must agree](size-and-placement.md#the-three-that-must-agree) |
| Can't figure out the **right `<Size>`** for a model | [size-and-placement → measuring a model](size-and-placement.md#measuring-a-model-from-its-glb) |
| **Ports dead** though it places & mounts fine (full walkthrough) | [dead-ports-case-study.md](dead-ports-case-study.md) |

---

## Pages in this folder

- **[export-errors.md](export-errors.md)** — SEUT console errors/warnings: E016, W012, the duplicate-`.sbc` trap, and how to read a clean export log.
- **[size-and-placement.md](size-and-placement.md)** — why a block acts 1×1 or clips through the world, and the `<Size>` ↔ bounding box ↔ mesh-origin rule for multi-cell blocks. Includes how to measure a `.glb`.
- **[dead-ports-case-study.md](dead-ports-case-study.md)** — a full real debugging arc: block places & mounts fine but conveyor ports are dead. What we ruled out (size, octant, geometry-vs-dummy) and the fix that worked (borrow a vanilla dummy).

**Conveyor ports not connecting?** That's its own topic → [how-to/conveyors/](../conveyors/README.md).
