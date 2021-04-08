using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;
using SMLHelper.V2.Utility;
using UnityEngine;

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
                    new Ingredient(SeaTruckSpeedMK1_Prefab.TechTypeID, 1),
                    new Ingredient(TechType.SeaTruckUpgradeAfterburner, 1),
                    new Ingredient(TechType.ReactorRod, 2)
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
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/SeaTruckSpeed_MK2.png");
        }
    }
}
