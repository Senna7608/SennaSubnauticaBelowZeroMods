using System.Collections.Generic;
using BZCommon.Helpers.SMLHelpers;
using SeaTruckArms.API;
using SMLHelper.V2.Crafting;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckGrapplingArm_Prefab : CraftableSeaTruckArm
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckGrapplingArm_Prefab(SeaTruckArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckGrapplingArmModule",
                  friendlyName: "Seatruck grappling arm",
                  description: "Fires a grappling hook for enhanced environment traversal.",                  
                  armTemplate: ArmTemplate.GrapplingArm,
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
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Benzene, 1),
                    new Ingredient(TechType.Titanium, 2),
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
            return SpriteManager.Get(TechType.ExosuitGrapplingArmModule);
        }
    }
}
