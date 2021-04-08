using BZCommon;
using HarmonyLib;
using SeaTruckArms.API;

namespace SeaTruckArms.Patches
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Awake")]
    internal class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {            
            __instance.gameObject.EnsureComponent<SeaTruckArmManager>();
            
            BZLogger.Log($"SeaTruckArmManager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("OnUpgradeModuleUse")]
    internal class SeaTruckUpgrades_OnUpgradeModuleUse_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, TechType techType, int slotID)
        {
            if (ArmServices.main.IsSeaTruckArm(techType))
            {                
                return false;
            }            

            return true;
        }
    }    
}
