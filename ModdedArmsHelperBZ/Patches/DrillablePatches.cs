using HarmonyLib;
using ModdedArmsHelperBZ.API;

namespace ModdedArmsHelperBZ.Patches
{
    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("Start")]
    internal class Drillable_Start_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Drillable __instance)
        {            
             __instance.gameObject.EnsureComponent<SeatruckDrillable>();                     
        }
    }

    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("HoverDrillable")]
    internal class Drillable_HoverDrillable_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(Drillable __instance)
        {
            if (Player.main.IsPilotingSeatruck())
            {
                __instance.GetComponent<SeatruckDrillable>().HoverDrillable();

                return false;
            }

            return true;
        }
    }    
}
