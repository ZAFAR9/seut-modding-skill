<!-- Sourced from official Mother docs (lukejamesmorrison.github.io/mother-docs) + the vendored PB script (Workshop 3411507973). The shipped script.cs is minified/obfuscated (single-letter + Unicode identifiers) and NOT included in the repo — there is nothing to study in it. This doc is the usable reference. 2026-07-20. -->

# Mother OS (Agentluke) — In-Game Command Framework for Programmable Blocks

**Workshop:** `3411507973` · **Author:** Agentluke · **Version studied:** 1.1.0
**Docs:** https://lukejamesmorrison.github.io/mother-docs/ · **Discord:** discord.gg/PrrmBujmXQ

> **Important:** Mother OS is an **in-game Programmable Block (PB) script**, *not a
> mod* like the others in `advanced/`. You don't edit its code — you **install the
> script into a PB and drive it with text commands** and Custom Data. The published
> `script.cs` is obfuscated (single-letter/Unicode identifiers), so there's nothing
> to read there; everything you need is below.

## 1. What it is / why use it

Mother OS turns one Programmable Block into a **command interpreter for your whole
grid**. Instead of wiring Timer Blocks + Event Controllers + toolbar actions, you
send short text commands like `door/open MainHangar` or `light/color #airlock red`
and Mother drives the blocks. Key wins:

- **No coding.** Just commands + Custom Data config.
- **Portable.** Automations live in Custom Data text — copy/paste to move them to
  another grid; no mods or extra blocks needed.
- **Intergrid.** Send commands to *other* grids remotely (`@Mothership door/open …`).
- **Reactive (hooks).** Blocks fire commands automatically on state changes.
- **Part of an ecosystem.** Mother OS, **Mother GUI** (interactive LCD menus),
  **MAPS** (autopilot / GPS flight plans) and **Mother Core** (the C#6/MDK2
  framework they're all built on) auto-cooperate; v1.1 **auto-shares commands**
  between all Mother scripts on a construct.

## 2. Install & run

1. Put the Mother OS script on a Programmable Block (in-game script, not a mod).
2. Type a command into the PB's **Argument** field and hit **Run** (or bind Run to a
   button/toolbar). `help` in the PB terminal lists all commands.
3. Store reusable commands/routines in the PB's **Custom Data** (see §5).

## 3. Command anatomy

```
<command> <arguments...> [--options]
```
| Term | Example | Meaning |
|---|---|---|
| command | `hinge/rotate`, `light/color`, `help` | which action Mother runs |
| argument | `Hinge`, `45`, `"Rotor 1"`, `#airlock` | details; **1st arg usually targets a block/group/tag**. Quote names with spaces. |
| option | `--speed=2`, `--offset=0.1`, `--force` | modifiers (with or without a value) |

**Targeting** (three ways):
- **By name:** `light/color AirlockLightInner red`
- **By terminal group:** `light/color "Airlock Lights" red`
- **By tag:** `light/color #airlock red` — tags span block *types*; the command
  decides which blocks it applies to. Define tags in a block's Custom Data:
  `[general]` → `tags=airlock,airlock-light`. Manage via `tag/set` / `tag/get`.

## 4. Command cheat-sheet (by module)

**All blocks:** `block/on` · `block/off` · `block/toggle` · `block/action <Block> <Action> <args…>` (runs a raw toolbar action — expensive, use sparingly) · `block/actions <Block>` (list actions) · `block/config <Block> <Section.Key> <Value>` · `block/rename <Block> <NewName>` · `tag/get <#tag>` · `tag/set <Block> <#tag>`

**Air Vent:** `vent/pressurize` · `vent/depressurize` · `vent/toggle`
**Battery:** `battery/charge` · `battery/discharge` · `battery/auto` · `battery/toggle`
**Cockpit/control:** `dampeners/on|off` · `handbrake/on|off`
**Connector:** `connector/lock` · `connector/unlock` · `connector/toggle`
**Door:** `door/open` · `door/close` · `door/toggle`
**Landing Gear:** `gear/lock` · `gear/unlock` · `gear/auto` · `gear/toggle`
**Hinge:** `hinge/rotate <deg>` · `hinge/speed` · `hinge/llimit` · `hinge/ulimit` · `hinge/lock` · `hinge/unlock` · `hinge/attach` · `hinge/detach` · `hinge/reset`
**Rotor:** `rotor/rotate` · `rotor/speed` · `rotor/llimit` · `rotor/ulimit` · `rotor/lock` · `rotor/unlock` · `rotor/attach` · `rotor/detach` · `rotor/reset`
**Piston:** `piston/distance <m>` · `piston/speed` · `piston/stop` · `piston/llimit` · `piston/ulimit` · `piston/attach` · `piston/detach` · `piston/reset`
**Light:** `light/color <color>` · `light/blink <interval> [--length=]` · `light/intensity` · `light/reset`
**Screen/Display:** `screen/print` · `screen/color` · `screen/bgcolor`
**Sorter:** `sorter/drain` (+ more)
**Programmable Block:** `pb/run`
**Core:** `wait <sec>` · `purge` · `dock`

(Full, always-current list: the docs **Cheatsheet**. Modules also exist for Gas Tank,
Sensor, Sound Block, Thruster, Timer Block, Wheel, Merge Block.)

## 5. Custom Data: routines, variables, hooks (the powerful part)

Custom Data uses MyIni sections. `|` marks a new line inside a value; `;` separates
commands; `{ }` runs commands in parallel.

**Routines** — named command sequences (`[commands]`):
```ini
[commands]
ExtendArm=wait 2; piston/distance LandingArm 3;
ActivateLandingLight=
 | light/color LandingLight red;
 | light/blink LandingLight 0.5 --length=0.5;
; parallel block:
ParallelRoutine=
 | { light/color Light1 red }
 | { light/color Light2 green }
```
Run `ActivateLandingLight;` in the PB terminal — or bind it to an Event Controller
action / Sensor / button.

**Variables**
- Global (`[variables]`, referenced with `$`):
  ```ini
  [variables]
  WARNING_COLOR=red
  [commands]
  Alert=light/color AirlockLight $WARNING_COLOR;
  ```
- Runtime (`{{name:default}}`, passed as `--name=`):
  ```ini
  [commands]
  OpenDoor=door/open {{door:AirlockDoor}};
  ```
  → `OpenDoor --door=MainHangar;`

**Hooks** — commands fired automatically on block state changes. This is Mother OS's
event system. Define under `[hooks]` on the block itself, or on Mother with a
`Block.` prefix:
```ini
; on the air vent's Custom Data
[hooks]
onPressurized=light/color "Airlock Light" green; door/open "Inner Door";
onDepressurized=light/color "Airlock Light" red; door/open "Outer Door";
```
Common hook keys: `onOn`/`onOff` (all blocks); vents `onPressurized`/
`onPressurizing`/`onDepressurized`/`onDepressurizing`; cockpit `onOccupied`/`onEmpty`
(these map to the state-watchers seen in the script's `IMyAirVent.Status` /
`IMyCockpit.IsUnderControl` monitors).

## 6. Intergrid

Prefix a command with `@<GridName>` to run it on another Mother-powered grid over
antenna range, e.g. `@Mothership door/open MainHangarDoor`. Grids also share
positions (used by MAPS docking). Communication is via Mother Core's secure
Intergrid Message Service.

## 7. How to use it later — quick recipes

- **Airlock:** tag both doors/lights `#airlock`; put `onPressurized`/`onDepressurized`
  hooks on the vent → one `vent/toggle #airlock` cycles the whole airlock.
- **Landing sequence:** a `[commands]` routine mixing `gear/unlock`,
  `piston/distance`, `hinge/rotate`, `light/blink`; trigger from a cockpit button.
- **Remote hangar:** from a fighter, `@Carrier door/open Hangar; @Carrier light/color HangarLights green;`
- **Reuse across ships:** copy Mother's Custom Data text to the new grid's PB —
  automations travel with it, no mods.

## 8. The wider Mother ecosystem

- **Mother GUI** — turns text surfaces into navigable menus / live mechanical views;
  driven by `view`/`screen` commands.
- **MAPS (Autopilot)** — GPS-waypoint flight plans that execute Mother commands at
  each waypoint (`dock`, `fcs`, `fp`); automated docking + atmospheric flight.
- **Mother Core** — the C#6 PB-script framework (deployed via **MDK2**) all of the
  above are built on: intergrid comms, command bus, event bus, modules. If you want
  to *build your own* Mother-compatible script, that's the layer to learn — see the
  docs' "Building A Module" + "Mother CLI" pages and Malware's MDK-SE API index.

> Attribution: Mother is MIT-licensed © Agentluke. This doc summarizes the official
> docs for use inside the modding skill; the actual script is installed in-game, not
> vendored here.
