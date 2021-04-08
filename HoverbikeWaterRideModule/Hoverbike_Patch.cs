using BZCommon;
using HarmonyLib;

namespace HoverbikeWaterRideModule
{
    [HarmonyPatch(typeof(Hoverbike))]
    [HarmonyPatch("Start")]
    public class Hoverbike_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Hoverbike __instance)
        {
            __instance.gameObject.EnsureComponent<WaterRideManager>();

            BZLogger.Debug("HoverbikeWaterRideModule", $"Water Ride Manager added in Hoverbike.Start -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
}