using SeaTruckArms.API;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckGrapplingArmFragment_Prefab : SeaTruckArmFragment
    {
        internal SeaTruckGrapplingArmFragment_Prefab()
            : base(
                  techTypeName: "SeaTruckGrapplingArmFragment",
                  friendlyName: "Seatruck grappling arm fragment",
                  fragmentTemplate: ArmFragmentTemplate.GrapplingArm,                  
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
                new LootDistributionData.BiomeData() { biome = BiomeType.LilyPads_Crevice_Ground, count = 1, probability = 0.05f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return SpriteManager.Get(TechType.ExosuitGrapplingArmModule);
        }

        protected override void PostModify()
        {
        }              
    }
}
