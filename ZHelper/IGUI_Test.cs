using System.Collections.Generic;
using BZCommon.Helpers.RuntimeGUI;
using UnityEngine;

namespace ZHelper
{
    public partial class Zhelper : GUI_ROOT, IGUI
    {
        private Rect mainWindowRect = new Rect(0, 30, 298, 700);
        private Rect secondWindowRect = new Rect(300, 30, 298, 400);
        private Rect outputWindowRect = new Rect(600, 30, 298, 400);

        void IGUI.WakeUp()
        {
            print("WakeUp called!");
        }

        void IGUI.OnWindowClose(int windowID)
        {
            print($"OnWindowClose({windowID}) called!");
        }

        void IGUI.GetWindows(ref List<Window> windows)
        {
            windows.Add(new Window(1, mainWindowRect, "Runtime Helper Zero v.1.0", hasToolTipButton: true));
            windows.Add(new Window(2, secondWindowRect, "Second Window", hasCloseButton: false));
            windows.Add(new Window(2, outputWindowRect, "Output Window", hasCloseButton: false));
        }

        void IGUI.GetGroups(ref List<Group> groups)
        {
            groups.Add(new Group(1, 1, GUI_Group_type.Normal, BaseText));            
            groups.Add(new Group(1, 3, GUI_Group_type.Scroll, ScrollGroup, "ScrollView 1", maxShowItems: 8));
            groups.Add(new Group(1, 4, GUI_Group_type.Normal, BaseButtons, columns: 6));
            groups.Add(new Group(1, 5, GUI_Group_type.Normal, ObjectButtons, columns: 4));
        }

        void IGUI.GUIEvent(GUI_event guiEvent)
        {
            print($"WindowID: {guiEvent.WindowID}\n" +
                $"GroupID: {guiEvent.GroupID}\n" +
                $"ItemID: {guiEvent.ItemID}\n" +
                $"MouseButton: {guiEvent.MouseButton}");
            
            switch (guiEvent.WindowID)
            {
                case 1:
                    switch (guiEvent.GroupID)
                    {
                        case 1:
                            switch (guiEvent.ItemID)
                            {
                                case 1:
                                    ScrollGroup.Add(new GUI_content(10, GUI_Item_Type.NORMALBUTTON, "new", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft));
                                    GuiBase.SetGroupLabel(1, 3, "próba");
                                    GuiBase.RefreshGroup(1, 3);                                    
                                    break;
                            }
                            break;

                        case 3:
                            switch (guiEvent.ItemID)
                            {
                                case 7:
                                    print($"Slider: {guiEvent.Value}");
                                    break;
                            }
                            break;

                        case 4:
                            switch (guiEvent.ItemID)
                            {
                                case (int)BaseButtonIDs.BUTTON_ON_OFF:
                                    print("On/Off pressed!");
                                    break;
                                case (int)BaseButtonIDs.BUTTON_MARK:
                                    print("Mark pressed!");
                                    break;
                                case (int)BaseButtonIDs.BUTTON_COPY:
                                    print("Copy pressed!");
                                    break;
                                case (int)BaseButtonIDs.BUTTON_PASTE:
                                    print("Paste pressed!");
                                    break;
                                case (int)BaseButtonIDs.BUTTON_DESTROY:
                                    print("Destroy pressed!");
                                    break;
                            }
                            break;
                    }                
                    break;                
            }
        }

        void IGUI.ShowMainWindow()
        {
            GuiBase.EnableWindow(1);
        }
    }
}
