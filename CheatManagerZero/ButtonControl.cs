using System;
using System.Collections.Generic;
using System.Diagnostics;
using BZCommon.Helpers;
using BZCommon.Helpers.GUIHelper;
using BZHelper;
using CheatManagerZero.Configuration;
using CheatManagerZero.NewCommands;

namespace CheatManagerZero
{
    public partial class CheatManagerZeroControl
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
                case Commands.unlockdoors:
                case Commands.encyall:                
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
                
        internal void ToggleButtonControl(ToggleCommands toggleButtonID)
        {
            switch (toggleButtonID)
            {
                case ToggleCommands.nocost when toggleCommands[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                    break;
                case ToggleCommands.nosurvival when toggleCommands[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                case ToggleCommands.nosurvival when toggleCommands[(int)ToggleCommands.freedom].State == GuiItemState.PRESSED:
                    break;
                case ToggleCommands.oxygen when toggleCommands[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                    break;
                case ToggleCommands.invisible when toggleCommands[(int)ToggleCommands.creative].State == GuiItemState.PRESSED:
                    break;
                case ToggleCommands.survival:
                case ToggleCommands.freedom:
                case ToggleCommands.hardcore:
                case ToggleCommands.creative:
                    ChangeGameMode((GameModePresetId)toggleButtonID);
                    break;
                case ToggleCommands.fastbuild:
                case ToggleCommands.fastscan:
                case ToggleCommands.fastgrow:
                case ToggleCommands.fasthatch:
                case ToggleCommands.nocost:
                case ToggleCommands.noenergy:
                case ToggleCommands.nosurvival:                
                case ToggleCommands.invisible:
                case ToggleCommands.nodamage:
                case ToggleCommands.alwaysday:                
                case ToggleCommands.overpower:
                case ToggleCommands.filterfast:                
                    ExecuteCommand("", toggleButtonID.ToString());
                    break;
                case ToggleCommands.resistcold:
                    ResistColdConsoleCommand.main.SetResistColdCheat(CMZ_Config.resistCold);
                    toggleCommands[(int)ToggleCommands.resistcold].State = CMZ_Config.resistCold ? GuiItemState.PRESSED : GuiItemState.NORMAL;
                    break;
                case ToggleCommands.noweather:
                    CMZ_Config.noWeather = !CMZ_Config.noWeather;
                    ToggleWeatherCheat();                    
                    break;
                case ToggleCommands.shotgun:
                    toggleCommands[(int)ToggleCommands.shotgun].State = SNGUI.InvertState(toggleCommands[(int)ToggleCommands.shotgun].State);
                    ExecuteCommand(toggleCommands[(int)ToggleCommands.shotgun].State == GuiItemState.PRESSED ? "shotgun cheat is now True" : "shotgun cheat is now False", toggleCommands[(int)ToggleCommands.shotgun].Name);
                    break;
                case ToggleCommands.noiceworm:
                    ToggleIcewormHuntingMode();
                    toggleCommands[(int)ToggleCommands.noiceworm].State = CMZ_Config.noIceWorm ? GuiItemState.PRESSED : GuiItemState.NORMAL;
                    break;
            }           
        }

        internal void ToggleWeatherCheat()
        {
            VFXWeatherManager wManager = VFXWeatherManager.main;           
                        
            if (CMZ_Config.noWeather)
            {
                wManager.parameters.smokinessIntensity = 0f;
                wManager.parameters.fogDensity = 0f;
                wManager.parameters.fogDensity = 0f;
                wManager.parameters.cloudCoverage = 0f;

                wManager.aerialFogEnabled = false;
                wManager.lightningsEnabled = false;
                wManager.meteorEnabled = false;
                wManager.precipitationEnabled = false;

                NoAerialFog(true);

                ExecuteCommand("", "weather");
            }
            else
            {
                NoAerialFog(false);

                ExecuteCommand("", "weather");
            }

            toggleCommands[(int)ToggleCommands.noweather].State = CMZ_Config.noWeather ? GuiItemState.PRESSED : GuiItemState.NORMAL;
            ErrorMessage.AddMessage($"Weather cheat now: {CMZ_Config.noWeather}");
        }

        private void NoAerialFog(bool value)
        {
            foreach (WaterscapeVolume waterscapeVolume in FindObjectsOfType<WaterscapeVolume>())
            {
                if (value)
                {
                    waterscapeVolume.aboveWaterDensityScale = 0;
                }
                else
                {
                    waterscapeVolume.aboveWaterDensityScale = 10;
                }
            }
        }



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
                case Categories.Blueprints:
                    ExecuteCommand($"Blueprint: {scrollItems[selected].Name} unlocked", $"unlock {selectedTech}");
                    break;
                case Categories.Warp:
                    Utils.PlayFMODAsset(warpSound, Player.main.transform, 20f);
                    prevCwPos = Teleport(scrollItems[selected].Name, GetIntVector(selected));
                    commands[(int)Commands.BackWarp].Enabled = true;                    
                    break;                
                default:
                    break;
            }
        }

        private Categories GetCategoryFromID(int id)
        {
            int[] result = (int[])Enum.GetValues(typeof(Categories));
            return (Categories)result[id];
        }

        private void ChangeGameMode(GameModePresetId newMode)
        {
            DebugGameModePresets();

            BZLogger.Debug($"Trying to change Game Mode to: [{newMode}]");

            switch (newMode)
            {
                case GameModePresetId.Survival:                    
                    GameModeManager.SetGameOptions(newMode);
                    CMZ_Config.resistCold = false;
                    ResistColdConsoleCommand.main.SetResistColdCheat(CMZ_Config.resistCold);
                    break;

                case GameModePresetId.Freedom:
                    GameModeManager.SetGameOptions(newMode);
                    CMZ_Config.resistCold = false;
                    ResistColdConsoleCommand.main.SetResistColdCheat(CMZ_Config.resistCold);
                    break;

                case GameModePresetId.Hardcore:
                    GameModeManager.SetGameOptions(newMode);
                    CMZ_Config.resistCold = false;
                    ResistColdConsoleCommand.main.SetResistColdCheat(CMZ_Config.resistCold);
                    break;

                case GameModePresetId.Creative:
                    GameModeManager.SetGameOptions(newMode);
                    CMZ_Config.resistCold = true;
                    ResistColdConsoleCommand.main.SetResistColdCheat(CMZ_Config.resistCold);
                    break;
            }

            UpdateButtonsState();
        }

        [Conditional("DEBUG")]
        private void DebugGameModePresets()
        {
            if (!GameModeManager.IsInitialized)
                return;
            
            Dictionary<GameModePresetId, GameModePreset> gameModePresets = ReflectionHelper.StaticClassGetPrivateField(typeof(GameModeManager), "gameModePresets") as Dictionary<GameModePresetId, GameModePreset>;

            foreach (KeyValuePair<GameModePresetId, GameModePreset> kvp in gameModePresets)
            {
                BZLogger.Debug($"Preset: [{kvp.Key}]");
                BZLogger.Debug($"isAvailableForPlayer: [{kvp.Value.isAvailableForPlayer}]");
                BZLogger.Debug($"nameLocalizationKey: [{kvp.Value.nameLocalizationKey}]");
                BZLogger.Debug($"descriptionLocalizationKey: [{kvp.Value.descriptionLocalizationKey}]");
                BZLogger.Debug($"allowToChangeOptions: [{kvp.Value.allowToChangeOptions}]");
                BZLogger.Debug($"Options:");
                BZLogger.Debug($"aggressiveCreatureDamageTakenModifier: [{kvp.Value.options.aggressiveCreatureDamageTakenModifier}]");
                BZLogger.Debug($"aggressiveCreatureSpawnModifier: [{kvp.Value.options.aggressiveCreatureSpawnModifier}]");
                BZLogger.Debug($"creatureAggressionModifier: [{kvp.Value.options.creatureAggressionModifier}]");
                BZLogger.Debug($"notAggressiveCreatureSpawnModifier: [{kvp.Value.options.notAggressiveCreatureSpawnModifier}]");
                BZLogger.Debug($"showHungerAlerts: [{kvp.Value.options.showHungerAlerts}]");
                BZLogger.Debug($"showOxygenAlerts: [{kvp.Value.options.showOxygenAlerts}]");
                BZLogger.Debug($"showTemperatureAlerts: [{kvp.Value.options.showTemperatureAlerts}]");
                BZLogger.Debug($"showThirstAlerts: [{kvp.Value.options.showThirstAlerts}]");
                BZLogger.Debug($"badWeatherFrequencyModifier: [{kvp.Value.options.badWeatherFrequencyModifier}]");
                BZLogger.Debug($"baseWaterPressureDamage: [{kvp.Value.options.baseWaterPressureDamage}]");
                BZLogger.Debug($"bodyTemperatureDecreases: [{kvp.Value.options.bodyTemperatureDecreases}]");
                BZLogger.Debug($"craftingRequiresResources: [{kvp.Value.options.craftingRequiresResources}]");
                BZLogger.Debug($"dayLength: [{kvp.Value.options.dayLength}]");
                BZLogger.Debug($"gameHasRadiationSources: [{kvp.Value.options.gameHasRadiationSources}]");
                BZLogger.Debug($"hunger: [{kvp.Value.options.hunger}]");
                BZLogger.Debug($"initialEquipmentPack: [{kvp.Value.options.initialEquipmentPack}]");
                BZLogger.Debug($"nightLength: [{kvp.Value.options.nightLength}]");
                BZLogger.Debug($"organicOxygenSources: [{kvp.Value.options.organicOxygenSources}]");
                BZLogger.Debug($"oxygenDepletes: [{kvp.Value.options.oxygenDepletes}]");
                BZLogger.Debug($"permanentDeath: [{kvp.Value.options.permanentDeath}]");
                BZLogger.Debug($"playerDamageTakenModifier: [{kvp.Value.options.playerDamageTakenModifier}]");
                BZLogger.Debug($"story: [{kvp.Value.options.story}]");
                BZLogger.Debug($"techDamageTakenModifier: [{kvp.Value.options.techDamageTakenModifier}]");
                BZLogger.Debug($"technologyRequiresPower: [{kvp.Value.options.technologyRequiresPower}]");
                BZLogger.Debug($"techRequiresUnlocking: [{kvp.Value.options.techRequiresUnlocking}]");
                BZLogger.Debug($"thirst: [{kvp.Value.options.thirst}]");
                BZLogger.Debug($"vegetarianDiet: [{kvp.Value.options.vegetarianDiet}]");
                BZLogger.Debug($"vehicleWaterPressureDamage: [{kvp.Value.options.vehicleWaterPressureDamage}]");
                BZLogger.Debug($"weatherEffects: [{kvp.Value.options.weatherEffects}]/n");
            }
        }
    }
}
