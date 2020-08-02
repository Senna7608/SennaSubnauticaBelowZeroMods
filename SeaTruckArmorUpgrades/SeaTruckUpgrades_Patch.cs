using System.Collections.Generic;
using Harmony;
using BZCommon;

namespace SeaTruckArmorUpgrades
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {
            __instance.gameObject.EnsureComponent<SeaTruckArmorManager>();

            BZLogger.Log("SeaTruckArmorUpgrades", $"Seatruck Armor Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
    
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("UpgradeModuleChanged")]
    public static class SeaTruckUpgrades_UpgradeModuleChanged_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, string slot, TechType techType, bool added)
        {
            if (__instance.gameObject.TryGetComponent(out SeaTruckArmorManager manager))
            {
                foreach (KeyValuePair<TechType, float> kvp in manager.ArmorRatings)
                {
                    if (kvp.Key == techType)
                    {
                        manager.CheckSlotsForArmorUpgrades();
                        return false;
                    }
                }
            }
            else
            {
                BZLogger.Debug("SeaTruckArmorUpgrades", "Seatruck Armor Manager is not ready!");
                return true;
            }                   
            
            return true;
        }
    }
}
