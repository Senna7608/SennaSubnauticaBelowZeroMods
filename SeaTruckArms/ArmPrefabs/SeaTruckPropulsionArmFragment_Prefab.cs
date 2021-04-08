using SeaTruckArms.API;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckPropulsionArmFragment_Prefab : SeaTruckArmFragment
    {
        internal SeaTruckPropulsionArmFragment_Prefab()
            : base(
                  techTypeName: "SeaTruckPropulsionArmFragment",
                  friendlyName: "Seatruck propulsion arm fragment",
                  fragmentTemplate: ArmFragmentTemplate.PropulsionArm,                  
                  cellLevel: LargeWorldEntity.CellLevel.Medium,                 
                  scanTime: 5,
                  totalFragments: 4
                  )
        {
        }

        protected override List<LootDistributionData.BiomeData> GetBiomeDatas()
        {
            return new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData() { biome = BiomeType.CrystalCave_Ground, count = 1, probability = 0.05f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return SpriteManager.Get(TechType.ExosuitPropulsionArmModule);
        }

        protected override void PostModify()
        {
        }
    }
}
