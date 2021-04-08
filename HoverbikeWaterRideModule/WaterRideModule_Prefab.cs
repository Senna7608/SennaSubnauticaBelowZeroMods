using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;

namespace HoverbikeWaterRideModule
{
    internal class WaterRideModule_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal WaterRideModule_Prefab()
            : base(
                  techTypeName: "WaterRideModule",                  
                  friendlyName: "Hoverbike water ride module",
                  description: "Allows the hoverbike to be used above water",
                  template: TechType.HoverbikeJumpModule,
                  requiredAnalysis: TechType.Hoverbike,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: EquipmentType.HoverbikeModule,
                  quickSlotType: QuickSlotType.Passive,
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
                    new Ingredient(TechType.Titanium, 2)                    
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

        protected override TabNode GetTabNodeData()
        {
            return null;
        }

        protected override CrafTreeTypesData GetCraftTreeTypesData()
        {
            return new CrafTreeTypesData()
            {
                TreeTypes = new List<CraftTreeType>(new CraftTreeType[1]
                {
                    new CraftTreeType(CraftTree.Type.Fabricator, new string[] { "Machines" } )                    
                })
            };
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.ComputerChip);
        }        
    }
}
