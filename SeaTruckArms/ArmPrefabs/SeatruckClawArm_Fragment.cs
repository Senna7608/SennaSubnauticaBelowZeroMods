using ModdedArmsHelperBZ.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckClawArm_Fragment : SpawnableArmFragment
    {
        internal SeatruckClawArm_Fragment()
            : base(
                  techTypeName: "SeaTruckClawArmFragment",
                  friendlyName: "Seatruck claw arm fragment",
                  fragmentTemplate: ArmTemplate.ClawArm,                                  
                  prefabFilePath: null,                  
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

        protected override IEnumerator PostModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
