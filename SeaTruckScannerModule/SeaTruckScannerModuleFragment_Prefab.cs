using BZHelper.NautilusHelpers;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeaTruckScannerModule
{
    internal class SeaTruckScannerModuleFragment_Prefab : ModPrefab_Fragment
    {
        internal SeaTruckScannerModuleFragment_Prefab()
            : base(
                  techTypeName: "SeaTruckScannerModuleFragment",
                  friendlyName: "Seatruck Scanner Module Fragment",
                  template: TechType.None,
                  prefabFilePath: "WorldEntities/Alterra/Fragments/seatruck_fabricatormodule_fragment_03.prefab",                  
                  slotType: EntitySlot.Type.Medium,
                  prefabZUp: false,
                  cellLevel: LargeWorldEntity.CellLevel.Far,
                  localScale: Vector3.one,                  
                  scanTime: 5,
                  totalFragments: 3,
                  destroyAfterScan: true                  
                  )
        {
        }

        protected override List<LootDistributionData.BiomeData> GetBiomeDatas()
        {
            return new List<LootDistributionData.BiomeData>()
            {                
                new LootDistributionData.BiomeData() { biome = BiomeType.PurpleVents_Ground, count = 1, probability = 0.04f },
                new LootDistributionData.BiomeData() { biome = BiomeType.PurpleVents_Crevice_Ground, count = 1, probability = 0.04f },
                new LootDistributionData.BiomeData() { biome = BiomeType.PurpleVents_Deep_Ground, count = 1, probability = 0.04f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{SeaTruckScannerModule_Main.modFolder}/Assets/Seatruck_ScannerModule_unlock.png");
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
