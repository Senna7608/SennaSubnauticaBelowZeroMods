using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using BZCommon;
using BZCommon.ConfigurationParser;

namespace SlotExtenderZero.Configuration
{
    public static class SEzConfig
    {
        public static string PROGRAM_VERSION = string.Empty;
        public static string CONFIG_VERSION = string.Empty;

        private static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string FILENAME = $"{modFolder}/config.txt";

        public static Dictionary<string, string> Hotkeys_Config;
        public static Dictionary<SlotConfigID, string> SLOTKEYBINDINGS = new Dictionary<SlotConfigID, string>();
        public static Dictionary<string, KeyCode> KEYBINDINGS;

        public static List<string> SLOTKEYSLIST = new List<string>();

        public static int MAXSLOTS;
        public static int EXTRASLOTS;
        public static Color TEXTCOLOR;
        public static int STORAGE_SLOTS_OFFSET = 4;
        public static SlotLayout SLOT_LAYOUT = SlotLayout.Grid;
        public static bool isSeatruckArmsExists = false;

        private static readonly string[] SECTION_HOTKEYS =
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

        private static readonly string[] SECTION_SETTINGS =
        {
            "MaxSlots",
            "TextColor",
            "SlotLayout"
        };

        private static readonly List<ConfigData> DEFAULT_CONFIG = new List<ConfigData>
        {
            new ConfigData("Settings", SECTION_SETTINGS[0], 12.ToString()),
            new ConfigData("Settings", SECTION_SETTINGS[1], COLORS.Green.ToString()),
            new ConfigData("Settings", SECTION_SETTINGS[2], SlotLayout.Circle.ToString()),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[0], InputHelper.GetKeyCodeAsInputName(KeyCode.T)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[1], InputHelper.GetKeyCodeAsInputName(KeyCode.R)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[2], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha6)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[3], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha7)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[4], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha8)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[5], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha9)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[6], InputHelper.GetKeyCodeAsInputName(KeyCode.Alpha0)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[7], InputHelper.GetKeyCodeAsInputName(KeyCode.Slash)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[8], InputHelper.GetKeyCodeAsInputName(KeyCode.Equals)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[9], InputHelper.GetKeyCodeAsInputName(KeyCode.O)),
            new ConfigData("Hotkeys", SECTION_HOTKEYS[10], InputHelper.GetKeyCodeAsInputName(KeyCode.P))
        };

        internal static void SLOTKEYBINDINGS_Update()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEzConfig.SLOTKEYBINDINGS_Update()");

            SLOTKEYBINDINGS.Clear();
            SLOTKEYSLIST.Clear();

            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_1, GameInput.GetBindingName(GameInput.Button.Slot1, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_2, GameInput.GetBindingName(GameInput.Button.Slot2, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_3, GameInput.GetBindingName(GameInput.Button.Slot3, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_4, GameInput.GetBindingName(GameInput.Button.Slot4, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_5, GameInput.GetBindingName(GameInput.Button.Slot5, GameInput.BindingSet.Primary));
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_6, Hotkeys_Config[SlotConfigID.Slot_6.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_7, Hotkeys_Config[SlotConfigID.Slot_7.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_8, Hotkeys_Config[SlotConfigID.Slot_8.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_9, Hotkeys_Config[SlotConfigID.Slot_9.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_10, Hotkeys_Config[SlotConfigID.Slot_10.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_11, Hotkeys_Config[SlotConfigID.Slot_11.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.Slot_12, Hotkeys_Config[SlotConfigID.Slot_12.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.SeaTruckArmLeft, Hotkeys_Config[SlotConfigID.SeaTruckArmLeft.ToString()]);
            SLOTKEYBINDINGS.Add(SlotConfigID.SeaTruckArmRight, Hotkeys_Config[SlotConfigID.SeaTruckArmRight.ToString()]);

            foreach (KeyValuePair<SlotConfigID, string> kvp in SLOTKEYBINDINGS)
            {
                SLOTKEYSLIST.Add(kvp.Value);
            }

        }

        internal static void Config_Load()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEzConfig.Config_Load()");

            PROGRAM_VERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            if (!Config_Check())
            {
                Config_CreateDefault();
            }

            try
            {
                Hotkeys_Config = ParserHelper.GetAllKeyValuesFromSection(FILENAME, "Hotkeys", SECTION_HOTKEYS);

                int.TryParse(ParserHelper.GetKeyValue(FILENAME, "Settings", SECTION_SETTINGS[0]), out int result);
                MAXSLOTS = result < 5 || result > 12 ? 12 : result;

                EXTRASLOTS = MAXSLOTS - 4;

                TEXTCOLOR = Modules.GetColor(ParserHelper.GetKeyValue(FILENAME, "Settings", SECTION_SETTINGS[1]));

                SLOT_LAYOUT = ParserHelper.GetKeyValue(FILENAME, "Settings", SECTION_SETTINGS[2]) == "Circle" ? SlotLayout.Circle : SlotLayout.Grid;

                isSeatruckArmsExists = ReflectionHelper.IsNamespaceExists("SeaTruckArms");

                BZLogger.Log("SlotExtenderZero", "Configuration loaded.");
            }
            catch
            {
                BZLogger.Error("SlotExtenderZero", "An error occurred while loading the configuration file!");
            }
        }

        internal static void Config_CreateDefault()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEzConfig.Config_CreateDefault()");

            BZLogger.Warn("SlotExtenderZero", "Configuration file is missing or wrong version. Trying to create a new one.");

            try
            {
                ParserHelper.CreateDefaultConfigFile(FILENAME, "SlotExtenderZero", PROGRAM_VERSION, DEFAULT_CONFIG);

                ParserHelper.AddInfoText(FILENAME, "MaxSlots possible values", "5 to 12");
                ParserHelper.AddInfoText(FILENAME, "TextColor possible values", "Red, Green, Blue, Yellow, White, Magenta, Cyan, Orange, Lime, Amethyst, Default");
                ParserHelper.AddInfoText(FILENAME, "SlotLayout possible values", "Grid, Circle");

                BZLogger.Log("SlotExtenderZero", "The new configuration file was successfully created.");
            }
            catch
            {
                BZLogger.Error("SlotExtenderZero", "An error occured while creating the new configuration file!");
            }
        }

        internal static void Config_Init()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEzConfig.Config_Init()");

            SLOTKEYBINDINGS_Update();

            KEYBINDINGS_Set();

            SLOTKEYBINDINGS_SyncToAll();

            BZLogger.Log("SlotExtenderZero", "Configuration initialized.");
        }

        internal static void Config_Write()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEzConfig.WriteConfig()");

            ParserHelper.SetAllKeyValuesInSection(FILENAME, "Hotkeys", Hotkeys_Config);
            ParserHelper.SetKeyValue(FILENAME, "Settings", SECTION_SETTINGS[0], MAXSLOTS.ToString());
            ParserHelper.SetKeyValue(FILENAME, "Settings", SECTION_SETTINGS[1], Modules.GetColorName(TEXTCOLOR));
            ParserHelper.SetKeyValue(FILENAME, "Settings", SECTION_SETTINGS[2], SLOT_LAYOUT.ToString());

            BZLogger.Log("SlotExtenderZero", "Configuration saved.");
        }

        internal static void KEYBINDINGS_ToConfig()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEConfig.KEYBINDINGS_ToConfig()");

            foreach (string key in SECTION_HOTKEYS)
            {
                Hotkeys_Config[key] = InputHelper.GetKeyCodeAsInputName(KEYBINDINGS[key]);
            }

            Config_Write();
        }

        internal static void SLOTKEYBINDINGS_SyncToAll()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEConfig.SLOTKEYBINDINGS_SyncToAll()");

            foreach (KeyValuePair<SlotConfigID, string> kvp in SLOTKEYBINDINGS)
            {
                BZLogger.Debug("SlotExtenderZero", $"key: {kvp.Key.ToString()}, Value: {kvp.Value}");

                string key = kvp.Key.ToString();

                if (Hotkeys_Config.ContainsKey(key))
                    Hotkeys_Config[key] = kvp.Value;

                KEYBINDINGS[key] = InputHelper.GetInputNameAsKeyCode(kvp.Value);
            }

            Config_Write();
        }


        internal static void KEYBINDINGS_Set()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEConfig.KEYBINDINGS_Set()");

            KEYBINDINGS = new Dictionary<string, KeyCode>();

            bool sync = false;

            foreach (KeyValuePair<string, string> kvp in Hotkeys_Config)
            {
                try
                {
                    KEYBINDINGS.Add(kvp.Key, InputHelper.GetInputNameAsKeyCode(kvp.Value));
                }
                catch (ArgumentException)
                {
                    BZLogger.Warn("SlotExtenderZero", $"[{kvp.Value}] is not a valid KeyCode! Setting default value!");

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
                KEYBINDINGS_ToConfig();
            }
        }

        private static bool Config_Check()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SEzConfig.Config_Check()");

            if (!File.Exists(FILENAME))
            {
                BZLogger.Error("SlotExtenderZero", "Configuration file open error!");
                return false;
            }

            CONFIG_VERSION = ParserHelper.GetKeyValue(FILENAME, "SlotExtenderZero", "Version");

            if (!CONFIG_VERSION.Equals(PROGRAM_VERSION))
            {
                BZLogger.Error("SlotExtenderZero", "Configuration file version error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "Hotkeys", SECTION_HOTKEYS))
            {
                BZLogger.Error("SlotExtenderZero", "Configuration file [Hotkeys] section error!");
                return false;
            }

            if (!ParserHelper.CheckSectionKeys(FILENAME, "Settings", SECTION_SETTINGS))
            {
                BZLogger.Error("SlotExtenderZero", "Configuration file [Settings] section error!");
                return false;
            }

            return true;
        }
    }
}
