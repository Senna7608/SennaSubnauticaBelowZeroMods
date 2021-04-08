using HarmonyLib;
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

            BZLogger.Log($"Seatruck Armor Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }    
}
