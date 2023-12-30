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
        public static ZHelper Instance { get; internal set; }        
        public static CommandRoot commandRoot = null;
        

        [QModPatch]
        public static void Load()
        {
            try
            {
                commandRoot = new CommandRoot("zhelperBase", true);

                commandRoot.AddCommand<ZHelper_Command>();                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }        
    }
}
