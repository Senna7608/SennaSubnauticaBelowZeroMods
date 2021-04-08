using BZCommon;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UWE;

namespace SeaTruckScannerModule
{
    /*
    [HarmonyPatch(typeof(uGUI_Equipment), "Awake")]
    internal class uGUI_Equipment_Awake_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(uGUI_Equipment __instance)
        {
            if (Main.uGUI_PatchComplete)
                return;

            Transform transform = __instance.gameObject.transform;

            GameObject original = null;

            for (int i = 1; i < 5; i++)
            {
                original = transform.Find($"BatteryCharger{i}").gameObject;

                GameObject clone = UnityEngine.Object.Instantiate(original, transform, false);

                clone.name = $"ScannerModuleBattery{i}";                

                clone.GetComponent<uGUI_EquipmentSlot>().slot = $"ScannerModuleBattery{i}";
            }

            Main.uGUI_PatchComplete = true;

            BZLogger.Log("SeatruckScannerModule", "uGUI_Equipment Slots Patched.");
        }
    }
    */

    [HarmonyPatch(typeof(uGUI_MainMenu))]
    [HarmonyPatch("Awake")]
    internal class uGUI_MainMenu_Awake_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_MainMenu __instance)
        {
            if (uGUI.main.gameObject.transform.Find("ScreenCanvas/SeaTruckResourceTracker") == null)
            {
                GameObject SeaTruckResourceTracker = new GameObject("SeaTruckResourceTracker", new Type[] { typeof(RectTransform) });

                SeaTruckResourceTracker.transform.SetParent(uGUI.main.gameObject.transform.Find("ScreenCanvas").transform, false);
                SeaTruckResourceTracker.layer = 5;

                RectTransform rt = SeaTruckResourceTracker.GetComponent<RectTransform>();

                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.anchoredPosition = new Vector2(0, 0);
                rt.sizeDelta = new Vector2(0, 0);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
                rt.offsetMin = new Vector2(0, 0);
                rt.offsetMax = new Vector2(0, 0);
                rt.position = new Vector3(21.0f, 0.0f, 0.0f);
                rt.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                rt.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                rt.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                rt.right = new Vector3(0.0f, 0.0f, -1.0f);
                rt.up = new Vector3(0.0f, 1.0f, 0.0f);
                rt.forward = new Vector3(1.0f, 0.0f, 0.0f);
                rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                SeaTruckResourceTracker.AddComponent<uGUI_SeaTruckResourceTracker>();
            }
        }
    }

    [HarmonyPatch(typeof(CrafterLogic))]
    [HarmonyPatch("NotifyCraftEnd")]
    internal class CrafterLogic_NotifyCraftEnd_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(CrafterLogic __instance, GameObject target, TechType techType)
        {
            if (techType == SeaTruckScannerModule_Prefab.TechTypeID)
            {
                if (!target.activeSelf)
                {
                    target.SetActive(true);
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(AddressablesUtility))]
    [HarmonyPatch("InstantiateAsync")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(Transform), typeof(Vector3), typeof(Quaternion), typeof(bool) })]
    internal class AddressablesUtility_InstantiateAsync_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(string key, Transform parent, Vector3 position, Quaternion rotation, bool awake, ref CoroutineTask<GameObject> __result)
        {            
            if (key.Equals(Main.fragment.VirtualPrefabFilename))
            {
                BZLogger.Debug($"InstantiateAsync_Patch(Scanner), key: {key}");
                __result = Main.fragment.SpawnFragmentAsync(parent, position, rotation, awake);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ResourceTrackerDatabase))]
    [HarmonyPatch("Start")]    
    internal class ResourceTrackerDatabase_Start_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(ResourceTrackerDatabase __instance)
        {
            HashSet<TechType> undetectableTechTypes = __instance.GetPrivateField("undetectableTechTypes", BindingFlags.Static) as HashSet<TechType>;            
            undetectableTechTypes.Clear();
        }
    }

}
