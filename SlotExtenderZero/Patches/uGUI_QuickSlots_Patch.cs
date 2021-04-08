using HarmonyLib;
using UnityEngine;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(uGUI_QuickSlots), "SetBackground")]
    internal class uGUI_QuickSlots_SetBackground_Patch
    {
        public static Sprite spriteExosuitArm = null;

        [HarmonyPrefix]
        internal static bool Prefix(uGUI_QuickSlots __instance, ref uGUI_ItemIcon icon, TechType techType, bool highlighted)
        {
            if (TechData.GetEquipmentType(techType) == (EquipmentType)ModdedEquipmentType.SeatruckArm)
            {
                if (icon == null)
                {
                    return false;
                }

                if (spriteExosuitArm == null)
                {
                    spriteExosuitArm = Object.Instantiate(__instance.spriteExosuitArm);
                }

                icon.SetBackgroundSprite(spriteExosuitArm);

                return false;
            }

            return true;
        }
    }
}
