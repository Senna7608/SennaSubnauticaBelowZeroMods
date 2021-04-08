using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;
using SeaTruckArms.API;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckClawArm_Prefab : CraftableSeaTruckArm
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckClawArm_Prefab(SeaTruckArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckClawArmModule",
                  friendlyName: "Seatruck claw arm",
                  description: "Allows Seatruck to use claw arm.",                  
                  armTemplate: ArmTemplate.ClawArm,
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
                Ingredients = new List<Ingredient>(new Ingredient[3]
                {
                    new Ingredient(TechType.TitaniumIngot, 1),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.ComputerChip, 1)
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
            return SpriteManager.Get(TechType.ExosuitClawArmModule);
        }
    }
}
