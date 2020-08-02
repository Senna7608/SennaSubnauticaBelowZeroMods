using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckFlyModule
{
    internal class SeaTruckFlyModule : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckFlyModule()
            : base(nameID: "SeaTruckFlyModule",                  
                  iconFilePath: $"{Main.modFolder}/Assets/SeaTruckFlyModule.png",
                  iconTechType: TechType.None,
                  friendlyName: "Seatruck Fly Module",
                  description: "Allows Seatruck to fly.",
                  template: TechType.SeaTruckUpgradeAfterburner,                  
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.Hoverbike,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: EquipmentType.SeaTruckModule,
                  quickSlotType: QuickSlotType.Instant,
                  backgroundType: CraftData.BackgroundType.Normal,
                  itemSize: new Vector2int(1, 1),
                  gamerResourceFileName: null
                  )
        {
        }

        public override void Patch()
        {
            base.Patch();
            TechTypeID = TechType;
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

        public override GameObject GetGameObject()
        {
            base.GetGameObject();            

            return _GameObject;
        }
    }
}
