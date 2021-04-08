using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;
using SeaTruckArms.API;
using SeaTruckArmAPITest.Requesters;
using UnityEngine;

namespace SeaTruckArmAPITest.ArmPrefabs
{
    internal class ClawArmPrefab_APITEST : CraftableSeaTruckArm
    {
        internal ClawArmPrefab_APITEST(SeaTruckArmFragment fragment)
            : base(
                  techTypeName: "ClawArmPrefab_APITEST",
                  friendlyName: "Seatruck claw arm APITEST",
                  description: "Allows Seatruck to use API based claw arm.",                  
                  armTemplate: ArmTemplate.ClawArm,
                  requiredForUnlock: TechType.None,
                  fragment: fragment
                  )
        {
        }

        protected override void PrePatch()
        {
        }

        protected override void PostPatch()
        {            
            ArmServices.main.RegisterArm(this, new ClawArmHandlerRequest_APITEST());
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
            return null;
        }
    }
}
