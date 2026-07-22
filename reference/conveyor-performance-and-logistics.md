# Reference: Conveyor Performance & Script-Based Logistics

← [Back to reference index](README.md)

Why conveyor networks cause sim-speed drops, what a mod **can** and **cannot** do
about it, and the design patterns for offloading logistics onto a script. Distilled
from a real design session plus the vendored **BuildInfo** conveyor-network source.

---

## What actually causes conveyor lag

The conveyor system is a **graph the engine keeps re-solving.** Every conveyor
block, tube, junction, port, and inventory is a node/edge. Whenever something asks
"can inventory A reach inventory B?" — a sorter pulling, an assembler/refinery
requesting, a script, a welder — the engine runs a **reachability/pathfinding walk**
over that graph. Cost scales with **two independent factors**:

1. **Graph size** — more ports/junctions/blocks = a bigger graph, so *every* walk is
   more expensive. **6-way junctions are the worst offenders**: a straight tube has
   ~one path through it, a 6-way junction is a branch point (up to 6 directions) that
   multiplies the branching factor the solver must explore.
2. **Re-solve frequency** — the graph re-solves on *demand/topology events*: a block
   added/removed/damaged, a door/connector opening/closing, a sorter toggling, an
   assembler/refinery in cooperative **pull** mode re-querying. Many inventories all
   pulling/pushing = many queries per tick.

So the two classic pieces of advice map cleanly onto the two factors:

| Advice | Which cost it attacks |
|---|---|
| **Fewer inventories** | Fewer endpoints registering demand → **fewer simultaneous queries**. |
| **Fewer ports / 6-way junctions** | Smaller, less-branchy graph → **each query is cheaper**. |

> **Honest caveat:** the official wiki page for conveyor internals is effectively
> blank. The graph-reachability explanation is solid and matches BuildInfo's
> `ConveyorNetworkCompute.cs`; the exact per-junction cost is community-empirical
> (Keen support threads, Splitsie / Space Busters tests), not a Keen spec.

---

## What a mod CANNOT do

The solver lives in **sealed engine C++** (`MyGridConveyorSystem`). BuildInfo only
copies *constants* out of it (`VanillaData/HardCoded.cs` — "from
`MyGridConveyorSystem`") because the class itself is not moddable. Therefore:

- ❌ No API to **inject a precomputed route** or a `SetConveyorRoute()`.
- ❌ No API to **throttle or suppress** the engine's re-solve / mark-dirty triggers.
- ❌ No way to **cache the engine's own path** and force it to reuse it.

Trying to "pre-map the path and lock it" is a dead end — you can *read* the topology
but not feed a route back in.

## What a mod CAN do (confirmed against vendored source)

- **Detect that a block is a conveyor endpoint:**
  `MyResourceDistributorComponent.GetConveyorEndpointBlock(block) != null`
  (BuildInfo `LiveDataHandler.cs:301`). This is read-only observation — it does
  **not** expose the internal line-by-line route.
- **Check reachability between two inventories:**
  `IMyInventory.CanTransferItemTo(otherInventory, itemDef)` (BuildInfo
  `BlockInfoAdditions.cs:354`). Returns true only if the network can *actually* route
  between them right now (respects closed doors/connectors, sorter direction, etc.).
  ⚠️ **This call runs the solver to answer** — so calling it every tick just *moves*
  the cost into your script.
- **Move items directly, bypassing the network:**
  `IMyInventory.TransferItemTo(dest, item, ...)` teleports items between two
  `IMyInventory` refs you already hold — **no graph walk**, because you're not asking
  the network to route.
- **Iterate inventories/items:** `block.GetInventory(i)` →
  `inv.GetItems(List<MyInventoryItem>)`, `inv.GetItemAmount(defId)` (EliDangHUD
  `CircleRenderer.cs:4889`, BuildInfo `BlockInfoAdditions.cs`).

---

## Design patterns for reducing conveyor load

### Pattern A — Consolidate inventories (passive, no script)
Replace many small containers with **one large one**. 30 containers = 30 endpoints
querying the network; one hub = 1 endpoint + a smaller graph. This is the single best
consolidation move and requires no code. A huge-capacity block (e.g. the
**BlackHoleContainer**) is a lag *feature* when it **replaces** scattered storage and
becomes the central hub everything feeds — **not** when it's just an extra block
bolted on (that *adds* an endpoint).
Limit: consolidation shrinks the graph; it does **not** reduce re-solve *frequency*
(pull-mode machines still churn).

### Pattern B — Reduce re-query frequency (script, keeps conveyors)
You can't touch the solver, but you can **feed it less work**: a lightweight
`MySessionComponentBase` that watches production/sorter blocks and toggles idle
pullers out of constant-pull mode, so they stop re-querying every tick. Conveyors
still carry everything; you've only cut redundant demand. This is the honest version
of "fix the lag while still using conveyors."

### Pattern C — Cached-connectivity teleport (script, hybrid) ⭐
The best design when you want routes handled cheaply **without** cheaty teleporting
between unconnected blocks:

1. **Rarely** (startup + on block add/remove/damage events — hook
   `IMyCubeGrid.OnBlockAdded`/`OnBlockRemoved`, pattern in AnimationEngine /
   DefinitionExtensions source): validate each route with
   `source.CanTransferItemTo(dest, item)` and **cache** the valid pairs. This is the
   *one* solver hit, amortized over minutes.
2. **Frequently + cheap** (slow tick, e.g. every 60–100 frames): call
   `TransferItemTo` on cached-valid routes only — **no solver**.
3. **Invalidate** the cache on the block events above, then re-validate.

This keeps the **conveyor network as the authority on *what's connected*** (checked
rarely) while the **teleport is the cheap executor** (runs often). It respects the
player's build — if two blocks aren't wired up, `CanTransferItemTo` returns false and
nothing moves.

> **Residual limitation:** flipping a sorter or closing a connector does *not* fire a
> block add/remove event, so a cached route can be briefly **stale** — you might
> teleport across a connection just closed, caught only on the next revalidation.
> Fine for most bases; a rough edge for precise sorter-based logistics.

### Pattern D — Pure teleport bus (script, ignores connectivity)
Skip the network entirely: cache source/dest inventory refs once and `TransferItemTo`
on a slow tick, never checking connectivity. Cheapest, but items move whether or not
blocks are conveyor-linked (feels cheaty, desyncs from BuildInfo's overlay). Only for
closed, self-authored logistics setups.

---

## Multiplayer / safety notes

- Inventory transfers must run **server-authoritative** (`!MyAPIGateway.Utilities`…
  guard, or gate on `MyAPIGateway.Multiplayer.IsServer`) or clients desync.
- All per-tick/event bodies need the mandatory **try/catch + first-frame guards**
  (see [scripting/crash-safety-checklist.md](../how-to/scripting/crash-safety-checklist.md)).
- **Cache `MyStringId`/`MyDefinitionId` lookups** at field scope; don't recompute per
  tick.

---

## TL;DR

- Lag = graph size × re-solve frequency; the solver is **sealed** (not moddable).
- Fewer inventories → fewer queries; fewer 6-way junctions → cheaper queries.
- A mod can **observe** endpoints and **teleport** items (`TransferItemTo`) but cannot
  precompute/lock the engine's route.
- Best hybrid: **cache `CanTransferItemTo` rarely, teleport often** (Pattern C).
- A huge central container (BlackHoleContainer) is real consolidation — but only when
  it **replaces** scattered storage.
