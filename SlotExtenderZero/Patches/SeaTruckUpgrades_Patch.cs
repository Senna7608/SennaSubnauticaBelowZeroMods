using Harmony;
using BZCommon;
using System.Reflection;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch(MethodType.Constructor)]
    public class SeaTruckUpgrades_Constructor_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(SeaTruckUpgrades __instance)
        {
            if (Main.SeatruckUpgradesPatched)
            {
                BZLogger.Debug("SlotExtenderZero", "SeaTruck constructor already patched. Exit method.");
                return;
            }
       
            __instance.SetPrivateField("slotIDs", SlotHelper.SessionSeatruckSlotIDs, BindingFlags.Static);

            BZLogger.Debug("SlotExtenderZero", $"SeaTruck constructor patched. ID: {__instance.GetInstanceID()}");

            Main.SeatruckUpgradesPatched = true;
        }
    }    
    
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {            
            __instance.gameObject.EnsureComponent<SlotExtenderZero>();

            BZLogger.Debug("SlotExtenderZero", $"SlotExtenderZero added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
    
}
