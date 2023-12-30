/*
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
#pragma warning disable CS1591 //XML documentation

namespace ModdedArmsHelperBZ
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.slotextenderzero", BepInDependency.DependencyFlags.HardDependency)]
    internal class ModdedArmsHelper : BaseUnityPlugin
    {
        private const string GUID = "com.senna.moddedarmshelperbz";
        private const string MODNAME = "ModdedArmsHelperBZ";
        private const string VERSION = "1.0";

        internal ManualLogSource BepinLogger;
        internal ModdedArmsHelper mInstance;
        internal Harmony hInstance;

        internal void Awake()
        {
            mInstance = this;            
            BepinLogger = BepInEx.Logging.Logger.CreateLogSource(MODNAME);
            BepinLogger.LogInfo("Awake");
            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }        
    }

    internal static class Main
    {
        internal static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static GameObject helperRoot = null;
        internal static GameObject cacheRoot = null;
        internal static ArmsCacheManager armsGraphics = null;
    }
}
*/