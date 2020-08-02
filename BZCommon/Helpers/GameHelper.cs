using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BZCommon.Helpers
{
    public class GameHelper
    {   
        public void SetProgressColor(Color color)
        {
            HandReticle.main.progressText.color = color;
            HandReticle.main.progressImage.color = color;
        }

        public void SetInteractColor(Color color, bool isSetSecondary = true)
        {
            HandReticle.main.compTextHand.color = color;

            if (isSetSecondary)
                HandReticle.main.compTextUse.color = color;
        }
        
        public void DebugObjectTransforms(Transform transform, string space = "")
        {
            Console.WriteLine($"{space}--{transform.name}");

            foreach (Transform child in transform)
            {
                DebugObjectTransforms(child, space + "   |");
            }
        }
        
        public void DebugComponent(Component component)
        {
            List<string> keywords = new List<string>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreReturn;

            keywords.Add("Properties:");

            foreach (PropertyInfo propertyInfo in component.GetType().GetProperties(bindingFlags))
            {
                keywords.Add($"{propertyInfo.Name}  [{propertyInfo.GetValue(component, bindingFlags, null, null, null).ToString()}]");
            }

            keywords.Add("Fields:");

            foreach (FieldInfo fieldInfo in component.GetType().GetFields(bindingFlags))
            {
                keywords.Add($"{fieldInfo.Name}  [{fieldInfo.GetValue(component).ToString()}]");
            }

            foreach (string key in keywords)
            {
                BZLogger.Log($"{key}");
            }
        }
        
       
        
        public int GetSlotIndex(Vehicle vehicle, TechType techType)
        {
            InventoryItem inventoryItem = null;

            for (int i = 0; i < vehicle.GetSlotCount(); i++)
            {
                try
                {
                    inventoryItem = vehicle.GetSlotItem(i);
                }
                catch
                {
                    continue;
                }

                if (inventoryItem != null && inventoryItem.item.GetTechType() == techType)
                {
                    return vehicle.GetType() == typeof(Exosuit) ? i - 2 : i;
                }
            }

            return -1;
        }        
    }
}
