using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZHelper.Components
{
    public static class ComponentHelper
    {
        public static T FindComponentWithID<T>(this GameObject gameObject, int ID) where T : Component
        {
            foreach (T component in gameObject.GetComponents<T>())
            {
                if (component.GetInstanceID() == ID)
                {
                    return component;
                }
            }

            return null;
        }

        public static List<string> CreateComponentInfoList(this UnityEngine.Object _object)
        {
            List<string> keywords = new List<string>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            
            keywords.Add("Properties:");

            foreach (PropertyInfo propertyInfo in _object.GetType().GetProperties(bindingFlags))
            {
                try
                {
                    object value = propertyInfo.GetValue(_object, bindingFlags, null, null, null);

                    Type valueType = value.GetType();

                    if (valueType == typeof(Scene))
                    {
                        Scene scene = (Scene)value;

                        keywords.Add($"{propertyInfo.Name} = {scene.name}");
                        continue;
                    }
                    else if (valueType == typeof(GameObject))
                    {
                        GameObject gameObject = (GameObject)value;

                        keywords.Add($"{propertyInfo.Name} = {gameObject.name}");
                        continue;
                    }
                    else if (valueType == typeof(Transform))
                    {
                        Transform transform = (Transform)value;

                        keywords.Add($"{propertyInfo.Name} = {transform.name}");
                        continue;
                    }
                    else if (valueType.IsArray)
                    {                         
                        keywords.Add($"{propertyInfo.Name} = {valueType.ToString().Split('.').GetLast()}");                        

                        Array array = value as Array;

                        int x = 0;

                        foreach (var item in array)
                        {                            
                            keywords.Add($"{propertyInfo.Name} {x} = {item}");
                            x++;
                        }
                        continue;
                    }
                    else
                    {
                        keywords.Add($"{propertyInfo.Name} = {value.ToString()}");
                    }
                }
                catch
                {
                    continue;
                }
            }

            keywords.Add("Fields:");

            foreach (FieldInfo fieldInfo in _object.GetType().GetFields(bindingFlags))
            {
                try
                {
                    object value = fieldInfo.GetValue(_object);
                    
                    Type valueType = value.GetType();
                    
                    if (valueType.IsArray)
                    {
                        keywords.Add($"{fieldInfo.Name} = {valueType.ToString().Split('.').GetLast()}");
                        
                        var array = value as Array;

                        int x = 0;

                        foreach (var item in array)
                        {
                            keywords.Add($"{fieldInfo.Name} {x} = {item}");
                            x++;
                        }
                    }
                    else
                    {
                        keywords.Add($"{fieldInfo.Name} = {value.ToString()}");
                    }
                }
                catch
                {
                    continue;
                }                
            }

            return keywords;
        }


        public static List<FieldInfo> GetComponentFieldsList(this Component component)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            foreach (FieldInfo fieldInfo in component.GetType().GetFields(bindingFlags))
            {
                try
                {
                    fieldInfos.Add(fieldInfo);
                }
                catch
                {
                    continue;
                }
            }

            return fieldInfos;
        }


        public static List<PropertyInfo> GetComponentPropertiesList(this Component component)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            foreach (PropertyInfo propertyInfo in component.GetType().GetProperties(bindingFlags))
            {
                try
                {
                    properties.Add(propertyInfo);
                }
                catch
                {
                    continue;
                }
            }

            return properties;
        }



    }

    
    
}
