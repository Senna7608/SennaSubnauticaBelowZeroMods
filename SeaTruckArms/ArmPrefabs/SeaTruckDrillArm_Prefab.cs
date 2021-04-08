using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using BZCommon.Helpers.SMLHelpers;
using SeaTruckArms.API;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckDrillArm_Prefab : CraftableSeaTruckArm
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckDrillArm_Prefab(SeaTruckArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckDrillArmModule",
                  friendlyName: "Seatruck drill arm",
                  description: "Enables the mining of large resource deposits.",                                    
                  armTemplate: ArmTemplate.DrillArm,
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
                    new Ingredient(TechType.Titanium, 5),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Diamond, 4)
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
            return SpriteManager.Get(TechType.ExosuitDrillArmModule);
        }
    }
}
