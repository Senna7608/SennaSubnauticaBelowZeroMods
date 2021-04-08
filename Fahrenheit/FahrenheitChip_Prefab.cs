using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;

namespace Fahrenheit
{
    internal class FahrenheitChip_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal FahrenheitChip_Prefab()
            : base(
                  techTypeName: "FahrenheitChip",
                  friendlyName: "Fahrenheit chip",
                  description: "This chip converts in-game temperatures set in degrees Celsius to Fahrenheit.",
                  template: TechType.Compass,
                  requiredAnalysis: TechType.None,
                  groupForPDA: TechGroup.Personal,
                  categoryForPDA: TechCategory.Equipment,
                  equipmentType: EquipmentType.Chip,
                  quickSlotType: QuickSlotType.None,
                  backgroundType: CraftData.BackgroundType.Normal,
                  itemSize: new Vector2int(1, 1),
                  fragment: null
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
                Ingredients = new List<Ingredient>(new Ingredient[1]
                {
                    new Ingredient(TechType.ComputerChip, 1)
                })
            };
        }

        protected override void ModifyGameObject()
        {
        }

        protected override EncyData GetEncyclopediaData()
        {
            return null;
        }

        protected override CrafTreeTypesData GetCraftTreeTypesData()
        {
            return new CrafTreeTypesData()
            {
                TreeTypes = new List<CraftTreeType>(new CraftTreeType[1]
                {
                    new CraftTreeType(CraftTree.Type.Fabricator, new string[] { "Personal", "Equipment" } )
                })
            };
        }

        protected override TabNode GetTabNodeData()
        {
            return null;
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ComputerChip);
        }        
    }
}
