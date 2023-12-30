using BZCommon;
using BZCommon.Helpers.RuntimeGUI;
using BZHelper;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZHelper.Components;
using ZHelper.Objects;
using ZHelper.VisualHelpers;
using static ZHelper.Helpers.SceneHelper;

namespace ZHelper
{
    public partial class ZHelper // Handlers
    {
        public void UpdateVisuals()
        {
            //RefreshTransformIndex();

            //RefreshEditModeList();

            SetObjectDrawing(true);

            RefreshComponentsList();
        }

        private void OnBaseObjectChange(GameObject newObject)
        {
            ReleaseObjectDrawing();

            baseObject = newObject;

            selectedObject = newObject;

            BaseText[0].SetText($"Base: {baseObject.name}");                      

            GuiBase.RefreshGroup(1, 0);

            RefreshTransformList(false);

            UpdateVisuals();

            Message(MESSAGE_TEXT[MESSAGES.BASE_OBJECT_CHANGED], baseObject.name);            
        }

        public void OnSelectedObjectChange(GameObject newObject)
        {
            ReleaseObjectDrawing();

            selectedObject = newObject;            

            UpdateVisuals();            
        }

        private void EmergencyRefresh()
        {
            Message(WARNING_TEXT[WARNINGS.EMERGENCY_METHOD_STARTED], MessageType.Warning);

            ReleaseObjectDrawing();

            OnBaseObjectChange(gameObject);
        }

        private void DestroyObject(bool immediate)
        {
            ReleaseObjectDrawing();

            if (selectedObject)
            {
                string parentName = selectedObject.IsRoot() ? "(Root)" : selectedObject.transform.parent.name;

                Message(MESSAGE_TEXT[MESSAGES.OBJECT_DESTROY], selectedObject.name, parentName, selectedObject.transform.root.name);

                if (immediate)
                    DestroyImmediate(selectedObject);
                else
                    Destroy(selectedObject);
            }

            try
            {
                selectedObject = TRANSFORMS[0].gameObject;
            }
            catch
            {
                selectedObject = gameObject;
            }

            if (!baseObject)
            {
                baseObject = selectedObject;
                Message(MESSAGE_TEXT[MESSAGES.BASE_OBJECT_CHANGED], baseObject.name);
            }

            RefreshTransformList(false);

            UpdateVisuals();            
        }

        private void OnRefreshBase()
        {
            ReleaseObjectDrawing();

            if (!baseObject)
            {
                return;
            }

            selectedObject = baseObject;

            RefreshTransformList(false);

            UpdateVisuals();

            SetScroll();

            Message(MESSAGE_TEXT[MESSAGES.CHILD_OBJECTS_LIST_REFRESHED]);            
        }

        private void OnPasteObject()
        {
            GameObject clone = null;

            if (tempObject)
            {
                clone = tempObject.PasteObject(baseObject.transform);
                Message(MESSAGE_TEXT[MESSAGES.OBJECT_PASTED], tempObject.name, baseObject.transform.name);
            }

            ReleaseObjectDrawing();

            RefreshTransformList(false);

            if (clone)
            {
                selectedObject = clone;
            }

            UpdateVisuals();
        }

        private void ReleaseObjectDrawing()
        {
            if (!selectedObject)
            {
                return;
            }

            isColliderSelected = false;
            //showCollider = false;

            SetObjectDrawing(false);
        }        

        /*
        private void RefreshTransformIndex()
        {
            current_transform_index = TRANSFORMS.FindIndex(item => item.Equals(selectedObject.transform));

            if (current_transform_index == -1)
            {
                current_transform_index = 0;
                selectedObject = TRANSFORMS[0].gameObject;
            }

            SetScroll();
        }
        */

        private void RefreshTransformList(bool isRootList)
        {
            TRANSFORMS.Clear();

            if (isRootList)
            {
                baseObject = gameObject;
                selectedObject = gameObject;

                GetRootTransforms(ref TRANSFORMS);

                TRANSFORMS.Sort(TransformsHelper.SortByName);

                GuiBase.SetGroupLabel(1, 3, "Active Scenes Root Game Objects:");

                Message(MESSAGE_TEXT[MESSAGES.GET_ROOTS]);
            }
            else
            {
                baseObject.ScanObjectTransforms(ref TRANSFORMS);

                GuiBase.SetGroupLabel(1, 3, $"{baseObject.transform.name} transforms:");
            }            

            FillGroupContents(ref TRANSFORMS, ref MainScrollContents);
            
            GuiBase.RefreshGroup(1, 3);            
        }

        private int FillGroupContents<T>(ref List<T> items, ref List<GUI_content> group, GUI_textColor guiTextColor = null, GUI_Item_Type guiItemType = GUI_Item_Type.TAB, TextAnchor textAnchor = TextAnchor.MiddleLeft)
        {
            int ID = 0;
            group.Clear();

            foreach (T value in items)
            {
                if (typeof(T) == typeof(string))
                {
                    string stringValue = value as string;

                    if (stringValue[0] == '#')
                    {
                        string[] splittedString = stringValue.Split(new char[] { '#', '#' }, StringSplitOptions.RemoveEmptyEntries);

                        if (Enum.TryParse(splittedString[0], out GUI_Color colorCode))
                        {
                            group.Add(new GUI_content(ID, guiItemType, splittedString[1], null, new GUI_textColor(normal: colorCode), textAlign: textAnchor));
                        }
                        else
                        {
                            Message("*** Unrecognized GUI color code!");
                            group.Add(new GUI_content(ID, guiItemType, splittedString[1], null, new GUI_textColor(), textAlign: textAnchor));
                        }                        
                    }
                    else
                    {
                        group.Add(new GUI_content(ID, guiItemType, value as string, null, guiTextColor ?? new GUI_textColor(), textAlign: textAnchor));
                    }
                }
                else if (typeof(T) == typeof(GameObject))
                    group.Add(new GUI_content(ID, guiItemType, (value as GameObject).name, null, guiTextColor ?? new GUI_textColor(), textAlign: textAnchor));

                else if (typeof(T) == typeof(Transform))
                {
                    Transform tr = value as Transform;

                    string postfix = string.Empty;

                    switch (ID)
                    {
                        case 0 when !baseObject.IsRoot():
                            postfix = " (Parent)";
                            break;

                        default:
                            if (tr.IsRoot())
                            {
                                postfix = $" (Root) ({tr.childCount})";
                            }
                            else
                            {
                                postfix = $" ({tr.childCount})";
                            }
                            break;
                    }

                    group.Add(new GUI_content(ID, guiItemType, tr.name + postfix, null, guiTextColor ?? new GUI_textColor(), textAlign: textAnchor));
                }

                else if (typeof(T) == typeof(MSG))
                {
                    group.Add(new GUI_content(ID, guiItemType, "> " + (value as MSG).message, null, new GUI_textColor(normal: msgTypeColors[(value as MSG).type]), textAlign: textAnchor));
                }

                ID++;
            }            

            return ID;
        }


        private void SetScroll()
        {
            //SetScrollPos(ref scrollpos_transforms, current_transform_index);
        }

        private void SetObjectDrawing(bool value)
        {
            if (RHZ_VISUAL_BASE == null)
            {
                RHZ_VISUAL_BASE = new GameObject("RHZ_VISUAL_BASE");
                RHZ_VISUAL_BASE.GetOrAddVisualBase(BaseType.Object).EnsureComponent<DrawObjectBounds>();
                RHZ_VISUAL_BASE.GetOrAddVisualBase(BaseType.Collider).EnsureComponent<DrawColliderBounds>();
            }

            RHZ_VISUAL_BASE.transform.SetParent(selectedObject.transform);
            Utils.ZeroTransform(RHZ_VISUAL_BASE.transform);
            RHZ_VISUAL_BASE.GetComponentInChildren<DrawObjectBounds>().IsDraw(value);
        }

        private void ShowAllCollider(bool value)
        {
            //foreach (DrawColliderControl dcc in selectedObject.GetComponentsInChildren<DrawColliderControl>(true))
            //{
            //dcc.DrawAllCollider(value);
            //}
        }

    }
}
