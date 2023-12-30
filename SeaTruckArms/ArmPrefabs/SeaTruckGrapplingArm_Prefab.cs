using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.ArmHandlerRequesters;
using ModdedArmsHelperBZ.API;
using Nautilus.Crafting;
using BZHelper.NautilusHelpers;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckGrapplingArm_Prefab : CraftableModdedArm
    {
        internal SeatruckGrapplingArm_Prefab(SpawnableArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckGrapplingArm",
                  friendlyName: "Seatruck grappling arm",
                  description: "Fires a grappling hook for enhanced environment traversal.",
                  armType: ArmType.SeatruckArm,
                  armTemplate: ArmTemplate.GrapplingArm,
                  requiredForUnlock: TechType.ExosuitGrapplingArmModule,
                  fragment: fragment
                  )
        {
        }

        protected override RegisterArmRequest RegisterArm()
        {
            return new RegisterArmRequest(this, new GrapplingArmModdingRequest());
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

        protected override void SetCustomLanguageText()
        {
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ExosuitGrapplingArmModule);
        }
    }
}
