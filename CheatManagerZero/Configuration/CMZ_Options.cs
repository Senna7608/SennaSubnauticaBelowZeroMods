using System;
using UnityEngine;
using Nautilus.Options;

namespace CheatManagerZero.Configuration
{
    public class CMZ_Options : ModOptions
    {
        public CMZ_Options() : base("Cheat Manager Zero settings")
        {
            OnChanged += GlobalOptions_Changed;

            AddItem(ModKeybindOption.Create("ToggleWindow", "Toggle window", GameInput.Device.Keyboard, (KeyCode)Enum.Parse(typeof(KeyCode), CMZ_Config.Section_hotkeys["ToggleWindow"])));
            AddItem(ModKeybindOption.Create("ToggleMouse", "Toggle mouse pointer", GameInput.Device.Keyboard, (KeyCode)Enum.Parse(typeof(KeyCode), CMZ_Config.Section_hotkeys["ToggleMouse"])));
            AddItem(ModKeybindOption.Create("ToggleConsole", "Toggle log window", GameInput.Device.Keyboard, (KeyCode)Enum.Parse(typeof(KeyCode), CMZ_Config.Section_hotkeys["ToggleConsole"])));

            AddItem(ModToggleOption.Create("EnableConsole", "Enable log window in the lower left corner", bool.Parse(CMZ_Config.Section_program["EnableConsole"])));
            AddItem(ModToggleOption.Create("EnableInfoBar", "Enable info bar on top of screen", bool.Parse(CMZ_Config.Section_program["EnableInfoBar"])));
            AddItem(ModToggleOption.Create("EnableFragmentTracker", "Enable fragment tracker", bool.Parse(CMZ_Config.Section_settings["EnableFragmentTracker"])));

            AddItem(ModToggleOption.Create("fastbuild", "Fast build", bool.Parse(CMZ_Config.Section_toggleButtons["fastbuild"])));
            AddItem(ModToggleOption.Create("fastscan", "Fast scan", bool.Parse(CMZ_Config.Section_toggleButtons["fastscan"])));
            AddItem(ModToggleOption.Create("fastgrow", "Fast grow", bool.Parse(CMZ_Config.Section_toggleButtons["fastgrow"])));
            AddItem(ModToggleOption.Create("fasthatch", "Fast hatch", bool.Parse(CMZ_Config.Section_toggleButtons["fasthatch"])));
            AddItem(ModToggleOption.Create("filterfast", "Filter fast", bool.Parse(CMZ_Config.Section_toggleButtons["filterfast"])));
            AddItem(ModToggleOption.Create("invisible", "Invisible", bool.Parse(CMZ_Config.Section_toggleButtons["invisible"])));
            AddItem(ModToggleOption.Create("nodamage", "No damage", bool.Parse(CMZ_Config.Section_toggleButtons["nodamage"])));
            AddItem(ModToggleOption.Create("alwaysday", "Always day", bool.Parse(CMZ_Config.Section_toggleButtons["alwaysday"])));
            AddItem(ModToggleOption.Create("overpower", "Overpower (Health and Oxygen)", bool.Parse(CMZ_Config.Section_toggleButtons["overpower"])));
            AddItem(ModToggleOption.Create("resistcold", "Resist cold", CMZ_Config.resistCold));
            AddItem(ModToggleOption.Create("noweather", "No weather", CMZ_Config.noWeather));
            AddItem(ModToggleOption.Create("noiceworm", "No iceworm", CMZ_Config.noIceWorm));

            AddItem(ModSliderOption.Create("OverPowerMultiplier", "Overpower multiplier", 2f, 10f, float.Parse(CMZ_Config.Section_settings["OverPowerMultiplier"]), 2f, "{0:F0}", 1f));
            AddItem(ModSliderOption.Create("HungerAndThirstInterval", "Hunger and thirst interval", 1f, 10f, float.Parse(CMZ_Config.Section_settings["HungerAndThirstInterval"]), 10f, "{0:F0}", 1f));
        }

        private void GlobalOptions_Changed(object sender, OptionEventArgs eventArgs)
        {
            switch (eventArgs)
            {
                case KeybindChangedEventArgs keybindArgs:

                    switch (keybindArgs.Id)
                    {
                        case "ToggleWindow":
                            CMZ_Config.Section_hotkeys["ToggleWindow"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "ToggleMouse":
                            CMZ_Config.Section_hotkeys["ToggleMouse"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "ToggleConsole":
                            CMZ_Config.Section_hotkeys["ToggleConsole"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;
                    }
                    break;

                case ToggleChangedEventArgs toggleArgs:

                    switch (toggleArgs.Id)
                    {
                        case "EnableConsole":
                            CMZ_Config.Section_program["EnableConsole"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "EnableInfoBar":
                            CMZ_Config.Section_program["EnableInfoBar"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "EnableFragmentTracker":
                            CMZ_Config.Section_settings["EnableFragmentTracker"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            CheatManagerZero.OnFragmentTrackerChanged();
                            break;

                        case "fastbuild":
                            CMZ_Config.Section_toggleButtons["fastbuild"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.fastbuild);
                            break;

                        case "fastscan":
                            CMZ_Config.Section_toggleButtons["fastscan"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.fastscan);
                            break;

                        case "fastgrow":
                            CMZ_Config.Section_toggleButtons["fastgrow"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.fastgrow);
                            break;

                        case "fasthatch":
                            CMZ_Config.Section_toggleButtons["fasthatch"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.fasthatch);
                            break;

                        case "filterfast":
                            CMZ_Config.Section_toggleButtons["filterfast"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.filterfast);
                            break;

                        case "invisible":
                            CMZ_Config.Section_toggleButtons["invisible"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.invisible);
                            break;

                        case "nodamage":
                            CMZ_Config.Section_toggleButtons["nodamage"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.nodamage);
                            break;

                        case "alwaysday":
                            CMZ_Config.Section_toggleButtons["alwaysday"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.alwaysday);
                            break;

                        case "overpower":
                            CMZ_Config.Section_toggleButtons["overpower"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.overpower);
                            break;

                        case "resistcold":
                            CMZ_Config.Section_toggleButtons["resistcold"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.resistcold);
                            break;

                        case "noweather":
                            CMZ_Config.Section_toggleButtons["noweather"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleWeatherCheat();
                            break;

                        case "noiceworm":
                            CMZ_Config.Section_toggleButtons["noiceworm"] = toggleArgs.Value.ToString();
                            SyncConfig();
                            Main.Instance?.ToggleButtonControl(ToggleCommands.noiceworm);
                            break;
                    }
                    break;

                case SliderChangedEventArgs sliderArgs:

                    switch (sliderArgs.Id)
                    {
                        case "OverPowerMultiplier":
                            CMZ_Config.Section_settings["OverPowerMultiplier"] = sliderArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "HungerAndThirstInterval":
                            CMZ_Config.Section_settings["HungerAndThirstInterval"] = sliderArgs.Value.ToString();
                            SyncConfig();
                            break;
                    }
                    break;
            }
        }

        private void SyncConfig()
        {
            CMZ_Config.Set();
            CMZ_Config.Save();            
        }
    }
}
