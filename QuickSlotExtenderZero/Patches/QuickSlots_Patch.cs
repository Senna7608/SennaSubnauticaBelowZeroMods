using System;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using BZCommon;
using QuickSlotExtenderZero.Configuration;

namespace QuickSlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(QuickSlots))]
    [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(GameObject), typeof(Transform), typeof(Transform), typeof(Inventory), typeof(Transform), typeof(int) })]
    public class QuickSlots_Constructor_Patch
    {
        public static readonly string[] ExpandedQuickSlotNames = new string[13]
        {
            "QuickSlot0",
            "QuickSlot1",
            "QuickSlot2",
            "QuickSlot3",
            "QuickSlot4",
            "QuickSlot5",
            "QuickSlot6",
            "QuickSlot7",
            "QuickSlot8",
            "QuickSlot9",
            "QuickSlot10",
            "QuickSlot11",
            "QuickSlot12",
        };

        [HarmonyPrefix]
        public static void Prefix(QuickSlots __instance, GameObject owner, Transform toolSocket, Transform cameraSocket,
                                    Inventory inv, Transform slotTr, ref int slotCount)
        {
            if (Main.isPatched)
                return;
            
            __instance.SetPrivateField("slotNames", ExpandedQuickSlotNames, BindingFlags.Static);

            slotCount = QSEzConfig.MAXSLOTS;

            BZLogger.Log("QuickSlots constructor Patched.");

            Main.isPatched = true;
        }
    }

    [HarmonyPatch(typeof(uGUI_QuickSlots))]
    [HarmonyPatch("Init")]
    public class uGUI_QuickSlots_Init_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_QuickSlots __instance)
        {
            Main.Instance = __instance.gameObject.EnsureComponent<QSEzHandler>();
            Main.Instance.AddQuickSlotText(__instance);
        }
    }

    [HarmonyPatch(typeof(DevConsole))]
    [HarmonyPatch("SetState")]
    public class DevConsole_SetState_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(DevConsole __instance, bool value)
        {          
            if (Main.Instance != null)
            {                
                Main.Instance.onConsoleInputFieldActive.Update(value);                
            }           
        }
    }
}
