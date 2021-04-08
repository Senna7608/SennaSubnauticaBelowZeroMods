using System.Collections.Generic;
using BZCommon.Helpers.SMLHelpers;
using SeaTruckArms.API;
using SMLHelper.V2.Crafting;
using UnityEngine;

namespace SeaTruckArms.ArmPrefabs
{
    internal class SeaTruckTorpedoArm_Prefab : CraftableSeaTruckArm
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckTorpedoArm_Prefab(SeaTruckArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckTorpedoArmModule",
                  friendlyName: "SeaTruck torpedo arm",
                  description: "A standard payload delivery system adapted to fire torpedoes.",                  
                  armTemplate: ArmTemplate.TorpedoArm,
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
                    new Ingredient(TechType.Aerogel, 1)
                })
            };
        }

        protected override EncyData GetEncyclopediaData()
        {
            return null;
        }

        protected override void PostModify()
        {
            SeamothStorageContainer container = PrefabClone.GetComponent<SeamothStorageContainer>();

            container.width = 4;
            container.height = 4;
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ExosuitTorpedoArmModule);
        }
    }
}
