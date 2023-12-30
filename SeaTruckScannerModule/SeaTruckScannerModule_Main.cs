extern alias SEZero;

using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckScannerModule
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.slotextenderzero", BepInDependency.DependencyFlags.HardDependency)]
    internal class SeaTruckScannerModule_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.seatruckscannermodule";
        private const string MODNAME = "SeaTruckScannerModule";
        private const string VERSION = "2.0";
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string FILEVERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        internal static List<SeaTruckScannerModuleManager> scannerModules = new List<SeaTruckScannerModuleManager>();
        internal static SeaTruckScannerModuleFragment_Prefab fragment;

        internal static ManualLogSource BepinLogger;
        internal static SeaTruckScannerModule_Main mInstance;
        internal Harmony hInstance;

        internal void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            ModCamp.Main.Register(GUID, VERSION);

            if (!Directory.Exists($"{modFolder}/SaveGame"))
            {
                Directory.CreateDirectory($"{modFolder}/SaveGame");
            }

            fragment = new SeaTruckScannerModuleFragment_Prefab();
            fragment.Patch();
            new SeaTruckScannerModule_Prefab(fragment).Patch();
            new SeaTruckScannerHUDChip_Prefab().Patch();
            new SeaTruckScannerHUDChipUpgrade_Prefab().Patch();

            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}
