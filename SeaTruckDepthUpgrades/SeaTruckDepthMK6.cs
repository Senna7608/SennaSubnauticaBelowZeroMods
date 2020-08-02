using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckDepthUpgrades
{
    internal class SeaTruckDepthMK6 : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckDepthMK6()
            : base(nameID: "SeaTruckDepthMK6",
                  iconFilePath: $"{Main.modFolder}/Assets/SeaTruckDepthUpgrade_MK6.png",
                  iconTechType: TechType.None,
                  friendlyName: "Seatruck Depth Upgrade MK6",
                  description: "Increases Seatruck safe diving depth to 1900m.\nDoes not stack.",
                  template: TechType.SeaTruckUpgradeHull1,                  
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.ExosuitTorpedoArmModule,
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
                Ingredients = new List<Ingredient>(new Ingredient[4]
                {
                    new Ingredient(SeaTruckDepthMK5.TechTypeID, 1),
                    new Ingredient(TechType.Diamond, 4),
                    new Ingredient(TechType.AluminumOxide, 4),
                    new Ingredient(TechType.Kyanite, 4)
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
