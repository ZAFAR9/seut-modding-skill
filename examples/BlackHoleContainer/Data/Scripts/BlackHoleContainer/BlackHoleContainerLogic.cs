using System;
using System.Text;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace BlackHoleContainer
{
    /// <summary>
    /// Game-logic for the "Black Hole Container": effectively-infinite storage
    /// with no stack limits.
    ///
    /// It attaches ONLY to the CargoContainer block whose SubtypeId is
    /// "BlackHoleContainer" (the last argument + false = exact-subtype match).
    ///
    /// On the first frame after the block exists it grabs the block's
    /// <see cref="MyInventory"/> and:
    ///   1. sets a gigantic max volume,
    ///   2. removes the item-count cap,
    ///   3. clears the item-type constraint (accept anything),
    ///   4. refreshes so the UI picks up the new limits.
    ///
    /// UI: the game's built-in "current / max L" readout and fill bar are drawn
    /// by the engine and cannot be replaced. Instead we hook AppendingCustomInfo
    /// and write our own DetailInfo lines. NOTE: SE's terminal font does NOT
    /// contain the infinity glyph (U+221E) — it renders as "?". So we use the
    /// word "Infinite" (and the ASCII string "8" is not used) for reliability.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "BlackHoleContainer")]
    public class BlackHoleContainerLogic : MyGameLogicComponent
    {
        // Effectively-infinite, but a *finite* huge number. Truly infinite
        // (float.MaxValue) risks NaN/overflow in the cargo-fill UI bar.
        private const float HugeVolumeCubicMeters = 1000000000f; // 1e9 m^3

        // SE's font has no U+221E infinity glyph (shows as "?"), so use a word
        // that always renders on every font/language.
        private const string InfiniteWord = "Infinite";

        private IMyTerminalBlock _terminal;
        private MyInventory _inventory;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            // We only need to reconfigure once, after the entity is fully built.
            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();

            var block = Entity as IMyCubeBlock;
            // Skip projections / build-preview ghosts (no physics yet).
            if (block == null || block.CubeGrid == null || block.CubeGrid.Physics == null)
                return;

            var myEntity = Entity as MyEntity;
            if (myEntity == null || !myEntity.HasInventory)
                return;

            _inventory = myEntity.GetInventory(0);
            if (_inventory == null)
                return;

            ConfigureInfinite(_inventory);

            // Hook the terminal DetailInfo panel so we can print our own lines.
            _terminal = Entity as IMyTerminalBlock;
            if (_terminal != null)
            {
                _terminal.AppendingCustomInfo += AppendCustomInfo;
                // Force the info box to rebuild now so it shows immediately.
                _terminal.RefreshCustomInfo();
            }
        }

        private static void ConfigureInfinite(MyInventory inventory)
        {
            // 1) Blow the volume cap wide open. FixInventoryVolume sets the max
            //    volume (m^3) and marks it fixed so the game won't recompute it
            //    from the definition.
            inventory.FixInventoryVolume(HugeVolumeCubicMeters);

            // 2) No cap on send/receive.
            inventory.SetFlags(MyInventoryFlags.CanReceive | MyInventoryFlags.CanSend);

            // 3) Accept every item type (drop the definition's constraint).
            inventory.Constraint = null;

            // 4) Refresh so the UI/relationships pick up the new limits.
            inventory.Refresh();
        }

        /// <summary>
        /// Draws our custom lines into the terminal's DetailInfo box (the panel
        /// on the right of the control-panel screen). This is the modder-facing
        /// surface for custom readouts; the engine's own "L" bar stays as-is.
        /// </summary>
        private void AppendCustomInfo(IMyTerminalBlock block, StringBuilder sb)
        {
            sb.Append("Capacity: ").Append(InfiniteWord).Append('\n');

            if (_inventory != null)
            {
                // Show how much is actually stored (this number is real/finite),
                // paired with an infinite cap so it reads "used / Infinite".
                double usedL = (double)_inventory.CurrentVolume * 1000.0; // m^3 -> L
                sb.Append("Stored: ")
                  .Append(usedL.ToString("N0"))
                  .Append(" L / ")
                  .Append(InfiniteWord)
                  .Append('\n');
                sb.Append("Stacks: no limit\n");
            }
        }

        public override void Close()
        {
            // Always unhook to avoid leaks when the block is removed.
            if (_terminal != null)
            {
                _terminal.AppendingCustomInfo -= AppendCustomInfo;
                _terminal = null;
            }
            base.Close();
        }
    }
}
