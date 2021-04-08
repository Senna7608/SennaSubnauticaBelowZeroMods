using BZCommon;
using HarmonyLib;
using SeaTruckArms.API;
using System;
using UnityEngine;

namespace SeaTruckArms.Patches
{
    [HarmonyPatch(typeof(AddressablesUtility))]
    [HarmonyPatch("InstantiateAsync")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(Transform), typeof(Vector3), typeof(Quaternion), typeof(bool) })]
    internal class AddressablesUtility_InstantiateAsync_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(string key, Transform parent, Vector3 position, Quaternion rotation, bool awake, ref CoroutineTask<GameObject> __result)
        {
            if (Main.ArmFragmentPrefabs.TryGetValue(key, out SeaTruckArmFragment fragment))
            {
                BZLogger.Debug($"InstantiateAsync_Patch(Arms), key: {key}");
                __result = fragment.SpawnFragmentAsync(parent, position, rotation, awake);
                return false;
            }

            return true;
        }
    }
}
