using HarmonyLib;

namespace SeaTruckFlyModule
{
    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("CanTurn")]
    public class SeaTruckMotor_CanTurn_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance, ref bool __result)
        {
            if (__instance.TryGetComponent(out FlyManager manager))
            {
                if (manager.isEnabled)
                {
                    __result = true;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("UpdateDrag")]
    public class SeaTruckMotor_UpdateDrag_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance)
        {
            if (__instance.TryGetComponent(out FlyManager manager))
            {
                if (manager.altitude > 0)
                {                    
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("FixedUpdate")]
    public class SeaTruckMotor_FixedUpdate_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance)
        {
            if (__instance.TryGetComponent(out FlyManager manager))
            {
                if (manager.altitude > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("StopPiloting")]
    public class SeaTruckMotor_StopPiloting_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeaTruckMotor __instance, bool waitForDocking, bool forceStop, ref bool __result)
        {
            if (__instance.TryGetComponent(out FlyManager manager))
            {
                if (manager.altitude > 0 && manager.isFlying.value)
                {
                    ErrorMessage.AddDebug("Cannot leave Seatruck while flying!");
                    __result = false;
                    return false;
                }
            }

            return true;
        }
    }
}
