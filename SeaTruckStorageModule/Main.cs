using System;
using UnityEngine;
using Harmony;
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

                HarmonyInstance.Create("BelowZero.SeaTruckStorage.mod").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
