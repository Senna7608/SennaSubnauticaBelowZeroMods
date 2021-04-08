using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using QModManager.API.ModLoading;

namespace SeaTruckDepthUpgrades
{
    [QModCore]
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [QModPatch]
        public static void Load()
        {
            try
            {
                new SeaTruckDepthMK4_Prefab().Patch();
                new SeaTruckDepthMK5_Prefab().Patch();
                new SeaTruckDepthMK6_Prefab().Patch();

                Assembly assembly = Assembly.GetExecutingAssembly();

                Harmony.CreateAndPatchAll(assembly, $"BelowZero.{assembly.GetName().Name}.mod");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }        
    }
}
