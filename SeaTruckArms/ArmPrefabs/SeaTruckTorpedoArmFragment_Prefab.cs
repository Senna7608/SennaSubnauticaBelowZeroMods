using SeaTruckArms.API;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckTorpedoArmFragment_Prefab : SeaTruckArmFragment
    {
        internal SeaTruckTorpedoArmFragment_Prefab()
            : base(
                  techTypeName: "SeaTruckTorpedoArmFragment",
                  friendlyName: "Seatruck torpedo arm fragment",
                  fragmentTemplate: ArmFragmentTemplate.TorpedoArm,                  
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
            return SpriteManager.Get(TechType.ExosuitTorpedoArmModule);
        }

        protected override void PostModify()
        {
        }
    }
}
