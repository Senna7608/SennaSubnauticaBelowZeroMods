using System;
using System.Collections.Generic;
using BZCommon.Helpers;
using BZCommon.Helpers.GUIHelper;

namespace CheatManagerZero
{
    public partial class CheatManagerZero
    {
        internal static readonly float[] DayNightSpeed = { 0.1f, 0.25f, 0.5f, 0.75f, 1f, 2f };
        
        internal void NormalButtonControl(int normalButtonID, ref List<GuiItem> Buttons, ref List<GuiItem> toggleButtons)
        {
            switch ((Commands)normalButtonID)
            {
                case Commands.day when toggleButtons[(int)ToggleCommands.alwaysday].State == GuiItemState.PRESSED:
                case Commands.night when toggleButtons[(int)ToggleCommands.alwaysday].State == GuiItemState.PRESSED:
                    break;
                case Commands.day:
                case Commands.night:
                case Commands.unlockall:
                //case Commands.warpme:
                case Commands.unlockdoors:
                case Commands.encyall:
                //case Commands.resetweather:
                //case Commands.nextweather:
                    ExecuteCommand("Send command to console: " + Buttons[normalButtonID].Name, Buttons[normalButtonID].Name);
                    break;
                case Commands.clearinventory:
                    ErrorMessage.AddMessage("Inventory Cleared");
                    Inventory.main.container.Clear(false);                    
                    break;
                case Commands.BackWarp:
                    Teleport("position", prevCwPos);
                    Utils.PlayFMODAsset(warpSound, Player.main.transform, 20f);                    
                    Buttons[(int)Commands.BackWarp].Enabled = false;
                    break;
            }
        }
                
        internal void ToggleButtonControl(int toggleButtonID, ref List<GuiItem> toggleButtons)
        {
            switch ((ToggleCommands)toggleButtonID)
            {
                case ToggleCommands.nocost when toggleButtons[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                    break;
                case ToggleCommands.nosurvival when toggleButtons[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                case ToggleCommands.nosurvival when toggleButtons[(int)ToggleCommands.freedom].State == GuiItemState.PRESSED:
                    break;
                case ToggleCommands.oxygen when toggleButtons[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                    break;
                //case ToggleCommands.radiation when toggleButtons[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                //    break;
                case ToggleCommands.invisible when toggleButtons[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                    break;
                case ToggleCommands.freedom:
                case ToggleCommands.creative:
                case ToggleCommands.survival:
                case ToggleCommands.hardcore:
                case ToggleCommands.fastbuild:
                case ToggleCommands.fastscan:
                case ToggleCommands.fastgrow:
                case ToggleCommands.fasthatch:
                case ToggleCommands.nocost:
                case ToggleCommands.noenergy:
                case ToggleCommands.nosurvival:
                case ToggleCommands.oxygen:
                //case ToggleCommands.radiation:
                case ToggleCommands.invisible:
                case ToggleCommands.nodamage:
                case ToggleCommands.alwaysday:                
                case ToggleCommands.overpower:
                case ToggleCommands.filterfast:
                case ToggleCommands.weather:
                case ToggleCommands.resistcold:
                    ExecuteCommand("", ((ToggleCommands)toggleButtonID).ToString());
                    break;
                case ToggleCommands.shotgun:
                    toggleButtons[(int)ToggleCommands.shotgun].State = SNGUI.InvertState(toggleButtons[(int)ToggleCommands.shotgun].State);
                    ExecuteCommand(toggleButtons[(int)ToggleCommands.shotgun].State == GuiItemState.PRESSED ? "shotgun cheat is now True" : "shotgun cheat is now False", toggleButtons[(int)ToggleCommands.shotgun].Name);
                    break;
                case ToggleCommands.noiceworm:
                    ToggleIcewormHuntingMode();
                    toggleButtons[(int)ToggleCommands.noiceworm].State = noIceWorm ? GuiItemState.PRESSED : GuiItemState.NORMAL;
                    break;
            }           
        }

        /*
        internal void WeatherButtonControl(int weatherButtonID, ref List<GuiItem> weatherButtons)
        {
            switch ((Weather)weatherButtonID)
            {
                case Weather.lightning when weatherButtons[(int)Weather.weather].State == GuiItemState.PRESSED:
                case Weather.precipitation when weatherButtons[(int)Weather.weather].State == GuiItemState.PRESSED:
                case Weather.wind when weatherButtons[(int)Weather.weather].State == GuiItemState.PRESSED:
                case Weather.cold when weatherButtons[(int)Weather.weather].State == GuiItemState.PRESSED:
                    break;                
                default:
                    ExecuteCommand("", Enum.GetName(typeof(Weather), weatherButtonID));
                    break;
            }           
        }
        */

        internal void DayNightButtonControl(int daynightTabID, ref int currentdaynightTab, ref List<GuiItem> daynightTab)
        {
            if (daynightTabID != currentdaynightTab)
            {
                daynightTab[currentdaynightTab].State = GuiItemState.NORMAL;
                daynightTab[daynightTabID].State = GuiItemState.PRESSED;
                currentdaynightTab = daynightTabID;
                DevConsole.SendConsoleCommand("daynightspeed " + DayNightSpeed[daynightTabID]);
            }
        }
        
        
        internal void ScrollViewControl(int categoryTabID, int selected, ref List<GuiItem> scrollItems, ref List<TechTypeData>[] tMatrix, ref List<GuiItem> commands)
        {            
            string selectedTech;
            Categories category = GetCategoryFromID(categoryTabID);

            switch (category)
            {
                case Categories.Warp:
                selectedTech = string.Empty;                
                break;

                case Categories.ALLTECH:
                    selectedTech = FullTechMatrix[selected].TechType.ToString();
                    break;
                
                default:
                    selectedTech = tMatrix[categoryTabID][selected].TechType.ToString();
                    break;
            }
            
            switch (category)
            {
                case Categories.Vehicles:
                    if (!Player.main.IsInBase() && !Player.main.IsInSubmarine() && !Player.main.IsInSub())
                    {
                        ExecuteCommand($"{scrollItems[selected].Name}  has spawned", $"spawn {selectedTech}");
                        break;
                    }
                    ErrorMessage.AddMessage("CheatManager Error!\nVehicles cannot spawn inside Lifepod, Base or Submarine!");
                    break;
                case Categories.Tools:
                case Categories.Equipment:
                case Categories.Materials:
                case Categories.Electronics:
                case Categories.Upgrades:
                case Categories.FoodAndWater:                
                case Categories.Eggs:
                case Categories.SeaSeed:
                case Categories.LandSeed:
                case Categories.FloraItem:
                    ExecuteCommand($"{scrollItems[selected].Name}  added to inventory", $"item {selectedTech}");                    
                    break;
                case Categories.LootAndDrill:
                case Categories.Herbivores:
                case Categories.Carnivores:
                case Categories.Parasites:
                case Categories.Leviathan:
                case Categories.SeaSpawn:
                case Categories.LandSpawn:
                    ExecuteCommand($"{scrollItems[selected].Name}  has spawned", $"spawn {selectedTech}");
                    break;
                //case Categories.Blueprints:
                //    ExecuteCommand($"Blueprint: {scrollItems[selected].Name} unlocked", $"unlock {selectedTech}");
                //    break;
                case Categories.Warp:
                    Utils.PlayFMODAsset(warpSound, Player.main.transform, 20f);
                    prevCwPos = Teleport(scrollItems[selected].Name, GetIntVector(selected));
                    commands[(int)Commands.BackWarp].Enabled = true;                    
                    break;
                case Categories.ALLTECH:
                    ExecuteCommand($"{scrollItems[selected].Name}  has spawned", $"spawn {selectedTech}");                    
                    break;
                default:
                    break;
            }

            //print($"TechType: {tMatrix[categoryTabID][selected].TechType.ToString()} : {(int)tMatrix[categoryTabID][selected].TechType}");
        }

        private Categories GetCategoryFromID(int id)
        {
            int[] result = (int[])Enum.GetValues(typeof(Categories));
            return (Categories)result[id];
        }
    }
}
