using ModdedArmsHelperBZ.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckTorpedoArm_Fragment : SpawnableArmFragment
    {
        internal SeatruckTorpedoArm_Fragment()
            : base(
                  techTypeName: "SeaTruckTorpedoArmFragment",
                  friendlyName: "Seatruck torpedo arm fragment",
                  fragmentTemplate: ArmTemplate.TorpedoArm,
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
            return SpriteManager.Get(TechType.ExosuitTorpedoArmModule);
        }

        protected override IEnumerator PostModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
