using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.IO;

namespace SeaTruckDepthUpgrades
{
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Load()
        {
            try
            {
                new SeaTruckDepthMK4().Patch();
                new SeaTruckDepthMK5().Patch();
                new SeaTruckDepthMK6().Patch();

                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "BelowZero.SeaTruckDepthUpgrades.mod");                                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }        
    }
}
