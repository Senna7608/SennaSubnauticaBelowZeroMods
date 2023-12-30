using BZHelper;
using HarmonyLib;
using ModdedArmsHelperBZ.API;

namespace ModdedArmsHelperBZ.Patches
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Awake")]
    internal class SeaTruckUpgrades_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {
            if (__instance.gameObject != null)
            {
                __instance.gameObject?.EnsureComponent<SeatruckArmManager>();

                BZLogger.Debug($"HarmonyPatch: SeaTruckUpgrades, Method: Awake, Mode: Postfix, Priority: Normal, Component: SeatruckArmManager, ID: {__instance.gameObject.GetInstanceID()}");
            }            
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("OnUpgradeModuleUse")]
    internal class SeaTruckUpgrades_OnUpgradeModuleUse_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, TechType techType, int slotID)
        {
            if (ArmServices.main.IsSeatruckArm(techType))
            {                
                return false;
            }            

            return true;
        }
    }    
}
