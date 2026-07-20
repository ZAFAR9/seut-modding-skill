# How-To: Install a Local Mod & Publish-Readiness Checklist

Once your `.mwm`, `.sbc`, script, and icon are in place, here's how to get the mod loaded
locally to test, plus what a complete mod should contain before you publish.

← [Back to How-To index](README.md) · [Main README](../README.md)

**On this page:** [install locally](#install-as-a-local-mod) · [enable in a world](#enable-it-in-a-world)
· [F11 script check](#test-with-f11) · [what a mod should have](#what-a-complete-mod-should-have)

---

## Install as a local mod

SE auto-detects folders in its **local Mods** directory:

```
%AppData%\SpaceEngineers\Mods\
```

Paste that path into a File Explorer address bar and press Enter. Drop your whole mod
folder inside, so you end up with:

```
%AppData%\SpaceEngineers\Mods\<YourMod>\
├── Data\
├── Models\
└── Textures\
```

**⚠️ OneDrive gotcha:** if your mod folder lives under OneDrive (e.g.
`OneDrive\Desktop\…`), **copy it out** to `%AppData%\SpaceEngineers\Mods\`. OneDrive sync
can lock or offload files, and SE sometimes fails to read them.

**⚠️ Duplicate `.sbc` gotcha:** if SEUT auto-generated its own `.sbc` in
`Data\CubeBlocks\`, delete it so only your real definition remains — two definitions
conflict. See
[troubleshooting/export-errors.md](troubleshooting/export-errors.md#seut-writes-its-own-sbc-duplicate-definition).

---

## Enable it in a world

1. Launch Space Engineers ▸ **New Game** (or edit an existing world's settings).
2. On the world-settings screen, click **Mods**.
3. Your mod appears in the left **Local** list → select it → click the **arrow** to move it
   to the **Active** list on the right → **OK**.
4. Start the world.
5. Open the build menu (**G**), search your block's **DisplayName** — your icon should
   show. Place it on the correct grid size (Large/Small).

---

## Test with F11

Press **F11** (twice cycles the debug screens) to check for **script errors**. If your
block has a `MyGameLogicComponent`, this confirms it attached and didn't throw. A clean
F11 with no red script errors = your logic loaded.

---

## What a complete mod should have

**Required / essential:**

| Item | Purpose |
|---|---|
| `Data\*.sbc` | block/item definitions |
| `Models\…\*.mwm` | the compiled model(s) |
| `Textures\GUI\Icons\*.dds` | block icon (128×128, BC7, mipmaps) |
| `modinfo.sbmi` | mod metadata (auto-managed; `WorkshopId=0` while local — SE fills it on publish) |
| `Data\Scripts\…` | C# game logic (only if the block has custom behavior) |

**Recommended (before publishing):**

- **`thumb.png`** — the Workshop thumbnail people see in the mod list. **~640×480 PNG** in
  the mod root. Highly recommended if you'll publish.
- **`description.txt`** — a readme/description some publishing workflows pull from.
- **DLC decision** — clear any `W012` DLC-material warnings (swap for free materials) so
  the block works for everyone. See
  [troubleshooting/export-errors.md](troubleshooting/export-errors.md#w012--dlc-material-warning).

**Optional / advanced:**

- `Data\BlockCategories.sbc` — puts the block in a custom G-menu category.
- `Data\BlueprintClasses.sbc` / `Blueprints.sbc` — custom production recipes.
- Localization `.resx` files — multi-language display names.

> Don't set `WorkshopId` by hand in `modinfo.sbmi`. Keep it `0` for local; SE auto-fills
> the real ID when you publish. Full metadata reference:
> [reference/mod-structure.md](../reference/mod-structure.md).
