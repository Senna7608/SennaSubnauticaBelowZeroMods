using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckArmorUpgrades
{
    internal class SeaTruckArmorMK1 : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckArmorMK1()
            : base(nameID: "SeaTruckArmorMK1",
                  iconFilePath: $"{Main.modFolder}/Assets/SeatruckArmor_MK1.png",
                  iconTechType: TechType.None,
                  friendlyName: "Seatruck Armor Upgrade MK1",
                  description: "Increases Seatruck hull strength by 25%.\nDoes not stack.",
                  template: TechType.VehicleArmorPlating,                  
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "SeatruckUpgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.Exosuit,
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
                Ingredients = new List<Ingredient>(new Ingredient[2]
                {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.EnameledGlass, 1)                    
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
