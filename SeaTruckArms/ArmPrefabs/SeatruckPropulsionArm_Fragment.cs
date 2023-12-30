using ModdedArmsHelperBZ.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckPropulsionArm_Fragment : SpawnableArmFragment
    {
        internal SeatruckPropulsionArm_Fragment()
            : base(
                  techTypeName: "SeaTruckPropulsionArmFragment",
                  friendlyName: "Seatruck propulsion arm fragment",
                  fragmentTemplate: ArmTemplate.PropulsionArm,
                  prefabFilePath: null,
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

        protected override IEnumerator PostModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
