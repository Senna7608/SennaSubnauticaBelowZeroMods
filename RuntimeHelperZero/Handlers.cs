using UnityEngine;
using RuntimeHelperZero.VisualHelpers;
using RuntimeHelperZero.Components;
using static RuntimeHelperZero.SceneHelper.SceneHelper;
using RuntimeHelperZero.Objects;
using BZCommon.Helpers.GUIHelper;
using BZHelper;

namespace RuntimeHelperZero
{
    public partial class RuntimeHelperZero
    {
        public void UpdateVisuals()
        {
            RefreshTransformIndex();                        

            RefreshEditModeList();

            SetObjectDrawing(true);

            RefreshComponentsList();            
        }

        private void OnBaseObjectChange(GameObject newObject)
        {            
            SetDirty(true);

            ReleaseObjectDrawing();
           
            baseObject = newObject;            

            selectedObject = newObject;

            RefreshTransformsList();

            UpdateVisuals();

            OutputWindow_Log(MESSAGE_TEXT[MESSAGES.BASE_OBJECT_CHANGED], baseObject.name);

            SetDirty(false);            
        }

        public void OnObjectChange(GameObject newObject)
        {            
            SetDirty(true);

            ReleaseObjectDrawing();

            selectedObject = newObject;

            if (!isRootList)
                RefreshTransformsList();

            UpdateVisuals();

            SetDirty(false);
        }

        private void EmergencyRefresh()
        {
            SetDirty(true);

            OutputWindow_Log(WARNING_TEXT[WARNINGS.EMERGENCY_METHOD_STARTED], LogType.Warning);

            ReleaseObjectDrawing();

            baseObject = gameObject;
            selectedObject = baseObject;

            RefreshTransformsList();

            UpdateVisuals();

            SetDirty(false);

            OutputWindow_Log(MESSAGE_TEXT[MESSAGES.BASE_OBJECT_CHANGED], baseObject.name);
        }

        private void DestroyObject(bool immediate)
        {
            SetDirty(true);

            ReleaseObjectDrawing();

            if (selectedObject)
            {
                string parentName = selectedObject.IsRoot() ? "(Root)" : selectedObject.transform.parent.name;

                OutputWindow_Log(MESSAGE_TEXT[MESSAGES.OBJECT_DESTROY],  selectedObject.name, parentName, selectedObject.transform.root.name);

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
                OutputWindow_Log(MESSAGE_TEXT[MESSAGES.BASE_OBJECT_CHANGED], baseObject.name);
            }

            RefreshTransformsList();

            UpdateVisuals();

            SetDirty(false);
        }
                
        private void OnRefreshBase()
        {            
            SetDirty(true);

            ReleaseObjectDrawing();

            if (!baseObject)
            {
                return;
            }

            selectedObject = baseObject;

            RefreshTransformsList();
            
            UpdateVisuals();

            SetScroll();

            OutputWindow_Log(MESSAGE_TEXT[MESSAGES.CHILD_OBJECTS_LIST_REFRESHED]);

            SetDirty(false);           
        }

        private void OnPasteObject()
        {
            SetDirty(true);

            GameObject clone = null;

            if (tempObject)
            {
                clone = tempObject.PasteObject(baseObject.transform);
                OutputWindow_Log(MESSAGE_TEXT[MESSAGES.OBJECT_PASTED], tempObject.name, baseObject.transform.name);                               
            }

            ReleaseObjectDrawing();

            RefreshTransformsList();

            if (clone)
            {
                selectedObject = clone;
            }

            UpdateVisuals();

            SetDirty(false);
        }

        private void ReleaseObjectDrawing()
        {
            if (!selectedObject)
            {
                return;
            }

            isColliderSelected = false;
            
            SetObjectDrawing(false);            
        }


        private void GetRoots()
        {
            SetDirty(true);

            ReleaseObjectDrawing();

            TRANSFORMS.Clear();            

            GetRootTransforms(ref TRANSFORMS);

            TRANSFORMS.Sort(TransformsHelper.SortByName);

            guiItems_transforms.SetScrollViewItems(TRANSFORMS.InitTransformNamesList(), 278f);           

            RefreshTransformIndex();

            UpdateVisuals();

            OutputWindow_Log(MESSAGE_TEXT[MESSAGES.GET_ROOTS]);

            isRootList = true;

            SetDirty(false);
        }        

        private void RefreshTransformIndex()
        {
            current_transform_index = TRANSFORMS.FindIndex(item => item.Equals(selectedObject.transform));

            if (current_transform_index == -1)
            {
                current_transform_index = 0;
                selectedObject = TRANSFORMS[0].gameObject;                               
            }

            guiItems_transforms.SetStateInverseTAB(current_transform_index);

            SetScroll();
        }

        private void SetScroll()
        {
            SetScrollPos(ref scrollpos_transforms, current_transform_index);
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
    }
}
