using BZHelper;
using HarmonyLib;

namespace SeaTruckSpeedUpgrades
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Awake")]
    public class SeaTruckUpgrades_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {
            if (__instance.gameObject != null)
            {
                __instance.gameObject?.EnsureComponent<SeaTruckSpeedManager>();

                BZLogger.Debug($"HarmonyPatch: SeaTruckUpgrades, Method: Awake, Mode: Postfix, Priority: Normal, Component: SeaTruckSpeedManager, ID: {__instance.gameObject.GetInstanceID()}");
            }            
        }
    }
}
