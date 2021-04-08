using BZCommon;
using HarmonyLib;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("UnlockDefaultEquipmentSlots")]
    internal class Inventory_UnlockDefaultEquipmentSlots_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(Inventory __instance)
        {
            __instance.equipment.AddSlots(SlotHelper.NewChipSlotIDs);

            BZLogger.Log("Inventory Chip Slots Patched.");

            Main.ChipSlotsPatched = true;
        }
    }
}
