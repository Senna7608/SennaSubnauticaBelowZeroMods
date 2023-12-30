using BZCommon.Helpers.RuntimeGUI;
using BZHelper;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZHelper.Helpers;
using ZHelper.VisualHelpers;

namespace ZHelper
{
    public partial class ZHelper // Component Window
    {
        private Rect componentWindowRect = new Rect(302, 30, 300, 400);
        private List<Component> components = new List<Component> ();
        public List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
        private List<string> componentNames = new List<string>();
        private List<GUI_content> ComponentScrollContents = new List<GUI_content>();

        private int selected_component = 0;

        private BoxCollider bc;
        private CapsuleCollider cc;
        private SphereCollider sc;

        private ColliderInfo colliderModify;

        private PropertyInfo changeEnabledproperty = null;

        private bool enabledValue;

        public bool isColliderSelected = false;

        //private bool isRectTransform = false;

        void ComponentWindow_Start()
        {
           GuiBase.gUIEventControl += ComponentWindow_EventControl;

           RefreshComponentsList();
        }       

        private void RefreshComponentsList()
        {
            isColliderSelected = false;
            //isRectTransform = false;

            components.Clear();
            componentNames.Clear();
            objects.Clear();

            selectedObject.GetComponents(components);

            objects.Add(selectedObject);

            foreach (Component component in components)
            {
                objects.Add(component);
            }

            objects.Sort(delegate (UnityEngine.Object a, UnityEngine.Object b)
            {
                return a.GetUeObjectShortType().CompareTo(b.GetUeObjectShortType());
            });

            foreach (UnityEngine.Object o in objects)
            {
                componentNames.Add(o.GetUeObjectShortType());
            }            

            FillGroupContents(ref componentNames, ref ComponentScrollContents);

            GuiBase.SetGroupLabel(2, 4, $"{selectedObject.name} components:");

            GuiBase.RefreshGroup(2, 4);

            selected_component = 0;

            changeEnabledproperty = null;
        }

        public void ComponentWindow_EventControl(GUI_event guiEvent)
        {
            if (guiEvent.WindowID == 2 && guiEvent.GroupID == 4)
            {
                selected_component = guiEvent.ItemID;

                Type componentType = objects[selected_component].GetType();

                if (IsSupportedCollider(objects[selected_component]))
                {
                    selectedObject.GetComponentInChildren<DrawColliderBounds>().DrawCollider(objects[selected_component].GetInstanceID());

                    GetColliderInfo();

                    //RefreshEditModeList();

                    isColliderSelected = true;
                }
                else
                {
                    isColliderSelected = false;
                    //RefreshEditModeList();
                }

                if (IsSupportedRenderer(objects[selected_component]))
                {
                    //showObjectInfoWindow = false;
                    //RendererWindow_Awake();
                    RendererWindow_Start(); 
                    GuiBase.DisableWindow(4);
                    GuiBase.EnableWindow(5);
                }
                else
                {
                    //showRendererWindow = false;
                    CompInfoWindow_Start(objects[selected_component]);
                    //showObjectInfoWindow = true;
                    GuiBase.DisableWindow(5);
                    GuiBase.EnableWindow(4);
                }

                if (IsRectTransform(objects[selected_component]))
                {
                    //isRectTransform = true;
                    //showObjectInfoWindow = true;
                    //RefreshEditModeList();
                }
                else
                {
                    //isRectTransform = false;
                    //RefreshEditModeList();
                }

                
                if (objects[selected_component].GetType() == typeof(ColorCustomizer))
                {
                    DebugColorCustomizer(objects[selected_component] as ColorCustomizer);
                }
                
                if (objects[selected_component].GetType() == typeof(SkyApplier))
                {
                    DebugSkyApplier(objects[selected_component] as SkyApplier);
                }
                

                if (IsComponentInBlacklist(objects[selected_component]))
                {
                    changeEnabledproperty = null;
                    return;
                }

                changeEnabledproperty = objects[selected_component].GetType().GetProperty("enabled");

                if (changeEnabledproperty != null)
                {
                    enabledValue = (bool)changeEnabledproperty.GetValue(objects[selected_component], BindingFlags.Instance | BindingFlags.Public, null, null, null);
                }

            }
        }

        private bool IsSupportedCollider(UnityEngine.Object component)
        {
            Type componentType = component.GetType();

            if (componentType == typeof(BoxCollider))
            {
                return true;
            }
            else if (componentType == typeof(SphereCollider))
            {
                return true;
            }
            else if (componentType == typeof(CapsuleCollider))
            {
                return true;
            }
            else if (componentType == typeof(MeshCollider))
            {
                return true;
            }
            return false;
        }

        private bool IsSupportedRenderer(UnityEngine.Object component)
        {
            Type componentType = component.GetType();

            if (componentType == typeof(MeshRenderer))
            {
                return true;
            }
            else if (componentType == typeof(SkinnedMeshRenderer))
            {
                return true;
            }

            return false;
        }

        private bool IsRectTransform(UnityEngine.Object component)
        {
            return component.GetType() == typeof(RectTransform);
        }

        private void RemoveComponent(UnityEngine.Object component)
        {
            if (IsComponentInBlacklist(component))
            {
                Message(WARNING_TEXT[WARNINGS.COMPONENT_IN_BLACKLIST], MessageType.Warning, component.GetUeObjectShortType());
                return;
            }

            if (IsSupportedCollider(component))
            {
                isColliderSelected = false;
            }

            try
            {
                DestroyImmediate(component);
                Message(MESSAGE_TEXT[MESSAGES.COMPONENT_REMOVED], MessageType.Warning, component.GetUeObjectShortType(), selectedObject.name);
            }
            catch
            {
                Message(MESSAGE_TEXT[MESSAGES.COMPONENT_CANNOT_REMOVED], MessageType.Warning, component.GetUeObjectShortType(), selectedObject.name);
            }

            UpdateVisuals();
        }

        private void GetColliderInfo()
        {
            Collider collider = (Collider)objects[selected_component];

            switch (collider.GetColliderType())
            {
                case ColliderType.BoxCollider:
                    bc = (BoxCollider)collider;
                    colliderModify.ColliderType = ColliderType.BoxCollider;
                    colliderModify.Size = bc.size;
                    colliderModify.Center = bc.center;
                    return;

                case ColliderType.CapsuleCollider:
                    cc = (CapsuleCollider)collider;
                    colliderModify.ColliderType = ColliderType.CapsuleCollider;
                    colliderModify.Center = cc.center;
                    colliderModify.Radius = cc.radius;
                    colliderModify.Direction = cc.direction;
                    colliderModify.Height = cc.height;
                    return;

                case ColliderType.SphereCollider:
                    sc = (SphereCollider)collider;
                    colliderModify.ColliderType = ColliderType.SphereCollider;
                    colliderModify.Center = sc.center;
                    colliderModify.Radius = sc.radius;
                    return;

                case ColliderType.MeshCollider:
                    MeshCollider mc = (MeshCollider)collider;
                    colliderModify.ColliderType = ColliderType.MeshCollider;
                    if (mc.sharedMesh.isReadable)
                    {
                        colliderModify.Vertices = mc.sharedMesh.vertices.Length;
                        colliderModify.Triangles = mc.sharedMesh.triangles.Length;
                    }
                    else
                    {
                        colliderModify.Vertices = 0;
                        colliderModify.Triangles = 0;
                    }
                    colliderModify.MeshIsReadable = mc.sharedMesh.isReadable;
                    return;
            }
        }

        public static readonly List<Type> Components_Blacklist = new List<Type>()
        {
            typeof(Transform),
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(MeshRenderer),
            typeof(SkinnedMeshRenderer),
            typeof(Mesh),
            typeof(MeshFilter),
            typeof(Shader),
            //typeof(RuntimeHelperZero),
            typeof(DrawObjectBounds),
            typeof(DrawColliderBounds),
            typeof(TracePlayerPos)
        };

        private bool IsComponentInBlacklist(UnityEngine.Object component)
        {
            Type componentType = component.GetType();

            foreach (Type type in Components_Blacklist)
            {
                if (type == componentType)
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }

            return false;
        }

        internal void DebugColorCustomizer(ColorCustomizer colorCustomizer)
        {
            foreach (ColorCustomizer.ColorData colorData in colorCustomizer.colorDatas)
            {
                DebugMessage($"Renderer: {colorData.renderer.name}, material index: {colorData.materialIndex}");
            }
        }

        internal void DebugSkyApplier(SkyApplier skyApplier)
        {
            foreach (Renderer renderer in skyApplier.renderers)
            {
                DebugMessage($"Renderer: {renderer.name}");
            }
        }
    }
}
