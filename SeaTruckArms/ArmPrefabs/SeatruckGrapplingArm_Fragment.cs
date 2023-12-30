using ModdedArmsHelperBZ.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckGrapplingArm_Fragment : SpawnableArmFragment
    {
        internal SeatruckGrapplingArm_Fragment()
            : base(
                  techTypeName: "SeaTruckGrapplingArmFragment",
                  friendlyName: "Seatruck grappling arm fragment",
                  fragmentTemplate: ArmTemplate.GrapplingArm,
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
                new LootDistributionData.BiomeData() { biome = BiomeType.LilyPads_Crevice_Ground, count = 1, probability = 0.05f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return SpriteManager.Get(TechType.ExosuitGrapplingArmModule);
        }

        protected override IEnumerator PostModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
