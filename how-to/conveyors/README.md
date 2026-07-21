# 🔗 How-To: Conveyor Ports & Mount Points

**Getting a block to physically attach to a grid and flow items through conveyors.**
These are the two things people get stuck on most after a model exports fine but "won't
connect."

← [Back to How-To index](../README.md) · [Main README](../../README.md)

---

## 🧭 Which page do I need?

| Your situation | Go to |
|---|---|
| Block won't **snap onto** other blocks / places over empty air | [mount-points.md](mount-points.md) |
| Conveyor **port isn't recognized** — items won't flow, no connection | [conveyor-dummies.md](conveyor-dummies.md) |
| Dummies **won't export / Debug Dummies shows none** — the reliable fix | [conveyor-dummies.md → borrow a vanilla dummy](conveyor-dummies.md#-the-reliable-method-borrow-a-working-dummy-from-a-vanilla-import) |
| Full **"places fine but ports dead"** debugging walkthrough | [../troubleshooting/dead-ports-case-study.md](../troubleshooting/dead-ports-case-study.md) |
| I want the mount points to be **exactly right** (visual tool + the cell math) | [mount-points.md → getting them perfect](mount-points.md#getting-mount-points-perfect) |
| I don't know how big to make the mount patch | [mount-points.md → sizing](mount-points.md#how-large-should-a-mount-point-be) |

---

## Pages in this folder

- **[mount-points.md](mount-points.md)** — what mount points are, the `0→N` cell math,
  full-face vs centered patches, how to size them to your model, and the SEUT visual tool
  (the reliable "perfect" method).
- **[conveyor-dummies.md](conveyor-dummies.md)** — the named **empty objects**
  (`detector_conveyor_N` / `detector_conveyorsmall_N`) that SE actually detects as ports,
  plus orientation and parenting — the usual reason a connector "isn't recognized."

**Deeper reference** (terse lookup of dummy names, subparts, interaction points):
[reference/conveyors-and-interactions.md](../../reference/conveyors-and-interactions.md).
