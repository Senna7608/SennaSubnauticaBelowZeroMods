/*
using BZCommon;
using BZCommon.RuntimeGUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZHelper
{
    public class ScriptableGUITest : MonoBehaviour
    {
        public ScriptableGUITest Instance;

        private GUI_root guiRoot;
        private GUI_window MAIN_WINDOW;
        private GUI_group Group_Commands, Group_ToggleCommands, Group_Categories;
        private GUI_scrollView ScrollView_Categories;
        private GUI_horizontalSlider Slider_DayNight, Slider_Seamoth, Slider_Exosuit, Slider_Cyclops;
        private float Slider_DayNight_Value = 1f;
        private float Slider_Seamoth_Value = 1f;
        private float Slider_Exosuit_Value = 1f;
        private float Slider_Cyclops_Value = 1f;

        private const float ASPECT = 4.8f;

        private Rect windowRect = new Rect(Screen.width - (Screen.width / ASPECT), 0, Screen.width / ASPECT, (Screen.height / 4 * 3) - 2);


        public ScriptableGUITest()
        {
            if (!Instance)
            {
                Instance = FindObjectOfType(typeof(ScriptableGUITest)) as ScriptableGUITest;

                if (!Instance)
                {
                    GameObject ZHelper = new GameObject("ZHelper");
                    Instance = ZHelper.GetOrAddComponent<ScriptableGUITest>();
                }
            }
        }

        private List<GUI_content> Commands = new List<GUI_content>()
        {            
            new GUI_content(1, GUI_Type.NORMALBUTTON, "day", "Sets time to day."),
            new GUI_content(2, GUI_Type.NORMALBUTTON, "night", "Sets time to night."),
            new GUI_content(3, GUI_Type.NORMALBUTTON, "unlockall", "Unlocks all blueprints."),
            new GUI_content(4, GUI_Type.NORMALBUTTON, "clearinventory", "Deletes everything in the Inventory."),
            new GUI_content(5, GUI_Type.NORMALBUTTON, "unlockdoors", "Unlocks all doors in the Aurora and Alien Bases.\nDoes not apply for sealed doors which need to be cut with the Laser Cutter."),
            new GUI_content(6, GUI_Type.NORMALBUTTON, "ency all", "Will give the player all of the data bank entries, even ones not in the normal game yet."),
            new GUI_content(7, GUI_Type.NORMALBUTTON, "warpme", "Teleports the player to the Cyclops, a Seabase, or Lifepod 5 depending on which the player last entered."),
            new GUI_content(8, GUI_Type.NORMALBUTTON, "BackWarp", "Teleports the player back to the previously teleport position.")            
        };

        private List<GUI_content> ToggleCommands = new List<GUI_content>()
        {            
            new GUI_content(1, GUI_Type.TOGGLEBUTTON, "freedom", "Change game mode to freedom.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(2, GUI_Type.TOGGLEBUTTON, "creative", "Change game mode to creative.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(3, GUI_Type.TOGGLEBUTTON, "survival", "Change game mode to survival.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(4, GUI_Type.TOGGLEBUTTON, "hardcore", "Change game mode to hardcore.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(5, GUI_Type.TOGGLEBUTTON, "fastbuild", "Allows the player to build modules quickly with the Habitat Builder.\nThis is good to use together with 'nocost' command.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(6, GUI_Type.TOGGLEBUTTON, "fastscan", "Reduces the scanning time when using the Scanner.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(7, GUI_Type.TOGGLEBUTTON, "fastgrow", "Plantable flora will grow within a few moments when placed in any type of planter.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(8, GUI_Type.TOGGLEBUTTON, "fasthatch", "Eggs will hatch within a few moments after being placed in an Alien Containment.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(9, GUI_Type.TOGGLEBUTTON, "filterfast", "Reduces the time Water Filtration Machines take to filter.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(10, GUI_Type.TOGGLEBUTTON,  "nocost", "Toggles nocost mode on and off.\n" +
                "While on, the player can use the Fabricator, Habitat Builder, Mobile Vehicle Bay, Vehicle Upgrade Console and Modification Station even if they do not have the materials required.\n" +
                "Note that if the player does have some or all of the materials required to make something, they will still be expended.\n" +
                "Deconstructing an item with the Habitat Builder in this mode does not refund its construction materials.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(11, GUI_Type.TOGGLEBUTTON, "noenergy", "Toggles power usage for all vehicles, tools as well as the Seabases.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(12, GUI_Type.TOGGLEBUTTON, "nosurvival", "Disables the player's Food & Water requirements.\nOnly applicable for Survival and Hardcore modes.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(13, GUI_Type.TOGGLEBUTTON, "oxygen", "Toggles loss of oxygen when underwater.\nYou will regain oxygen normally.\nWarning: If used after oxygen has reached 0, it won't save you from dying.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(14, GUI_Type.TOGGLEBUTTON, "radiation", "Toggles radiation.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(15, GUI_Type.TOGGLEBUTTON, "invisible", "Creatures will ignore the player completely.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(16, GUI_Type.TOGGLEBUTTON, "shotgun", "Toggles shotgun mode. If shotgun mode is on press right mouse button to shoot.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(17, GUI_Type.TOGGLEBUTTON, "nodamage", "Toggles all creatures' health, acting as invincibility.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(18, GUI_Type.TOGGLEBUTTON, "alwaysday", "Sets time to day always.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(19, GUI_Type.TOGGLEBUTTON, "noinfect", "Player has immune to Kharaa Bacterium.", new GUI_textColor(normal: GUI_Color.Red)),
            new GUI_content(20, GUI_Type.TOGGLEBUTTON, "overpower", "Toggle the Player maximal Health, Oxygen capacity, hunger and thirst interval to the value set in the configuration file or default.", new GUI_textColor(normal: GUI_Color.Red)),
        };

        private List<GUI_content> CategoryButtons = new List<GUI_content>()
        {
            new GUI_content(1, GUI_Type.TAB, "Vehicles"),
            new GUI_content(2, GUI_Type.TAB, "Tools"),
            new GUI_content(3, GUI_Type.TAB, "Equipment"),
            new GUI_content(4, GUI_Type.TAB, "Materials"),
            new GUI_content(5, GUI_Type.TAB, "Electronics"),
            new GUI_content(6, GUI_Type.TAB, "Upgrades"),
            new GUI_content(7, GUI_Type.TAB, "Food & Water"),
            new GUI_content(8, GUI_Type.TAB, "Loot & Drill"),
            new GUI_content(9, GUI_Type.TAB, "Herbivores"),
            new GUI_content(10, GUI_Type.TAB, "Carnivores"),
            new GUI_content(11, GUI_Type.TAB, "Parasites"),
            new GUI_content(12, GUI_Type.TAB, "Leviathan"),
            new GUI_content(13, GUI_Type.TAB, "Eggs"),
            new GUI_content(14, GUI_Type.TAB, "Sea: Seed"),
            new GUI_content(15, GUI_Type.TAB, "Land: Seed"),
            new GUI_content(16, GUI_Type.TAB, "Flora: Item"),
            new GUI_content(17, GUI_Type.TAB, "Sea: Spawn"),
            new GUI_content(18, GUI_Type.TAB, "Land: Spawn"),
            new GUI_content(19, GUI_Type.TAB, "Blueprints"),
            new GUI_content(20, GUI_Type.TAB, "Warp")
        };

        private List<GUI_content> Scroll_1 = new List<GUI_content>()
        {            
            new GUI_content(0, GUI_Type.NORMALBUTTON, "Item 1", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft)           
        };

        private List<GUI_content> Scroll_2 = new List<GUI_content>()
        {
            new GUI_content(1, GUI_Type.NORMALBUTTON, "Item 21", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(2, GUI_Type.NORMALBUTTON, "Item 22", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(3, GUI_Type.NORMALBUTTON, "Item 23", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(4, GUI_Type.NORMALBUTTON, "Item 24", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(5, GUI_Type.NORMALBUTTON, "Item 25", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(6, GUI_Type.NORMALBUTTON, "Item 26", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(7, GUI_Type.NORMALBUTTON, "Item 27", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(8, GUI_Type.NORMALBUTTON, "Item 28", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
            new GUI_content(9, GUI_Type.NORMALBUTTON, "Item 29", null, new GUI_textColor(), textAlign: TextAnchor.MiddleLeft),
        };


        TechnologyMatrix tMatrix;


        private void Awake()
        {
            DontDestroyOnLoad(this);

            tMatrix = new TechnologyMatrix();

            guiRoot = new GUI_root(this);

            Window window = new Window(windowRect, "Main Window", true, true, true, true, false);

            MAIN_WINDOW = new GUI_window(guiRoot, window, WindowControl)
            {
                Enabled = true
            };

            guiRoot.mainWindow = MAIN_WINDOW;

            Group_Commands = new GUI_group(1, MAIN_WINDOW.ID, guiRoot, Commands, ButtonControl, MAIN_WINDOW.RemainDrawableArea, "Commands", 4);

            Group_ToggleCommands = new GUI_group(2, MAIN_WINDOW.ID, guiRoot, ToggleCommands, ButtonControl, Group_Commands.RemainDrawableArea, "Toggle Commands",  4);

            Group_Categories = new GUI_group(3, MAIN_WINDOW.ID, guiRoot, CategoryButtons, ButtonControl, Group_ToggleCommands.RemainDrawableArea, "Categories", 4);

            ScrollView_Categories = new GUI_scrollView(4, MAIN_WINDOW.ID, guiRoot, ButtonControl, Scroll_2, Group_Categories.RemainDrawableArea, "Scroll View", 3);

            Slider_Seamoth = new GUI_horizontalSlider(2, MAIN_WINDOW.ID, guiRoot, SliderControl, "Seamoth speed multiplier:", ScrollView_Categories.RemainDrawableArea, Slider_Seamoth_Value, 1, 5);

            Slider_Exosuit = new GUI_horizontalSlider(3, MAIN_WINDOW.ID, guiRoot, SliderControl, "Exosuit speed multiplier:", Slider_Seamoth.RemainDrawableArea, Slider_Exosuit_Value, 1, 5);

            Slider_Cyclops = new GUI_horizontalSlider(4, MAIN_WINDOW.ID, guiRoot, SliderControl, "Cyclops speed multiplier:", Slider_Exosuit.RemainDrawableArea, Slider_Cyclops_Value, 1, 5);
        }

        private void SliderControl(GUI_event guiEvent)
        {
            switch (guiEvent.ItemID)
            {
                case 1:
                    break;

                case 2:
                    print($"Slider_Seamoth_Value: {guiEvent.Value}");
                    Slider_Seamoth_Value = guiEvent.Value;
                    Slider_Seamoth._sliderValue = Slider_Seamoth_Value;
                    break;

                case 3:
                    print($"Slider_Exosuit_Value: {guiEvent.Value}");
                    Slider_Exosuit_Value = guiEvent.Value;
                    Slider_Exosuit._sliderValue = Slider_Exosuit_Value;
                    break;

                case 4:
                    print($"Slider_Cyclops_Value: {guiEvent.Value}");
                    Slider_Cyclops_Value = guiEvent.Value;
                    Slider_Cyclops._sliderValue = Slider_Cyclops_Value;
                    break;
            }
        }

        private void ButtonControl(GUI_event guiEvent)
        {
            print($"WindowID: {guiEvent.WindowID}");
            print($"GroupID: {guiEvent.GroupID}");
            print($"ItemID: {guiEvent.ItemID}");
            print($"MouseButton: {guiEvent.MouseButton}");

            switch (guiEvent.GroupID)
            {
                case 1:
                    print($"Item state: {Group_Commands.GetItemByID(guiEvent.ItemID).State}");
                    break;

                case 2:
                    print($"Item state: {Group_ToggleCommands.GetItemByID(guiEvent.ItemID).State}");
                    break;

                case 3:
                    print($"Item state: {Group_Categories.GetItemByID(guiEvent.ItemID).State}");
                    break;

                case 4:
                    print($"Item state: {ScrollView_Categories._group.GetItemByID(guiEvent.ItemID).State}");
                    break;
            }
        }

        private void OnGUI()
        {
            MAIN_WINDOW.DrawWindow();            
        }

        public void WindowControl(int id)
        {
            Group_Commands.DrawGroup();
            Group_ToggleCommands.DrawGroup();
            Group_Categories.DrawGroup();
            ScrollView_Categories.DrawScrollView();
            
            if (Group_Categories.GetItemByID(1).State == GUI_State.PRESSED)
            {
                Slider_Seamoth.DrawHorizontalSlider();
                Slider_Exosuit.DrawHorizontalSlider();
                Slider_Cyclops.DrawHorizontalSlider();
            }
        }

        

    }
}
*/