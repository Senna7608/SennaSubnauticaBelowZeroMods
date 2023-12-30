using BZHelper;
using HarmonyLib;

namespace SeaTruckDepthUpgrades
{
    [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.Awake))]
    public class SeaTruckUpgrades_Awake_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(SeaTruckUpgrades __instance)
        {
            if (__instance.gameObject != null)
            {
                __instance.gameObject?.EnsureComponent<SeaTruckDepthManager>();
            }            
        }
    }    

    [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.LazyInitialize))]    
    public class SeaTruckUpgrades_LazyInitialize_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(SeaTruckUpgrades __instance)
        {
            if (!SeaTruckUpgrades.crushDepths.ContainsKey(SeaTruckDepthMK4_Prefab.TechTypeID))
            {
                SeaTruckUpgrades.crushDepths.Add(SeaTruckDepthMK4_Prefab.TechTypeID, 1200f);
            }

            if (!SeaTruckUpgrades.crushDepths.ContainsKey(SeaTruckDepthMK5_Prefab.TechTypeID))
            {
                SeaTruckUpgrades.crushDepths.Add(SeaTruckDepthMK5_Prefab.TechTypeID, 1550f);
            }

            if (!SeaTruckUpgrades.crushDepths.ContainsKey(SeaTruckDepthMK6_Prefab.TechTypeID))
            {
                SeaTruckUpgrades.crushDepths.Add(SeaTruckDepthMK6_Prefab.TechTypeID, 1900f);
            }

            BZLogger.Log($"SeaTruck 'crushDepths' patched in 'Start' method. ID: {__instance.gameObject.GetInstanceID()}");

            return true;
        }    
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.OnUpgradeModuleChange))]     
    internal class SeaTruckUpgrades_OnUpgradeModuleChange_Patch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        public static bool Prefix(SeaTruckUpgrades __instance, TechType techType, int slotID)
        {
            if (techType == SeaTruckDepthMK4_Prefab.TechTypeID ||
                techType == SeaTruckDepthMK5_Prefab.TechTypeID ||
                techType == SeaTruckDepthMK6_Prefab.TechTypeID)
            {
                SeaTruckDepthManager depthManager = __instance.gameObject?.EnsureComponent<SeaTruckDepthManager>();
                depthManager.StartCoroutine(depthManager.CheckDepthUpgradesAsync());
                return false;
            }    

            return true;
        }
    }
}
