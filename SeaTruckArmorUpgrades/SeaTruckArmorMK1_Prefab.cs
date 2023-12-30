using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BZHelper.NautilusHelpers;
using Nautilus.Crafting;
using Nautilus.Utility;

namespace SeaTruckArmorUpgrades
{
    internal class SeaTruckArmorMK1_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckArmorMK1_Prefab()
            : base(techTypeName: "SeaTruckArmorMK1",                  
                  friendlyName: "Seatruck Armor Upgrade MK1",
                  description: "Increases Seatruck hull strength by 25%.\nDoes not stack.",
                  template: TechType.SeaTruckUpgradeHull1,
                  gamerResourceFileName: null,
                  requiredAnalysis: TechType.Exosuit,
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
                Ingredients = new List<Ingredient>(new Ingredient[2]
                {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.EnameledGlass, 1)                    
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
                    new CraftTreeType(CraftTree.Type.Fabricator, new string[] { "Upgrades", "SeatruckUpgrades" } )                    
                }
            };
        }

        protected override TabNode GetTabNodeData()
        {
            return null;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{SeaTruckArmorUpgrades_Main.modFolder}/Assets/SeatruckArmor_MK1.png");
        }
    }
}
