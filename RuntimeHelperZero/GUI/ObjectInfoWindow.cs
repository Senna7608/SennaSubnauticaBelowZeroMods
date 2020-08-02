using System.Collections.Generic;
using UnityEngine;
using BZCommon.Helpers.GUIHelper;
using BZCommon;
using System;
using System.Reflection;
using static RuntimeHelperZero.Objects.PropertyHelper;
using static RuntimeHelperZero.Objects.FieldHelper;

namespace RuntimeHelperZero
{
    public partial class RuntimeHelperZero
    {
        private List<string> objectInfoText = new List<string>();
        private List<GuiItem> guiItems_objectInfo = new List<GuiItem>();
        private bool showObjectInfoWindow;
        private Vector2 scrollPos_objectInfo;
        private GuiItemEvent ScrollView_objectInfo_event;
        private int current_objectInfo_index = 0;
        private bool showSetButton = false;
        private UnityEngine.Object selected;
        private ObjectProperties objectProperties;
        private ObjectFields objectFields;

        public void ObjectInfoWindow_Awake(UnityEngine.Object _object)
        {
            selected = _object;

            showSetButton = false;

            objectInfoText.Clear();

            //componentInfoNames = _object.CreateComponentInfoList();            

            objectProperties = new ObjectProperties(_object);

            objectInfoText.Add("Properties:");

            foreach (ObjectProperty objectProperty in objectProperties)
            {
                objectInfoText.Add(objectProperty.ToString());
            }

            objectFields = new ObjectFields(_object);

            objectInfoText.Add("Fields:");

            foreach (ObjectField objectField in objectFields)
            {
                objectInfoText.Add(objectField.ToString());
            }

            guiItems_objectInfo.SetScrollViewItems(objectInfoText, 530f);

        }

        private void ObjectInfoWindow_OnGUI()
        {
            if (!showObjectInfoWindow)
                return;

            Rect windowrect = SNWindow.CreateWindow(new Rect(700, 732, 550, 348), "Object Information Window");

            ScrollView_objectInfo_event = SNScrollView.CreateScrollView(new Rect(windowrect.x + 5, windowrect.y, windowrect.width - 10, windowrect.height - 60), ref scrollPos_objectInfo, ref guiItems_objectInfo, $"Information of this", componentNames[selected_component] , 10);

            if (GUI.Button(new Rect(windowrect.x + 5, windowrect.y + (windowrect.height - 50), 120, 22), ComponentInfoWindow[0]))
            {
                AddToLog();
            }

            if (showSetButton)
            {
                if (GUI.Button(new Rect(windowrect.x + 130, windowrect.y + (windowrect.height - 50), 60, 22), ComponentInfoWindow[1]))
                {
                    SwitchGameObject();
                }
            }
        }

        private void ObjectInfoWindow_Update()
        {
            if (!showObjectInfoWindow)
            {
                return;
            }

            if (ScrollView_objectInfo_event.ItemID != -1 && ScrollView_objectInfo_event.MouseButton == 0)
            {
                current_objectInfo_index = ScrollView_objectInfo_event.ItemID;
                
                if (objectInfoText[current_objectInfo_index].Contains("UnityEngine.GameObject"))
                {
                    showSetButton = true;
                }
                else
                {
                    showSetButton = false;
                }                
            }
        }

        private void SwitchGameObject()
        {
            string[] splittedItem = objectInfoText[current_objectInfo_index].Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);

            GameObject probeGO = null;

            try
            {
                if (probeGO == null)
                {
                    probeGO = objectProperties.GetProperty(splittedItem[0]) as GameObject;
                }

                if (probeGO == null)
                {
                    probeGO = objectFields.GetField(splittedItem[0]) as GameObject;
                }

                if (probeGO == null)
                {
                    throw new Exception("Can't set this object!");
                }
                else
                {
                    OnBaseObjectChange(probeGO);
                }
            }
            catch
            {
                OutputWindow_Log(ERROR_TEXT[ERRORS.CANNOT_SWITCH_TO_OBJECT], LogType.Error, probeGO.name);
            }
        }

        private void AddToLog()
        {
            BZLogger.Log("\n*** Rhzero: Object Information Log ***");

            BZLogger.Log($"Object: {componentNames[selected_component]}");

            foreach (string item in objectInfoText)
            {
                BZLogger.Log(item);
            }

            BZLogger.Log("\n*** Rhzero: End of Log ***");
        }
    }
}
