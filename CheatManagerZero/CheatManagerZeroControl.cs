using System.Collections.Generic;
using UnityEngine;
using UWE;
using BZCommon.Helpers.GUIHelper;
using CheatManagerZero.Configuration;
using CheatManagerZero.NewCommands;
using BZCommon.Helpers;
using System.Linq;
using BZHelper;

namespace CheatManagerZero
{
    public partial class CheatManagerZeroControl : MonoBehaviour
    {                 
        internal ButtonText buttonText;
        internal TechnologyMatrix techMatrix;        
        private Vector2 scrollPos;

        private static Rect windowRect = new Rect(Screen.width - (Screen.width / CMZ_Config.ASPECT), 0, Screen.width / CMZ_Config.ASPECT, (Screen.height / 4 * 3) - 2);
        private Rect drawRect;
        private Rect scrollRect;

        internal FMODAsset warpSound;

        public Utils.MonitoredValue<bool> isSeaglideFast = new Utils.MonitoredValue<bool>();
        public Utils.MonitoredValue<bool> isHoverBikeMoveOnWater = new Utils.MonitoredValue<bool>();
        public Event<Player.MotorMode> onPlayerMotorModeChanged = new Event<Player.MotorMode>();
        
        public Event<string> onConsoleCommandEntered = new Event<string>();
        public Event<bool> onFilterFastChanged = new Event<bool>();
        
        public Event<object> onExosuitSpeedValueChanged = new Event<object>();
        public Event<object> onHoverbikeSpeedValueChanged = new Event<object>();
        public Event<object> onSeatruckSpeedValueChanged = new Event<object>();

        internal List<TechTypeData>[] tMatrix;
        internal List<GuiItem>[] scrollItemsList;             

        internal List<GuiItem> commands = new List<GuiItem>();
        internal List<GuiItem> toggleCommands = new List<GuiItem>();
        internal List<GuiItem> daynightTab = new List<GuiItem>();
        
        internal List<GuiItem> categoriesTab = new List<GuiItem>();
        internal List<GuiItem> vehicleSettings = new List<GuiItem>();
        internal List<GuiItem> sliders = new List<GuiItem>();
        internal List<GuiItem> warpExtras = new List<GuiItem>();
                
        internal bool isActive;
        internal bool isDirty = false;
        internal bool initToggleButtons = false;        
        internal IntVector3 prevCwPos;
       
        internal string exosuitName;
        internal string hoverbikeName;
        internal string seatruckName;

        internal float seamothSpeedMultiplier;
        internal float exosuitSpeedMultiplier;
        internal float hoverbikeSpeedMultiplier;

        private const int SPACE = 4;
        private const int ITEMSIZE = 22;
        private const int SLIDERHEIGHT = 30;
        private const int MAXSHOWITEMS = 6;

        private string windowTitle;

        private GuiItemEvent CommandsGroup = new GuiItemEvent(-1, -1, false);
        private GuiItemEvent ToggleCommandsGroup = new GuiItemEvent(-1, -1, false);
        private GuiItemEvent DayNightGroup = new GuiItemEvent(-1, -1, false);
        private GuiItemEvent CategoriesGroup = new GuiItemEvent(-1, -1, false);
        private GuiItemEvent ScrollViewGroup = new GuiItemEvent(-1, -1, false);
        private GuiItemEvent VehicleSettingsGroup = new GuiItemEvent(-1, -1, false);
        private GuiItemEvent WarpExtrasGroup = new GuiItemEvent(-1, -1, false);

        private int currentdaynightTab = 4;
        private int currentTab = 0;        

        public void Awake()
        {            
            DontDestroyOnLoad(this);
            useGUILayout = false;
            gameObject.AddComponent<AlwaysDayConsoleCommand>();
            gameObject.AddComponent<OverPowerConsoleCommand>();
            gameObject.AddComponent<ResistColdConsoleCommand>();
            
            UpdateTitle();            
            warpSound = ScriptableObject.CreateInstance<FMODAsset>();
            warpSound.path = "event:/tools/gravcannon/fire";

            techMatrix = new TechnologyMatrix();
            tMatrix = new List<TechTypeData>[techMatrix.baseTechMatrix.Count];
            techMatrix.InitializeBlueprints();
            techMatrix.InitTechMatrixList(ref tMatrix);
            
            if (CMZ_Config.Section_userWarpTargets.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in CMZ_Config.Section_userWarpTargets)
                {
                    WarpTargets_User.Add(new IntVector3(kvp.Key), kvp.Value);
                }
            }
           
            techMatrix.GetModdedTechTypes(ref tMatrix);            
            
            techMatrix.SortTechLists(ref tMatrix);
            
            buttonText = new ButtonText();                        

            drawRect = SNWindow.InitWindowRect(windowRect, true);            

            List<Rect> commandRects = SNWindow.SetGridItemsRect(drawRect, 4, 2, ITEMSIZE, SPACE, SPACE, true, true);

            commands.CreateGuiItemsGroup(buttonText.Buttons, commandRects, GuiItemType.NORMALBUTTON, new GuiItemColor());
            commands.SetGuiItemsGroupLabel("Commands", commandRects.GetLast(), new GuiItemColor(GuiColor.White));            

            List<Rect> toggleCommandRects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref commandRects), drawRect.width, drawRect.height), 4, 6, ITEMSIZE, SPACE, SPACE, true, true);
            toggleCommands.CreateGuiItemsGroup(buttonText.ToggleButtons, toggleCommandRects, GuiItemType.TOGGLEBUTTON, new GuiItemColor(GuiColor.Red, GuiColor.Green));
            toggleCommands.SetGuiItemsGroupLabel("Toggle Commands", toggleCommandRects.GetLast(), new GuiItemColor(GuiColor.White));
                        
            List<Rect> daynightTabrects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref toggleCommandRects), drawRect.width, drawRect.height), 6, 1, 24, SPACE, SPACE, true, true);
            daynightTab.CreateGuiItemsGroup(buttonText.DayNightTab, daynightTabrects, GuiItemType.TAB, new GuiItemColor());
            daynightTab.SetGuiItemsGroupLabel("Day/Night Speed:", daynightTabrects.GetLast(), new GuiItemColor(GuiColor.White));
            
            List<Rect> categoriesTabrects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref daynightTabrects), drawRect.width, drawRect.height), 4, 5, ITEMSIZE, SPACE, SPACE, true, true);
            categoriesTab.CreateGuiItemsGroup(buttonText.CategoriesTab, categoriesTabrects, GuiItemType.TAB, new GuiItemColor(GuiColor.Gray, GuiColor.Green, GuiColor.White));
            categoriesTab.SetGuiItemsGroupLabel("Categories:", categoriesTabrects.GetLast(), new GuiItemColor(GuiColor.White));

            float nextYpos = SNWindow.GetNextYPos(ref categoriesTabrects);
            scrollRect = new Rect(drawRect.x + SPACE, nextYpos, drawRect.width - (SPACE * 2), drawRect.height - nextYpos);
            
            List<Rect>[] scrollItemRects = new List<Rect>[tMatrix.Length + 1];

            for (int i = 0; i < tMatrix.Length; i++)
            {
                float width = drawRect.width;

                if (i == 0 && tMatrix[0].Count > MAXSHOWITEMS)
                    width -= 20;

                else if (tMatrix[i].Count * (ITEMSIZE + SPACE) > scrollRect.height)
                    width -= 20;                
                
                scrollItemRects[i] = SNWindow.SetGridItemsRect(new Rect(0, 0, width, tMatrix[i].Count * (ITEMSIZE + SPACE)), 1, tMatrix[i].Count, ITEMSIZE, SPACE, 2, false, false, true);
            }           

            int warpCounts = WarpTargets_Internal.Count + WarpTargets_User.Count;

            scrollItemRects[tMatrix.Length] = SNWindow.SetGridItemsRect(new Rect(0, 0, drawRect.width - 20, warpCounts * (ITEMSIZE + SPACE)), 1, warpCounts, ITEMSIZE, SPACE, 2, false, false, true);
            
            scrollItemsList = new List<GuiItem>[tMatrix.Length + 1];
            
            for (int i = 0; i < tMatrix.Length; i++)
            {
                scrollItemsList[i] = new List<GuiItem>();
                CreateTechGroup(tMatrix[i], scrollItemRects[i], GuiItemType.NORMALBUTTON, ref scrollItemsList[i], new GuiItemColor(GuiColor.Gray, GuiColor.Green, GuiColor.White),  GuiItemState.NORMAL, true, FontStyle.Normal, TextAnchor.MiddleLeft);                
            }
            
            scrollItemsList[tMatrix.Length] = new List<GuiItem>();
            
            AddListToGroup(GetWarpTargetNames(), scrollItemRects[tMatrix.Length], GuiItemType.NORMALBUTTON, ref scrollItemsList[tMatrix.Length], new GuiItemColor(GuiColor.Gray, GuiColor.Green, GuiColor.White), GuiItemState.NORMAL, true, FontStyle.Normal, TextAnchor.MiddleLeft);
                        
            var searchSeaGlide = new TechnologyMatrix.TechTypeSearch(TechType.Seaglide);
            string seaglideName = tMatrix[1][tMatrix[1].FindIndex(searchSeaGlide.EqualsWith)].Name;
                        
            var searchExosuit = new TechnologyMatrix.TechTypeSearch(TechType.Exosuit);
            exosuitName = tMatrix[0][tMatrix[0].FindIndex(searchExosuit.EqualsWith)].Name;

            var searchHoverBike = new TechnologyMatrix.TechTypeSearch(TechType.Hoverbike);
            hoverbikeName = tMatrix[0][tMatrix[0].FindIndex(searchHoverBike.EqualsWith)].Name;

            var searchSeaTruck = new TechnologyMatrix.TechTypeSearch(TechType.SeaTruck);
            seatruckName = tMatrix[0][tMatrix[0].FindIndex(searchSeaTruck.EqualsWith)].Name;

            string[] vehicleSetButtons = { /*$"{seamothName} Can Fly",*/ $"{seaglideName} Speed Fast" , $"{hoverbikeName} Move on Water" };            

            float scrollRectheight = (MAXSHOWITEMS + 1) * (scrollItemsList[0][0].Rect.height + 2);
            float y = scrollRect.y + scrollRectheight + SPACE;

            List<Rect> vehicleSettingsRects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, y, drawRect.width, drawRect.height), 2, 2, ITEMSIZE, SPACE, SPACE, false, true);
            vehicleSettings.CreateGuiItemsGroup(vehicleSetButtons, vehicleSettingsRects, GuiItemType.TOGGLEBUTTON, new GuiItemColor(GuiColor.Red, GuiColor.Green));
            vehicleSettings.SetGuiItemsGroupLabel("Vehicle settings:", vehicleSettingsRects.GetLast(), new GuiItemColor(GuiColor.White));

            string[] sliderLabels = { /*$"{seamothName} speed multiplier:",*/ $"{exosuitName} speed multiplier:", $"{hoverbikeName} speed multiplier:"/*, $"{seatruckName} speed multiplier:"*/ };

            List<Rect> slidersRects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref vehicleSettingsRects), drawRect.width, drawRect.height), 1, 4, SLIDERHEIGHT, SPACE, SPACE, false, false);
            sliders.CreateGuiItemsGroup(sliderLabels, slidersRects, GuiItemType.HORIZONTALSLIDER, new GuiItemColor());
                        
            sliders[0].OnChangedEvent = onExosuitSpeedValueChanged;
            sliders[1].OnChangedEvent = onHoverbikeSpeedValueChanged;           

            string[] warpExtrasButtons = { "Add current position to list", "Remove selected from list" };
            scrollRectheight = 11 * (scrollItemsList[0][0].Rect.height + 2);
            y = scrollRect.y + scrollRectheight + SPACE + 2;
            List<Rect> warpExtrasRects = new Rect(drawRect.x, y, drawRect.width, drawRect.height).SetGridItemsRect(2, 1, ITEMSIZE, SPACE, SPACE, false, false);
            warpExtras.CreateGuiItemsGroup(warpExtrasButtons, warpExtrasRects, GuiItemType.NORMALBUTTON, new GuiItemColor());
                        
            commands[(int)Commands.BackWarp].Enabled = false;
            commands[(int)Commands.BackWarp].State = GuiItemState.PRESSED;
            
            daynightTab[4].State = GuiItemState.PRESSED;
            categoriesTab[0].State = GuiItemState.PRESSED;

            seamothSpeedMultiplier = 1;
            exosuitSpeedMultiplier = 1;
            hoverbikeSpeedMultiplier = 1;                                
        }

        public void AddListToGroup(List<string> names, List<Rect> rects, GuiItemType type, ref List<GuiItem> guiItems, GuiItemColor itemColor,
                                               GuiItemState state = GuiItemState.NORMAL, bool enabled = true, FontStyle fontStyle = FontStyle.Normal,
                                               TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {            
            for (int i = 0; i < names.Count; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = names[i],
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor
                });
            }            
        }


        public void AddTechListToGroup(List<TechTypeData> techTypeDatas, List<Rect> rects, GuiItemType type, ref List<GuiItem> guiItems, GuiItemColor itemColor,
                                               GuiItemState state = GuiItemState.NORMAL, bool enabled = true, FontStyle fontStyle = FontStyle.Normal,
                                               TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            for (int i = 0; i < techTypeDatas.Count; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = techTypeDatas[i].Name,
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor
                });
            }
        }
        public void CreateTechGroup(List<TechTypeData> techTypeDatas, List<Rect> rects, GuiItemType type, ref List<GuiItem> guiItems, GuiItemColor itemColor,
                                               GuiItemState state = GuiItemState.NORMAL, bool enabled = true, FontStyle fontStyle = FontStyle.Normal,
                                               TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            guiItems.Clear();
                       
            for (int i = 0; i < techTypeDatas.Count; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = techTypeDatas[i].Name,
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor
                });
            }
        }
     
        public void OnDestroy()
        {
            CMZ_Config.Save();
            commands = null;            
            toggleCommands = null;
            daynightTab = null;
            categoriesTab = null;
            vehicleSettings = null;
            tMatrix = null;
            initToggleButtons = false;            
            warpSound = null;            
            isActive = false;            
            onConsoleCommandEntered.RemoveHandler(this, OnConsoleCommandEntered);
            onFilterFastChanged.RemoveHandler(this, OnFilterFastChanged);
        }

        public void Start()
        {                       
            onConsoleCommandEntered.AddHandler(this, new Event<string>.HandleFunction(OnConsoleCommandEntered));
            onFilterFastChanged.AddHandler(this, new Event<bool>.HandleFunction(OnFilterFastChanged));            
        }        

        private void OnFilterFastChanged(bool enabled)
        {
            CMZ_Config.filterFast = enabled;
        }

        private void OnConsoleCommandEntered(string command)
        {
            UpdateButtonsState();            
        }
        
        internal void UpdateTitle()
        {            
           windowTitle = $"CheatManagerZero v.{CMZ_Config.CONFIG_VERSION}, {CMZ_Config.KEYBINDINGS["ToggleWindow"]} Toggle Window, {CMZ_Config.KEYBINDINGS["ToggleMouse"]} Toggle Mouse";
        }

        public void Update()
        {
            if (Player.main != null)
            {
                if (!initToggleButtons && !uGUI.main.loading.IsLoading)
                {
                    UpdateToggleButtons();
                    initToggleButtons = true;
                    UpdateButtonsState();
                }

                if (Input.GetKeyDown(CMZ_Config.KEYBINDINGS["ToggleWindow"]))
                {
                    isActive = !isActive;
                }

                if (!isActive)
                    return;
                
                if (Input.GetKeyDown(CMZ_Config.KEYBINDINGS["ToggleMouse"]))
                {
                    UWE.Utils.lockCursor = !UWE.Utils.lockCursor;
                }                

                if (CommandsGroup.ItemID != -1 && CommandsGroup.MouseButton == 0)
                {
                    NormalButtonControl(CommandsGroup.ItemID, ref commands, ref toggleCommands);
                }

                if (ToggleCommandsGroup.ItemID != -1 && ToggleCommandsGroup.MouseButton == 0)
                {
                    ToggleButtonControl((ToggleCommands)ToggleCommandsGroup.ItemID);
                }

                if (DayNightGroup.ItemID != -1 && DayNightGroup.MouseButton == 0)
                {
                    DayNightButtonControl(DayNightGroup.ItemID, ref currentdaynightTab, ref daynightTab);
                }

                if (CategoriesGroup.ItemID != -1 && CategoriesGroup.MouseButton == 0)
                {
                    if (CategoriesGroup.ItemID != currentTab)
                    {                        
                        currentTab = CategoriesGroup.ItemID;
                        scrollPos = Vector2.zero;
                    }
                }

                if (ScrollViewGroup.ItemID != -1)
                {
                    if (ScrollViewGroup.MouseButton == 0)
                    {
                        ScrollViewControl(currentTab, ScrollViewGroup.ItemID, ref scrollItemsList[currentTab], ref tMatrix, ref commands);
                    }
                    else if (currentTab == 18 && ScrollViewGroup.MouseButton == 1)
                    {
                        if (ScrollViewGroup.ItemID < WarpTargets_Internal.Count)
                        {
                            ErrorMessage.AddMessage($"CheatManagerZero Warning!\nInternal list items cannot be selected!");
                            return;
                        }

                        scrollItemsList[currentTab].UnmarkAll();
                        scrollItemsList[currentTab][ScrollViewGroup.ItemID].SetStateInverse();
                    }
                }

                if (VehicleSettingsGroup.ItemID != -1 && VehicleSettingsGroup.MouseButton == 0)
                {
                    if (VehicleSettingsGroup.ItemID == 0)
                    {
                        isSeaglideFast.Update(!isSeaglideFast.value);
                        vehicleSettings[0].State = SNGUI.ConvertBoolToState(isSeaglideFast.value);                           
                    }
                    else if (VehicleSettingsGroup.ItemID == 1)
                    {
                        isHoverBikeMoveOnWater.Update(!isHoverBikeMoveOnWater.value);
                        vehicleSettings[1].State = SNGUI.ConvertBoolToState(isHoverBikeMoveOnWater.value);
                    }
                }

                if (WarpExtrasGroup.ItemID != -1 && WarpExtrasGroup.MouseButton == 0)
                {
                    if (WarpExtrasGroup.ItemID == 0)
                    {
                        IntVector3 position = new IntVector3(Player.main.transform.position);

                        if (IsPositionWithinRange(position, out string nearestWarpName))
                        {
                            ErrorMessage.AddMessage($"CheatManagerZero Warning!\nThis position cannot be added to the Warp list\nbecause it is very close to:\n{nearestWarpName} warp point!");
                        }
                        else
                        {
                            AddToList(position);
                        }                        
                    }

                    if (WarpExtrasGroup.ItemID == 1)
                    {                        
                        int item = scrollItemsList[currentTab].GetMarkedItem();
                        
                        if (item == -1)
                        {
                            ErrorMessage.AddMessage("CheatManagerZero Error!\nNo item selected from the user Warp list!");
                            return;
                        }

                        isDirty = true;

                        int userIndex = item - WarpTargets_Internal.Count;

                        IntVector3 intVector = WarpTargets_User.Keys.ElementAt(userIndex);
                                                
                        RemoveFormList(intVector);

                        scrollItemsList[currentTab].RemoveGuiItemFromGroup(item);
                        
                        isDirty = false;
                    }
                }
            }
        }
                
        internal void UpdateToggleButtons()
        {
            if (CMZ_Config.noIceWorm)
            {
                ToggleIcewormHuntingMode();
            }

            if (CMZ_Config.noWeather)
            {
                ToggleWeatherCheat();
            }

            foreach (KeyValuePair<string, string> kvp in CMZ_Config.Section_toggleButtons)
            {
                bool.TryParse(kvp.Value, out bool result);

                if (result)
                {
                    ExecuteCommand("", kvp.Key);
                }
            }
        }        

        internal void UpdateButtonsState()
        {
            toggleCommands[(int)ToggleCommands.freedom].State = SNGUI.ConvertBoolToState(CheckGameModePreset(GameModePresetId.Freedom));            
            toggleCommands[(int)ToggleCommands.creative].State = SNGUI.ConvertBoolToState(CheckGameModePreset(GameModePresetId.Creative));            
            toggleCommands[(int)ToggleCommands.survival].State = SNGUI.ConvertBoolToState(CheckGameModePreset(GameModePresetId.Survival));
            toggleCommands[(int)ToggleCommands.hardcore].State = SNGUI.ConvertBoolToState(CheckGameModePreset(GameModePresetId.Hardcore));
            toggleCommands[(int)ToggleCommands.fastbuild].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastBuildCheat);
            toggleCommands[(int)ToggleCommands.fastscan].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastScanCheat);
            toggleCommands[(int)ToggleCommands.fastgrow].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastGrowCheat);
            toggleCommands[(int)ToggleCommands.fasthatch].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastHatchCheat);
            toggleCommands[(int)ToggleCommands.filterfast].State = SNGUI.ConvertBoolToState(CMZ_Config.filterFast);
            toggleCommands[(int)ToggleCommands.nocost].State = SNGUI.ConvertBoolToState(!GameModeManager.GetOption<bool>(GameOption.CraftingRequiresResources));
            toggleCommands[(int)ToggleCommands.noenergy].State = SNGUI.ConvertBoolToState(!GameModeManager.GetOption<bool>(GameOption.TechnologyRequiresPower));
            toggleCommands[(int)ToggleCommands.nosurvival].State = SNGUI.ConvertBoolToState(!CheckGameModePreset(GameModePresetId.Survival));
            toggleCommands[(int)ToggleCommands.oxygen].State = SNGUI.ConvertBoolToState(!GameModeManager.GetOption<bool>(GameOption.OxygenDepletes));
            toggleCommands[(int)ToggleCommands.invisible].State = SNGUI.ConvertBoolToState(GameModeManager.GetOption<float>(GameOption.CreatureAggressionModifier) == 0);
            toggleCommands[(int)ToggleCommands.nodamage].State = SNGUI.ConvertBoolToState(NoDamageConsoleCommand.main.GetNoDamageCheat());
            toggleCommands[(int)ToggleCommands.alwaysday].State = SNGUI.ConvertBoolToState(AlwaysDayConsoleCommand.main.GetAlwaysDayCheat());
            toggleCommands[(int)ToggleCommands.overpower].Enabled = CheckGameModePreset(GameModePresetId.Survival);
            toggleCommands[(int)ToggleCommands.noweather].State = CMZ_Config.noWeather ? GuiItemState.PRESSED : GuiItemState.NORMAL;
            toggleCommands[(int)ToggleCommands.resistcold].State = SNGUI.ConvertBoolToState(ResistColdConsoleCommand.main.GetResistColdCheat());
            toggleCommands[(int)ToggleCommands.noiceworm].State = CMZ_Config.noIceWorm ?  GuiItemState.PRESSED : GuiItemState.NORMAL;

            if (toggleCommands[(int)ToggleCommands.overpower].Enabled)
                toggleCommands[(int)ToggleCommands.overpower].State = SNGUI.ConvertBoolToState(OverPowerConsoleCommand.main.GetOverPowerCheat());
           
            vehicleSettings[0].State = SNGUI.ConvertBoolToState(isSeaglideFast.value);
        }       
        
        public void OnGUI()
        {
            if (!isActive || isDirty)
                return;
            
            SNWindow.CreateWindow(windowRect, windowTitle);

            CommandsGroup = commands.DrawGuiItemsGroup();
            ToggleCommandsGroup = toggleCommands.DrawGuiItemsGroup();
            DayNightGroup = daynightTab.DrawGuiItemsGroup();
            CategoriesGroup = categoriesTab.DrawGuiItemsGroup();

            if (currentTab == 0)
            {
                ScrollViewGroup = SNScrollView.CreateScrollView(scrollRect, ref scrollPos, ref scrollItemsList[currentTab], "Select Item in Category:", categoriesTab[currentTab].Name, MAXSHOWITEMS);

                VehicleSettingsGroup = vehicleSettings.DrawGuiItemsGroup();

                SNHorizontalSlider.CreateHorizontalSlider(sliders[0].Rect, ref exosuitSpeedMultiplier, 1f, 5f, sliders[0].Name, sliders[0].OnChangedEvent);
                SNHorizontalSlider.CreateHorizontalSlider(sliders[1].Rect, ref hoverbikeSpeedMultiplier, 1f, 5f, sliders[1].Name, sliders[1].OnChangedEvent);               
            }
            else if (currentTab == 19)
            {
                ScrollViewGroup = SNScrollView.CreateScrollView(scrollRect, ref scrollPos, ref scrollItemsList[currentTab], "Select Item in Category:", categoriesTab[currentTab].Name, 10);

                WarpExtrasGroup = warpExtras.DrawGuiItemsGroup();
            }
            else
            {
                ScrollViewGroup = SNScrollView.CreateScrollView(scrollRect, ref scrollPos, ref scrollItemsList[currentTab], "Select Item in Category:", categoriesTab[currentTab].Name);
            }
        }

        public void ExecuteCommand(object message, object command)
        {
            if (message != null)
            {
                ErrorMessage.AddMessage(message.ToString());
            }

            if (command != null)
            {
                BZLogger.Log((string)command);
                DevConsole.SendConsoleCommand(command.ToString());
            }
        }

        public bool IsPlayerInVehicle()
        {
            return Player.main.inSeamoth || Player.main.inExosuit ? true : false;
        }

        public void ToggleIcewormHuntingMode()
        {
            GameObject phantomManager = UnityHelper.GetRootGameObject("IceCliffs", "IceWormPhantomManager");

            if (phantomManager)
            {
                if (phantomManager.activeSelf)
                {
                    phantomManager.SetActive(false);
                    CMZ_Config.noIceWorm = true;
                    ExecuteCommand($"noIceWorm cheat now: {CMZ_Config.noIceWorm}", null);                   
                }
                else
                {
                    phantomManager.SetActive(true);
                    CMZ_Config.noIceWorm = false;
                    ExecuteCommand($"noIceWorm cheat now: {CMZ_Config.noIceWorm}", null);
                }
            }
        }

        public bool CheckGameModePreset(GameModePresetId presetId)
        {
            return GameModeManager.GetCurrentPresetId() == presetId;
        }

        public bool CheckGameOptionIsActive(GameOption gameOption)
        {
            GameOptionsManager gameOptionsManager = GameModeManager.GetCurrentGameOptionsManager();

            GameOptionsManager.IValueManager iValueManager = gameOptionsManager.GeValueManager(gameOption);

            return iValueManager.Equals(true);
        }
    }
}