using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace SeaTruckFlyModule
{
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);        

        public static void Load()
        {
            try
            {
                new SeaTruckFlyModule().Patch();

                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "BelowZero.SeaTruckFlyModule.mod");                                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }        
    }
}
