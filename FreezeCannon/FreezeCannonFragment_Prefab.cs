using BZCommon.Helpers.SMLHelpers;
using System.Collections.Generic;
using UnityEngine;

namespace FreezeCannon
{
    internal class FreezeCannonFragment_Prefab : ModPrefab_Fragment
    {
        internal FreezeCannonFragment_Prefab()
            : base(
                  techTypeName: "FreezeCannonFragment",
                  friendlyName: "Freeze Cannon Fragment",
                  template: TechType.PropulsionCannonFragment,                  
                  slotType: EntitySlot.Type.Small,
                  prefabZUp: false,
                  cellLevel: LargeWorldEntity.CellLevel.Near,
                  localScale: Vector3.one,                  
                  scanTime: 3,
                  totalFragments: 3,
                  destroyAfterScan: true                  
                  )
        {
        }

        protected override List<LootDistributionData.BiomeData> GetBiomeDatas()
        {
            return new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData() { biome = BiomeType.PurpleVents_ShipWreck_Ground, count = 1, probability = 0.1f }
            };
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.PropulsionCannon);
        }

        protected override void ModifyGameObject()
        {
        }
    }
}
