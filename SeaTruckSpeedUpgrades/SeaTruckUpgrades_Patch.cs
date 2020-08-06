using BZCommon;
using HarmonyLib;
using System.Collections.Generic;

namespace SeaTruckSpeedUpgrades
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {
            __instance.gameObject.EnsureComponent<SeaTruckSpeedManager>();

            BZLogger.Debug("SeaTruckSpeedUpgrades", $"Seatruck Speed Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("UpgradeModuleChanged")]
    public static class SeaTruckUpgrades_UpgradeModuleChanged_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, string slot, TechType techType, bool added)
        {
            if (__instance.gameObject.TryGetComponent(out SeaTruckSpeedManager manager))
            {
                foreach (KeyValuePair<TechType, float> kvp in manager.Accelerations)
                {
                    if (kvp.Key == techType)
                    {
                        manager.CheckSlotsForSpeedUpgrades();
                        return false;
                    }
                }
            }
            else
            {
                BZLogger.Error("SeaTruckSpeedUpgrades", "Seatruck Speed Manager is not ready!");
                return true;
            }









            /*
            SeaTruckSpeedManager manager = null;

            try
            {
                manager = __instance.gameObject.GetComponent<SeaTruckSpeedManager>();
            }
            catch
            {
                BZLogger.Error("SeaTruckSpeedUpgrades", "SeaTruckSpeedManager is not ready!");
                return true;
            }

            if (manager)
            {
                foreach (KeyValuePair<TechType, float> kvp in manager.Accelerations)
                {
                    if (kvp.Key == techType)
                    {
                        manager.CheckSlotsForSpeedUpgradeMark1_3();
                        return false;
                    }
                }                
            }
            */
            return true;
        }
    }

}
