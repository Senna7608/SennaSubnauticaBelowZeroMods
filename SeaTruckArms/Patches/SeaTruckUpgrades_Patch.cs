using BZCommon;
using Harmony;
using SeaTruckArms.ArmPrefabs;

namespace SeaTruckArms.Patches
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("Awake")]
    public class SeaTruckUpgrades_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeaTruckUpgrades __instance)
        {            
            __instance.gameObject.EnsureComponent<SeaTruckArmManager>();

            BZLogger.Log("SeaTruckArms", $"SeaTruckArmManager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    [HarmonyPatch("OnUpgradeModuleUse")]    
    public class SeaTruckUpgrades_OnUpgradeModuleUse_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckUpgrades __instance, TechType techType, int slotID)
        {
            if (techType == SeaTruckDrillArmPrefab.TechTypeID)
            {                
                return false;
            }
            else if (techType == SeaTruckClawArmPrefab.TechTypeID)
            {                
                return false;
            }
            else if (techType == SeaTruckGrapplingArmPrefab.TechTypeID)
            {
                return false;
            }
            else if (techType == SeaTruckTorpedoArmPrefab.TechTypeID)
            {
                return false;
            }

            return true;
        }
    }    
}
