using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckDepthUpgrades
{
    internal class SeaTruckDepthMK4 : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckDepthMK4()
            : base(nameID: "SeaTruckDepthMK4",
                  iconFilePath: $"{Main.modFolder}/Assets/SeaTruckDepthUpgrade_MK4.png",
                  iconTechType: TechType.None,
                  friendlyName: "Seatruck Depth Upgrade MK4",
                  description: "Increases Seatruck safe diving depth to 1100m.\nDoes not stack.",
                  template: TechType.SeaTruckUpgradeHull1,                  
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.BaseMoonpool,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: EquipmentType.SeaTruckModule,
                  quickSlotType: QuickSlotType.Passive,
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
                    new Ingredient(TechType.SeaTruckUpgradeHull3, 1),
                    new Ingredient(TechType.PlasteelIngot, 2),
                    new Ingredient(TechType.Diamond, 2)                    
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
