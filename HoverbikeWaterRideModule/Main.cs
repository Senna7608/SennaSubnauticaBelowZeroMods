using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using QModManager.API.ModLoading;
using System.IO;

namespace HoverbikeWaterRideModule
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
                new WaterRideModule_Prefab().Patch();

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
