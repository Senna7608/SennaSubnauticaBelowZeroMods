using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using QModManager.API.ModLoading;
using System.IO;

namespace Fahrenheit
{
    [QModCore]
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static bool FahrenheitEnabled { get; set; } = false;
        public static bool NightVisionEnabled { get; set; } = false;

        [QModPatch]
        public static void Load()
        {
            try
            {
                new FahrenheitChip_Prefab().Patch();

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
