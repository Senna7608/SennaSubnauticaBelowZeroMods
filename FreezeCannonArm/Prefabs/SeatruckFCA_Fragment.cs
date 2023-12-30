using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModdedArmsHelperBZ.API;
using BZHelper;
using Nautilus.Utility;

namespace FreezeCannonArm.Prefabs
{
    internal class SeatruckFCA_Fragment : SpawnableArmFragment
    {
        internal SeatruckFCA_Fragment()
            : base(
                  techTypeName: "SeatruckFreezeCannonArmFragment",
                  friendlyName: "Seatruck freeze cannon arm fragment",
                  fragmentTemplate: ArmTemplate.PropulsionArm,
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
                new LootDistributionData.BiomeData() { biome = BiomeType.LilyPads_ShipWreck_Grass, count = 1, probability = 0.05f },
                new LootDistributionData.BiomeData() { biome = BiomeType.LilyPads_ShipWreck_Ground, count = 1, probability = 0.05f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/FreezeCannonArm_icon.png");
        }

        protected override IEnumerator PostModifyGameObjectAsync(IOut<bool> success)
        {
            GameObject model = GameObjectClone.transform.Find("exosuit_propulsion_geo").gameObject;
            GraphicsHelper.ChangeObjectTexture(model, 2, illumTex: Main.illumTex);

            success.Set(true);
            yield break;
        }
    }
}
