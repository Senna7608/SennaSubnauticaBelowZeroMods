using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using QModManager.API.ModLoading;
using RuntimeHelperZero.Configuration;
using RuntimeHelperZero.Command;
using UWE;
using System.Collections;
using BZCommon;

namespace RuntimeHelperZero
{
    [QModCore]
    public static class Main
    {        
        public static RuntimeHelperZero Instance { get; internal set; }
        public static List<GameObject> AllVisuals = new List<GameObject>();

        [QModPatch]
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

        /*
        public static IEnumerator WaitForUGUI()
        {
            while (!uGUI.isInitialized)
            {
                yield return new WaitForSeconds(1);
            }

            if (uGUI.main.transform.Find("RHZeroRoot") == null)
            {
                GameObject RHZeroRoot = new GameObject("RHZeroRoot");

                RHZeroRoot.transform.SetParent(uGUI.main.transform, false);

                RHZeroRoot.AddComponent<RHZCommand>();
            }
            //new RHZCommand();

            if (RuntimeHelperZero_Config.AUTOSTART)
            {
                new RuntimeHelperZero();
            }

            yield break;
        }
        */

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BZLogger.Debug($"Scene: [{scene.name}] loaded");

            if (scene.name == "XMenu")
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
