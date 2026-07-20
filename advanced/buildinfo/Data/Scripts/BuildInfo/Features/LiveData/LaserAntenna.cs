using System.Collections.Generic;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using VRage.Game.Entity;
using VRage.Game.ModAPI;

namespace Digi.BuildInfo.Features.LiveData
{
    public class BData_LaserAntenna : BData_Base
    {
        public TurretInfo TurretInfo;

        const string DummyBase1 = "LaserComTurret";
        const string DummyBase2 = "LaserCom";

        // from MyLaserAntenna.OnModelChange()
        public bool GetTurretParts(IMyCubeBlock block, out MyEntity subpartBase1, out MyEntity subpartBase2, out MyEntity barrelPart)
        {
            MyCubeBlock internalBlock = (MyCubeBlock)block;

            subpartBase1 = internalBlock.Subparts.GetValueOrDefault(DummyBase1, null);
            subpartBase2 = subpartBase1?.Subparts.GetValueOrDefault(DummyBase2, null);
            barrelPart = subpartBase2;

            return barrelPart != null;
        }

        protected override bool IsValid(IMyCubeBlock block, MyCubeBlockDefinition def)
        {
            bool valid = false;
            MyEntity subpartBase1;
            MyEntity subpartBase2;
            MyEntity barrelPart;
            if(GetTurretParts(block, out subpartBase1, out subpartBase2, out barrelPart))
            {
                TurretInfo = new TurretInfo();
                TurretInfo.AssignData((MyCubeBlock)block, subpartBase1, subpartBase2);
                valid = true;
            }

            return base.IsValid(block, def) || valid;
        }
    }
}