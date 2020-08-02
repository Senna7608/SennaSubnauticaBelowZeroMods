/*
using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckPropulsionArmPrefab : CraftableModItem
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckPropulsionArmPrefab()
            : base(nameID: "SeaTruckPropulsionArmModule",
                  iconFileName: null,
                  iconTechType: TechType.None,
                  friendlyName: "SeaTruck Propulsion Arm",
                  description: "Allows SeaTruck to use Propulsion Arm.",
                  template: TechType.ExosuitPropulsionArmModule,
                  fabricatorType: CraftTree.Type.SeamothUpgrades,
                  fabricatorTab: new string[] { "SeaTruckUpgrade" },
                  requiredAnalysis: TechType.ExosuitPropulsionArmModule,
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
                    new Ingredient(TechType.ExosuitPropulsionArmModule, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
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
*/