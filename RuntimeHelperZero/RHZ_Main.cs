using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using RuntimeHelperZero.Configuration;
using RuntimeHelperZero.Command;
using BZHelper;
using BepInEx;
using BepInEx.Logging;

namespace RuntimeHelperZero
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class RHZ_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.runtimehelperzero";
        private const string MODNAME = "RuntimeHelperZero";
        private const string VERSION = "2.0";        

        internal static ManualLogSource BepinLogger;        
        
        public void Awake()
        {            
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            try
            {                
                RuntimeHelperZero_Config.LoadConfig();
                RuntimeHelperZero_Config.InitConfig();                                

                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }                       
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BZLogger.Debug($"Scene: [{scene.name}] loaded");

            if (scene.name == "XMenu")
            {
                PlatformUtils.SetDevToolsEnabled(true);                
                BZLogger.Log("Console enabled");
                new RHZCommand();

                if (RuntimeHelperZero_Config.AUTOSTART)
                {
                    new RuntimeHelperZero();
                }
            }
            
            if (scene.name =="Main")
            {
                new RHZCommand();
            }
        }
    }
    
    public static class Main
    {
        public static RuntimeHelperZero Instance { get; internal set; }
    }
}
