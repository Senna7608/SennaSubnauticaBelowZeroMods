using BZHelper;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SeaTruckScannerModule
{
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
