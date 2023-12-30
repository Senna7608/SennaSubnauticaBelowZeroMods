using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using BZHelper.ConfigurationParser;
using BZHelper;

namespace CheatManagerZero.Configuration
{
    public static class CMZ_Config
    {
        public static string PROGRAM_VERSION = string.Empty;
        public static string CONFIG_VERSION = string.Empty;

        private static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string FILENAME = $"{modFolder}/config.txt";
        internal const float ASPECT = 4.8f;

        internal static int overPowerMultiplier;        
        internal static int hungerAndThirstInterval;        

        private static readonly string[] SECTIONS = { "Hotkeys", "Program", "Settings", "ToggleButtons", "UserWarpTargets" };
        internal static Dictionary<string, KeyCode> KEYBINDINGS;
        internal static Dictionary<string, string> Section_hotkeys;
        internal static Dictionary<string, string> Section_program;
        internal static Dictionary<string, string> Section_settings;
        internal static Dictionary<string, string> Section_toggleButtons;
        internal static Dictionary<string, string> Section_userWarpTargets;

        internal static bool isConsoleEnabled { get; set; }
        internal static bool isInfoBarEnabled { get; set; }
        internal static bool isFragmentTrackerEnabled { get; set; }
        internal static bool noIceWorm { get; set; }
        internal static bool noWeather { get; set; }
        internal static bool resistCold { get; set; }
        internal static bool filterFast { get; set; }

        private static readonly string[] SECTIONKEYS_Hotkeys = new string[3]
        {
            "ToggleWindow",
            "ToggleMouse",
            "ToggleConsole"
        };

        private static readonly string[] SECTIONKEYS_Program = new string[2]
        {
            "EnableConsole",
            "EnableInfoBar"
        };

        private static readonly string[] SECTIONKEYS_Settings = new string[3]
        {
            "OverPowerMultiplier",
            "HungerAndThirstInterval",
            "EnableFragmentTracker"
        };

        private static readonly string[] SECTIONKEYS_ToggleButtons = new string[13]
        {
            "fastbuild",
            "fastscan",
            "fastgrow",
            "fasthatch",
            "filterfast",
            "radiation",
            "invisible",
            "nodamage",
            "alwaysday",            
            "overpower",
            "resistcold",
            "noweather",
            "noiceworm"
        };
        
        private static readonly List<ConfigData> DEFAULT_CONFIG = new List<ConfigData>
        {
            new ConfigData(SECTIONS[0], SECTIONKEYS_Hotkeys[0], KeyCode.F5.ToString()),
            new ConfigData(SECTIONS[0], SECTIONKEYS_Hotkeys[1], KeyCode.F4.ToString()),
            new ConfigData(SECTIONS[0], SECTIONKEYS_Hotkeys[2], KeyCode.Delete.ToString()),

            new ConfigData(SECTIONS[1], SECTIONKEYS_Program[0], true.ToString()),
            new ConfigData(SECTIONS[1], SECTIONKEYS_Program[1], false.ToString()),

            new ConfigData(SECTIONS[2], SECTIONKEYS_Settings[0], 2.ToString()),
            new ConfigData(SECTIONS[2], SECTIONKEYS_Settings[1], 10.ToString()),
            new ConfigData(SECTIONS[2], SECTIONKEYS_Settings[2], false.ToString()),

            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[0], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[1], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[2], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[3], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[4], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[5], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[6], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[7], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[8], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[9], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[10], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[11], false.ToString()),
            new ConfigData(SECTIONS[3], SECTIONKEYS_ToggleButtons[12], false.ToString()),

            new ConfigData(SECTIONS[4], string.Empty, string.Empty)
        };

        internal static void Load()
        {
            PROGRAM_VERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            if (!Check())
            {
                CreateDefault();
            }

            Section_hotkeys = ParserHelper.GetAllKeyValuesFromSection(FILENAME, "Hotkeys", SECTIONKEYS_Hotkeys);
            Section_program = ParserHelper.GetAllKeyValuesFromSection(FILENAME, "Program", SECTIONKEYS_Program);
            Section_settings = ParserHelper.GetAllKeyValuesFromSection(FILENAME, "Settings", SECTIONKEYS_Settings);
            Section_toggleButtons = ParserHelper.GetAllKeyValuesFromSection(FILENAME, "ToggleButtons", SECTIONKEYS_ToggleButtons);
            Section_userWarpTargets = ParserHelper.GetAllKVPFromSection(FILENAME, "UserWarpTargets");
        }

        internal static void Save()
        {
            ParserHelper.SetAllKeyValuesInSection(FILENAME, "Hotkeys", Section_hotkeys);
            ParserHelper.SetAllKeyValuesInSection(FILENAME, "Program", Section_program);
            ParserHelper.SetAllKeyValuesInSection(FILENAME, "Settings", Section_settings);
            ParserHelper.SetAllKeyValuesInSection(FILENAME, "ToggleButtons", Section_toggleButtons);

            if (Main.Instance != null)
            {
                SyncWarpTargets();
            }
        }

        internal static void Set()
        {
            isConsoleEnabled = bool.Parse(Section_program["EnableConsole"]);
            isInfoBarEnabled = bool.Parse(Section_program["EnableInfoBar"]);
            isFragmentTrackerEnabled = bool.Parse(Section_settings["EnableFragmentTracker"]);

            noIceWorm = bool.Parse(CMZ_Config.Section_toggleButtons["noiceworm"]);
            noWeather = bool.Parse(CMZ_Config.Section_toggleButtons["noweather"]);
            resistCold = bool.Parse(CMZ_Config.Section_toggleButtons["resistcold"]);
            filterFast = bool.Parse(CMZ_Config.Section_toggleButtons["filterfast"]);

            int.TryParse(Section_settings["OverPowerMultiplier"], out overPowerMultiplier);

            if (overPowerMultiplier < 0 && overPowerMultiplier > 10)
            {
                overPowerMultiplier = 2;
                ParserHelper.SetKeyValue(FILENAME, "Settings", "OverPowerMultiplier", 2.ToString());
            }

            int.TryParse(Section_settings["HungerAndThirstInterval"], out hungerAndThirstInterval);

            if (hungerAndThirstInterval < 0 && hungerAndThirstInterval > 10)
            {
                hungerAndThirstInterval = 10;
                ParserHelper.SetKeyValue(FILENAME, "Settings", "HungerAndThirstInterval", 1.ToString());
            }

            SetKeyBindings();
        }

        internal static void SyncWarpTargets()
        {
            Dictionary<IntVector3, string> warpTargets_User = Main.Instance.WarpTargets_User;

            Section_userWarpTargets.Clear();

            ParserHelper.ClearSection(FILENAME, "UserWarpTargets");

            if (warpTargets_User.Count > 0)
            {
                foreach (KeyValuePair<IntVector3, string> kvp in warpTargets_User)
                {
                    Section_userWarpTargets.Add(kvp.Key.ToString(), kvp.Value);
                }
            }

            ParserHelper.SetAllKeyValuesInSection(FILENAME, "UserWarpTargets", Section_userWarpTargets);
        }

        internal static void SyncConfig()
        {
            foreach (string key in SECTIONKEYS_Hotkeys)
            {
                Section_hotkeys[key] = KEYBINDINGS[key].ToString();
            }
        }

        internal static void SetKeyBindings()
        {
            KEYBINDINGS = new Dictionary<string, KeyCode>();

            bool sync = false;

            foreach (KeyValuePair<string, string> kvp in Section_hotkeys)
            {
                try
                {
                    KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), kvp.Value);
                    KEYBINDINGS.Add(kvp.Key, keyCode);
                }
                catch (ArgumentException)
                {
                    BZLogger.Warn($"({kvp.Value}) is not a valid KeyCode! Setting default value!");

                    for (int i = 0; i < DEFAULT_CONFIG.Count; i++)
                    {
                        if (DEFAULT_CONFIG[i].Key.Equals(kvp.Key))
                        {
                            KEYBINDINGS.Add(kvp.Key, (KeyCode)Enum.Parse(typeof(KeyCode), DEFAULT_CONFIG[i].Value, true));
                            sync = true;
                        }
                    }
                }
            }

            if (sync)
                SyncConfig();
        }

        internal static void CreateDefault()
        {
            BZLogger.Warn("Configuration file is missing or wrong version. Trying to create a new one.");

            try
            {
                ParserHelper.CreateDefaultConfigFile(FILENAME, "CheatManager", PROGRAM_VERSION, DEFAULT_CONFIG);

                ParserHelper.AddInfoText(FILENAME, "OverPowerMultiplier and HungerAndThirstInterval possible values", "1 to 10");

                BZLogger.Log("The new configuration file was successfully created.");
            }
            catch
            {
                BZLogger.Error("An error occured while creating the new configuration file!");
            }
        }

        private static bool Check()
        {
            if (!File.Exists(FILENAME))
            {
                BZLogger.Error("Configuration file open error!");
                return false;
            }

            CONFIG_VERSION = ParserHelper.GetKeyValue(FILENAME, "CheatManager", "Version");

            if (!CONFIG_VERSION.Equals(PROGRAM_VERSION))
            {
                BZLogger.Error("Configuration file version error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "Hotkeys", SECTIONKEYS_Hotkeys))
            {
                BZLogger.Error("Configuration file [Hotkeys] section error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "Program", SECTIONKEYS_Program))
            {
                BZLogger.Error("Configuration file [Program] section error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "Settings", SECTIONKEYS_Settings))
            {
                BZLogger.Error("Configuration file [Settings] section error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "ToggleButtons", SECTIONKEYS_ToggleButtons))
            {
                BZLogger.Error("Configuration file [ToggleButtons] section error!");
                return false;
            }

            return true;
        }
    }
}
