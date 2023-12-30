using BZHelper;
using HarmonyLib;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UnlockDefaultEquipmentSlots))]    
    internal class Inventory_UnlockDefaultEquipmentSlots_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.First)]
        internal static void Postfix(Inventory __instance)
        {
            if (Main.ChipSlotsPatched)
                return;

            __instance.equipment.AddSlots(SlotHelper.NewChipSlotIDs);

            BZLogger.Log("Inventory Chip Slots Patched.");            

            Main.ChipSlotsPatched = true;
        }        
    }
}
