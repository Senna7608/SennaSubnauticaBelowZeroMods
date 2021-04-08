using System;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using QModManager.API.ModLoading;

namespace BZTestMOD
{
    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            try
            {
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
