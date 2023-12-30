using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using TMPro;
using BZCommon;
using QuickSlotExtenderZero.Configuration;
using BZCommon.Helpers;
using BZHelper;

namespace QuickSlotExtenderZero
{
    public class QSEzHandler : MonoBehaviour
    {
        public QSEzHandler Instance { get; private set; }
        public uGUI_QuickSlots quickSlots { get; private set; }
        public uGUI_ItemIcon[] icons { get; private set; }
        private bool islabelsAdded = false;
        public static object slotextenderzero_SLOTKEYSLIST;
        public List<string> SLOTEXTENDERZERO_SLOTKEYSLIST;        
        public Utils.MonitoredValue<bool> onConsoleInputFieldActive = new Utils.MonitoredValue<bool>();
        private bool isConsoleActive = false;        

        public void Awake()
        {
            Instance = GetComponent<QSEzHandler>();
            quickSlots = GetComponent<uGUI_QuickSlots>();

            if (Main.isExists_SlotExtenderZero)
            {
                SLOTEXTENDERZERO_SLOTKEYSLIST = new List<string>();

                ReadSlotExtenderZeroConfig();
            }
        }

        public void ReadSlotExtenderZeroConfig()
        {
            SLOTEXTENDERZERO_SLOTKEYSLIST.Clear();

            try
            {                
                slotextenderzero_SLOTKEYSLIST = Main.GetAssemblyClassPublicField("SlotExtenderZero.Configuration.SEzConfig", "SLOTKEYSLIST", BindingFlags.Static);                

                foreach (string item in (List<string>)slotextenderzero_SLOTKEYSLIST)
                {
                    SLOTEXTENDERZERO_SLOTKEYSLIST.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void Start()
        {            
            onConsoleInputFieldActive.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(OnConsoleInputFieldActive));
        }

        private void OnConsoleInputFieldActive(Utils.MonitoredValue<bool> isActive)
        {
            isConsoleActive = isActive.value;            
        }

        public void OnDestroy()
        {
            onConsoleInputFieldActive.changedEvent.RemoveHandler(this, OnConsoleInputFieldActive);            
        }
        
        internal void Update()
        {
            if (isConsoleActive)
                return;

            if (Main.isKeyBindigsUpdate)
                return;

            if (Player.main == null)
                return;

            if (Inventory.main == null)
                return;           

            if (!islabelsAdded)
                AddQuickSlotText(quickSlots);

            if (Player.main.isPiloting)
               return;            

            if (Input.GetKeyDown(QSEzConfig.KEYBINDINGS["Slot_6"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(5);
                return;
            }
            else if (Input.GetKeyDown(QSEzConfig.KEYBINDINGS["Slot_7"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(6);
                return;
            }
            else if (Input.GetKeyDown(QSEzConfig.KEYBINDINGS["Slot_8"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(7);
                return;
            }
            else if (Input.GetKeyDown(QSEzConfig.KEYBINDINGS["Slot_9"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(8);
                return;
            }
            else if (Input.GetKeyDown(QSEzConfig.KEYBINDINGS["Slot_10"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(9);
                return;
            }
            else if (Input.GetKeyDown(QSEzConfig.KEYBINDINGS["Slot_11"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(10);
                return;
            }
            else if (Input.GetKeyDown(QSEzConfig.KEYBINDINGS["Slot_12"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(11);
                return;
            }
        }

        internal void AddQuickSlotText(uGUI_QuickSlots instance)
        {
            if (instance == null)
            {
                BZLogger.Debug("uGUI_QuickSlots instance is null!");
                return;
            }

            BZLogger.Debug("uGUI_QuickSlots instance is ready.");

            icons = (uGUI_ItemIcon[])instance.GetPrivateField("icons");

            if (icons == null)
            {
                BZLogger.Debug("Cannot get uGUI_ItemIcons array!");
                return;
            }

            BZLogger.Debug($"Found [{icons.Length}] uGUI_ItemIcon in array.");

            if (HandReticle.main == null)
            {
                BZLogger.Debug("HandReticle.main is null!");
                return;
            }

            BZLogger.Debug("HandReticle.main is ready.");

            for (int i = 0; i < icons.Length; i++)
            {                
                if (Main.isExists_SlotExtenderZero)
                {
                    if (Player.main.GetPDA().state != PDA.State.Opening)
                    {
                        if (Player.main.inSeamoth)
                        {
                            AddTextToSlot(icons[i].transform, SLOTEXTENDERZERO_SLOTKEYSLIST[i]);
                        }
                        else if (Player.main.inExosuit)
                        {
                            if (i < 2)
                            {
                                continue;
                            }
                            else
                            {
                                AddTextToSlot(icons[i].transform, SLOTEXTENDERZERO_SLOTKEYSLIST[i - 2]);
                            }
                        }
                        else if (Player.main.IsPilotingSeatruck())
                        {
                            AddTextToSlot(icons[i].transform, SLOTEXTENDERZERO_SLOTKEYSLIST[i]);                            
                        }
                        else
                        {
                            AddTextToSlot(icons[i].transform, QSEzConfig.SLOTKEYSLIST[i]);                            
                        }
                    }
                }                
                else if (Player.main.inExosuit && Player.main.GetPDA().state != PDA.State.Opening)
                {
                    if (i < 2)
                    {
                        continue;
                    }
                    else
                    {
                        AddTextToSlot(icons[i].transform, QSEzConfig.SLOTKEYSLIST[i - 2]);                       
                    }
                }
                else
                {
                    AddTextToSlot(icons[i].transform, QSEzConfig.SLOTKEYSLIST[i]);                    
                }
            }

            islabelsAdded = true;
        }        

        private TextMeshProUGUI AddTextToSlot(Transform parent, string buttonText)
        {
            TextMeshProUGUI TMProtext = Instantiate(HandReticle.main.compTextHand);            
            TMProtext.gameObject.layer = parent.gameObject.layer;
            TMProtext.gameObject.name = "QSEZLabel";
            TMProtext.transform.SetParent(parent, false);
            TMProtext.transform.localScale = new Vector3(1, 1, 1);
            TMProtext.gameObject.SetActive(true);
            TMProtext.enabled = true;
            TMProtext.text = buttonText;
            TMProtext.fontSize = 17;
            TMProtext.color = QSEzConfig.TEXTCOLOR;
            RectTransformExtensions.SetParams(TMProtext.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), parent);
            TMProtext.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            TMProtext.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            TMProtext.rectTransform.anchoredPosition = new Vector2(0, -40);
            TMProtext.alignment = TextAlignmentOptions.Midline;
            TMProtext.raycastTarget = false;

            return TMProtext;
        }
    }
}
