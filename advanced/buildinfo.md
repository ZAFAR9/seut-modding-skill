<!-- Extracted from advanced/buildinfo/ source (Digi's BuildInfo), verified against files. 2026-07-20. -->

# BuildInfo (Digi) — In-Game Block Info & Modder Toolkit

Digi's **BuildInfo** is one of SE's most-used HUD/info mods. Dual purpose: rich
real-time block/item/ship stats + overlays for players, and a **PublicAPI** for
other mods to control how their blocks integrate with its HUD.

## 1. Player features

- **Enhanced block-info panel** — highlights functional/ownership component
  thresholds, counts components in inventory, scrolls long lists.
- **World overlays** — conveyor networks, airtightness leaks, mount points, block
  bounds, upgrade-module links, subpart offsets.
- **Ship analysis** — cumulative mass, volume, component counts, DLC + mod deps.
- **Detailed terminal info** — appends fire rate, power formulas, thruster
  efficiency, inventory volume to the terminal Detailed Info pane.
- **Interactive config menu** — click-and-drag HUD layout + feature toggles.

`Features/` tree (feature modules): `BlockLogic/`, `ChatCommands/`, `Config/` +
`ConfigMenu/`, `Fonts/`, `GUI/`, `HUD/`, `LiveData/` (mesh/subpart cache),
`ModelPreview/`, `ModderHelp/`, `MultiTool/`, `Overlays/`, `ReloadTracker/`,
`Terminal/`, `ToolbarInfo/`, `Tooltips/`.

## 2. Architecture (ComponentLib)

```
KSH ModAPI session lifecycle
   └─ BuildInfo_GameSession.cs  [MySessionComponentDescriptor(NoUpdate)]
        └─ BuildInfoMod.cs       (ModBase<BuildInfoMod>: central registry)
             ├─ ModComponent  (AnalyseShip, BlockInfoAdditions, InterModAPI, …)
             └─ ModComponent  …
```
- `GameSession` tethers ModAPI to the framework and forwards all lifecycle hooks
  (`LoadData`/`BeforeStart`/`Draw`/…) to `BuildInfoMod`. Has a killswitch
  (`IsKilled`) that unregisters gracefully on duplicate/kill-config to avoid crashes.
- `BuildInfoMod` instantiates every module in its constructor → **deterministic
  init order**.
- Every feature extends `ModComponent : ComponentBase<BuildInfoMod>`, which
  auto-registers itself. A static `Order++` assigns `UpdateOrder`, so update order
  == instantiation order. Modules opt into loops via
  `SetUpdateMethods(UpdateFlags.UPDATE_DRAW, true)` and turn off when idle.

## 3. Modder-facing PublicAPI (the useful part)

Assembly-decoupled — no reference to BuildInfo needed; uses
`MyAPIGateway.Utilities.SendModMessage`.

- **Channel ID:** `514062285` (`InterModAPI.MOD_API_ID`)
- **Message:** `MyTuple<string, string, MyDefinitionId>`
  - `Item1` = your mod name (for logging)
  - `Item2` = target to omit (case-insensitive)
  - `Item3` = the block/item `MyDefinitionId`
- **Targets:** `"NoDetailInfo"`, `"NoItemTooltip"`, `"NoDescriptionInfo"`, `"All"`.
- **Back-compat:** sending a bare `MyDefinitionId` defaults to `"NoDetailInfo"`.
- **Timing:** send in `BeforeStart()`, within the first ~10 sim ticks of world load.

```csharp
[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
public class MyModIntegration : MySessionComponentBase
{
    const long BUILDINFO_API_ID = 514062285;
    public override void BeforeStart()
    {
        var id = new MyDefinitionId(typeof(MyObjectBuilder_UpgradeModule), "MySpecialModule");
        var msg = new MyTuple<string,string,MyDefinitionId>("My Mod", "All", id);
        MyAPIGateway.Utilities.SendModMessage(BUILDINFO_API_ID, msg);
    }
}
```
Use this so BuildInfo doesn't append misleading conveyor lines / cost tables to
script-driven custom blocks.

## 4. Chat commands (`/bi` or `/buildinfo`)

`help` · `changelog` · `clearcache` · `conveyorvis` (`conveyors`,`cn`) · `getblock`
· `getgroup` · `laserpower <km>` · `lcdres` · `measure` · `modlink` · `profile` ·
`menu` (quick config) · `reload` · `screencoords` · `serverinfo` (`worldinfo`,
`mods`) · `shipmods` · `sort` · `toolbarLabel` (`tl`) · `toolbarErasePrefix` (`tep`)
· `workshop`.

## 5. Patterns worth copying from this codebase

1. **Deterministic sequential init** — one central class instantiates all modules;
   static-incrementing `UpdateOrder` makes update/shutdown order predictable
   (solves SE's arbitrary session-component order).
2. **GC/allocation discipline** — cache object builders (e.g. `ProjectorOB` in
   `AnalyseShip.cs`), reuse class-level `StringBuilder`s instead of concatenating.
3. **Built-in profiling** — `ModBase` stopwatch-profiles each update step; view via
   `/bi profile`.
4. **Defensive external-API wrappers** — `TextAPIHandler` / `CoreSystemsAPIHandler`
   validate presence on load and degrade gracefully (fall back to billboards /
   disable feature) instead of crashing.
