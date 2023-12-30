﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SlotExtenderZero.Configuration;
using BZHelper;

namespace SlotExtenderZero
{
    internal class uGUI_SlotTextHandler : MonoBehaviour
    {
        public static uGUI_SlotTextHandler Instance { get; private set; }

        private Dictionary<string, TextMeshProUGUI> ALLSLOTS_Text = new Dictionary<string, TextMeshProUGUI>();

        public void Awake()
        {
            Instance = this;

            ALLSLOTS_Text.Clear();

            uGUI_Equipment uGUIequipment = gameObject.GetComponent<uGUI_Equipment>();

            //Dictionary<string, uGUI_EquipmentSlot> ALLSLOTS = (Dictionary<string, uGUI_EquipmentSlot>)uGUIequipment.GetPrivateField("allSlots");

            BZLogger.Trace("uGUI_SlotTextHandler tracing uGUIequipment.allSlots...");

            foreach (KeyValuePair<string, uGUI_EquipmentSlot> item in uGUIequipment.allSlots)
            {
                BZLogger.Trace($"slot name: {item.Key}");

                if (SlotHelper.ALLSLOTS.TryGetValue(item.Key, out SlotData slotData))
                {
                    TextMeshProUGUI TMProText = AddTextToSlot(item.Value.transform, slotData);

                    ALLSLOTS_Text.Add(slotData.SlotID, TMProText);
                }
            }

            BZLogger.Trace("Trace completed.");

            BZLogger.Log("uGUI_SlotTextHandler added.");
        }

        public void UpdateSlotText()
        {
            foreach (KeyValuePair<string, SlotData> kvp in SlotHelper.ALLSLOTS)
            {
                ALLSLOTS_Text[kvp.Key].text = kvp.Value.KeyCodeName;
            }
        }
        
        private TextMeshProUGUI AddTextToSlot(Transform parent, SlotData slotData)
        {
            TextMeshProUGUI TMProText = Instantiate(HandReticle.main.compTextHand);
            TMProText.gameObject.layer = parent.gameObject.layer;
            TMProText.gameObject.name = slotData.SlotConfigIDName;
            TMProText.transform.SetParent(parent, false);
            TMProText.transform.localScale = new Vector3(1, 1, 1);
            TMProText.gameObject.SetActive(true);
            TMProText.enabled = true;
            TMProText.text = slotData.KeyCodeName;
            TMProText.fontSize = 17;
            TMProText.color = SEzConfig.TEXTCOLOR;
            RectTransformExtensions.SetParams(TMProText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), parent);
            TMProText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            TMProText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            TMProText.rectTransform.anchoredPosition = new Vector2(0, 70);
            TMProText.alignment = TextAlignmentOptions.Center;
            TMProText.raycastTarget = false;

            return TMProText;
        }
    }
}
