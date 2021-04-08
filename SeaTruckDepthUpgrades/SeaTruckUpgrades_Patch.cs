using BZCommon;
using HarmonyLib;

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

            BZLogger.Debug($"Seatruck Depth Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
}
