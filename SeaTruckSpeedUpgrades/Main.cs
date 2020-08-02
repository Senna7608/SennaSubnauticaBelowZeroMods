using System;
using UnityEngine;
using Harmony;
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

                HarmonyInstance.Create("BelowZero.SeaTruckSpeedUpgrades.mod").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
