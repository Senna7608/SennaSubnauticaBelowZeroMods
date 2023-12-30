extern alias SEZero;
#if DEBUG
using SEZero::SlotExtenderZero.API;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace SeaTruckFlyModule
{
    public partial class FlyManager //DebugHUD
    {
        GameObject DebugHUD, FrontCenterRay, FrontLeftRay, FrontRightRay;
        LineRenderer FCenterRay, FLeftRay, FRightRay;

        TextMeshProUGUI truckPositionText;
        TextMeshProUGUI truckStateText;
        TextMeshProUGUI isFlyingText;
        TextMeshProUGUI surfaceText;
        TextMeshProUGUI surfaceLeftText;
        TextMeshProUGUI surfaceRightText;
        TextMeshProUGUI speedText;

        Material lineMaterial;

        private bool isDebugGraphicsReady = false;

        private static readonly Dictionary<TruckState, string> flyStateStringCache = new Dictionary<TruckState, string>()
        {
            { TruckState.None, "---" },
            { TruckState.Diving, "Diving" },
            { TruckState.TakeOff, "Take Off" },
            { TruckState.Flying, "Flying" },
            { TruckState.AutoFly, "AutoFly" },
            { TruckState.Landing, "Landing" },
            { TruckState.Landed, "Landed" }
        };

        private static readonly Dictionary<TruckPosition, string> positionStringCache = new Dictionary<TruckPosition, string>()
        {
            { TruckPosition.None, "---" },
            { TruckPosition.AboveSurface, "Above Surface" },
            { TruckPosition.AboveWater, "Above Water" },
            { TruckPosition.BelowWater, "Below Water" },
            { TruckPosition.NearSurface, "Near Surface" },
            { TruckPosition.OnSurface, "On Surface" },
        };

        private IEnumerator InitDebugHUD()
        {
            while (!isGraphicsComplete)
            {
                yield return null;
            }

            Transform Indicators = HUD.transform.Find("Indicators");

            DebugHUD = new GameObject("DebugHUD", new Type[] { typeof(RectTransform) });

            DebugHUD.transform.SetParent(Indicators);
            Utils.ZeroTransform(DebugHUD.transform);

            Sprite backGround = Resources.Load<Sprite>("Sprites/scannerroomUI_listbg");
            GameObject debugHudBackground = Instantiate(Indicators.Find("Background").gameObject, DebugHUD.transform);
            debugHudBackground.name = "Background";
            debugHudBackground.transform.localPosition = new Vector3(5f, 154f, 0f);
            debugHudBackground.transform.localScale = new Vector3(0.97f, 1.20f, 1f);
            Image image = debugHudBackground.GetComponent<Image>();
            image.sprite = backGround;

            GameObject truckPositionTitle = Instantiate(Indicators.Find("Power").gameObject, DebugHUD.transform);
            truckPositionTitle.name = "truckPositionTitle";
            truckPositionTitle.transform.localPosition = new Vector3(-100f, 210.0f, 0f);
            TextMeshProUGUI truckPositionTitleText = truckPositionTitle.GetComponent<TextMeshProUGUI>();            
            truckPositionTitleText.text = "Position:";
            truckPositionTitleText.fontSize = 20;
            truckPositionTitleText.color = Color.red;
            truckPositionTitleText.alignment = TextAlignmentOptions.BaselineRight;

            GameObject truckPosition = Instantiate(truckPositionTitle, DebugHUD.transform);
            truckPosition.name = "truckPosition";
            truckPosition.transform.localPosition = new Vector3(0f, 210.0f, 0f);
            truckPositionText = truckPosition.GetComponent<TextMeshProUGUI>();            
            truckPositionText.text = string.Empty;
            truckPositionText.alignment = TextAlignmentOptions.BaselineLeft;

            GameObject truckStateTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            truckStateTitle.name = "truckStateTitle";
            truckStateTitle.transform.localPosition = new Vector3(-100f, 185.0f, 0f);
            TextMeshProUGUI truckStateTitleText = truckStateTitle.GetComponent<TextMeshProUGUI>();            
            truckStateTitleText.text = "State:";

            GameObject truckState = Instantiate(truckPosition, DebugHUD.transform);
            truckState.name = "truckState";
            truckState.transform.localPosition = new Vector3(0f, 185.0f, 0f);
            truckStateText = truckState.GetComponent<TextMeshProUGUI>();            
            truckStateText.text = string.Empty;

            GameObject isFlyingTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            isFlyingTitle.name = "isFlyingTitle";
            isFlyingTitle.transform.localPosition = new Vector3(-100f, 160.0f, 0f);
            TextMeshProUGUI isFlyingTitleText = isFlyingTitle.GetComponent<TextMeshProUGUI>();
            isFlyingTitleText.text = "isFlying:";

            GameObject isFlying = Instantiate(truckPosition, DebugHUD.transform);
            isFlying.name = "isFlying";
            isFlying.transform.localPosition = new Vector3(0f, 160.0f, 0f);
            isFlyingText = isFlying.GetComponent<TextMeshProUGUI>();
            isFlyingText.text = string.Empty;

            GameObject surfaceTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            surfaceTitle.name = "surfaceTitle";
            surfaceTitle.transform.localPosition = new Vector3(-100f, 135.0f, 0f);
            TextMeshProUGUI surfaceTitleText = surfaceTitle.GetComponent<TextMeshProUGUI>();
            surfaceTitleText.text = "Surface Down:";

            GameObject surface = Instantiate(truckPosition, DebugHUD.transform);
            surface.name = "surface";
            surface.transform.localPosition = new Vector3(0f, 135.0f, 0f);
            surfaceText = surface.GetComponent<TextMeshProUGUI>();
            surfaceText.text = string.Empty;

            GameObject surfaceLeftTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            surfaceLeftTitle.name = "surfaceLeftTitle";
            surfaceLeftTitle.transform.localPosition = new Vector3(-100f, 110.0f, 0f);
            TextMeshProUGUI surfaceLeftTitleText = surfaceLeftTitle.GetComponent<TextMeshProUGUI>();
            surfaceLeftTitleText.text = "Surface LeftDown:";

            GameObject surfaceLeft = Instantiate(truckPosition, DebugHUD.transform);
            surfaceLeft.name = "surfaceLeft";
            surfaceLeft.transform.localPosition = new Vector3(0f, 110.0f, 0f);
            surfaceLeftText = surfaceLeft.GetComponent<TextMeshProUGUI>();
            surfaceLeftText.text = string.Empty;

            GameObject surfaceRightTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            surfaceRightTitle.name = "surfaceRightTitle";
            surfaceRightTitle.transform.localPosition = new Vector3(-100f, 85.0f, 0f);
            TextMeshProUGUI surfaceRightTitleText = surfaceRightTitle.GetComponent<TextMeshProUGUI>();
            surfaceRightTitleText.text = "Surface RightDown:";

            GameObject surfaceRight = Instantiate(truckPosition, DebugHUD.transform);
            surfaceRight.name = "surfaceRight";
            surfaceRight.transform.localPosition = new Vector3(0f, 85.0f, 0f);
            surfaceRightText = surfaceRight.GetComponent<TextMeshProUGUI>();
            surfaceRightText.text = string.Empty;

            GameObject speedTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            speedTitle.name = "speedTitle";
            speedTitle.transform.localPosition = new Vector3(-100f, 60.0f, 0f);
            TextMeshProUGUI speedTitleText = speedTitle.GetComponent<TextMeshProUGUI>();
            speedTitleText.text = "Speed:";

            GameObject speed = Instantiate(truckPosition, DebugHUD.transform);
            speed.name = "speed";
            speed.transform.localPosition = new Vector3(0f, 60.0f, 0f);
            speedText = speed.GetComponent<TextMeshProUGUI>();
            speedText.text = string.Empty;


            InitDebugRays();

            isDebugGraphicsReady = true;

            yield break;
        }

        private void InitDebugRays()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");

            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            lineMaterial.SetInt(ShaderPropertyID._SrcBlend, 5);
            lineMaterial.SetInt(ShaderPropertyID._DstBlend, 10);

            FrontCenterRay = new GameObject("FrontCenterRay");
            FrontCenterRay.transform.SetParent(telemetry.altitudeMeter.transform);
            Utils.ZeroTransform(FrontCenterRay.transform);
            FCenterRay = AddLineRenderer(FrontCenterRay, Color.green);

            FrontLeftRay = new GameObject("FrontLeftRay");
            FrontLeftRay.transform.SetParent(telemetry.altitudeMeter.transform);
            Utils.ZeroTransform(FrontLeftRay.transform);            
            FLeftRay = AddLineRenderer(FrontLeftRay, Color.yellow);

            FrontRightRay = new GameObject("FrontRightRay");
            FrontRightRay.transform.SetParent(telemetry.altitudeMeter.transform);
            Utils.ZeroTransform(FrontRightRay.transform);
            FRightRay = AddLineRenderer(FrontRightRay, Color.red);
        }

        private LineRenderer AddLineRenderer(GameObject container, Color lineColor)
        {
            LineRenderer lineRenderer = container.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = 0.008f;
            lineRenderer.endWidth = 0.008f;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.receiveShadows = false;
            lineRenderer.loop = false;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.alignment = LineAlignment.View;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.positionCount = 2;
            return lineRenderer;
        }


        private void UpdateDebugHUD()
        {
            if (!isDebugGraphicsReady)
                return;

            UpdateSeatruckPosition();

            UpdateSeatruckState();

            surfaceText.text = $"{IntStringCache.GetStringForInt((int)telemetry.distanceFromSurface)} m";
            surfaceLeftText.text = $"{IntStringCache.GetStringForInt((int)telemetry.FLeftDist)} m";
            surfaceRightText.text = $"{IntStringCache.GetStringForInt((int)telemetry.FRightDist)} m";
            speedText.text = $"{IntStringCache.GetStringForInt((int)telemetry.speed)} km/h";

            isFlyingText.text = isFlying.value.ToString();            

            FCenterRay.SetPosition(0, Vector3.zero);
            FCenterRay.SetPosition(1, Vector3.down * 5f);

            FLeftRay.SetPosition(0, Vector3.zero);            
            FLeftRay.SetPosition(1, telemetry.LeftDown * 5);

            FRightRay.SetPosition(0, Vector3.zero);
            FRightRay.SetPosition(1, telemetry.RightDown * 5);
        }

        private void UpdateSeatruckPosition()
        {
            if (!isDebugGraphicsReady)
                return;
            truckPositionText.text = positionStringCache[telemetry.SeatruckPosition];            
        }

        private void UpdateSeatruckState()
        {
            if (!isDebugGraphicsReady)
                return;
            truckStateText.text = flyStateStringCache[telemetry.SeatruckState];
        }


    }
}
#endif
