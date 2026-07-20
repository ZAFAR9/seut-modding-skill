// ExampleCargoLogic.cs
// A minimal MOD game-logic component (NOT a Programmable Block script).
//
// Lives at:  <Mod>\Data\Scripts\ExampleConveyorCargo\ExampleCargoLogic.cs
// Compiled at world load. A compile error here silently disables the mod's
// scripts - always test with logging on (see ../../reference/xml-and-scripting.md).
//
// HOW IT BINDS TO THE BLOCK:
//   [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false,
//                                "ExampleConveyorCargo")]
//   ^ the SubtypeId string matches <SubtypeId> in ../CubeBlocks.sbc.
//
// WHAT IT DOES (demo): when the block is placed, it logs once, and every ~100
// frames it reads its own inventory fill level. Replace the body with real logic.

using System;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using IMyCargoContainer = Sandbox.ModAPI.Ingame.IMyCargoContainer;

namespace ExampleConveyorCargo
{
    [MyEntityComponentDescriptor(
        typeof(MyObjectBuilder_CargoContainer),  // base block type
        false,                                   // useEntityUpdate
        "ExampleConveyorCargo")]                 // SubtypeId(s) this applies to
    public class ExampleCargoLogic : MyGameLogicComponent
    {
        private IMyCubeBlock _block;
        private int _tick;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            _block = Entity as IMyCubeBlock;

            // Ask the engine to call UpdateBeforeSimulation for this entity.
            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME
                        |  MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            // Runs once after the entity is fully in the world.
            if (_block?.CubeGrid?.Physics == null)
                return; // ignore ghost/projection blocks

            MyAPIGateway.Utilities?.ShowMessage(
                "ExampleCargo", $"{_block.DisplayNameText} initialized.");
        }

        public override void UpdateBeforeSimulation()
        {
            // Server-authoritative work should be guarded; keep client-only calls safe.
            if (++_tick % 100 != 0)
                return;

            var inv = (Entity as IMyEntity)?.GetInventory();
            if (inv == null)
                return;

            // Example read: current vs max volume (do real logic here).
            var fill = (double)inv.CurrentVolume / (double)inv.MaxVolume;
            // e.g. trigger behavior when > 90% full:
            if (fill > 0.9)
            {
                // ... your logic ...
            }
        }

        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return base.GetObjectBuilder(copy);
        }
    }
}
