using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckSpeedUpgrades
{
    internal class SeaTruckSpeedMK3 : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckSpeedMK3()
            : base(nameID: "SeaTruckSpeedMK3",
                  iconFilePath: $"{Main.modFolder}/Assets/SeaTruckSpeed_MK3.png",
                  iconTechType: TechType.None,
                  friendlyName: "Seatruck Speed Upgrade MK3",
                  description: "Increases Seatruck speed by 150%.\n Does not stack.",
                  template: TechType.SeaTruckUpgradeAfterburner,                  
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.SeaTruckUpgradeHorsePower,
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
                Ingredients = new List<Ingredient>(new Ingredient[3]
                {
                    new Ingredient(SeaTruckSpeedMK2.TechTypeID, 1),
                    new Ingredient(TechType.SeaTruckUpgradeHorsePower, 1),
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
