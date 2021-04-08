using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HarmonyLib;
using QModManager.API.ModLoading;
using BZCommon;
using QuickSlotExtenderZero.Configuration;
using System.Collections;
using UWE;
using SMLHelper.V2.Handlers;

namespace QuickSlotExtenderZero
{
    [QModCore]
    public static class Main
    {        
        public static bool isExists_SlotExtenderZero = false;
        public static bool isKeyBindigsUpdate = false;
        public static QSEzHandler Instance { get; internal set; }
        public static bool isPatched = false;

        [QModPatch]
        public static void Load()
        {
            //load and init config from file   
            QSEzConfig.LoadConfig();            

            isExists_SlotExtenderZero = ReflectionHelper.IsNamespaceExists("SlotExtenderZero");

            if (isExists_SlotExtenderZero)
                BZLogger.Log("SlotExtenderZero found! trying to work together..");            

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                Harmony.CreateAndPatchAll(assembly, $"BelowZero.{assembly.GetName().Name}.mod");

                BZLogger.Log("Harmony Patches installed");

                //SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);

                CoroutineHost.StartCoroutine(WaitForUGUI());

                IngameMenuHandler.Main.RegisterOnQuitEvent(OnQuitEvent);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }                        
        }

        private static void OnQuitEvent()
        {
            isPatched = false;
        }

        public static IEnumerator WaitForUGUI()
        {
            while (!uGUI.isInitialized)
            {
                yield return new WaitForSeconds(1);
            }
            
            QSEzConfig.InitConfig();
            
            new QSEzCommand();
            
            GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;

            yield break;
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
