using BZCommon;
using HarmonyLib;

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

            BZLogger.Debug($"Seatruck Speed Manager added in SeaTruckUpgrades.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
}
