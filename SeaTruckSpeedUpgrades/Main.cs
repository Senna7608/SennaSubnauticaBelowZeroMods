using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.IO;

namespace SeaTruckSpeedUpgrades
{
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Load()
        {
            try
            {
                new SeaTruckSpeedMK1().Patch();
                new SeaTruckSpeedMK2().Patch();
                new SeaTruckSpeedMK3().Patch();

                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "BelowZero.SeaTruckSpeedUpgrades.mod");                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
