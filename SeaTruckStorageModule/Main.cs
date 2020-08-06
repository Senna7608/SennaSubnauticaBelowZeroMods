using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.IO;

namespace SeaTruckStorage
{
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Load()
        {
            try
            {
                new SeaTruckStorage().Patch();

                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "BelowZero.SeaTruckStorage.mod");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
