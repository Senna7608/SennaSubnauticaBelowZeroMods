using BZHelper;
using Nautilus.Options;

namespace SlotExtenderZero.Configuration
{
    public class SEzOptions : ModOptions
    {
        public SEzOptions() : base("Slot Extender Zero settings (Requires restart)")
        {
            OnChanged += GlobalOptions_Changed;

            AddItem(ModKeybindOption.Create("Upgrade", "Access to upgrades from inside", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Upgrade"])));
            AddItem(ModKeybindOption.Create("Storage", "Access to storage from inside", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Storage"])));
            AddItem(ModKeybindOption.Create("Slot_6", "Slot 6", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Slot_6"])));
            AddItem(ModKeybindOption.Create("Slot_7", "Slot 7", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Slot_7"])));
            AddItem(ModKeybindOption.Create("Slot_8", "Slot 8", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Slot_8"])));
            AddItem(ModKeybindOption.Create("Slot_9", "Slot 9", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Slot_9"])));
            AddItem(ModKeybindOption.Create("Slot_10", "Slot 10", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Slot_10"])));
            AddItem(ModKeybindOption.Create("Slot_11", "Slot 11", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Slot_11"])));
            AddItem(ModKeybindOption.Create("Slot_12", "Slot 12", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["Slot_12"])));
            AddItem(ModKeybindOption.Create("SeaTruckArmLeft", "Left Seatruck arm", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["SeaTruckArmLeft"])));
            AddItem(ModKeybindOption.Create("SeaTruckArmRight", "Right Seatruck arm", GameInput.Device.Keyboard, InputHelper.GetInputNameAsKeyCode(SEzConfig.Section_Hotkeys["SeaTruckArmRight"])));

            AddItem(ModSliderOption.Create("MaxSlots", "Maximum number of slots", 5f, 12f, float.Parse(SEzConfig.Section_Settings["MaxSlots"]), 12f, "{0:F0}", 1f));
            AddItem(ModChoiceOption<string>.Create("TextColor", "Textcolor", ColorHelper.ColorNames, ColorHelper.GetColorInt(SEzConfig.Section_Settings["TextColor"])));
            AddItem(ModChoiceOption<string>.Create("SlotLayout", "Slot layout", new string[] { "Grid", "Circle" }, (int)SEzConfig.SLOT_LAYOUT));            
        }

        private void GlobalOptions_Changed(object sender, OptionEventArgs eventArgs)
        {
            switch (eventArgs)
            {
                case KeybindChangedEventArgs keybindArgs:

                    switch (keybindArgs.Id)
                    {
                        case "Upgrade":
                            SEzConfig.Section_Hotkeys["Upgrade"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Storage":
                            SEzConfig.Section_Hotkeys["Storage"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Slot_6":
                            SEzConfig.Section_Hotkeys["Slot_6"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Slot_7":
                            SEzConfig.Section_Hotkeys["Slot_7"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Slot_8":
                            SEzConfig.Section_Hotkeys["Slot_8"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Slot_9":
                            SEzConfig.Section_Hotkeys["Slot_9"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Slot_10":
                            SEzConfig.Section_Hotkeys["Slot_10"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Slot_11":
                            SEzConfig.Section_Hotkeys["Slot_11"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "Slot_12":
                            SEzConfig.Section_Hotkeys["Slot_12"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "SeaTruckArmLeft":
                            SEzConfig.Section_Hotkeys["SeaTruckArmLeft"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;

                        case "SeaTruckArmRight":
                            SEzConfig.Section_Hotkeys["SeaTruckArmRight"] = keybindArgs.Value.ToString();
                            SyncConfig();
                            break;
                    }
                    break;


                case ChoiceChangedEventArgs<string> choiceArgs:

                    switch (choiceArgs.Id)
                    {
                        case "TextColor":
                            SEzConfig.Section_Settings["TextColor"] = choiceArgs.Value;
                            SyncConfig();
                            break;

                        case "SlotLayout":
                            SEzConfig.Section_Settings["SlotLayout"] = choiceArgs.Value;
                            SyncConfig();
                            break;
                    }
                    break;


                case SliderChangedEventArgs sliderArgs:

                    switch (sliderArgs.Id)
                    {
                        case "MaxSlots":
                            SEzConfig.Section_Settings["MaxSlots"] = sliderArgs.Value.ToString();
                            SyncConfig();
                            break;                        
                    }
                    break;               
            }
        }

        private void SyncConfig()
        {
            SEzConfig.Init();
        }
    }
}
