using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using BZHelper.ConfigurationParser;
using BZHelper;
using SlotExtenderZero.API;

namespace SlotExtenderZero.Configuration
{
    internal static class SEzConfig
    {
        public static string PROGRAM_VERSION = string.Empty;
        public static string CONFIG_VERSION = string.Empty;

        private static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string FILENAME = $"{modFolder}/config.txt";

        public static Dictionary<string, string> Section_Hotkeys;
        public static Dictionary<string, string> Section_Settings;
        public static Dictionary<SlotConfigID, string> SLOTKEYBINDINGS = new Dictionary<SlotConfigID, string>();
        public static Dictionary<string, KeyCode> KEYBINDINGS;

        public static List<string> SLOTKEYSLIST = new List<string>();

        public static int MAXSLOTS;
        public static int EXTRASLOTS;
        public static Color TEXTCOLOR;
        public static int STORAGE_SLOTS_OFFSET = 4;
        public static SlotLayout SLOT_LAYOUT = SlotLayout.Circle;
        public static bool isSeatruckArmsExists = false;
        public static bool isSeatruckScannerModuleExists = false;

        private static readonly string[] SECTIONKEYS_HOTKEYS =
        {
            "Upgrade",
            "Storage",
            SlotConfigID.Slot_6.ToString(),
            SlotConfigID.Slot_7.ToString(),
            SlotConfigID.Slot_8.ToString(),
            SlotConfigID.Slot_9.ToString(),
            SlotConfigID.Slot_10.ToString(),
            SlotConfigID.Slot_11.ToString(),
            SlotConfigID.Slot_12.ToString(),
            SlotConfigID.SeaTruckArmLeft.ToString(),
            SlotConfigID.SeaTruckArmRight.ToString()
        };

        private static readonly string[] SECTIONKEYS_SETTINGS =
        {
            "MaxSlots",
            "TextColor",
            "SlotLayout"
        };

        private static readonly List<ConfigData> DEFAULT_CONFIG = new List<ConfigData>
        {
            new ConfigData("Settings", SECTIONKEYS_SETTINGS[0], 12.ToString()),
            new ConfigData("Settings", SECTIONKEYS_SETTINGS[1], COLORS.Green.ToString()),
            new ConfigData("Settings", SECTIONKEYS_SETTINGS[2], SlotLayout.Circle.ToString()),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[0], InputHelper.GetKeyCodeAsInputName(KeyCode.T)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[1], InputHelper.GetKeyCodeAsInputName(KeyCode.R)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[2], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha6)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[3], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha7)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[4], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha8)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[5], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha9)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[6], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha0)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[7], InputHelper.GetKeyCodeAsInputName(KeyCode.Slash)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[8], InputHelper.GetKeyCodeAsInputName(KeyCode.Equals)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[9], InputHelper.GetKeyCodeAsInputName(KeyCode.O)),
            new ConfigData("Hotkeys", SECTIONKEYS_HOTKEYS[10],InputHelper.GetKeyCodeAsInputName(KeyCode.P))
        };

        internal static void SLOTKEYBINDINGS_Update()
        {
            BZLogger.Trace("SEzConfig.SLOTKEYBINDINGS_Update()");

            SLOTKEYBINDINGS.Clear();
            SLOTKEYSLIST.Clear();

            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_1, GameInput.GetBindingName(GameInput.Button.Slot1, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_2, GameInput.GetBindingName(GameInput.Button.Slot2, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_3, GameInput.GetBindingName(GameInput.Button.Slot3, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_4, GameInput.GetBindingName(GameInput.Button.Slot4, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_5, GameInput.GetBindingName(GameInput.Button.Slot5, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_6, Section_Hotkeys[SlotConfigID.Slot_6.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_7, Section_Hotkeys[SlotConfigID.Slot_7.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_8, Section_Hotkeys[SlotConfigID.Slot_8.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_9, Section_Hotkeys[SlotConfigID.Slot_9.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_10, Section_Hotkeys[SlotConfigID.Slot_10.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_11, Section_Hotkeys[SlotConfigID.Slot_11.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_12, Section_Hotkeys[SlotConfigID.Slot_12.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.SeaTruckArmLeft, Section_Hotkeys[SlotConfigID.SeaTruckArmLeft.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.SeaTruckArmRight, Section_Hotkeys[SlotConfigID.SeaTruckArmRight.ToString()]);

            foreach (KeyValuePair<SlotConfigID, string> kvp in SLOTKEYBINDINGS)
            {
                SLOTKEYSLIST.Add(kvp.Value);
            }

        }

        internal static void Load()
        {
            BZLogger.Trace("SEzConfig.Load()");

            PROGRAM_VERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            if (!Check())
            {
                CreateDefault();
            }

            try
            {
                Section_Settings = ParserHelper.GetAllKeyValuesFromSection(FILENAME, "Settings", SECTIONKEYS_SETTINGS);
                Section_Hotkeys = ParserHelper.GetAllKeyValuesFromSection(FILENAME, "Hotkeys", SECTIONKEYS_HOTKEYS);

                int.TryParse(Section_Settings["MaxSlots"], out int maxslots);
                MAXSLOTS = maxslots < 5 || maxslots > 12 ? 12 : maxslots;
                EXTRASLOTS = SEzConfig.MAXSLOTS - 4;

                TEXTCOLOR = ColorHelper.GetColor(Section_Settings["TextColor"]);

                SLOT_LAYOUT = Section_Settings["SlotLayout"] == "Circle" ? SlotLayout.Circle : SlotLayout.Grid;

                BZLogger.Log("Configuration loaded.");
            }
            catch
            {
                BZLogger.Error("An error occurred while loading the configuration file!");
            }
        }

        internal static void CreateDefault()
        {
            BZLogger.Trace("SEzConfig.CreateDefault()");

            BZLogger.Warn("Configuration file is missing or wrong version. Trying to create a new one.");

            try
            {
                ParserHelper.CreateDefaultConfigFile(FILENAME, "SlotExtenderZero", PROGRAM_VERSION, DEFAULT_CONFIG);

                ParserHelper.AddInfoText(FILENAME, "MaxSlots possible values", "5 to 12");
                ParserHelper.AddInfoText(FILENAME, "TextColor possible values", "Red, Green, Blue, Yellow, White, Magenta, Cyan, Orange, Lime, Amethyst");
                ParserHelper.AddInfoText(FILENAME, "SlotLayout possible values", "Grid, Circle");

                BZLogger.Log("The new configuration file was successfully created.");
            }
            catch
            {
                BZLogger.Error("An error occured while creating the new configuration file!");
            }
        }

        internal static void Init()
        {
            BZLogger.Trace("SEzConfig.Init()");

            SLOTKEYBINDINGS_Update();

            SetKeyBindings();

            SyncKeyBindings();

            BZLogger.Log("Configuration initialized.");
        }

        internal static void Save()
        {
            BZLogger.Trace("SEzConfig.Save()");
            ParserHelper.SetAllKeyValuesInSection(FILENAME, "Hotkeys", Section_Hotkeys);
            ParserHelper.SetAllKeyValuesInSection(FILENAME, "Settings", Section_Settings);
            
            BZLogger.Log("Configuration saved.");
        }

        internal static void SetKeyBindingsToConfig()
        {
            BZLogger.Trace("SEConfig.SetKeyBindingsToConfig()");

            foreach (string key in SECTIONKEYS_HOTKEYS)
            {
                Section_Hotkeys[key] = InputHelper.GetKeyCodeAsInputName(KEYBINDINGS[key]);
            }

            Save();
        }

        internal static void SyncKeyBindings()
        {
            BZLogger.Trace("SEConfig.SyncKeyBindings()");

            foreach (KeyValuePair<SlotConfigID, string> kvp in SLOTKEYBINDINGS)
            {
                BZLogger.Debug($"key: {kvp.Key.ToString()}, Value: {kvp.Value}");

                string key = kvp.Key.ToString();

                if (Section_Hotkeys.ContainsKey(key))
                    Section_Hotkeys[key] = kvp.Value;

                KEYBINDINGS[key] = InputHelper.GetInputNameAsKeyCode(kvp.Value);
            }

            Save();
        }


        internal static void SetKeyBindings()
        {
            BZLogger.Trace("SEConfig.SetKeyBindings()");

            KEYBINDINGS = new Dictionary<string, KeyCode>();

            bool sync = false;

            foreach (KeyValuePair<string, string> kvp in Section_Hotkeys)
            {
                try
                {
                    KEYBINDINGS.Add(kvp.Key, InputHelper.GetInputNameAsKeyCode(kvp.Value));
                }
                catch (ArgumentException)
                {
                    BZLogger.Warn($"[{kvp.Value}] is not a valid KeyCode! Setting default value!");

                    for (int i = 0; i < DEFAULT_CONFIG.Count; i++)
                    {
                        if (DEFAULT_CONFIG[i].Key.Equals(kvp.Key))
                        {
                            KEYBINDINGS.Add(kvp.Key, InputHelper.GetInputNameAsKeyCode(DEFAULT_CONFIG[i].Value));
                            sync = true;
                        }
                    }
                }
            }

            if (sync)
            {
                SetKeyBindingsToConfig();
            }
        }

        private static bool Check()
        {
            BZLogger.Trace("SEzConfig.Check()");

            if (!File.Exists(FILENAME))
            {
                BZLogger.Error("Configuration file open error!");
                return false;
            }

            CONFIG_VERSION = ParserHelper.GetKeyValue(FILENAME, "SlotExtenderZero", "Version");

            if (!CONFIG_VERSION.Equals(PROGRAM_VERSION))
            {
                BZLogger.Error("Configuration file version error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "Hotkeys", SECTIONKEYS_HOTKEYS))
            {
                BZLogger.Error("Configuration file [Hotkeys] section error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "Settings", SECTIONKEYS_SETTINGS))
            {
                BZLogger.Error("Configuration file [Settings] section error!");
                return false;
            }

            return true;
        }
    }
}
