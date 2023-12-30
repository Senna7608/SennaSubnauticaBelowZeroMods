using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BZHelper.NautilusHelpers;
using Nautilus.Crafting;
using Nautilus.Utility;

namespace SeaTruckArmorUpgrades
{
    internal class SeaTruckArmorMK3_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckArmorMK3_Prefab()
            : base(
                  techTypeName: "SeaTruckArmorMK3",                  
                  friendlyName: "Seatruck Armor Upgrade MK3",
                  description: "Increases Seatruck hull strength by 75%.\nDoes not stack.",
                  template: TechType.SeaTruckUpgradeHull1,
                  gamerResourceFileName: null,
                  requiredAnalysis: TechType.ExosuitPropulsionArmModule,
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
                Ingredients = new List<Ingredient>(new Ingredient[4]
                {
                    new Ingredient(SeaTruckArmorMK2_Prefab.TechTypeID, 1),                    
                    new Ingredient(TechType.EnameledGlass, 5),
                    new Ingredient(TechType.AluminumOxide, 5),
                    new Ingredient(TechType.Kyanite, 5),
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
            return ImageUtils.LoadSpriteFromFile($"{SeaTruckArmorUpgrades_Main.modFolder}/Assets/SeatruckArmor_MK3.png");
        }
    }
}
