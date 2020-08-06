using BZCommon;
using HarmonyLib;
using System.Collections.Generic;

namespace SeaTruckDepthUpgrades
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {            
            __instance.gameObject.EnsureComponent<SeaTruckDepthManager>();

            BZLogger.Debug("SeaTruckDepthUpgrades", $"Seatruck Depth Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("UpgradeModuleChanged")]
    public static class SeaTruckUpgrades_UpgradeModuleChanged_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, string slot, TechType techType, bool added)
        {
            if (__instance.gameObject.TryGetComponent(out SeaTruckDepthManager manager))
            {
                foreach (KeyValuePair<TechType, float> kvp in manager.CrushDepths)
                {
                    if (kvp.Key == techType)
                    {
                        manager.CheckSlotsForDepthUpgrades();
                        return false;
                    }
                }
            }
            else
            {
                BZLogger.Debug("SeaTruckDepthUpgrades", "Seatruck Depth Manager is not ready!");
                return true;
            }
            
            return true;
        }
    }
}
