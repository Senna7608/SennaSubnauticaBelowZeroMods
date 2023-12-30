using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BZHelper.NautilusHelpers;
using Nautilus.Crafting;
using Nautilus.Utility;

namespace SeaTruckStorage
{
    internal class SeaTruckStorage_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal SeaTruckStorage_Prefab()
            : base(
                  techTypeName: "SeaTruckStorage",                  
                  friendlyName: "Seatruck Storage",
                  description: "A big storage locker. Seatruck compatible.",
                  template: TechType.VehicleStorageModule,
                  gamerResourceFileName: null,
                  requiredAnalysis: TechType.Workbench,
                  groupForPDA: TechGroup.VehicleUpgrades,
                  categoryForPDA: TechCategory.VehicleUpgrades,
                  equipmentType: EquipmentType.SeaTruckModule,
                  quickSlotType: QuickSlotType.Passive,
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
                    new Ingredient(TechType.TitaniumIngot, 2),
                    new Ingredient(TechType.Lithium, 2)
                })
            };
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            SeamothStorageContainer container = GameObjectClone.GetComponent<SeamothStorageContainer>();
            container.width = 7;
            container.height = 8;

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
                TreeTypes = new List<CraftTreeType>()
                {
                    new CraftTreeType(CraftTree.Type.Fabricator, new string[] { "Upgrades", "SeatruckUpgrades" } )
                }
            };
        }

        protected override TabNode GetTabNodeData()
        {
            return null;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{SeaTruckStorage_Main.modFolder}/Assets/SeaTruckStorageIcon.png");
        }
    }
}
