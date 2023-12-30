using System.Collections.Generic;
using BZCommon.Helpers.RuntimeGUI;
using UnityEngine;
using ZHelper.Objects;

namespace ZHelper
{
    public partial class ZHelper : GUI_ROOT, IGUI
    {
        void IGUI.WakeUp()
        {
            Main.Instance = this;

            GameObject_Blacklist.Add(Main.commandRoot.gameObject);
            GameObject_Blacklist.Add(gameObject);            

            MessageWindow_Start();

            Message(MESSAGE_TEXT[MESSAGES.PROGRAM_STARTED]);

            RefreshTransformList(true);

            ComponentWindow_Start();

            CompInfoWindow_Awake();

            RendererWindow_Awake();

            GuiBase.DisableWindow(5);
        }


        void IGUI.Update()
        {
            SafetyCheck();

            if (isRayEnabled)
            {
                RaycastMode_Update();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                UWE.Utils.lockCursor = !UWE.Utils.lockCursor;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!Player.main || uGUI.main.loading.IsLoading || IsPlayerInVehicle())
                {
                    isRayEnabled = false;
                    Message(WARNING_TEXT[WARNINGS.UNABLE_TO_RAYCAST], MessageType.Warning);
                    return;
                }
                else
                {
                    ReleaseObjectDrawing();

                    isRayEnabled = !isRayEnabled;

                    Message(MESSAGE_TEXT[MESSAGES.RAYCAST_STATE], isRayEnabled);
                }
            }
        }

        void IGUI.OnWindowClose(int windowID)
        {
            ReleaseObjectDrawing();
        }

        void IGUI.GetWindows(ref List<Window> windows)
        {
            windows.Add(new Window(1, mainWindowRect, "Runtime Helper Zero v.2.0", hasToolTipButton: true));
            windows.Add(new Window(2, componentWindowRect, "Component Window", hasCloseButton: false));
            windows.Add(new Window(3, messageWindowRect, "Message Window", hasCloseButton: false));
            windows.Add(new Window(4, compInfoWindowRect, "Component Info Window", hasCloseButton: false));
            windows.Add(new Window(5, rendererWindowRect, "Renderer Info Window", hasCloseButton: false));
        }

        void IGUI.GetGroups(ref List<Group> groups)
        {
            groups.Add(new Group(1, (int)GroupID.BASETEXT, GUI_Group_type.Normal, BaseText));            
            groups.Add(new Group(1, (int)GroupID.MAINSCROLLGROUP, GUI_Group_type.Scroll, MainScrollContents, " ", maxShowItems: 8));
            groups.Add(new Group(1, (int)GroupID.BASEGROUP, GUI_Group_type.Normal, BaseButtons, columns: 4));
            groups.Add(new Group(1, (int)GroupID.OBJECTGROUP, GUI_Group_type.Normal, ObjectButtons, columns: 6));
            groups.Add(new Group(2, (int)GroupID.COMPONENTSCROLLGROUP, GUI_Group_type.Scroll, ComponentScrollContents, " ", maxShowItems: 12));
            groups.Add(new Group(3, (int)GroupID.OUTPUTGROUP, GUI_Group_type.Scroll, MessageScrollContents, " ", maxShowItems: 8, itemHeight: 18));
            groups.Add(new Group(4, (int)GroupID.COMPONENTINFOSCROLLGROUP, GUI_Group_type.Scroll, compInfoScrollContents, " ", maxShowItems: 12));
            groups.Add(new Group(5, (int)GroupID.RENDERERSCROLLGROUP, GUI_Group_type.Scroll, RendererScrollContents, " ", maxShowItems: 12));
            groups.Add(new Group(5, (int)GroupID.RENDERERBUTTONS, GUI_Group_type.Normal, RendererWindowButtons, columns: 3));
        }

        void IGUI.GUIEvent(GUI_event guiEvent)
        {           
            DebugMessage($"WindowID: {guiEvent.WindowID} GroupID: {guiEvent.GroupID} ItemID: {guiEvent.ItemID} MouseButton: {guiEvent.MouseButton}");           

            if (guiEvent.WindowID == 1)
            {
                switch (guiEvent.GroupID)
                {
                    /*
                    case 1:
                        switch (guiEvent.ItemID)
                        {
                            case 1:
                                ScrollContents.Add(new GUI_content(10, GUI_Item_Type.NORMALBUTTON, "new", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft));
                                GuiBase.SetGroupLabel(1, 3, "próba");
                                GuiBase.RefreshGroup(1, 3);                                    
                                break;
                        }
                        break;
                    */

                    case 3:
                        switch (guiEvent.ItemID)
                        {
                            default:
                                selectedObject = TRANSFORMS[guiEvent.ItemID].gameObject;
                                //ScrollContents[guiEvent.ItemID].
                                OnSelectedObjectChange(TRANSFORMS[guiEvent.ItemID].gameObject);
                                
                                break;
                        }
                        break;

                    case (int)GroupID.OBJECTGROUP:
                        switch (guiEvent.ItemID)
                        {
                            case (int)ButtonID.BTN_ONOFF:

                                if (GameObject_Blacklist.Contains(selectedObject))
                                {
                                    Message(WARNING_TEXT[WARNINGS.OBJECT_STATE_CANNOT_MODIFIED], MessageType.Warning, selectedObject.name);
                                    break;
                                }

                                selectedObject.SetActive(!selectedObject.activeSelf);
                                Message(MESSAGE_TEXT[MESSAGES.ACTIVE_STATE_CHANGE], selectedObject.name, selectedObject.activeSelf.ToString());
                                break;

                            case (int)ButtonID.BTN_MARK:
                                //AddToMarkList(selectedObject);
                                break;

                            case (int)ButtonID.BTN_COPY:

                                if (GameObject_Blacklist.Contains(selectedObject))
                                {
                                    Message(WARNING_TEXT[WARNINGS.OBJECT_CANNOT_COPIED], MessageType.Warning, selectedObject.name);
                                }
                                else
                                {
                                    if (tempObject)
                                    {
                                        DestroyImmediate(tempObject);
                                    }

                                    selectedObject.CopyObject(out tempObject);
                                    Message(MESSAGE_TEXT[MESSAGES.OBJECT_COPIED], tempObject.name);
                                }
                                break;

                            case (int)ButtonID.BTN_PASTE:

                                if (tempObject)
                                {
                                    OnPasteObject();
                                }
                                else
                                {
                                    Message(WARNING_TEXT[WARNINGS.TEMP_OBJECT_EMPTY], MessageType.Warning);
                                }
                                break;

                            case (int)ButtonID.BTN_DESTROY:

                                if (GameObject_Blacklist.Contains(selectedObject))
                                {
                                    Message(WARNING_TEXT[WARNINGS.PROGRAM_OBJECT_CANNOT_DESTROY], MessageType.Warning, selectedObject.name);
                                }
                                else
                                {
                                    DestroyObject(true);
                                }
                                break;
                        }
                        break;

                    case (int)GroupID.BASEGROUP:

                        switch (guiEvent.ItemID)
                        {
                            case (int)ButtonID.BTN_SET:

                                if (selectedObject != baseObject)
                                {
                                    baseObject = selectedObject;
                                    OnBaseObjectChange(baseObject);
                                }

                                break;

                            case (int)ButtonID.BTN_REFRESH:
                                OnRefreshBase();
                                break;

                            case (int)ButtonID.BTN_GETROOTS:
                                RefreshTransformList(true);
                                break;                                
                        }
                        break;
                }                
                    
            }
        }

        void IGUI.ShowMainWindow()
        {
            GuiBase?.EnableWindow(1);
        }

        private void SafetyCheck()
        {
            if (!selectedObject)
            {
                Message(ERROR_TEXT[ERRORS.SELECTED_OBJECT_DESTROYED], MessageType.Error);
                EmergencyRefresh();
            }

            if (!baseObject)
            {
                Message(ERROR_TEXT[ERRORS.BASE_OBJECT_DESTROYED], MessageType.Error);
                EmergencyRefresh();
            }
        }

        public bool IsPlayerInVehicle()
        {
            return Player.main.inExosuit || Player.main.IsPilotingSeatruck() ? true : false;
        }

    }
}
