extern alias SEZero;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SEZero::SlotExtenderZero.API;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ModdedArmsHelperBZ
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.slotextenderzero", BepInDependency.DependencyFlags.HardDependency)]
    internal class ModdedArmsHelperBZ_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.moddedarmshelperbz";
        private const string MODNAME = "ModdedArmsHelperBZ";
        private const string VERSION = "2.0";
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static ManualLogSource BepinLogger;
        internal static ModdedArmsHelperBZ_Main mInstance;
        internal Harmony hInstance;

        internal static GameObject helperRoot = null;
        internal static GameObject cacheRoot = null;
        internal static ArmsCacheManager armsCacheManager = null;

        internal void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            ModCamp.Main.Register(GUID, VERSION);

            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }
    }
}
