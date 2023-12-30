using BZHelper.NautilusHelpers;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreezeCannonTool
{
    internal class FreezeCannon_Fragment : ModPrefab_Fragment
    {
        internal FreezeCannon_Fragment()
            : base(
                  techTypeName: "FreezeCannonFragment",
                  friendlyName: "Freeze Cannon Fragment",
                  template: TechType.PropulsionCannonFragment,
                  prefabFilePath: null, //"WorldEntities/Alterra/Fragments/propulsioncannonfragment.prefab",                  
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
                new LootDistributionData.BiomeData() { biome = BiomeType.PurpleVents_Ground, count = 1, probability = 0.5f },
                new LootDistributionData.BiomeData() { biome = BiomeType.PurpleVents_Crevice_Ground, count = 1, probability = 0.5f },
                new LootDistributionData.BiomeData() { biome = BiomeType.PurpleVents_ShipWreck_Ground, count = 1, probability = 0.5f },                
                new LootDistributionData.BiomeData() { biome = BiomeType.TwistyBridges_Ground, count = 1, probability = 0.5f },
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/FreezeCannon_icon.png");
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
