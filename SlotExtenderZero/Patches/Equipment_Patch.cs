using System;
using BZCommon;
using HarmonyLib;
using UnityEngine;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    [HarmonyPatch("GetSlotType")]
    public class Equipment_GetSlotType_Patch
    {
        public static bool isPatched = false;

        [HarmonyPrefix]
        public static void Prefix(string slot, ref EquipmentType __result)
        {
            if (!isPatched)
            {
#if DEBUG
                BZLogger.Log($"[SlotExtenderZero] Equipment.GetSlotType() Prefix patch start.\n" +
                $"slot = [{slot}]");
#endif

                SlotHelper.ExpandSlotMapping();

                isPatched = true;

#if DEBUG
                BZLogger.Log("[SlotExtenderZero] Equipment.GetSlotType() Prefix patch end.");
#endif
            }
        }
    }


    /*
    [HarmonyPatch(typeof(Equipment))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(GameObject), typeof(Transform) })]
    public class Equipment_Constructor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(GameObject owner, Transform tr)
        {
            if (!Main.isEquipmentPatched && owner.name == "Player")
            {
                BZLogger.Debug("SlotExtenderZero", $"Equipment.ctor({owner.name}, ID: {owner.GetInstanceID()}) Postfix patch start.");

                SlotHelper.ExpandSlotMapping();

                Main.isEquipmentPatched = true;
                
                BZLogger.Debug("SlotExtenderZero", $"Equipment.ctor({owner.name}, ID: {owner.GetInstanceID()}) Postfix patch end.");
            }
        }
    }
    */

    [HarmonyPatch(typeof(Equipment))]
    [HarmonyPatch("AllowedToAdd")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(Pickupable), typeof(bool) })]
    public class Equipment_AllowedToAdd_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Equipment __instance, string slot, Pickupable pickupable, bool verbose, ref bool __result)
        {
            TechType techTypeInSlot = pickupable.GetTechType();

            if (techTypeInSlot == TechType.VehicleStorageModule)
            {
                if (__instance.owner.GetComponent<Exosuit>())
                {
                    // Do not allow more than four storage modules in Exosuit slots
                    if (__instance.GetCount(TechType.VehicleStorageModule) >= 4)
                    {
                        __result = false;
                        ErrorMessage.AddDebug("Slot Extender:\nStorage module limit reached!");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}