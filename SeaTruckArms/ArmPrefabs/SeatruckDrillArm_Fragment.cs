using ModdedArmsHelperBZ.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckDrillArm_Fragment : SpawnableArmFragment
    {
        internal SeatruckDrillArm_Fragment()
            : base(
                  techTypeName: "SeaTruckDrillArmFragment",
                  friendlyName: "Seatruck drill arm fragment",
                  fragmentTemplate: ArmTemplate.DrillArm,
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
                new LootDistributionData.BiomeData() { biome = BiomeType.MiningSite_Ground, count = 1, probability =   0.15f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return SpriteManager.Get(SpriteManager.Group.Item, "exosuitdrillarmmodule");
        }

        protected override IEnumerator PostModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
