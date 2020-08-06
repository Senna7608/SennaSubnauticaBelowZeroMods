using BZCommon;
using HarmonyLib;

namespace SeaTruckStorage
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Start")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {            
            __instance.gameObject.EnsureComponent<SeaTruckStorageManager>();

            BZLogger.Debug("SeaTruckStorage", $"Storage Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("IsAllowedToAdd")]
    public static class SeaTruckUpgrades_IsAllowedToAdd_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, Pickupable pickupable, bool verbose, ref bool __result)
        {
            TechType techType = pickupable.GetTechType();

            if (techType == SeaTruckStorage.TechTypeID)
            {
                if (__instance.modules.GetCount(techType) <= 2)
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}
