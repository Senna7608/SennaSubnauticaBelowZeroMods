using BZCommon;
using Harmony;

namespace SeaTruckFlyModule
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {
            __instance.gameObject.EnsureComponent<SeaTruckFlyManager>();

            BZLogger.Debug("SeaTruckFlyModule", $"Seatruck Fly Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("UpgradeModuleChanged")]
    public static class SeaTruckUpgrades_UpgradeModuleChanged_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, string slot, TechType techType, bool added)
        {
            if (__instance.TryGetComponent(out SeaTruckFlyManager manager))
            { 
                if (manager)
                {
                    if (techType == SeaTruckFlyModule.TechTypeID)
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
