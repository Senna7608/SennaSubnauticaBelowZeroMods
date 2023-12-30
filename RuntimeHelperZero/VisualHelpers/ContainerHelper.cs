﻿using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHelperZero.VisualHelpers
{
    public enum BaseType
    {
        Object,
        Collider
    }

    public enum ContainerType
    {  
        Rectangle,
        Box,
        Triangle,
        Sphere,
        Capsule,
        Mesh,
        ContinuousLine
    }

    public static class ContainerHelper
    {
        public static void EnableContainer(this GameObject lineContainer)
        {
            lineContainer.SetActive(true);
        }       

        public static void DisableContainer(this GameObject lineContainer)
        {
            lineContainer.SetActive(false);
        }        

        public static void SetContainerState(this GameObject lineContainer, bool state)
        {
            lineContainer.SetActive(state);
        }        

        public static void DestroyContainer(this GameObject lineContainer)
        {
            UnityEngine.Object.Destroy(lineContainer);
        }

        public static void DestroyContainers(this List<GameObject> lineContainers)
        {
            foreach (GameObject container in lineContainers)
            {
                UnityEngine.Object.Destroy(container);
            }

            lineContainers.Clear();
        }

        public static void SetContainerTransform(this GameObject lineContainer,  GameObject containerBase)
        {
            lineContainer.transform.SetParent(containerBase.transform, false);
            lineContainer.transform.localPosition = Vector3.zero;
            lineContainer.transform.localRotation = Quaternion.identity;            
        }

        public static void CreateLineContainers(this GameObject containerBase, ref List<GameObject> lineContainers, ContainerType containerType, float lineWidth, Color lineColor, bool useWorldSpace)
        {
            if (lineContainers == null)
            {
                lineContainers = new List<GameObject>();
            }

            lineContainers.Clear();

            switch (containerType)
            {
                case ContainerType.Triangle:
                    lineContainers.Add(new GameObject { name = $"RHZ_TRIANGLE" });
                    lineContainers[0].SetContainerTransform(containerBase);
                    lineContainers[0].AddLineRendererToContainer(lineWidth, lineColor, useWorldSpace);
                    break;

                case ContainerType.Rectangle:
                    lineContainers.Add(new GameObject { name = $"RHZ_RECTANGLE" });
                    lineContainers[0].SetContainerTransform(containerBase);
                    lineContainers[0].AddLineRendererToContainer(lineWidth, lineColor, useWorldSpace);
                    break;

                case ContainerType.Box:
                    for (int i = 0; i < 12; i++)
                    {
                        lineContainers.Add(new GameObject { name = $"RHZ_BOX_{i}" });
                        lineContainers[i].SetContainerTransform(containerBase);
                        lineContainers[i].AddLineRendererToContainer(lineWidth, lineColor, useWorldSpace);
                    }
                    break;

                case ContainerType.Sphere:
                    for (int i = 0; i < 3; i++)
                    {
                        lineContainers.Add(new GameObject { name = $"RHZ_SPHERE_{i}" });
                        lineContainers[i].SetContainerTransform(containerBase);
                        lineContainers[i].AddLineRendererToContainer(lineWidth, lineColor, useWorldSpace);
                    }                    
                    break;

                case ContainerType.Capsule:
                    for (int i = 0; i < 6; i++)
                    {
                        lineContainers.Add(new GameObject { name = $"RHZ_CAPSULE_circle_{i}" });                        
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        lineContainers.Add(new GameObject { name = $"RHZ_CAPSULE_line_{i}" });
                        
                    }
                    foreach (GameObject lineContainer in lineContainers)
                    {
                        lineContainer.SetContainerTransform(containerBase);
                        lineContainer.AddLineRendererToContainer(lineWidth, lineColor, useWorldSpace);
                    }
                    break;

                case ContainerType.ContinuousLine:
                    lineContainers.Add(new GameObject { name = $"RHZ_ContinuousLine" });
                    lineContainers[0].SetContainerTransform(containerBase);
                    lineContainers[0].AddLineRendererToContainer(lineWidth, lineColor, useWorldSpace);
                    break;               
            }            
        }
        
        public static GameObject GetOrAddVisualBase(this GameObject gameObject, BaseType baseType)
        {
            GameObject containerBase = null;

            switch (baseType)
            {
                case BaseType.Object:
                    containerBase = gameObject.FindChild("OBJECT_VISUAL_BASE");

                    if (containerBase)
                    {
                        return containerBase;
                    }
                    else
                    {
                        containerBase = new GameObject("OBJECT_VISUAL_BASE");
                    }
                    break;

                case BaseType.Collider:
                    containerBase = gameObject.FindChild("COLLIDER_VISUAL_BASE");

                    if (containerBase)
                    {
                        return containerBase;
                    }
                    else
                    {
                        containerBase = new GameObject("COLLIDER_VISUAL_BASE");
                    }
                    break;
            }

            containerBase.transform.SetParent(gameObject.transform, false);
            containerBase.transform.localPosition = Vector3.zero;
            containerBase.transform.localRotation = Quaternion.identity;

            return containerBase;
        }

        public static GameObject GetVisualBase(this GameObject gameObject, BaseType baseType)
        {
            GameObject containerBase = null;

            switch (baseType)
            {
                case BaseType.Object:
                    containerBase = gameObject.FindChild("OBJECT_VISUAL_BASE");                                  
                    break;

                case BaseType.Collider:
                    containerBase = gameObject.FindChild("COLLIDER_VISUAL_BASE");                                     
                    break;
            }
            
            return containerBase;
        }
    }
}
