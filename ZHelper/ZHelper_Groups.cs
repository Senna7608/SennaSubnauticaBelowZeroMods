using BZCommon.Helpers.RuntimeGUI;
using System.Collections.Generic;
using UnityEngine;

namespace ZHelper
{
    public enum GroupID
    {
        BASETEXT,
        BASEGROUP,
        OBJECTGROUP,
        MAINSCROLLGROUP,
        COMPONENTSCROLLGROUP,
        OUTPUTGROUP,
        COMPONENTINFOSCROLLGROUP,
        RENDERERSCROLLGROUP,
        RENDERERBUTTONS
    }

    public enum ButtonID
    {
        TEXT = 1,
        BTN_ONOFF,
        BTN_MARK,
        BTN_COPY,
        BTN_PASTE,
        BTN_DESTROY,
        BTN_SET,
        BTN_REFRESH,
        BTN_GETROOTS,
        BTN_UNDO,
        BTN_RONOFF
    }

    public partial class ZHelper // Groups
    {
        private List<GUI_content> BaseText = new List<GUI_content>()
        {
            new GUI_content(1, GUI_Item_Type.LABEL, "Base :", null, new GUI_textColor(normal: GUI_Color.Green), textAlign: TextAnchor.MiddleLeft)
        };
        
        private List<GUI_content> ObjectButtons = new List<GUI_content>()
        {
            new GUI_content((int)ButtonID.TEXT, GUI_Item_Type.LABEL, "Object :", null, new GUI_textColor(normal: GUI_Color.Green), textAlign: TextAnchor.MiddleLeft, fixedWidth: 45f),
            new GUI_content((int)ButtonID.BTN_ONOFF, GUI_Item_Type.NORMALBUTTON, "Off", "ez mi ez?", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 34f),
            new GUI_content((int)ButtonID.BTN_MARK, GUI_Item_Type.NORMALBUTTON, "Mark", "Marks the gameobject and adds it to the list.", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 40f),
            new GUI_content((int)ButtonID.BTN_COPY, GUI_Item_Type.NORMALBUTTON, "Copy", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 40f),
            new GUI_content((int)ButtonID.BTN_PASTE, GUI_Item_Type.NORMALBUTTON, "Paste", "Paste a previously copied gameobject to a child of current Base gameobject.\nName set to 'newPastedObject' and state set to active.", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 42f),
            new GUI_content((int)ButtonID.BTN_DESTROY, GUI_Item_Type.NORMALBUTTON, "Destroy", "Destroy the selected gameobject and all childs.", new GUI_textColor(), textAlign: TextAnchor.MiddleCenter)
        };

        private List<GUI_content> BaseButtons = new List<GUI_content>()
        {
            new GUI_content((int)ButtonID.TEXT, GUI_Item_Type.LABEL, "Base :", null, new GUI_textColor(normal: GUI_Color.Green), textAlign: TextAnchor.MiddleLeft, fixedWidth: 45f),
            new GUI_content((int)ButtonID.BTN_SET, GUI_Item_Type.NORMALBUTTON, "Set", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter),
            new GUI_content((int)ButtonID.BTN_REFRESH, GUI_Item_Type.NORMALBUTTON, "Refresh", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter),
            new GUI_content((int)ButtonID.BTN_GETROOTS, GUI_Item_Type.NORMALBUTTON, "Get Roots", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter),
        };

        private List<GUI_content> MainScrollContents = new List<GUI_content>();

        private List<GUI_content> TestContents = new List<GUI_content>()
        {            
            new GUI_content_hSlider(4, "Rot. X:", null, new GUI_textColor(), 0, 0, 360, textAlign: TextAnchor.MiddleLeft),
            new GUI_content_hSlider(5, "Rot. Y:", null, new GUI_textColor(), 0, 0, 360, textAlign: TextAnchor.MiddleLeft),
            new GUI_content_hSlider(6, "Rot. Z:", null, new GUI_textColor(), 0, 0, 360, textAlign: TextAnchor.MiddleLeft)
        };
    }
}
