using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using BZCommon;
using SlotExtenderZero.Configuration;
using SlotExtenderZero.Patches;

namespace SlotExtenderZero
{
    public static class Main
    {
        public static CommandRoot commandRoot = null;            
        public static Harmony hInstance;
        internal static InputFieldListener ListenerInstance { get; set; }

        public static bool isConsoleActive;
        public static bool isKeyBindigsUpdate = false;

        public static bool uGUI_PrefixComplete = false;
        public static bool uGUI_PostfixComplete = false;
        public static bool EquipmentPatched = false;
        public static bool SeatruckUpgradesPatched = false;

        public static void Load()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: Main.Load()");

            try
            {
                //loading config from file
                SEzConfig.Config_Load();
                SlotHelper.InitSlotIDs();                

                hInstance = new Harmony("BelowZero.SlotExtenderZero.mod");
                                              
                BZLogger.Debug("SlotExtenderZero", $"Harmony instance created, Name = [{hInstance.Id}]");

                hInstance.Patch(typeof(SeaTruckUpgrades).GetMethod("Start"), null,
                    new HarmonyMethod(typeof(SeaTruckUpgrades_Start_Patch), "Postfix"));

                MethodBase STU_ctor_0 = GetConstructorMethodBase(typeof(SeaTruckUpgrades), ".ctor");

                hInstance.Patch(STU_ctor_0, new HarmonyMethod(typeof(SeaTruckUpgrades_Constructor_Patch), "Prefix"));

                hInstance.Patch(typeof(DevConsole).GetMethod("SetState"),
                    new HarmonyMethod(typeof(DevConsole_SetState_Patch), "Prefix"));

                /*
                MethodBase Equipment_ctor_0 = GetConstructorMethodBase(typeof(Equipment), ".ctor");

                hInstance.Patch(Equipment_ctor_0, null, new HarmonyMethod(typeof(Equipment_Constructor_Patch), "Postfix"));
                    */
                hInstance.Patch(typeof(Equipment).GetMethod("GetSlotType"),
                new HarmonyMethod(typeof(Equipment_GetSlotType_Patch), "Prefix"));

                hInstance.Patch(typeof(Equipment).GetMethod("AllowedToAdd"),
                    new HarmonyMethod(typeof(Equipment_AllowedToAdd_Patch), "Prefix"));                

                hInstance.Patch(typeof(uGUI_QuickSlots).GetMethod("SetBackground",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance),
                    new HarmonyMethod(typeof(uGUI_QuickSlots_SetBackground_Patch), "Prefix"));

                hInstance.Patch(typeof(uGUI_Equipment).GetMethod("Awake",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.SetField),
                    new HarmonyMethod(typeof(uGUI_Equipment_Awake_Patch), "Prefix"),
                    new HarmonyMethod(typeof(uGUI_Equipment_Awake_Patch), "Postfix"));                

                hInstance.Patch(typeof(Exosuit).GetProperty("slotIDs",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.GetProperty).GetGetMethod(true),
                    new HarmonyMethod(typeof(Exosuit_slotIDs_Patch), "Prefix"));

                hInstance.Patch(typeof(Exosuit).GetMethod("Awake"), null,
                    new HarmonyMethod(typeof(Exosuit_Awake_Patch), "Postfix"));                

                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);

                BZLogger.Debug("SlotExtenderZero", "Main.Load(): Added action OnSceneLoaded to SceneManager.sceneLoaded event.");

                // add console command for configuration window
                commandRoot = new CommandRoot("SEzConfigGO");
                commandRoot.AddCommand<SEzCommand>();

                IngameMenuHandler.Main.RegisterOnQuitEvent(OnQuitEvent);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }                
        }

        private static void OnQuitEvent()
        {
            uGUI_PrefixComplete = false;
            uGUI_PostfixComplete = false;
            EquipmentPatched = false;
            SeatruckUpgradesPatched = false;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "StartScreen")
            {                
                // enabling game console
                DevConsole.disableConsole = false;

                // init config
                SEzConfig.Config_Init();

                // add an action if changed keybindings
                GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;

                SlotHelper.InitSessionAllSlots();
            }
            if (scene.name == "Main")
            {
                // creating a console input field listener to skip SlotExdender Update method key events conflict while console is active in game
                ListenerInstance = InitializeListener();
            }
        }

        public static void GameInput_OnBindingsChanged()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: Main.GameInput_OnBindingsChanged()");

            // SlotExtender Update() method now disabled until all keybinding updates are complete
            isKeyBindigsUpdate = true;

            // updating slot key bindings
            SEzConfig.SLOTKEYBINDINGS_Update();

            // synchronizing keybindings to config file
            SEzConfig.SLOTKEYBINDINGS_SyncToAll();

            // updating ALLSLOTS dictionary
            SlotHelper.ALLSLOTS_Update();

            // updating SlotTextHandler
            if (uGUI_SlotTextHandler.Instance != null)
            {
                uGUI_SlotTextHandler.Instance.UpdateSlotText();
            }

            // SlotExtender Update() method now enabled
            isKeyBindigsUpdate = false;
        }

        private static InputFieldListener InitializeListener()
        {
            if (ListenerInstance == null)
            {
                ListenerInstance = UnityEngine.Object.FindObjectOfType(typeof(InputFieldListener)) as InputFieldListener;

                if (ListenerInstance == null)
                {
                    GameObject inputFieldListener = new GameObject("InputFieldListener");
                    ListenerInstance = inputFieldListener.AddComponent<InputFieldListener>();
                }
            }

            return ListenerInstance;
        }

        private static MethodBase GetConstructorMethodBase(Type type, string ctorName)
        {
            List<ConstructorInfo> ctor_Infos = new List<ConstructorInfo>();

            ctor_Infos = AccessTools.GetDeclaredConstructors(type);

            foreach (ConstructorInfo ctor_info in ctor_Infos)
            {
                if (ctor_info.Name == ctorName)
                {
                    return ctor_info as MethodBase;
                }
            }

            BZLogger.Debug("SlotExtenderZero", $"the required constructor [{ctorName}] in class [{type}] has not found!");

            return null;
        }
    }    
}
