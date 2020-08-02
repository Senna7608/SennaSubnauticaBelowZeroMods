using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;

namespace SeaTruckStorage
{
    internal class SeaTruckStorage : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckStorage()
            : base(nameID: "SeaTruckStorage",
                  iconFilePath: $"{Main.modFolder}/Assets/SeaTruckStorageIcon.png",
                  iconTechType: TechType.None,
                  friendlyName: "Seatruck Storage",
                  description: "A big storage locker. Seatruck compatible.",
                  template: TechType.VehicleStorageModule,                  
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.Workbench,
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
                Ingredients = new List<Ingredient>(new Ingredient[2]
                {
                    new Ingredient(TechType.TitaniumIngot, 2),
                    new Ingredient(TechType.Lithium, 2)

                })
            };
        }

        public override GameObject GetGameObject()
        {
            base.GetGameObject();

            SeamothStorageContainer container = _GameObject.GetComponent<SeamothStorageContainer>();

            container.width = 8;
            container.height = 8;

            return _GameObject;
        }
    }
}
