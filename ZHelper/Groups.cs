using BZCommon.Helpers.RuntimeGUI;
using System.Collections.Generic;
using UnityEngine;

namespace ZHelper
{
    public enum BaseButtonIDs
    {
        TEXT = 1,
        BUTTON_ON_OFF,
        BUTTON_MARK,
        BUTTON_COPY,
        BUTTON_PASTE,
        BUTTON_DESTROY
    }

    public partial class IGUI_Test
    {
        private List<GUI_content> BaseText = new List<GUI_content>()
        {
            new GUI_content(1, GUI_Item_Type.NORMALBUTTON, "Base :", null, new GUI_textColor(normal: GUI_Color.Green), textAlign: TextAnchor.MiddleLeft)
        };
        
        private List<GUI_content> BaseButtons = new List<GUI_content>()
        {
            new GUI_content((int)BaseButtonIDs.TEXT, GUI_Item_Type.LABEL, "Base :", null, new GUI_textColor(normal: GUI_Color.Green), textAlign: TextAnchor.MiddleLeft, fixedWidth: 50f),
            new GUI_content((int)BaseButtonIDs.BUTTON_ON_OFF, GUI_Item_Type.NORMALBUTTON, "Off", "ez mi ez?", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 30f),
            new GUI_content((int)BaseButtonIDs.BUTTON_MARK, GUI_Item_Type.NORMALBUTTON, "Mark", "Marks the gameobject and adds it to the list.", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 40f),
            new GUI_content((int)BaseButtonIDs.BUTTON_COPY, GUI_Item_Type.NORMALBUTTON, "Copy", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 40f),
            new GUI_content((int)BaseButtonIDs.BUTTON_PASTE, GUI_Item_Type.NORMALBUTTON, "Paste", "Paste a previously copied gameobject to a child of current Base gameobject.\nName set to 'newPastedObject' and state set to active.", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 40f),
            new GUI_content((int)BaseButtonIDs.BUTTON_DESTROY, GUI_Item_Type.NORMALBUTTON, "Destroy", "Destroy the selected gameobject and all childs.", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter)
        };

        private List<GUI_content> ObjectButtons = new List<GUI_content>()
        {
            new GUI_content(1, GUI_Item_Type.LABEL, "Object :", null, new GUI_textColor(normal: GUI_Color.Green), textAlign: TextAnchor.MiddleLeft, fixedWidth: 50f),
            new GUI_content(2, GUI_Item_Type.NORMALBUTTON, "Set", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter),
            new GUI_content(3, GUI_Item_Type.NORMALBUTTON, "Refresh", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter),
            new GUI_content(4, GUI_Item_Type.NORMALBUTTON, "Get Roots", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter),

        };

        private List<GUI_content> ScrollGroup = new List<GUI_content>()
        {
            new GUI_content(1, GUI_Item_Type.NORMALBUTTON, "Base", null, new GUI_textColor(normal: GUI_Color.Green), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(2, GUI_Item_Type.NORMALBUTTON, "Off", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(3, GUI_Item_Type.NORMALBUTTON, "Mark", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content_hSlider(4, "Rot. X:", null, new GUI_textColor(), 0, 0, 360, textAlign: TextAnchor.MiddleLeft),
            new GUI_content_hSlider(5, "Rot. Y:", null, new GUI_textColor(), 0, 0, 360, textAlign: TextAnchor.MiddleLeft),
            new GUI_content_hSlider(6, "Rot. Z:", null, new GUI_textColor(), 0, 0, 360, textAlign: TextAnchor.MiddleLeft),            
            new GUI_content(7, GUI_Item_Type.NORMALBUTTON, "Copy", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(8, GUI_Item_Type.NORMALBUTTON, "Paste", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(9, GUI_Item_Type.NORMALBUTTON, "Destroy", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            
            
        };
    }
}
