# how-to / scripting

Guides for writing safe, performant C# for Space Engineers mods
(`MyGameLogicComponent` / `MySessionComponentBase`).

| Page | What it covers |
|------|----------------|
| [crash-safety-checklist.md](crash-safety-checklist.md) | **Mandatory** pre-flight before shipping any script: try/catch every per-frame callback, guard first-frame races, verify APIs against vendored source, master switches, distance-cull + client-only, validate & test. Born from a real crash-to-desktop caused by an unguarded `UpdateAfterSimulation`. |

> **Rule of thumb:** a mod feature failing should mean "the feature doesn't work,"
> never "the game crashes." Every guideline here enforces that.
