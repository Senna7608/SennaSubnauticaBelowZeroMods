extern alias SEZero;

using System.IO;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArmorUpgrades
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.slotextenderzero", BepInDependency.DependencyFlags.HardDependency)]
    internal class SeaTruckArmorUpgrades_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.seatruckarmorupgrades";
        private const string MODNAME = "SeaTruckArmorUpgrades";
        private const string VERSION = "2.0";
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static ManualLogSource BepinLogger;
        internal static SeaTruckArmorUpgrades_Main mInstance;
        internal Harmony hInstance;

        internal void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            ModCamp.Main.Register(GUID, VERSION);

            new SeaTruckArmorMK1_Prefab().Patch();
            new SeaTruckArmorMK2_Prefab().Patch();
            new SeaTruckArmorMK3_Prefab().Patch();

            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}
