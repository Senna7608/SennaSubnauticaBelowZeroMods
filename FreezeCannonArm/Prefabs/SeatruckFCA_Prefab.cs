using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModdedArmsHelperBZ.API;
using BZHelper;
using FreezeCannonArm.Requests;
using Nautilus.Crafting;
using BZHelper.NautilusHelpers;
using Nautilus.Handlers;
using Nautilus.Utility;

namespace FreezeCannonArm.Prefabs
{
    internal class SeatruckFCA_Prefab : CraftableModdedArm
    {
        internal SeatruckFCA_Prefab(SpawnableArmFragment fragment)
            : base(
                  techTypeName: "SeaTruckFreezeCannonArm",
                  friendlyName: "SeaTruck freeze cannon arm",
                  description: "Allows SeaTruck to use freeze cannon arm.",
                  armType: ArmType.SeatruckArm,
                  armTemplate: ArmTemplate.PropulsionArm,
                  requiredForUnlock: TechType.None,
                  fragment: fragment
                  )
        {
        }

        protected override RegisterArmRequest RegisterArm()
        {
            return new RegisterArmRequest(this, new SeatruckFCA_ModdingRequest());
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[4]
                {
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.Magnetite, 2),
                    new Ingredient(TechType.Diamond, 1)
                })
            };
        }

        protected override EncyData GetEncyclopediaData()
        {
            return null;
        }

        protected override void SetCustomLanguageText()
        {
            LanguageHandler.SetLanguageLine("FreezeCannonArm_Targeting", "Targeting...");
            LanguageHandler.SetLanguageLine("FreezeCannonArm_Lock", "Lock target");
            LanguageHandler.SetLanguageLine("FreezeCannonArm_Release", "Release target");
            LanguageHandler.SetLanguageLine("FreezeCannonArm_Freeze", "Freeze target");
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            GameObject model = PrefabClone.transform.Find("model/exosuit_rig_armLeft:exosuit_propulsion_geo").gameObject;

            GraphicsHelper.ChangeObjectTexture(model, 2, illumTex: Main.illumTex);

            success.Set(true);
            yield break;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/FreezeCannonArm_icon.png");
        }
    }
}
