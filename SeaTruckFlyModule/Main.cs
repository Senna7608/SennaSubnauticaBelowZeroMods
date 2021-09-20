using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using QModManager.API.ModLoading;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using BZCommon.Helpers.Testing;
using UnityEngine.UI;
using BZCommon;

namespace SeaTruckFlyModule
{
    [QModCore]
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [QModPatch]
        public static void Load()
        {
            try
            {
                new SeaTruckFlyModule_Prefab().Patch();

                Assembly assembly = Assembly.GetExecutingAssembly();

                Harmony.CreateAndPatchAll(assembly, $"BelowZero.{assembly.GetName().Name}.mod");

                //SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "XMenu")
            {
                uGUIListener listener = uGUIHelper.CreateuGUITest();

                listener.testMethod += uGUITest;
            }            
        }

        public static void uGUITest()
        {            
            Sprite backGround = Resources.Load<Sprite>("Sprites/scannerroomUI_listbg");

            uGUI_SeaTruckHUD HUD = uGUI.main.GetComponentInChildren<uGUI_SeaTruckHUD>(true);
            
            Transform Indicators = HUD.root.transform.Find("Indicators");

            GameObject DebugHUD = new GameObject("DebugHUD", new Type[] { typeof(RectTransform) });

            DebugHUD.transform.SetParent(Indicators);
            Utils.ZeroTransform(DebugHUD.transform);
            
            GameObject debugHudBackground = UnityEngine.Object.Instantiate(Indicators.Find("Background").gameObject, DebugHUD.transform);
            debugHudBackground.name = "Background";
            debugHudBackground.transform.localPosition = new Vector3(5f, 128f, 0f);
            debugHudBackground.transform.localScale = new Vector3(0.97f, 0.84f, 1f);
            Image image = debugHudBackground.GetComponent<Image>();
            image.sprite = backGround;            

            GameObject truckPositionTitle = UnityEngine.Object.Instantiate(Indicators.Find("Power").gameObject, DebugHUD.transform);
            truckPositionTitle.name = "truckPositionTitle";
            TextMeshProUGUI truckPositionTitleText = truckPositionTitle.GetComponent<TextMeshProUGUI>();
            truckPositionTitle.transform.localPosition = new Vector3(-100f, 160.0f, 0f);
            truckPositionTitleText.text = "Position:";            
            truckPositionTitleText.fontSize = 20;
            truckPositionTitleText.color = new Color(1f, 0.831f, 0f, 1f);
            truckPositionTitleText.alignment = TextAlignmentOptions.BaselineRight;            
            
            GameObject truckPosition = UnityEngine.Object.Instantiate(truckPositionTitle, DebugHUD.transform);
            truckPosition.name = "truckPosition";
            TextMeshProUGUI truckPositionText = truckPosition.GetComponent<TextMeshProUGUI>();
            truckPositionText.transform.localPosition = new Vector3(0f, 160.0f, 0f);
            truckPositionText.text = "Empty";            
            truckPositionText.alignment = TextAlignmentOptions.BaselineLeft;            

            GameObject truckStateTitle = UnityEngine.Object.Instantiate(truckPositionTitle, DebugHUD.transform);
            truckStateTitle.name = "truckStateTitle";
            truckStateTitle.transform.localPosition = new Vector3(-100f, 135.0f, 0f);
            TextMeshProUGUI truckStateTitleText = truckStateTitle.GetComponent<TextMeshProUGUI>();            
            truckStateTitleText.text = "Fly State:";

            GameObject truckState = UnityEngine.Object.Instantiate(truckPosition, DebugHUD.transform);
            truckState.name = "truckState";
            truckState.transform.localPosition = new Vector3(0f, 135.0f, 0f);
            TextMeshProUGUI truckStateText = truckState.GetComponent<TextMeshProUGUI>();            
            truckStateText.text = "Empty";

            GameObject isFlyingTitle = UnityEngine.Object.Instantiate(truckPositionTitle, DebugHUD.transform);
            isFlyingTitle.name = "isFlyingTitle";
            isFlyingTitle.transform.localPosition = new Vector3(-100f, 110.0f, 0f);
            TextMeshProUGUI isFlyingTitleText = isFlyingTitle.GetComponent<TextMeshProUGUI>();
            isFlyingTitleText.text = "isFlying:";

            GameObject isFlying = UnityEngine.Object.Instantiate(truckPosition, DebugHUD.transform);
            isFlying.name = "isFlying";
            isFlying.transform.localPosition = new Vector3(0f, 110.0f, 0f);
            TextMeshProUGUI isFlyingText = isFlying.GetComponent<TextMeshProUGUI>();            
            isFlyingText.text = "Empty";

            GameObject surfaceTitle = UnityEngine.Object.Instantiate(truckPositionTitle, DebugHUD.transform);
            surfaceTitle.name = "surfaceTitle";
            surfaceTitle.transform.localPosition = new Vector3(-100f, 85.0f, 0f);
            TextMeshProUGUI surfaceTitleText = surfaceTitle.GetComponent<TextMeshProUGUI>();
            surfaceTitleText.text = "Surface:";

            GameObject surface = UnityEngine.Object.Instantiate(truckPosition, DebugHUD.transform);
            surface.name = "surface";
            surface.transform.localPosition = new Vector3(0f, 85.0f, 0f);
            TextMeshProUGUI surfaceText = surface.GetComponent<TextMeshProUGUI>();
            surfaceText.text = "Empty";

            HUD.enabled = false;

            HUD.root.SetActive(true);
            
            /*
            for (int i = -1; i < 32; i++)
            {
                BZLogger.Log($"Layer [{i}], name: {LayerMask.LayerToName(i)}");
            }
            */

        }
    }
}
