# ⚙️ Custom Terminal Readouts (DetailInfo Panel)

**Show your own live text in a block's terminal — the info box on the right of the
control-panel screen.** This is how mods add lines like `Status: Online`,
`Stored: 1,234 L`, custom capacity labels, cooldowns, etc.

← [Back to advanced README](README.md) · [Main README](../README.md)

> **Level:** intermediate. You need a `MyGameLogicComponent` on your block (see
> [examples/BlackHoleContainer](../examples/BlackHoleContainer/README.md) for a minimal one).

---

## What you can and can't change

| Terminal element | Moddable? |
|---|---|
| The **DetailInfo box** (right-side text panel) | ✅ Yes — via `AppendingCustomInfo` |
| The **engine readouts** (e.g. inventory `current / max L` and the fill bar) | ❌ No — engine-drawn, not exposed to mods |
| **LCD / text-panel** surfaces | ✅ Yes — separate API, fuller font |

So to surface a custom value (capacity, a label, a computed number) you **add a line to the
DetailInfo box**; you cannot overwrite the built-in `L` bar. Your custom line appears
alongside it.

---

## The pattern

Hook `IMyTerminalBlock.AppendingCustomInfo`, write into the provided `StringBuilder`, and
call `RefreshCustomInfo()` when you want it rebuilt. **Always unhook in `Close()`.**

```csharp
using System.Text;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.Common.ObjectBuilders;

[MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "MyBlockSubtype")]
public class MyBlockInfo : MyGameLogicComponent
{
    private IMyTerminalBlock _terminal;

    public override void Init(MyObjectBuilder_EntityBase ob)
    {
        NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
    }

    public override void UpdateOnceBeforeFrame()
    {
        var block = Entity as IMyCubeBlock;
        if (block == null || block.CubeGrid?.Physics == null) return; // skip ghosts/projections

        _terminal = Entity as IMyTerminalBlock;
        if (_terminal != null)
        {
            _terminal.AppendingCustomInfo += AppendInfo;
            _terminal.RefreshCustomInfo();      // build it now
        }
    }

    private void AppendInfo(IMyTerminalBlock block, StringBuilder sb)
    {
        // Write whatever you want; called each time the panel refreshes.
        sb.Append("Capacity: Infinite\n");
        sb.Append("Status: Online\n");
    }

    public override void Close()
    {
        if (_terminal != null)
        {
            _terminal.AppendingCustomInfo -= AppendInfo;  // avoid leaks
            _terminal = null;
        }
        base.Close();
    }
}
```

To update a **live** value, call `_terminal.RefreshCustomInfo()` whenever it changes (or on a
throttled `UpdateAfterSimulation` tick — don't rebuild every frame if you don't need to).

---

## ⚠️ Gotcha: the terminal font can't render every character

SE's **terminal font is a limited atlas.** Characters outside it fall back to `?`.

- The **infinity glyph** `∞` (U+221E) is **not** in the terminal font → it shows as **`?`**.
- Same risk for other symbols/emoji and many non-Latin characters.

**Fix:** use **words or ASCII** for terminal text. Instead of `∞`, write `"Infinite"`. It
renders on every font and language.

```csharp
sb.Append("Capacity: Infinite\n");   // ✅ always renders
// sb.Append("Capacity: \u221E\n");  // ❌ shows "Capacity: ?"
```

> **Where ∞ *does* work:** **LCD / text-panel** surfaces use a fuller font and can draw ∞.
> The limitation is specific to the block **DetailInfo** panel.

---

## Checklist

- [ ] Block has a `MyGameLogicComponent` bound by exact subtype.
- [ ] `AppendingCustomInfo += handler` set in `UpdateOnceBeforeFrame`.
- [ ] `RefreshCustomInfo()` called to show it immediately / on value change.
- [ ] Handler writes **words/ASCII only** (no `∞`, no emoji) to avoid `?`.
- [ ] `Close()` unhooks the handler.
- [ ] You did **not** expect to replace the engine's `L` bar — added a line instead.

---

## See also

- [examples/BlackHoleContainer](../examples/BlackHoleContainer/README.md) — a real block that
  uses this to print a `Capacity: Infinite` line.
- [reference/xml-and-scripting](../reference/xml-and-scripting.md) — game-logic script basics.
