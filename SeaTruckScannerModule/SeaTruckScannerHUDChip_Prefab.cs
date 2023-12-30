using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BZHelper.NautilusHelpers;
using Nautilus.Crafting;
using Nautilus.Utility;

namespace SeaTruckScannerModule
{
    internal class SeaTruckScannerHUDChip_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckScannerHUDChip_Prefab()
            : base(
                  techTypeName: "SeaTruckScannerHUDChip",
                  friendlyName: "Seatruck scanner module HUD chip",
                  description: "Displays discovered resources on HUD.",
                  template: TechType.MapRoomHUDChip,
                  gamerResourceFileName: null,
                  requiredAnalysis: SeaTruckScannerModule_Prefab.TechTypeID,
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
        }

        protected override void PostPatch()
        {
            TechTypeID = Info.TechType;
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[2]
                {
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Magnetite, 1)
                })
            };
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
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
            return ImageUtils.LoadSpriteFromFile($"{SeaTruckScannerModule_Main.modFolder}/Assets/Seatruck_Scanner_HUDchip_icon.png");
        }
    }
}
