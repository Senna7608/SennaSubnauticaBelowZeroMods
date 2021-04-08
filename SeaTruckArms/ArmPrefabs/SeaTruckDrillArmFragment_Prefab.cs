using SeaTruckArms.API;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckDrillArmFragment_Prefab : SeaTruckArmFragment
    {
        internal SeaTruckDrillArmFragment_Prefab()
            : base(
                  techTypeName: "SeaTruckDrillArmFragment",
                  friendlyName: "Seatruck drill arm fragment",
                  fragmentTemplate: ArmFragmentTemplate.DrillArm,                  
                  cellLevel: LargeWorldEntity.CellLevel.Medium,                  
                  scanTime: 5,
                  totalFragments: 3                  
                  )
        {
        }

        protected override List<LootDistributionData.BiomeData> GetBiomeDatas()
        {
            return new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData() { biome = BiomeType.MiningSite_Ground, count = 1, probability =   0.15f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return SpriteManager.Get(SpriteManager.Group.Item, "exosuitdrillarmmodule");
        }

        protected override void PostModify()
        {
        }
    }
}
