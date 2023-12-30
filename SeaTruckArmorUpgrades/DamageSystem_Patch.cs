using UnityEngine;
using HarmonyLib;
using System;
using BZHelper;

namespace SeaTruckArmorUpgrades
{
    [HarmonyPatch(typeof(DamageSystem))]
    [HarmonyPatch("CalculateDamage")]
    [HarmonyPatch(new Type[] { typeof(float), typeof(DamageType), typeof(GameObject), typeof(GameObject) })]
    public class DamageSystem_CalculateDamage_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(DamageSystem __instance, ref float damage, DamageType type, GameObject target, GameObject dealer = null)
        {
            if (target.GetComponent<SeaTruckSegment>() != null)
            {
                if (target.TryGetComponent(out SeaTruckArmorManager armorManager))
                {
                    armorManager.originalDamage = damage;

                    damage *= armorManager.DamageReductionMultiplier;
                }
                else
                {
                    BZLogger.Debug("Manager not found in Seatruck gameobject!");
                }                           
            }

            return true;
        }
    }
}
