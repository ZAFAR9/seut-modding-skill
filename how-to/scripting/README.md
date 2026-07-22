# how-to / scripting

Guides for writing safe, performant C# for Space Engineers mods
(`MyGameLogicComponent` / `MySessionComponentBase`).

**Start here** if you're new to mod scripting, then work down:

| Page | What it covers |
|------|----------------|
| [mod-script-setup.md](mod-script-setup.md) | Where scripts live (`\Data\Scripts\<ModName>\`), the one-folder-per-assembly rule, `.cs`-only whitelist compile, MDK2, the `Ingame` namespace alias gotcha, and the official + community ModAPI doc sites. **The entry point.** |
| [game-logic-components.md](game-logic-components.md) | The `MyGameLogicComponent` lifecycle (`[MyEntityComponentDescriptor]`, `Init`, `UpdateOnceBeforeFrame`, `NeedsUpdate` flags, `Close`), how `MySessionComponentBase` differs, and the injected-profiler hot-path warning — with the repo's `BlackHoleContainer` script as the worked example. |
| [the-whitelist.md](the-whitelist.md) | What the modding whitelist is, why it exists, the VS/MDK2 Roslyn analyzer, common blocked APIs (including the `Matrix.Decompose` / `MatrixD.Decompose` trap and its pre-multiply fix), and how to find allowed alternatives. |
| [save-and-sync.md](save-and-sync.md) | Persisting mod data across sessions and server-authoritative multiplayer sync patterns. |
| [finding-things-in-the-world.md](finding-things-in-the-world.md) | Querying entities/objects in an area, plus the "exploring the game code" technique for digging into decompiled source when the API docs are thin. |
| [crash-safety-checklist.md](crash-safety-checklist.md) | **Mandatory** pre-flight before shipping any script: try/catch every per-frame callback, guard first-frame races, verify APIs against vendored source, master switches, distance-cull + client-only, validate & test. Born from a real crash-to-desktop caused by an unguarded `UpdateAfterSimulation`. |

> **Rule of thumb:** a mod feature failing should mean "the feature doesn't work,"
> never "the game crashes." Every guideline here enforces that.
