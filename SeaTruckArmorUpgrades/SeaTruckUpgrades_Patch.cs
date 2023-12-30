using BZHelper;
using HarmonyLib;

namespace SeaTruckArmorUpgrades
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
                __instance.gameObject?.EnsureComponent<SeaTruckArmorManager>();

                BZLogger.Debug($"HarmonyPatch: SeaTruckUpgrades, Method: Awake, Mode: Postfix, Priority: Normal, Component: SeaTruckArmorManager, ID: {__instance.gameObject.GetInstanceID()}");
            }           
        }
    }    
}
