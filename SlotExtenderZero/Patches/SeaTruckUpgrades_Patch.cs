using BZCommon;
using HarmonyLib;
using System.Reflection;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch(MethodType.Constructor)]
    internal class SeaTruckUpgrades_Constructor_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(SeaTruckUpgrades __instance)
        {
            if (Main.SeatruckUpgradesCtorPatched)
            {
                BZLogger.Debug("SeaTruck constructor already patched. Exit method.");
                return;
            }                        

            __instance.SetPrivateField("slotIDs", SlotHelper.SessionSeatruckSlotIDs, BindingFlags.Static);

            BZLogger.Debug($"SeaTruck constructor patched. ID: {__instance.GetInstanceID()}");

            Main.SeatruckUpgradesCtorPatched = true;
        }
    }    
    
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    internal class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(SeaTruckUpgrades __instance)
        {            
            __instance.gameObject.EnsureComponent<SlotExtenderZero>();            

            BZLogger.Debug($"SlotExtenderZero added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
    
}
