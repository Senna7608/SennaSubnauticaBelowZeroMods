using UnityEngine;
using System.Collections.Generic;
using RuntimeHelperZero.VisualHelpers;
using RuntimeHelperZero.Components;
using RuntimeHelperZero.Configuration;
using RuntimeHelperZero.Objects;
using BZCommon.Helpers.GUIHelper;

namespace RuntimeHelperZero
{
    public partial class RuntimeHelperZero : MonoBehaviour
    {        
        private List<Transform> TRANSFORMS = new List<Transform>();
        private List<GuiItem> guiItems_transforms = new List<GuiItem>();
        private Vector2 scrollpos_transforms = Vector2.zero;
        private GuiItemEvent ScrollView_transforms_event;
        private int current_transform_index = 0;

        private int addedChildObjectCount = 0;

        private GameObject baseObject, selectedObject, tempObject;

        private GameObject RHZ_VISUAL_BASE;

        private string OBJECTINFO = string.Empty;
        private string COLLIDERINFO = string.Empty;                       
        
        private Vector3 lPos, lScale, lRot;
        private Vector2 rtPos, rtSize, rtPivot;
        public bool isDirty = false;       

        private string sizeText = string.Empty;             
        
        private bool isRootList = false;

        private bool showLocal = true;

        public RuntimeHelperZero()
        {
            if (Main.Instance == null)
            {
                Main.Instance = FindObjectOfType(typeof(RuntimeHelperZero)) as RuntimeHelperZero;

                if (Main.Instance == null)
                {
                    GameObject runtimeHelperZero = new GameObject("RuntimeHelperZero").gameObject;
                    Main.Instance = runtimeHelperZero.AddComponent<RuntimeHelperZero>();
                }
            }
            else
            {
                Main.Instance.Awake();
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            OutputWindow_Awake();
            OutputWindow_Log(MESSAGE_TEXT[MESSAGES.PROGRAM_STARTED]);

            baseObject = gameObject;
            selectedObject = gameObject;           

            InitFullTechTypeList(ref techTypeDatas);            

            EditWindow_Awake();

            ObjectWindow_Awake();

            AddComponentWindow_Awake();

            MarkWindow_Awake();            

            FMODWindow_Awake();
        }

        private void Start()
        {
            OnBaseObjectChange(gameObject);
        }

        private void SetDirty(bool value)
        {
            isDirty = value;

            if (isDirty)
            {
                showRendererWindow = false;
                showObjectInfoWindow = false;
            }
        }

        private void SetScrollPos(ref Vector2 scrollpos, int index)
        {
            scrollpos.y = index * 24;
        }

        private void OnDestroy()
        {
            DestroyImmediate(RHZ_VISUAL_BASE);

            Destroy(this);
        }               
        
        private void RefreshTransformsList()
        {            
            baseObject.ScanObjectTransforms(ref TRANSFORMS);           
            guiItems_transforms.SetScrollViewItems(TRANSFORMS.InitTransformNamesList(), 278f);

            isRootList = false;
        }

        private void SafetyCheck()
        {
            if (!selectedObject)
            {
                OutputWindow_Log(ERROR_TEXT[ERRORS.SELECTED_OBJECT_DESTROYED], LogType.Exception);
                EmergencyRefresh();
            }

            if (!baseObject)
            {
                OutputWindow_Log(ERROR_TEXT[ERRORS.BASE_OBJECT_DESTROYED], LogType.Exception);
                EmergencyRefresh();
            }
        }

        private void PrintObjectInfo()
        {
            if (isRectTransform)
            {
                PrintRectTransformInfo();
                return;
            }

            Vector3 tr, qt, sc;

            if (showLocal)
            {
                tr = selectedObject.transform.localPosition;                
                qt = selectedObject.transform.localEulerAngles;
            }
            else
            {
                tr = selectedObject.transform.position;                
                qt = selectedObject.transform.eulerAngles;
            }

            sc = selectedObject.transform.localScale;

            OBJECTINFO =
                $"{selectedObject.transform.name} :\n\n" +
                $"Position{" ",4}x:{tr.x,8:F2},  y:{tr.y,8:F2},  z:{tr.z,8:F2}\n" +
                $"Scale{" ",7}x:{sc.x,8:F2},  y:{sc.y,8:F2},  z:{sc.z,8:F2}\n" +
                $"Rotation{" ",3}x:{qt.x,8:F0},  y:{qt.y,8:F0},  z:{qt.z,8:F0}";
        }

        private void PrintRectTransformInfo()
        {
            RectTransform rt = objects[selected_component] as RectTransform;

            OBJECTINFO =
                "RectTransform :\n\n" +
                $"anchoredPosition:{" ",3}x:{rt.anchoredPosition.x,8:F2},  y:{rt.anchoredPosition.y,8:F2}\n" +
                $"sizeDelta:{" ",14}x:{rt.sizeDelta.x,8:F2},  y:{rt.sizeDelta.y,8:F2}\n" +
                $"pivot:{" ",21}x:{rt.pivot.x,8:F2},  y:{rt.pivot.y,8:F2}";
        }


        private void PrintColliderInfo()
        {
            if (colliderModify.ColliderType == ColliderType.MeshCollider)
            {
                COLLIDERINFO = 
                $"Collider type: {colliderModify.ColliderType}\n" +
                $"Mesh isReadable: {colliderModify.MeshIsReadable}\n" +
                $"Vertices: {colliderModify.Vertices}\n" +
                $"Triangles: {colliderModify.Triangles}";
            }
            else
            {
                COLLIDERINFO =
                $"Collider type: {colliderModify.ColliderType}\n\n" +
                $"Center x:{colliderModify.Center.x,6:F2}  y:{colliderModify.Center.y,6:F2}  z:{colliderModify.Center.z,6:F2}\n" + sizeText;
            }
        }

        private void OnGUI()
        {
            if (isDirty)
                return;

            SafetyCheck();

            Rect windowRect = SNWindow.CreateWindow(new Rect(0, 30, 298, 700), "Runtime Helper Zero v.1.0");                       

            GUI.Label(new Rect(windowRect.x + 5, windowRect.y, 290, 25), $"Base : {baseObject.name}", SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, TextAnchor.MiddleLeft));

            ScrollView_transforms_event = SNScrollView.CreateScrollView(new Rect(windowRect.x + 5, windowRect.y + 22, windowRect.width - 10, 212), ref scrollpos_transforms, ref guiItems_transforms, isRootList ? "Active Scenes Root Game Objects" : "Childs of", isRootList ? string.Empty : baseObject.name, 10);

            GUI.Label(new Rect(windowRect.x + 5, windowRect.y + 300, 40, 22), "Base :", SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, TextAnchor.MiddleLeft));


            if (GUI.Button(new Rect(windowRect.x + 60, windowRect.y + 300, 60, 22), MainWindow[0], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                OnBaseObjectChange(selectedObject);
            }

            if (GUI.Button(new Rect(windowRect.x + 125, windowRect.y + 300, 60, 22), MainWindow[1], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                OnRefreshBase();
            }

            if (GUI.Button(new Rect(windowRect.x + 190, windowRect.y + 300, 103, 22), MainWindow[2], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                GetRoots();
            }

            GUI.Label(new Rect(windowRect.x + 5, windowRect.y + 325, 40, 22), "Object :", SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, TextAnchor.MiddleLeft));

            if (GUI.Button(new Rect(windowRect.x + 60, windowRect.y + 325, 30, 22), selectedObject.activeSelf ? MainWindow[4] : MainWindow[3], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                if (selectedObject == gameObject)
                {
                    OutputWindow_Log(WARNING_TEXT[WARNINGS.OBJECT_STATE_CANNOT_MODIFIED], LogType.Warning, selectedObject.name);
                    return;
                }

                selectedObject.SetActive(!selectedObject.activeSelf);
                OutputWindow_Log(MESSAGE_TEXT[MESSAGES.ACTIVE_STATE_CHANGE] ,selectedObject.name, selectedObject.activeSelf.ToString());
            }

            if (GUI.Button(new Rect(windowRect.x + 95, windowRect.y + 325, 40, 22), MainWindow[17], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                AddToMarkList(selectedObject);
            }
        
            if (GUI.Button(new Rect(windowRect.x + 140, windowRect.y + 325, 40, 22), MainWindow[5], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                if (selectedObject == gameObject)
                {
                    OutputWindow_Log(WARNING_TEXT[WARNINGS.OBJECT_CANNOT_COPIED], LogType.Warning, selectedObject.name);
                }
                else
                {
                    if (tempObject)
                    {
                        DestroyImmediate(tempObject);
                    }

                    selectedObject.CopyObject(out tempObject);
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.OBJECT_COPIED], tempObject.name);
                }
            }

            
            if (GUI.Button(new Rect(windowRect.x + 185, windowRect.y + 325, 45, 22), MainWindow[6], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                if (tempObject)
                {
                    OnPasteObject();
                }
                else
                {
                    OutputWindow_Log(WARNING_TEXT[WARNINGS.TEMP_OBJECT_EMPTY], LogType.Warning);
                }
            }

            if (GUI.Button(new Rect(windowRect.x + 235, windowRect.y + 325, 58, 22), MainWindow[7], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                if (selectedObject == gameObject)
                {
                    OutputWindow_Log(WARNING_TEXT[WARNINGS.PROGRAM_OBJECT_CANNOT_DESTROY], LogType.Warning, selectedObject.name);
                }
                /*
                else if (selectedObject.IsRoot())
                {
                    OutputWindow_Log(WARNING_TEXT[WARNINGS.ROOT_OBJECT_CANNOT_DESTROY], LogType.Warning);
                }*/
                else
                {
                    DestroyObject(true);
                }
            }

            GUI.TextArea(new Rect(windowRect.x + 5, windowRect.y + 355, windowRect.width - 10, 100), OBJECTINFO);

            if (isColliderSelected)
            {
                GUI.TextArea(new Rect(windowRect.x + 5, windowRect.y + 465, windowRect.width - 10, 72), COLLIDERINFO);
            }

            GUI.Label(new Rect(windowRect.x + 5, windowRect.y + 542, 145, 22), "Transform shorthands:", SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, TextAnchor.MiddleLeft));

            if (GUI.Button(new Rect(windowRect.x + 150, windowRect.y + 542, 140, 22), showLocal ? MainWindow[8] : MainWindow[9], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                showLocal = !showLocal;

                if (showLocal)
                {
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.TRANSFORM_TO_LOCAL]);
                }
                else
                {
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.TRANSFORM_TO_WORLD]);
                }
                
            }

            if (GUI.Button(new Rect(windowRect.x + 5, windowRect.y + 567, 140, 22), MainWindow[10], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                if (showLocal)
                {
                    selectedObject.transform.SetLocalsToZero();
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.LOCALS_TO_ZERO], selectedObject.name);
                }
                else
                {
                    selectedObject.transform.SetWorldToZero();
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.WORLD_TO_ZERO], selectedObject.name);
                }                
            }

            if (GUI.Button(new Rect(windowRect.x + 150, windowRect.y + 567, 140, 22), MainWindow[11], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                if (showLocal)
                {
                    selectedObject.transform.SetLocalPositionToZero();
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.LOCAL_POS_TO_ZERO], selectedObject.name);
                }
                else
                {
                    selectedObject.transform.SetPositionToZero();
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.WORLD_POS_TO_ZERO], selectedObject.name);
                }                
            }

            if (GUI.Button(new Rect(windowRect.x + 5, windowRect.y + 592, 140, 22), MainWindow[12], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                if (showLocal)
                {
                    selectedObject.transform.SetLocalRotationToZero();
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.LOCAL_ROT_TO_ZERO], selectedObject.name);
                }
                else
                {
                    selectedObject.transform.SetRotationToZero();
                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.WORLD_ROT_TO_ZERO], selectedObject.name);
                }                
            }

            if (GUI.Button(new Rect(windowRect.x + 150, windowRect.y + 592, 140, 22), MainWindow[13], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                selectedObject.transform.SetLocalScaleToOne();
                OutputWindow_Log(MESSAGE_TEXT[MESSAGES.LOCAL_SCALE_TO_ONE], selectedObject.name);
            }

            if (GUI.Button(new Rect(windowRect.x + 5, windowRect.y + 617, 140, 22), MainWindow[14], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                DrawObjectBounds dob = selectedObject.GetComponentInChildren<DrawObjectBounds>();

                int transformID = selectedObject.transform.GetInstanceID();                

                selectedObject.transform.SetTransformInfo(dob.transformOriginals[transformID]);  
                
                OutputWindow_Log(MESSAGE_TEXT[MESSAGES.TRANSFORM_TO_ORIGINAL], selectedObject.name);
            }

            if (isColliderSelected)
            {
                if (GUI.Button(new Rect(windowRect.x + 150, windowRect.y + 617, 140, 22), MainWindow[15], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
                {
                    DrawColliderBounds dcb = selectedObject.GetComponentInChildren<DrawColliderBounds>();

                    int colliderID = objects[selected_component].GetInstanceID();

                    selectedObject.ResetCollider(dcb.colliderOriginals[colliderID], colliderID);

                    GetColliderInfo();

                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.COLLIDER_TO_ORIGINAL], dcb.colliderOriginals[colliderID].ColliderType.ToString());
                }
            }

            if (GUI.Button(new Rect(windowRect.x + 5, (windowRect.y + windowRect.height) - 27, windowRect.width - 10, 22), MainWindow[16], SNStyles.GetGuiItemStyle(GuiItemType.NORMALBUTTON, GuiColor.Gray)))
            {
                OnDestroy();                               
            }             

            OutputWindow_OnGUI();

            EditWindow_OnGUI();

            ObjectWindow_OnGUI();

            ComponentWindow_OnGUI();

            RendererWindow_OnGUI();

            ObjectInfoWindow_OnGUI();

            AddComponentWindow_OnGUI();

            MarkWindow_OnGUI();

            FMODWindow_OnGUI();

            if (GUI.tooltip != "")
            {
                GUIStyle gUIStyle = SNStyles.GetGuiItemStyle(GuiItemType.TEXTAREA, GuiColor.Green, TextAnchor.MiddleLeft);

                Vector2 vector2 = gUIStyle.CalcSize(new GUIContent(GUI.tooltip));

                GUI.Label(new Rect(Event.current.mousePosition.x + 10, Event.current.mousePosition.y + 10, vector2.x, vector2.y), GUI.tooltip, gUIStyle);
            }
        }       

        public void Update()
        {
            SafetyCheck();

            if (isRayEnabled)
            {
                RaycastMode_Update();
            }

            if (isDirty)
            {
                return;
            }

            PrintObjectInfo();

            if (isColliderSelected)
            {
                PrintColliderInfo();
            }

            if (Input.GetKeyDown(RuntimeHelperZero_Config.KEYBINDINGS["ToggleMouse"]))
            {
                UWE.Utils.lockCursor = !UWE.Utils.lockCursor;
            }

            if (ScrollView_transforms_event.ItemID != -1)
            {
                current_transform_index = ScrollView_transforms_event.ItemID;

                if (ScrollView_transforms_event.MouseButton == 0)
                {                    
                    OnObjectChange(TRANSFORMS[current_transform_index].gameObject);
                    return;
                }

                if (ScrollView_transforms_event.MouseButton == 1)
                {
                    OnBaseObjectChange(TRANSFORMS[current_transform_index].gameObject);
                    return;
                }                
            }

            GetObjectVectors();

            if (isColliderSelected)
            {
                switch (colliderModify.ColliderType)
                {
                    case ColliderType.BoxCollider:
                        sizeText = $"Size x:{colliderModify.Size.x,6:F2}  y:{colliderModify.Size.y,6:F2}  z:{colliderModify.Size.z,6:F2}\n";
                        break;
                    case ColliderType.CapsuleCollider:
                        sizeText = $"Radius:{colliderModify.Radius,6:F2}  height:{colliderModify.Height,6:F2} direction:{EditModeStrings.DIRECTION[colliderModify.Direction]}\n";
                        break;
                    case ColliderType.SphereCollider:
                        sizeText = $"Radius:{colliderModify.Radius,6:F2}\n";
                        break;
                }
            }           

            if (Input.GetKeyDown(RuntimeHelperZero_Config.KEYBINDINGS["ToggleRaycastMode"]))
            {
                if (!Player.main || uGUI.main.loading.IsLoading || IsPlayerInVehicle())
                {
                    isRayEnabled = false;
                    OutputWindow_Log(WARNING_TEXT[WARNINGS.UNABLE_TO_RAYCAST], LogType.Warning);
                    return;
                }
                else
                {
                    ReleaseObjectDrawing();

                    isRayEnabled = !isRayEnabled;

                    OutputWindow_Log(MESSAGE_TEXT[MESSAGES.RAYCAST_STATE], isRayEnabled);
                }
            }

            if (Event.current == null)
            {
                OutputWindow_Log(WARNING_TEXT[WARNINGS.EVENT_CURRENT_NOT_READY], LogType.Warning);
                return;
            }

            if (Event.current.Equals(Event.KeyboardEvent("up")))
            {
                if (isRectTransform)
                {
                    value = 2;
                }
                else
                {
                    value = scaleFactor;
                }

                EditObjectVectors();
            }
            else if (Event.current.Equals(Event.KeyboardEvent("down")))
            {
                if (isRectTransform)
                {
                    value = -2;
                }
                else
                {
                    value = -scaleFactor;
                }

                EditObjectVectors();                
            }

            EditWindow_Update();

            ComponentWindow_Update();

            AddComponentWindow_Update();

            MarkWindow_Update();

            FMODWindow_Update();

            if (showRendererWindow)
                RendererWindow_Update();

            if (showObjectInfoWindow)
                ObjectInfoWindow_Update();
        }       

        private void GetObjectVectors()
        {         
            if (showLocal)
            {
                lRot = selectedObject.transform.localEulerAngles;
                lPos = selectedObject.transform.localPosition;                
            }
            else
            {
                lRot = selectedObject.transform.eulerAngles;
                lPos = selectedObject.transform.position;                
            }

            lScale = selectedObject.transform.localScale;
        }

        private void SetObjectVectors()
        {
            if (showLocal)
            {
                selectedObject.transform.localPosition = new Vector3(lPos.x, lPos.y, lPos.z);                               
            }
            else
            {
                selectedObject.transform.position = new Vector3(lPos.x, lPos.y, lPos.z);                
            }

            selectedObject.transform.localScale = new Vector3(lScale.x, lScale.y, lScale.z);
        }        

        private void EditObjectVectors()
        {
            if (isRectTransform)
            {
                EditRectTransform();
                return;
            }

            switch (EDIT_MODE[current_editmode_index])
            {                
                case "Rotation: x":                    
                    selectedObject.transform.Rotate(Vector3.right, value < 0 ? -1 : 1, showLocal? Space.Self : Space.World);
                    break;

                case "Rotation: y":                    
                    selectedObject.transform.Rotate(Vector3.up, value < 0 ? -1 : 1, showLocal ? Space.Self : Space.World);
                    break;

                case "Rotation: z":                    
                    selectedObject.transform.Rotate(Vector3.forward, value < 0 ? -1 : 1, showLocal ? Space.Self : Space.World);
                    break;

                case "Position: x":
                    lPos.x += value;
                    break;
                case "Position: y":
                    lPos.y += value;
                    break;
                case "Position: z":
                    lPos.z += value;
                    break;
                case "Scale: x,y,z":
                    lScale.x += value;
                    lScale.y += value;
                    lScale.z += value;
                    break;
                case "Scale: x":
                    lScale.x += value;
                    break;
                case "Scale: y":
                    lScale.y += value;
                    break;
                case "Scale: z":
                    lScale.z += value;
                    break;
                case "Collider Size: x":
                    colliderModify.Size = new Vector3(colliderModify.Size.x + value, colliderModify.Size.y, colliderModify.Size.z);
                    break;
                case "Collider Size: y":
                    colliderModify.Size = new Vector3(colliderModify.Size.x, colliderModify.Size.y + value, colliderModify.Size.z);
                    break;
                case "Collider Size: z":
                    colliderModify.Size = new Vector3(colliderModify.Size.x, colliderModify.Size.y, colliderModify.Size.z + value);
                    break;
                case "Collider Center: x":
                    colliderModify.Center = new Vector3(colliderModify.Center.x + value, colliderModify.Center.y, colliderModify.Center.z);
                    break;
                case "Collider Center: y":
                    colliderModify.Center = new Vector3(colliderModify.Center.x, colliderModify.Center.y + value, colliderModify.Center.z);
                    break;
                case "Collider Center: z":
                    colliderModify.Center = new Vector3(colliderModify.Center.x, colliderModify.Center.y, colliderModify.Center.z + value);
                    break;
                case "Collider Radius":
                    colliderModify.Radius += value;
                    break;
                case "Collider Height":
                    colliderModify.Height += value;
                    break;
                case "Collider Direction":

                    switch (colliderModify.Direction)
                    {
                        case 0:
                            colliderModify.Direction = 1;
                            break;
                        case 1:
                            colliderModify.Direction = 2;
                            break;
                        case 2:
                            colliderModify.Direction = 0;
                            break;
                    }
                    break;                
            }

            SetObjectVectors();
            
            if (isColliderSelected)
            {
                switch (colliderModify.ColliderType)
                {
                    case ColliderType.BoxCollider:
                        bc.center = colliderModify.Center;
                        bc.size = colliderModify.Size;                        
                        break;
                    case ColliderType.CapsuleCollider:
                        cc.center = colliderModify.Center;
                        cc.radius = colliderModify.Radius;
                        cc.height = colliderModify.Height;
                        cc.direction = colliderModify.Direction;                        
                        break;
                    case ColliderType.SphereCollider:
                        sc.center = colliderModify.Center;
                        sc.radius = colliderModify.Radius;                        
                        break;
                }                
            }            
        }

        public bool IsPlayerInVehicle()
        {
            return Player.main.inExosuit || Player.main.IsPilotingSeatruck() ? true : false;
        }

    }
}