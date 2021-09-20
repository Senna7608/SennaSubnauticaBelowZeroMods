using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HarmonyLib;
using QModManager.API.ModLoading;
using CheatManagerZero.Configuration;

namespace CheatManagerZero
{
    [QModCore]
    public static class Main
    {
        public static GameObject cheatManagerZero { get; private set; }
        public static CheatManagerZero Instance { get; private set; }
        public static CMZ_InfoBar cMZ_InfoBar { get; private set; }
        public static CMZ_Console cMZ_Console { get; private set; }

        [QModPatch]
        public static void Load()
        {
            try
            {
                CmZConfig.InitConfig();

                Assembly assembly = Assembly.GetExecutingAssembly();

                Harmony.CreateAndPatchAll(assembly, $"BelowZero.{assembly.GetName().Name}.mod");                
                
                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);                                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }                    
        }        

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "XMenu")
            {
                if (cheatManagerZero == null)
                {
                    cheatManagerZero = new GameObject("CheatManagerZero");
                }

                if (CmZConfig.isInfoBarEnabled && cMZ_InfoBar == null)
                {
                    cMZ_InfoBar = cheatManagerZero?.AddComponent<CMZ_InfoBar>();
                }
               
                if (CmZConfig.isConsoleEnabled && cMZ_Console == null)
                {
                    cMZ_Console = cheatManagerZero?.AddComponent<CMZ_Console>();
                }

                new CmZConfigCommand();
            }
            
            if (scene.name == "Main")
            {
                if (Instance == null)
                {
                    Instance = cheatManagerZero?.AddComponent<CheatManagerZero>();
                }
            }
        }
        
        /*
        private static void GameInput_OnBindingsChanged()
        {
            Config.WriteConfig();
        }
        */
        /*
        public static void OnDisplayChanged()
        {
            Debug.Log($"Resolution changed!");
        }
        */


    }  
}
