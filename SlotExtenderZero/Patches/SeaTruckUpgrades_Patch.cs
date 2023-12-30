using HarmonyLib;
using System.Reflection;
using BZHelper;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch(MethodType.StaticConstructor)]
    [HarmonyPatch(".cctor")]
    internal class SeaTruckUpgrades_Constructor_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(SeaTruckUpgrades __instance)
        {
            __instance.SetPrivateField("slotIDs", SlotHelper.SessionSeatruckSlotIDs, BindingFlags.Static);
        }
    }
    
    [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.UnlockDefaultModuleSlots))]    
    internal class SeaTruckUpgrades_UnlockDefaultModuleSlots_Patch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        internal static void Prefix(SeaTruckUpgrades __instance)
        {            
            __instance.SetPrivateField("slotIDs", SlotHelper.SessionSeatruckSlotIDs, BindingFlags.Static);            

            if (__instance.gameObject != null)
            {
                __instance.gameObject?.EnsureComponent<SlotExtenderZeroControl>();
            }            
        }        
    }
}
