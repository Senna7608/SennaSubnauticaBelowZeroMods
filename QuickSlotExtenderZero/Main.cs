using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Harmony;
using BZCommon;
using QuickSlotExtenderZero.Configuration;

namespace QuickSlotExtenderZero
{
    public static class Main
    {        
        public static bool isExists_SlotExdenerZero = false;
        public static bool isKeyBindigsUpdate = false;
        public static QSHandler Instance { get; internal set; }
        public static bool isPatched;

        public static void Load()
        {
            //load and init config from file   
            QSEzConfig.LoadConfig();            

            isExists_SlotExdenerZero = ReflectionHelper.IsNamespaceExists("SlotExtenderZero");

            if (isExists_SlotExdenerZero)
                BZLogger.Log("QuickSlotExtenderZero", "SlotExtenderZero found! trying to work together..");            

            try
            {        
                HarmonyInstance.Create("BelowZero.QuickSlotExtenderZero.mod").PatchAll(Assembly.GetExecutingAssembly());

                BZLogger.Log("QuickSlotExtenderZero", "Harmony Patches installed");
                
                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }                        
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "StartScreen")
            {
                isPatched = false;
                //enabling game console
                DevConsole.disableConsole = false;
                //init config
                QSEzConfig.InitConfig();
                //add console commad for configuration window
                new QSEzCommand();
                //add an action if changed controls
                GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;                
            }
        }

        internal static void GameInput_OnBindingsChanged()
        {
            isKeyBindigsUpdate = true;

            //input changed, refreshing key bindings
            QSEzConfig.InitSLOTKEYS();

            if (Instance != null)
                Instance.ReadSlotExtenderZeroConfig();

            isKeyBindigsUpdate = false;
        }

        public static object GetAssemblyClassPublicField(string className, string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int i = 0; i < assemblies.Length; i++)
                {
                    Type[] types = assemblies[i].GetTypes();

                    for (int j = 0; j < types.Length; j++)
                    {
                        if (types[j].FullName == className)
                        {
                            return types[j].GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | bindingFlags).GetValue(types[j]);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }    
}
