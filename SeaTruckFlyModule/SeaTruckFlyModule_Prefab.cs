using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;
using SMLHelper.V2.Utility;

namespace SeaTruckFlyModule
{
    internal class SeaTruckFlyModule_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckFlyModule_Prefab()
            : base(
                  techTypeName: "SeaTruckFlyModule",                  
                  friendlyName: "Seatruck Fly Module",
                  description: "Allows Seatruck to fly.",
                  template: TechType.SeaTruckUpgradeAfterburner,
                  requiredAnalysis: TechType.Hoverbike,
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
                Ingredients = new List<Ingredient>(new Ingredient[4]
                {                    
                    new Ingredient(TechType.Hoverbike, 1),
                    new Ingredient(TechType.WiringKit, 2),
                    new Ingredient(TechType.ComputerChip, 2),
                    new Ingredient(TechType.PrecursorIonPowerCell, 2)
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
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/SeaTruckFlyModule.png");
        }        
    }
}
