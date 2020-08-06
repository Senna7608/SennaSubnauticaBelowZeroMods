using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using BZCommon.Helpers;
using BZCommon;

namespace ZHelper
{
    public static class Main
    {
        public static CommandRoot commandRoot = null;

        public static void Load()
        {
            try
            {                
                //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "SubnauticaBelowZero.ZHelper.mod");

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
