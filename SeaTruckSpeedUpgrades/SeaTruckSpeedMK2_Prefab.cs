using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BZHelper.NautilusHelpers;
using Nautilus.Crafting;
using Nautilus.Utility;

namespace SeaTruckSpeedUpgrades
{
    internal class SeaTruckSpeedMK2_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckSpeedMK2_Prefab()
            : base(
                  techTypeName: "SeaTruckSpeedMK2",                  
                  friendlyName: "Seatruck Speed Upgrade MK2",
                  description: "Increases Seatruck speed by 100%.\nDoes not stack.",
                  template: TechType.SeaTruckUpgradeAfterburner,
                  gamerResourceFileName: null,
                  requiredAnalysis: TechType.SeaTruckUpgradeAfterburner,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: EquipmentType.SeaTruckModule,
                  quickSlotType: QuickSlotType.Instant,
                  backgroundType: CraftData.BackgroundType.Normal,
                  itemSize: new Vector2int(1, 1),                 
                  fragment: null
                  )
        {
        }

        protected override void PrePatch()
        {
        }

        protected override void PostPatch()
        {
            TechTypeID = Info.TechType;
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                {
                    new Ingredient(SeaTruckSpeedMK1_Prefab.TechTypeID, 1),
                    new Ingredient(TechType.SeaTruckUpgradeAfterburner, 1),
                    new Ingredient(TechType.ReactorRod, 1)
                })
            };
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }

        protected override EncyData GetEncyclopediaData()
        {
            return null;
        }

        protected override CrafTreeTypesData GetCraftTreeTypesData()
        {
            return new CrafTreeTypesData()
            {
                TreeTypes = new List<CraftTreeType>()
                {                    
                    new CraftTreeType(CraftTree.Type.Workbench, new string[] { "ModdedWorkbench" } )
                }
            };
        }

        protected override TabNode GetTabNodeData()
        {
            return null;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{SeaTruckSpeedUpgrades_Main.modFolder}/Assets/SeaTruckSpeed_MK2.png");
        }
    }
}
