using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;
using SMLHelper.V2.Utility;

namespace SeaTruckDepthUpgrades
{
    internal class SeaTruckDepthMK4_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckDepthMK4_Prefab()
            : base(
                  techTypeName: "SeaTruckDepthMK4",                  
                  friendlyName: "Seatruck Depth Upgrade MK4",
                  description: "Increases Seatruck safe diving depth to 1350m.\nDoes not stack.",
                  template: TechType.SeaTruckUpgradeHull1,
                  requiredAnalysis: TechType.BaseMoonpool,
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
                Ingredients = new List<Ingredient>(new Ingredient[3]
                {
                    new Ingredient(TechType.SeaTruckUpgradeHull3, 1),
                    new Ingredient(TechType.PlasteelIngot, 2),
                    new Ingredient(TechType.Diamond, 2)                    
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
                TreeTypes = new List<CraftTreeType>(new CraftTreeType[4]
                {
                    new CraftTreeType(CraftTree.Type.Fabricator, new string[] { "Upgrades", "SeatruckUpgrades" } ),
                    new CraftTreeType(CraftTree.Type.SeaTruckFabricator, new string[] { "Upgrades" } ),
                    new CraftTreeType(CraftTree.Type.SeamothUpgrades, new string[] { "SeaTruckUpgrade" } ),
                    new CraftTreeType(CraftTree.Type.Workbench, new string[] { "SeaTruckWBUpgrades" } )
                })
            };
        }

        protected override TabNode GetTabNodeData()
        {
            return new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck));
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/SeaTruckDepthUpgrade_MK4.png");
        }
    }
}
