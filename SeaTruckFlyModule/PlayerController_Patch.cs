using UnityEngine;
using HarmonyLib;

namespace SeaTruckFlyModule
{
    [HarmonyPatch(typeof(PlayerController))]
    [HarmonyPatch("WayToPositionClear")]
    public static class PlayerController_WayToPositionClear_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Vector3 position, GameObject ignoreObj, bool ignoreLiving, Vector3 fromPosition, ref bool __result)
        {
            GameObject parent = ignoreObj.transform.parent.gameObject;

            if (parent.TryGetComponent(out SeaTruckFlyManager manager))
            {
                if (manager)
                {
                    if (manager.isFlying.value)
                    {
                        __result = false;
                        return false;
                    }
                }
            }

            return true;
        }
    }

}
