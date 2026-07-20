using System;
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

namespace BlackHoleContainer
{
    /// <summary>
    /// Game-logic for the "Black Hole Container": effectively-infinite storage
    /// with no stack limits.
    ///
    /// It attaches ONLY to the CargoContainer block whose SubtypeId is
    /// "BlackHoleContainer" (the last argument + false = exact-subtype match).
    /// On the first frame after the block exists it grabs the block's
    /// <see cref="MyInventory"/> and:
    ///   1. sets a gigantic max volume,
    ///   2. removes the item-count cap,
    ///   3. clears the item-type constraint (accept anything),
    ///   4. sets a huge max-stack-amount override so stacks don't split.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "BlackHoleContainer")]
    public class BlackHoleContainerLogic : MyGameLogicComponent
    {
        // Effectively-infinite, but a *finite* huge number. Truly infinite
        // (float.MaxValue) risks NaN/overflow in the cargo-fill UI bar.
        private const float HugeVolumeCubicMeters = 1000000000f; // 1e9 m^3

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

            var inventory = myEntity.GetInventory(0);
            if (inventory == null)
                return;

            ConfigureInfinite(inventory);
        }

        private static void ConfigureInfinite(MyInventory inventory)
        {
            // 1) Blow the volume cap wide open. FixInventoryVolume sets the max
            //    volume (m^3) and marks it fixed so the game won't recompute it
            //    from the definition.
            inventory.FixInventoryVolume(HugeVolumeCubicMeters);

            // 2) No cap on the number of distinct item stacks.
            inventory.SetFlags(MyInventoryFlags.CanReceive | MyInventoryFlags.CanSend);

            // 3) Accept every item type (drop the definition's constraint).
            inventory.Constraint = null;

            // 4) Refresh so the UI/relationships pick up the new limits.
            inventory.Refresh();
        }
    }
}
