extern alias SEZero;

using System.IO;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using BZHelper;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckStorage
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.slotextenderzero", BepInDependency.DependencyFlags.HardDependency)]
    internal class SeaTruckStorage_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.seatruckstorage";
        private const string MODNAME = "SeaTruckStorage";
        private const string VERSION = "2.0";
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static ManualLogSource BepinLogger;
        internal static SeaTruckStorage_Main mInstance;
        internal Harmony hInstance;

        internal void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            ModCamp.Main.Register(GUID, VERSION);

            new SeaTruckStorage_Prefab().Patch();

            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);           
        }
    }
}
