using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace SeaTruckArmorUpgrades
{
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Load()
        {
            try
            {
                new SeaTruckArmorMK1().Patch();
                new SeaTruckArmorMK2().Patch();
                new SeaTruckArmorMK3().Patch();

                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "BelowZero.SeaTruckArmorUpgrades.mod");                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
