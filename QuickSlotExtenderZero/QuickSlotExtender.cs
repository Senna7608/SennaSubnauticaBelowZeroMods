using System;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HarmonyLib;
using QuickSlotExtenderZero.Configuration;
using BZHelper;
using BepInEx;
using BepInEx.Logging;
using Nautilus.Utility;

namespace QuickSlotExtenderZero
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    internal class QuickSlotExtender : BaseUnityPlugin
    {
        private const string GUID = "com.senna.quickslotextenderzero";
        private const string MODNAME = "QuickSlotExtender";
        private const string VERSION = "2.0";

        internal ManualLogSource BepinLogger;
        internal QuickSlotExtender mInstance;
        internal Harmony hInstance;      

        internal void Awake()
        {
            mInstance = this;
            BepinLogger = BepInEx.Logging.Logger.CreateLogSource(MODNAME);
            BepinLogger.LogInfo("Awake");

            //load and init config from file   
            QSEzConfig.LoadConfig();

            Main.isExists_SlotExtenderZero = BZHelper.ReflectionHelper.IsNamespaceExists("SlotExtenderZero");

            if (Main.isExists_SlotExtenderZero)
                BZLogger.Log("SlotExtenderZero found! trying to work together..");

            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            BZLogger.Log("Harmony Patches installed");

            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);

            //CoroutineHost.StartCoroutine(WaitForUGUI());

            SaveUtils.RegisterOnQuitEvent(OnQuitEvent);
        }        

        private static void OnQuitEvent()
        {
            Main.isPatched = false;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "XMenu")
            {
                QSEzConfig.InitConfig();

                new QSEzCommand();

                GameInput.OnBindingsChanged += Main.GameInput_OnBindingsChanged;
            }            
        }

        /*
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
        */        
    }

    internal static class Main
    {
        internal static bool isExists_SlotExtenderZero = false;
        internal static bool isKeyBindigsUpdate = false;
        public static QSEzHandler Instance { get; internal set; }
        internal static bool isPatched = false;

        internal static void GameInput_OnBindingsChanged()
        {
            Main.isKeyBindigsUpdate = true;

            //input changed, refreshing key bindings
            QSEzConfig.InitSLOTKEYS();

            
            if (Main.Instance != null)
                Main.Instance.ReadSlotExtenderZeroConfig();
            

            Main.isKeyBindigsUpdate = false;
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
