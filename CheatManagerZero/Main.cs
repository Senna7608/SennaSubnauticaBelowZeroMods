using System;
using System.Reflection;
using Harmony;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using CheatManagerZero.Configuration;
using BZCommon;

namespace CheatManagerZero
{
    public static class Main
    {
        public static GameObject cheatManagerZero = null;
        public static CheatManagerZero Instance = null;
        public static CMZ_InfoBar cMZ_InfoBar = null;
        public static CMZ_Console cMZ_Console = null;        

        public static void Load()
        {
            try
            {               
                HarmonyInstance.Create("SubnauticaBelowZero.CheatManagerZero.mod").PatchAll(Assembly.GetExecutingAssembly());
                DevConsole.disableConsole = false;
                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);

                Config.InitConfig();                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }                    
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Main")
            {
                if (Instance == null)
                {
                    Instance = cheatManagerZero.AddComponent<CheatManagerZero>();
                }
            }
            else if (scene.name == "StartScreen")
            {
                // DisplayManager.OnDisplayChanged += OnDisplayChanged;               
                if (!cheatManagerZero)
                {
                    cheatManagerZero = new GameObject("CheatManagerZero");
                }

                if (Config.isInfoBarEnabled && cMZ_InfoBar == null)
                {
                    cMZ_InfoBar = cheatManagerZero.AddComponent<CMZ_InfoBar>();
                }

                if (Config.isConsoleEnabled && cMZ_Console == null)
                {
                    cMZ_Console = cheatManagerZero.AddComponent<CMZ_Console>();
                }

                new CMZConfigCommand();

                //GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;
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
