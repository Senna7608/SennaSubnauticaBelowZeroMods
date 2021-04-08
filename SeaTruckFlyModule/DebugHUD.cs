using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SeaTruckFlyModule
{
    public partial class FlyManager //DebugHUD
    {
        GameObject DebugHUD;

        TextMeshProUGUI truckPositionText;
        TextMeshProUGUI truckStateText;
        TextMeshProUGUI isFlyingText;
        TextMeshProUGUI surfaceText;

        private static readonly Dictionary<TruckState, string> flyStateStringCache = new Dictionary<TruckState, string>()
        {
            { TruckState.Diving, "Diving" },
            { TruckState.TakeOff, "Take Off" },
            { TruckState.Flying, "Flying" },
            { TruckState.Landing, "Landing" },
            { TruckState.Landed, "Landed" }
        };

        private static readonly Dictionary<TruckPosition, string> positionStringCache = new Dictionary<TruckPosition, string>()
        {
            { TruckPosition.AboveSurface, "Above Surface" },
            { TruckPosition.AboveWater, "AboveWater" },
            { TruckPosition.BelowWater, "Below Water" },
            { TruckPosition.NearSurface, "Near Surface" },
            { TruckPosition.OnSurface, "On Surface" },
        };

        private void InitDebugHUD()
        {
            Transform Indicators = HUD.transform.Find("Indicators");

            DebugHUD = new GameObject("DebugHUD", new Type[] { typeof(RectTransform) });

            DebugHUD.transform.SetParent(Indicators);
            Utils.ZeroTransform(DebugHUD.transform);

            Sprite backGround = Resources.Load<Sprite>("Sprites/scannerroomUI_listbg");
            GameObject debugHudBackground = Instantiate(Indicators.Find("Background").gameObject, DebugHUD.transform);
            debugHudBackground.name = "Background";
            debugHudBackground.transform.localPosition = new Vector3(5f, 128f, 0f);
            debugHudBackground.transform.localScale = new Vector3(0.97f, 0.84f, 1f);
            Image image = debugHudBackground.GetComponent<Image>();
            image.sprite = backGround;

            GameObject truckPositionTitle = Instantiate(Indicators.Find("Power").gameObject, DebugHUD.transform);
            truckPositionTitle.name = "truckPositionTitle";
            truckPositionTitle.transform.localPosition = new Vector3(-100f, 160.0f, 0f);
            TextMeshProUGUI truckPositionTitleText = truckPositionTitle.GetComponent<TextMeshProUGUI>();            
            truckPositionTitleText.text = "Position:";
            truckPositionTitleText.fontSize = 20;
            truckPositionTitleText.color = Color.red;
            truckPositionTitleText.alignment = TextAlignmentOptions.BaselineRight;

            GameObject truckPosition = Instantiate(truckPositionTitle, DebugHUD.transform);
            truckPosition.name = "truckPosition";
            truckPosition.transform.localPosition = new Vector3(0f, 160.0f, 0f);
            truckPositionText = truckPosition.GetComponent<TextMeshProUGUI>();            
            truckPositionText.text = string.Empty;
            truckPositionText.alignment = TextAlignmentOptions.BaselineLeft;

            GameObject truckStateTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            truckStateTitle.name = "truckStateTitle";
            truckStateTitle.transform.localPosition = new Vector3(-100f, 135.0f, 0f);
            TextMeshProUGUI truckStateTitleText = truckStateTitle.GetComponent<TextMeshProUGUI>();            
            truckStateTitleText.text = "State:";

            GameObject truckState = Instantiate(truckPosition, DebugHUD.transform);
            truckState.name = "truckState";
            truckState.transform.localPosition = new Vector3(0f, 135.0f, 0f);
            truckStateText = truckState.GetComponent<TextMeshProUGUI>();            
            truckStateText.text = string.Empty;

            GameObject isFlyingTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            isFlyingTitle.name = "isFlyingTitle";
            isFlyingTitle.transform.localPosition = new Vector3(-100f, 110.0f, 0f);
            TextMeshProUGUI isFlyingTitleText = isFlyingTitle.GetComponent<TextMeshProUGUI>();
            isFlyingTitleText.text = "isFlying:";

            GameObject isFlying = Instantiate(truckPosition, DebugHUD.transform);
            isFlying.name = "isFlying";
            isFlying.transform.localPosition = new Vector3(0f, 110.0f, 0f);
            isFlyingText = isFlying.GetComponent<TextMeshProUGUI>();
            isFlyingText.text = string.Empty;

            GameObject surfaceTitle = Instantiate(truckPositionTitle, DebugHUD.transform);
            surfaceTitle.name = "surfaceTitle";
            surfaceTitle.transform.localPosition = new Vector3(-100f, 85.0f, 0f);
            TextMeshProUGUI surfaceTitleText = surfaceTitle.GetComponent<TextMeshProUGUI>();
            surfaceTitleText.text = "Surface:";

            GameObject surface = Instantiate(truckPosition, DebugHUD.transform);
            surface.name = "surface";
            surface.transform.localPosition = new Vector3(0f, 85.0f, 0f);
            surfaceText = surface.GetComponent<TextMeshProUGUI>();
            surfaceText.text = string.Empty;
        }

        private void UpdateDebugHUD()
        {
            //UpdateSeatruckPosition();

            //UpdateSeatruckState();

            surfaceText.text = IntStringCache.GetStringForInt((int)distanceFromSurface);

            isFlyingText.text = isFlying.value.ToString();
        }

        private void UpdateSeatruckPosition()
        {
            truckPositionText.text = positionStringCache[SeatruckPosition];            
        }

        private void UpdateSeatruckState()
        {
            truckStateText.text = flyStateStringCache[SeatruckState];
        }


    }
}
