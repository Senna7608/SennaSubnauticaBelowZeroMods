using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Harmony;

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

                HarmonyInstance.Create("BelowZero.SeaTruckArmorUpgrades.mod").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
