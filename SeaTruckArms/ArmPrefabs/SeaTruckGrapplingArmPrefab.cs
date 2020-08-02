using System.Collections.Generic;
using BZCommon.Helpers.SMLHelpers;
using SMLHelper.V2.Crafting;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckGrapplingArmPrefab : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckGrapplingArmPrefab()
            : base(nameID: "SeaTruckGrapplingArmModule",
                  iconFilePath: null,
                  iconTechType: TechType.None,
                  friendlyName: "Seatruck grappling arm",
                  description: "Allows Seatruck to use a  grappling arm.",
                  template: TechType.ExosuitGrapplingArmModule,
                  newTabNode: new TabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck)),
                  fabricatorTypes: new CraftTree.Type[] { CraftTree.Type.SeaTruckFabricator, CraftTree.Type.SeamothUpgrades, CraftTree.Type.Workbench },
                  fabricatorTabs: new string[][] { new string[] { "Upgrades" }, new string[] { "SeaTruckUpgrade" }, new string[] { "SeaTruckWBUpgrades" } },
                  requiredAnalysis: TechType.ExosuitGrapplingArmModule,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: (EquipmentType)200,
                  quickSlotType: QuickSlotType.Selectable,
                  backgroundType: CraftData.BackgroundType.ExosuitArm,
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
                    new Ingredient(TechType.ExosuitGrapplingArmModule, 1),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.ComputerChip, 1)
                })
            };
        }


        public override GameObject GetGameObject()
        {
            base.GetGameObject();

            _GameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            return _GameObject;
        }

    }
}
