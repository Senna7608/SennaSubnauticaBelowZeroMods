using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.ArmHandlerRequesters;
using ModdedArmsHelperBZ.API;
using Nautilus.Crafting;
using BZHelper.NautilusHelpers;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckDrillArm_Prefab : CraftableModdedArm
    {
        
        internal SeatruckDrillArm_Prefab(SpawnableArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckDrillArm",
                  friendlyName: "Seatruck drill arm",
                  description: "Enables the mining of large resource deposits.",
                  armType: ArmType.SeatruckArm,
                  armTemplate: ArmTemplate.DrillArm,
                  requiredForUnlock: TechType.ExosuitDrillArmModule,
                  fragment: fragment                  
                  )
        {
        }

        protected override RegisterArmRequest RegisterArm()
        {
            return new RegisterArmRequest(this, new DrillArmModdingRequest());
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
            return SpriteManager.Get(TechType.ExosuitDrillArmModule);
        }
    }
}
