using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Harmony;

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

                HarmonyInstance.Create("BelowZero.SeaTruckFlyModule.mod").PatchAll(Assembly.GetExecutingAssembly());                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }        
    }
}
