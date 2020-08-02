using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using RuntimeHelperZero.Configuration;
using System.Collections.Generic;
using RuntimeHelperZero.Command;

namespace RuntimeHelperZero
{
    public static class Main
    {        
        public static RuntimeHelperZero Instance { get; internal set; }
        public static List<GameObject> AllVisuals = new List<GameObject>();

        public static void Load()
        {
            try
            {                
                RuntimeHelperZero_Config.LoadConfig();
                RuntimeHelperZero_Config.InitConfig();
                DevConsole.disableConsole = false;
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
}
