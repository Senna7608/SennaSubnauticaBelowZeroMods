using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.ArmHandlerRequesters;
using ModdedArmsHelperBZ.API;
using Nautilus.Crafting;
using BZHelper.NautilusHelpers;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckClawArm_Prefab : CraftableModdedArm
    {
        internal SeatruckClawArm_Prefab(SpawnableArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckClawArm",
                  friendlyName: "Seatruck claw arm",
                  description: "Allows Seatruck to use claw arm.",
                  armType: ArmType.SeatruckArm,
                  armTemplate: ArmTemplate.ClawArm,
                  requiredForUnlock: TechType.Exosuit,
                  fragment: fragment
                  )
        {
        }

        protected override RegisterArmRequest RegisterArm()
        {
            return new RegisterArmRequest(this, new ClawArmModdingRequest());
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
            return SpriteManager.Get(TechType.ExosuitClawArmModule);
        }
    }
}
