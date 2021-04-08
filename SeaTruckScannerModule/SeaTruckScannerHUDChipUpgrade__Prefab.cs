using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckScannerModule
{
    internal class SeaTruckScannerHUDChipUpgrade_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckScannerHUDChipUpgrade_Prefab()
            : base(
                  techTypeName: "SeaTruckScannerHUDChipUpgrade",
                  friendlyName: "Seatruck scanner module HUD chip with distance upgrade",
                  description: "Scanner module HUD chip with distance text display.",
                  template: TechType.MapRoomHUDChip,
                  requiredAnalysis: SeaTruckScannerModule_Prefab.TechTypeID,
                  groupForPDA: TechGroup.Personal,
                  categoryForPDA: TechCategory.Equipment,
                  equipmentType: EquipmentType.Chip,
                  quickSlotType: QuickSlotType.None,
                  backgroundType: CraftData.BackgroundType.Normal,
                  itemSize: new Vector2int(1, 1),
                  fragment: null
                  )
        {
        }

        protected override void PrePatch()
        {
            TechTypeID = TechType;
        }

        protected override void PostPatch()
        {
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[2]
                {
                    new Ingredient(SeaTruckScannerHUDChip_Prefab.TechTypeID, 1),
                    new Ingredient(TechType.RadioTowerPPU, 1)                    
                })
            };
        }

        protected override void ModifyGameObject()
        {
        }

        protected override EncyData GetEncyclopediaData()
        {
            return null;
        }

        protected override CrafTreeTypesData GetCraftTreeTypesData()
        {
            return new CrafTreeTypesData()
            {
                TreeTypes = new List<CraftTreeType>(new CraftTreeType[1]
                {
                    new CraftTreeType(CraftTree.Type.Workbench, new string[] { string.Empty } )
                })
            };
        }

        protected override TabNode GetTabNodeData()
        {
            return null;
        }

        protected override Sprite GetItemSprite()
        {            
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/Seatruck_Scanner_HUDchip_icon.png");
        }
    }
}
