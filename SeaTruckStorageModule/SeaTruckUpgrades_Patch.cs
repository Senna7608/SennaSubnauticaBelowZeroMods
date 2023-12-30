using BZHelper;
using HarmonyLib;

namespace SeaTruckStorage
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
                __instance.gameObject?.EnsureComponent<SeaTruckStorageManager>();

                BZLogger.Debug($"HarmonyPatch: SeaTruckUpgrades, Method: Awake, Mode: Prefix, Priority: Normal, Component: SeaTruckStorageManager, ID: {__instance.gameObject.GetInstanceID()}");
            }            
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

            if (techType == SeaTruckStorage_Prefab.TechTypeID)
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
