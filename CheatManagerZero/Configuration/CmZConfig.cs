﻿using System;
using System.Collections.Generic;
using UnityEngine;
using BZCommon;
using BZCommon.ConfigurationParser;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace CheatManagerZero.Configuration
{
    public static class CmZConfig
    {
        internal static string VERSION = string.Empty;        
        internal const string PROGRAM_NAME = "CheatManagerZero";
        private static readonly string FILENAME = $"{Environment.CurrentDirectory}/QMods/{PROGRAM_NAME}/config.txt";
        internal static int overPowerMultiplier;
        internal const float ASPECT = 4.8f;
        internal static int hungerAndThirstInterval;
        private static readonly string[] SECTIONS = { "Hotkeys", "Program", "Settings", "ToggleButtons", "UserWarpTargets" };
        internal static Dictionary<string, KeyCode> KEYBINDINGS;
        internal static Dictionary<string, string> Section_hotkeys;        
        internal static Dictionary<string, string> Section_settings;
        internal static Dictionary<string, string> Section_toggleButtons;
        public static Dictionary<string, string> Section_userWarpTargets;
        internal static bool isConsoleEnabled;
        internal static bool isInfoBarEnabled;

        private static readonly string[] SECTION_HOTKEYS =
        {
            "ToggleWindow",
            "ToggleMouse",
            "ToggleConsole"
        };

        private static readonly string[] SECTION_PROGRAM =
        {
            "EnableConsole",
            "EnableInfoBar"
        };

        private static readonly string[] SECTION_SETTINGS =
        {
            "OverPowerMultiplier",
            "HungerAndThirstInterval"
        };

        private static readonly string[] SECTION_TOGGLEBUTTONS =
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
            "resistcold"
        };
        
        private static readonly List<ConfigData> DEFAULT_CONFIG = new List<ConfigData>
        {
            new ConfigData(SECTIONS[0], SECTION_HOTKEYS[0], KeyCode.F5.ToString()),
            new ConfigData(SECTIONS[0], SECTION_HOTKEYS[1], KeyCode.F4.ToString()),
            new ConfigData(SECTIONS[0], SECTION_HOTKEYS[2], KeyCode.Delete.ToString()),

            new ConfigData(SECTIONS[1], SECTION_PROGRAM[0], true.ToString()),
            new ConfigData(SECTIONS[1], SECTION_PROGRAM[1], false.ToString()),

            new ConfigData(SECTIONS[2], SECTION_SETTINGS[0], 2.ToString()),
            new ConfigData(SECTIONS[2], SECTION_SETTINGS[1], 1.ToString()),

            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[0], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[1], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[2], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[3], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[4], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[5], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[6], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[7], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[8], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[9], false.ToString()),
            new ConfigData(SECTIONS[3], SECTION_TOGGLEBUTTONS[10], false.ToString()),

            new ConfigData(SECTIONS[4], string.Empty, string.Empty)
        };        

        internal static void InitConfig()
        {
            VERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;            

            if (!File.Exists(FILENAME))
            {
                BZLogger.Log($"[{PROGRAM_NAME}]Warning! Configuration file is missing. Creating a new one.");                
                
                ParserHelper.CreateDefaultConfigFile(FILENAME, PROGRAM_NAME, VERSION, DEFAULT_CONFIG);
                ParserHelper.AddInfoText(FILENAME, SECTIONS[2], "OverPowerMultiplier and HungerAndThirstInterval possible values: 1 to 10");
            }

            isConsoleEnabled = bool.Parse(ParserHelper.GetKeyValue(FILENAME, SECTIONS[1], SECTION_PROGRAM[0]));
            isInfoBarEnabled = bool.Parse(ParserHelper.GetKeyValue(FILENAME, SECTIONS[1], SECTION_PROGRAM[1]));

            Section_hotkeys = ParserHelper.GetAllKeyValuesFromSection(FILENAME, SECTIONS[0], SECTION_HOTKEYS);            
            Section_settings = ParserHelper.GetAllKeyValuesFromSection(FILENAME, SECTIONS[2], SECTION_SETTINGS);
            Section_toggleButtons = ParserHelper.GetAllKeyValuesFromSection(FILENAME, SECTIONS[3], SECTION_TOGGLEBUTTONS);
            Section_userWarpTargets = ParserHelper.GetAllKVPFromSection(FILENAME, SECTIONS[4]);

            int.TryParse(Section_settings[SECTION_SETTINGS[0]], out overPowerMultiplier);
            
            if (overPowerMultiplier < 0 && overPowerMultiplier > 10)
            {
                overPowerMultiplier = 10;
                ParserHelper.SetKeyValue(FILENAME, SECTIONS[2], Section_settings[SECTION_SETTINGS[0]], 2.ToString());
            }

            int.TryParse(Section_settings[SECTION_SETTINGS[1]], out hungerAndThirstInterval);

            if (hungerAndThirstInterval < 0 && hungerAndThirstInterval > 10)
            {
                hungerAndThirstInterval = 10;
                ParserHelper.SetKeyValue(FILENAME, SECTIONS[2], Section_settings[SECTION_SETTINGS[1]], 2.ToString());
            }

            SetKeyBindings();            
        }
        
        internal static void WriteConfig()
        {
            ParserHelper.SetAllKeyValuesInSection(FILENAME, SECTIONS[0], Section_hotkeys);
            ParserHelper.SetAllKeyValuesInSection(FILENAME, SECTIONS[2], Section_settings);

            if (Main.Instance != null)
            {
                SyncWarpTargets();
            }

           
        }

        internal static void SyncWarpTargets()
        {
            Dictionary<IntVector, string> warpTargets_User = Main.Instance.WarpTargets_User;

            Section_userWarpTargets.Clear();

            ParserHelper.ClearSection(FILENAME, SECTIONS[4]);

            if (warpTargets_User.Count > 0)
            {
                foreach (KeyValuePair<IntVector, string> kvp in warpTargets_User)
                {
                    //UnityEngine.Debug.Log($"key: {kvp.Key}, value: {kvp.Value}");

                    Section_userWarpTargets.Add(kvp.Key.ToString(), kvp.Value);
                }                
            }

            ParserHelper.SetAllKeyValuesInSection(FILENAME, SECTIONS[4], Section_userWarpTargets);
        }



        internal static void SyncConfig()
        {
            foreach (string key in SECTION_HOTKEYS)
            {
                Section_hotkeys[key] = KEYBINDINGS[key].ToString();
            }

            WriteConfig();
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
                    BZLogger.Log($"[{PROGRAM_NAME}]Warning! ({kvp.Value}) is not a valid KeyCode! Setting default value!");

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
    }
}
