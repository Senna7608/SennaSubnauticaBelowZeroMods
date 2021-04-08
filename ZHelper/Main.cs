using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using QModManager.API.ModLoading;
using BZCommon.Helpers;
using BZCommon;
using System.Collections.Generic;

namespace ZHelper
{
    [QModCore]
    public static class Main
    {
        public static Zhelper Instance { get; internal set; }
        public static List<GameObject> AllVisuals = new List<GameObject>();
        public static CommandRoot commandRoot = null;
        

        [QModPatch]
        public static void Load()
        {
            try
            {
                commandRoot = new CommandRoot("zhelperBase");

                commandRoot.AddCommand<ZHelperCommand>();                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }        
    }
}
