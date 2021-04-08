using SeaTruckArms.API;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckClawArmFragment_Prefab : SeaTruckArmFragment
    {
        internal SeaTruckClawArmFragment_Prefab()
            : base(
                  techTypeName: "SeaTruckClawArmFragment",
                  friendlyName: "Seatruck claw arm fragment",
                  fragmentTemplate: ArmFragmentTemplate.ClawArm,                                  
                  cellLevel: LargeWorldEntity.CellLevel.Far,                  
                  scanTime: 5,
                  totalFragments: 3
                  )
        {
        }

        protected override List<LootDistributionData.BiomeData> GetBiomeDatas()
        {
            return new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData() { biome = BiomeType.ThermalSpires_Ground, count = 1, probability =   0.03f }                
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return SpriteManager.Get(TechType.ExosuitClawArmModule);
        }

        protected override void PostModify()
        {          
        }
    }
}
