using SeaTruckArms.API;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArmAPITest.Fragments
{
    internal class ClawArmFragment_APITEST : SeaTruckArmFragment
    {
        internal ClawArmFragment_APITEST()
            : base(
                  techTypeName: "ClawArmFragment_APITEST",
                  friendlyName: "Seatruck claw arm fragment (API TEST)",
                  fragmentTemplate: ArmFragmentTemplate.ClawArm,                  
                  cellLevel: LargeWorldEntity.CellLevel.Near,                  
                  scanTime: 5,
                  totalFragments: 4                  
                  )
        {
        }

        protected override List<LootDistributionData.BiomeData> GetBiomeDatas()
        {
            return new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData() { biome = BiomeType.ThermalSpires_Ground, count = 1, probability =   0.1f }
            };
        }

        protected override Sprite GetItemSprite()
        {
            return null;
        }

        protected override void PostModify()
        {
        }
    }
}
