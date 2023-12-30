using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.ArmHandlerRequesters;
using ModdedArmsHelperBZ.API;
using Nautilus.Crafting;
using BZHelper.NautilusHelpers;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeatruckTorpedoArm_Prefab : CraftableModdedArm
    {
        internal SeatruckTorpedoArm_Prefab(SpawnableArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckTorpedoArm",
                  friendlyName: "SeaTruck torpedo arm",
                  description: "A standard payload delivery system adapted to fire torpedoes.",
                  armType: ArmType.SeatruckArm,
                  armTemplate: ArmTemplate.TorpedoArm,
                  requiredForUnlock: TechType.ExosuitTorpedoArmModule,
                  fragment: fragment
                  )
        {
        }

        protected override RegisterArmRequest RegisterArm()
        {
            return new RegisterArmRequest(this, new TorpedoArmModdingRequest());
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
                    new Ingredient(TechType.Aerogel, 1)
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
            SeamothStorageContainer container = PrefabClone.GetComponent<SeamothStorageContainer>();

            container.width = 4;
            container.height = 4;

            success.Set(true);
            yield break;
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ExosuitTorpedoArmModule);
        }
    }
}
