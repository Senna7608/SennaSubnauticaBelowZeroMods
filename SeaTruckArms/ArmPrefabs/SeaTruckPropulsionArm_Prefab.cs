using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;
using SeaTruckArms.API;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckPropulsionArm_Prefab : CraftableSeaTruckArm
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckPropulsionArm_Prefab(SeaTruckArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckPropulsionArmModule",
                  friendlyName: "SeaTruck propulsion arm",
                  description: "Allows SeaTruck to use propulsion arm.",                  
                  armTemplate: ArmTemplate.PropulsionArm,
                  requiredForUnlock: TechType.None,
                  fragment: fragment
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
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Magnetite, 2),
                    new Ingredient(TechType.Lithium, 1)
                })
            };
        }

        protected override EncyData GetEncyclopediaData()
        {
            return null;
        }

        protected override void PostModify()
        {
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ExosuitPropulsionArmModule);
        }
    }
}
