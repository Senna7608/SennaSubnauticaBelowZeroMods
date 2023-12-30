using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BZHelper.NautilusHelpers;
using Nautilus.Crafting;
using Nautilus.Utility;

namespace SeaTruckDepthUpgrades
{
    internal class SeaTruckDepthMK5_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckDepthMK5_Prefab()
            : base(
                  techTypeName: "SeaTruckDepthMK5",                  
                  friendlyName: "Seatruck Depth Upgrade MK5",
                  description: "Increases Seatruck safe diving depth to 1700m.\nDoes not stack.",
                  template: TechType.SeaTruckUpgradeHull1,
                  gamerResourceFileName: null,
                  requiredAnalysis: TechType.SeaTruckFabricatorModule,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: EquipmentType.SeaTruckModule,
                  quickSlotType: QuickSlotType.Passive,
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
                Ingredients = new List<Ingredient>(new Ingredient[4]
                {
                    new Ingredient(SeaTruckDepthMK4_Prefab.TechTypeID, 1),
                    new Ingredient(TechType.Diamond, 2),
                    new Ingredient(TechType.AluminumOxide, 2),
                    new Ingredient(TechType.Nickel, 2)
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
            return ImageUtils.LoadSpriteFromFile($"{SeaTruckDepthUpgrades_Main.modFolder}/Assets/SeaTruckDepthUpgrade_MK5.png");
        }
    }
}
