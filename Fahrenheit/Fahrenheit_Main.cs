using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.IO;
using BepInEx;
using BepInEx.Logging;

namespace Fahrenheit
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]      
    public class Fahrenheit_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.fahrenheit";
        private const string MODNAME = "Fahrenheit";
        private const string VERSION = "2.0";

        internal static ManualLogSource BepinLogger;
        internal static Fahrenheit_Main mInstance;
        internal Harmony hInstance;        

        public void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            //new FahrenheitChip_Prefab().Patch();

            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }        
    }

    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool FahrenheitEnabled { get; set; } = false;
        public static bool NightVisionEnabled { get; set; } = false;
    }
}
