using BZCommon;
using Harmony;

namespace SeaTruckArms.Patches
{
    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("Start")]
    public class Drillable_Start_Patch
    {
        static void Postfix(Drillable __instance)
        {
            __instance.gameObject.EnsureComponent<SeaTruckDrillable>();

            BZLogger.Log("SeaTruckArms", $"SeatruckDrillable component added in Drillable.Start -> Postfix Patch. ID: {__instance.GetInstanceID()}");
        }
    }

    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("HoverDrillable")]
    public class Drillable_HoverDrillable_Patch
    {
        static bool Prefix(Drillable __instance)
        {
            if (Player.main.IsPilotingSeatruck())
            {
                var seatruckDrillable = __instance.GetComponent<SeaTruckDrillable>();
                seatruckDrillable.HoverDrillable();

                return false;
            }

            return true;
        }
    }    
}
