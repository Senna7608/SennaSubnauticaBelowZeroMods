using System;
using UnityEngine;
using System.Reflection;
using QModManager.API.ModLoading;
using System.IO;
using SeaTruckArmAPITest.ArmPrefabs;
using SeaTruckArms.API;
using SeaTruckArmAPITest.Fragments;

namespace SeaTruckArmAPITest
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
                SeaTruckArmFragment fragment = new ClawArmFragment_APITEST();
                fragment.Patch();

                new ClawArmPrefab_APITEST(fragment).Patch();                                             
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
