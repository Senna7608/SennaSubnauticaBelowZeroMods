using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using QModManager.API.ModLoading;
using HarmonyLib;
using System.Diagnostics;
using SMLHelper.V2.Handlers;

namespace SeaTruckScannerModule
{
    [QModCore]
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        public static readonly string VERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;        

        internal static List<SeaTruckScannerModuleManager> scannerModules = new List<SeaTruckScannerModuleManager>();

        internal static bool uGUI_PatchComplete = false;

        internal static SeaTruckScannerModuleFragment_Prefab fragment;

        [QModPatch]
        public static void Load()
        {
            try
            {
                if (!Directory.Exists($"{modFolder}/SaveGame"))
                {
                    Directory.CreateDirectory($"{modFolder}/SaveGame");
                }

                //ExpandSlotMapping();

                fragment = new SeaTruckScannerModuleFragment_Prefab();
                fragment.Patch();
                new SeaTruckScannerModule_Prefab(fragment).Patch();
                new SeaTruckScannerHUDChip_Prefab().Patch();
                new SeaTruckScannerHUDChipUpgrade_Prefab().Patch();

                Assembly assembly = Assembly.GetExecutingAssembly();

                Harmony.CreateAndPatchAll(assembly, $"BelowZero.{assembly.GetName().Name}.mod");               

                IngameMenuHandler.Main.RegisterOnQuitEvent(OnQuitEvent);                
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        private static void OnQuitEvent()
        {
            uGUI_PatchComplete = false;            
        }        

        /*
        public static void ExpandSlotMapping()
        {
            for (int i = 1; i < 5; i++)
            {
                Equipment.slotMapping.Add($"ScannerModuleBattery{i}", EquipmentType.BatteryCharger);
            }
        }
        */
    }
}
