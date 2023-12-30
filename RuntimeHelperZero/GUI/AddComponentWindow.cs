using System.Collections.Generic;
using UnityEngine;
using System;
using RuntimeHelperZero.VisualHelpers;
using BZCommon.Helpers.GUIHelper;

namespace RuntimeHelperZero
{
    public partial class RuntimeHelperZero
    {
        private List<string> componentsToAdd = new List<string>();
        private List<GuiItem> guiItems_componentsToAdd = new List<GuiItem>();
        private bool showAddComponentWindow = false;
        private Vector2 scrollPos_componentsToAdd;
        private GuiItemEvent ScrollView_addComponents_event;

        public void AddComponentWindow_Awake()
        {
            componentsToAdd.Clear();

            foreach (Type type in AvailableComponents)
            {
                componentsToAdd.Add(type.ToString().Split('.').GetLast());
            }

            guiItems_componentsToAdd.SetScrollViewItems(componentsToAdd, 530f);
        }

        private void AddComponentWindow_OnGUI()
        {
            if (!showAddComponentWindow)
                return;

            Rect windowrect = SNWindow.CreateWindow(new Rect(700, 732, 550, 348), "Add Component Window");

            ScrollView_addComponents_event = SNScrollView.CreateScrollView(new Rect(windowrect.x + 5, windowrect.y, windowrect.width - 10, windowrect.height - 60), ref scrollPos_componentsToAdd, ref guiItems_componentsToAdd, "Available Components", "" , 10);
                      
        }

        private void AddComponentWindow_Update()
        {
            if (!showAddComponentWindow)
                return;

            if (ScrollView_addComponents_event.ItemID != -1 && ScrollView_addComponents_event.MouseButton == 0)
            {
                int x = ScrollView_addComponents_event.ItemID;

                Component addedComponent = selectedObject.AddComponent(AvailableComponents[x]);

                OutputWindow_Log(MESSAGE_TEXT[MESSAGES.COMPONENT_ADDED], GetComponentShortType(addedComponent), selectedObject.name);

                showAddComponentWindow = false;

                UpdateVisuals();

                if (addedComponent.GetType() == typeof(Collider))
                {
                    selectedObject.GetComponentInChildren<DrawColliderBounds>().DrawCollider(addedComponent.GetInstanceID());
                }               
            }
        }

        public static readonly List<Type> AvailableComponents = new List<Type>()
        {
            typeof(BoxCollider),
            typeof(SphereCollider),
            typeof(CapsuleCollider)
        };

    }
}
