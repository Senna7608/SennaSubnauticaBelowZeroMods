using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using CheatManagerZero.Configuration;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using System.IO;
using Nautilus.Handlers;

namespace CheatManagerZero
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    internal class CheatManagerZero : BaseUnityPlugin
    {
        private const string GUID = "com.senna.cheatmanagerzero";
        private const string MODNAME = "CheatManagerZero";
        private const string VERSION = "2.0";

        internal static ManualLogSource BepinLogger;
        internal static CheatManagerZero mInstance;
        internal Harmony hInstance;

        internal void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            CMZ_Config.Load();
            CMZ_Config.Set();

            OptionsPanelHandler.RegisterModOptions(new CMZ_Options());

            hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Main")
            {
                if (Main.Instance == null)
                {
                    Main.Instance = Main.cheatManagerZero?.AddComponent<CheatManagerZeroControl>();
                }
            }

            if (scene.name == "XMenu")
            {
                if (Main.cheatManagerZero == null)
                {
                    Main.cheatManagerZero = new GameObject("CheatManagerZero");
                }

                if (Main.cMZ_InfoBar == null)
                {
                    Main.cMZ_InfoBar = Main.cheatManagerZero?.AddComponent<CMZ_InfoBar>();
                }

                if (Main.cMZ_Console == null)
                {
                    Main.cMZ_Console = Main.cheatManagerZero?.AddComponent<CMZ_Logger>();
                }
            }
        }

        public static void OnFragmentTrackerChanged()
        {
            foreach (FragmentTracker tracker in Main.trackerDb)
            {
                if (tracker != null)
                {
                    tracker.OnTrackerVisibleChange();
                }
            }
        }
    }    

    internal static class Main
    {
        internal static GameObject cheatManagerZero;
        internal static CheatManagerZeroControl Instance;
        internal static CMZ_Logger cMZ_Console;
        internal static CMZ_InfoBar cMZ_InfoBar;
        internal static List<FragmentTracker> trackerDb = new List<FragmentTracker>();
        internal static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }    
}
