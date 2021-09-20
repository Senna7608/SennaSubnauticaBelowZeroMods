using BZCommon;
using HarmonyLib;

namespace SeaTruckFlyModule
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {
            __instance.gameObject.EnsureComponent<FlyManager>();

            BZLogger.Debug($"Seatruck Fly Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("UpgradeModuleChanged")]
    public static class SeaTruckUpgrades_UpgradeModuleChanged_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, string slot, TechType techType, bool added)
        {
            if (__instance.TryGetComponent(out FlyManager manager))
            { 
                if (manager)
                {
                    if (techType == SeaTruckFlyModule_Prefab.TechTypeID)
                    {
                        manager.CheckSlotsForFlyModule();
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
