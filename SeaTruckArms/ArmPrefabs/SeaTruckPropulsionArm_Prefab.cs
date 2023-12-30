using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.ArmHandlerRequesters;
using ModdedArmsHelperBZ.API;
using Nautilus.Crafting;
using BZHelper.NautilusHelpers;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckPropulsionArm_Prefab : CraftableModdedArm
    {
        internal SeatruckPropulsionArm_Prefab(SpawnableArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckPropulsionArm",
                  friendlyName: "SeaTruck propulsion arm",
                  description: "Allows SeaTruck to use propulsion arm.",
                  armType: ArmType.SeatruckArm,
                  armTemplate: ArmTemplate.PropulsionArm,
                  requiredForUnlock: TechType.ExosuitPropulsionArmModule,
                  fragment: fragment
                  )
        {
        }

        protected override RegisterArmRequest RegisterArm()
        {
            return new RegisterArmRequest(this, new PropulsionArmModdingRequest());
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
            return SpriteManager.Get(TechType.ExosuitPropulsionArmModule);
        }
    }
}
