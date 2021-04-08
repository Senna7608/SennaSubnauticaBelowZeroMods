/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BZCommon;
using BZCommon.Helpers.GUIHelper;

namespace SlotExtenderZero.Configuration
{
    internal class SEzConfigUI : MonoBehaviour
    {
        internal SEzConfigUI Instance { get; private set; }

        private Rect windowRect;
        private List<Rect> hotkeyButtonsRect;
        private List<Rect> hotkeyLabelsRect;

        private const int space = 10;
        private int selected = -1;
        private Event keyEvent;
        private string newKey;
        private bool waitingForKey = false;
        private static bool isVisible = false;
        private static int dropdownSelection = SEzConfig.MAXSLOTS - 5;

        private List<string> hotkeyLabels = new List<string>();
        private List<string> hotkeyButtons = new List<string>();

        private List<GuiItem> guiItem_Buttons = new List<GuiItem>();
        private List<GuiItem> guiItem_Labels = new List<GuiItem>();

        private static GUIContent[] dropDownContent = new GUIContent[]
        {
            new GUIContent("5"),
            new GUIContent("6"),
            new GUIContent("7"),
            new GUIContent("8"),
            new GUIContent("9"),
            new GUIContent("10"),
            new GUIContent("11"),
            new GUIContent("12"),
            new GUIContent("SeamothArmLeft"),
            new GUIContent("SeamothArmRight")
        };

        public void Awake()
        {
            useGUILayout = false;
            Instance = this;
            InitItems();
        }

        private void InitItems()
        {
            foreach (KeyValuePair<string, string> key in SEzConfig.Hotkeys_Config)
            {
                hotkeyLabels.Add(key.Key);
                hotkeyButtons.Add(key.Value);
            }

            hotkeyLabels.Add("MaxSlots");

            windowRect = SNWindow.InitWindowRect(new Rect(0, 0, Screen.width / 6, hotkeyLabels.Count * 47));

            hotkeyLabelsRect = SNWindow.SetGridItemsRect(new Rect(windowRect.x, windowRect.y, windowRect.width / 2, windowRect.height),
                                                1, hotkeyLabels.Count, Screen.height / 45, space, space, false, true);

            hotkeyButtonsRect = SNWindow.SetGridItemsRect(new Rect(windowRect.x + windowRect.width / 2, windowRect.y, windowRect.width / 2, windowRect.height),
                                                  1, hotkeyButtons.Count + 1, Screen.height / 45, space, space, false, true);

            guiItem_Labels.CreateGuiItemsGroup(hotkeyLabels, hotkeyLabelsRect, GuiItemType.LABEL, new GuiItemColor(normal: GuiColor.White), fontStyle: FontStyle.Bold, textAnchor: TextAnchor.MiddleLeft);

            guiItem_Buttons.CreateGuiItemsGroup(hotkeyButtons, hotkeyButtonsRect, GuiItemType.NORMALBUTTON, new GuiItemColor(), fontStyle: FontStyle.Bold, textAnchor: TextAnchor.MiddleCenter);

            guiItem_Labels.SetGuiItemsGroupLabel("Functions", hotkeyLabelsRect.GetLast(), new GuiItemColor());

            guiItem_Buttons.SetGuiItemsGroupLabel("Hotkeys", hotkeyButtonsRect.GetLast(), new GuiItemColor());
        }

        public void OnGUI()
        {
            SNWindow.CreateWindow(new Rect(0, 0, Screen.width / 6, hotkeyLabels.Count * 47), $"SlotExtender Configuration Window ({SEzConfig.PROGRAM_VERSION})", false, false);

            GUI.FocusControl("SlotExtender.ConfigUI");

            guiItem_Labels.DrawGuiItemsGroup();

            GuiItemEvent sBtn = guiItem_Buttons.DrawGuiItemsGroup();

            if (sBtn.ItemID != -1 && sBtn.MouseButton == 0)
            {
                StartAssignment(hotkeyButtons[sBtn.ItemID]);
                selected = sBtn.ItemID;
                guiItem_Buttons[sBtn.ItemID].Name = "Press any key!";
            }

            SNDropDown.CreateDropdown(hotkeyButtonsRect[hotkeyButtonsRect.Count - 2], ref isVisible, ref dropdownSelection, dropDownContent);

            float y = hotkeyLabelsRect[hotkeyLabelsRect.Count - 2].y + space * 2 + hotkeyLabelsRect[0].height;

            if (GUI.Button(new Rect(hotkeyLabelsRect[0].x, y, hotkeyLabelsRect[0].width, Screen.height / 22.5f), "Save", SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON)))
            {
                SaveAndExit();
            }

            if (!isVisible)
            {
                if (GUI.Button(new Rect(hotkeyButtonsRect[0].x, y, hotkeyButtonsRect[0].width, Screen.height / 22.5f), "Cancel", SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON)))
                {
                    Destroy(Instance);
                }
            }

            keyEvent = Event.current;

            if (keyEvent.isKey && waitingForKey)
            {
                newKey = InputHelper.GetKeyCodeAsInputName(keyEvent.keyCode);
                waitingForKey = false;
            }
        }

        private void StartAssignment(object keyName)
        {
            if (!waitingForKey)
                StartCoroutine(AssignKey(keyName));
        }

        private IEnumerator AssignKey(object keyName)
        {
            waitingForKey = true;

            yield return WaitForKey();

            int isFirst = 0;
            int keyCount = 0;

            for (int i = 0; i < hotkeyButtons.Count; i++)
            {
                if (hotkeyButtons[i].Equals(newKey))
                {
                    if (keyCount == 0)
                        isFirst = i;

                    keyCount++;
                }
            }

            if (keyCount > 0 && isFirst != selected)
            {
                BZLogger.Warn("SlotExtenderZero", "Duplicate keybind found, swapping keys...");
                hotkeyButtons[isFirst] = hotkeyButtons[selected];
                guiItem_Buttons[isFirst].Name = hotkeyButtons[selected];
            }

            hotkeyButtons[selected] = newKey;
            guiItem_Buttons[selected].Name = hotkeyButtons[selected];
            selected = -1;

            yield return null;
        }

        private IEnumerator WaitForKey()
        {
            while (!keyEvent.isKey)
            {
                yield return null;
            }
        }

        private void SaveAndExit()
        {
            for (int i = 0; i < hotkeyButtons.Count; i++)
            {
                SEzConfig.Hotkeys_Config[hotkeyLabels[i]] = hotkeyButtons[i];
            }

            int.TryParse(dropDownContent[dropdownSelection].text, out int result);

            if (result != SEzConfig.MAXSLOTS)
            {
                ErrorMessage.AddMessage("SlotExtender Warning!\nMaxSlots changed!\nPlease restart the game!");
                SEzConfig.MAXSLOTS = result;
            }

            SEzConfig.Config_Write();
            SEzConfig.KEYBINDINGS_Set();
            Main.GameInput_OnBindingsChanged();
            Destroy(Instance);
        }

        public SEzConfigUI()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(SEzConfigUI)) as SEzConfigUI;

                if (Instance == null)
                {
                    GameObject se_configUI = new GameObject("SEzConfigUIGO");
                    Instance = se_configUI.AddComponent<SEzConfigUI>();
                }
            }
            else
            {
                Instance.Awake();
            }
        }
    }
}
*/