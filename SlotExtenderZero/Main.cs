using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SlotExtenderZero.Configuration;
using SlotExtenderZero.Patches;
using BZCommon;
using System.Collections;

namespace SlotExtenderZero
{
    [QModCore]
    public static class Main
    {
        //public static CommandRoot commandRoot = null;            
        internal static Harmony hInstance;
        internal static InputFieldListener ListenerInstance { get; set; }

        internal static bool isConsoleActive;
        internal static bool isKeyBindigsUpdate = false;

        internal static bool uGUI_PrefixComplete = false;
        internal static bool uGUI_PostfixComplete = false;
        //internal static bool EquipmentPatched = false;
        internal static bool SeatruckUpgradesCtorPatched = false;
        internal static bool ExosuitCtorPatched = false;
        internal static bool ChipSlotsPatched = false;        
        
        [QModPatch]
        public static void Load()
        {
            try
            {
                //loading config from file
                SEzConfig.Config_Load();
                SlotHelper.InitSlotIDs();
                SlotHelper.ExpandSlotMapping();

                hInstance = new Harmony("BelowZero.SlotExtenderZero.mod");
                                              
                BZLogger.Debug($"Harmony instance created, Name = [{hInstance.Id}]");                

                hInstance.Patch(typeof(SeaTruckUpgrades).GetMethod("Start"), null, new HarmonyMethod(typeof(SeaTruckUpgrades_Start_Patch), "Postfix"));

                MethodBase STU_ctor = GetConstructorMethodBase(typeof(SeaTruckUpgrades), ".ctor");

                hInstance.Patch(STU_ctor, new HarmonyMethod(typeof(SeaTruckUpgrades_Constructor_Patch), "Prefix"));

                hInstance.Patch(typeof(DevConsole).GetMethod("SetState"), new HarmonyMethod(typeof(DevConsole_SetState_Patch), "Prefix"));

                MethodBase Exosuit_ctor = GetConstructorMethodBase(typeof(Exosuit), ".ctor");

                hInstance.Patch(Exosuit_ctor, new HarmonyMethod(typeof(Exosuit_Constructor_Patch), "Prefix"));

                /*
                MethodBase Hoverbike_ctor = GetConstructorMethodBase(typeof(Hoverbike), ".ctor");

                hInstance.Patch(Hoverbike_ctor, new HarmonyMethod(typeof(Hoverbike_Constructor_Patch), "Prefix"));
                */
                /*
                MethodBase Equipment_ctor_0 = GetConstructorMethodBase(typeof(Equipment), ".ctor");

                hInstance.Patch(Equipment_ctor_0, null, new HarmonyMethod(typeof(Equipment_Constructor_Patch), "Postfix"));
                    */
                /*
            hInstance.Patch(typeof(Equipment).GetMethod("GetSlotType"),
            new HarmonyMethod(typeof(Equipment_GetSlotType_Patch), "Prefix"));
            */



                /*
                hInstance.Patch(typeof(Equipment).GetMethod("AllowedToAdd"),
                    new HarmonyMethod(typeof(Equipment_AllowedToAdd_Patch), "Prefix"));
                    */

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

                hInstance.Patch(typeof(Hoverbike).GetMethod("Awake",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.SetField),
                    new HarmonyMethod(typeof(Hoverbike_Awake_Patch), "Prefix"));

                /*
                hInstance.Patch(typeof(Exosuit).GetProperty("slotIDs",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.GetProperty).GetGetMethod(true),
                    new HarmonyMethod(typeof(Exosuit_slotIDs_Patch), "Prefix"));
                    */


                hInstance.Patch(typeof(Exosuit).GetMethod("Awake"), null,
                    new HarmonyMethod(typeof(Exosuit_Awake_Patch), "Postfix"));

                hInstance.Patch(typeof(Inventory).GetMethod("UnlockDefaultEquipmentSlots",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance), null,
                    new HarmonyMethod(typeof(Inventory_UnlockDefaultEquipmentSlots_Patch), "Postfix"));

                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);

                //BZLogger.Debug("SlotExtenderZero", "Main.Load(): Added action OnSceneLoaded to SceneManager.sceneLoaded event.");

                // add console command for configuration window
                //commandRoot = new CommandRoot("SEzConfigGO");
                //commandRoot.AddCommand<SEzCommand>();       
                
                //CoroutineHost.StartCoroutine(WaitForUGUI());

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
            //EquipmentPatched = false;
            SeatruckUpgradesCtorPatched = false;            
            ExosuitCtorPatched = false;
            ChipSlotsPatched = false;
            GameInput.OnBindingsChanged -= GameInput_OnBindingsChanged;
        }

        /*
        public static IEnumerator WaitForUGUI()
        {
            while (!uGUI.isInitialized)
            {
                yield return new WaitForSeconds(1);
            }

            SEzConfig.Config_Init();

            GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;

            SlotHelper.InitSessionAllSlots();

            yield break;
        }
        */

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {            
            if (scene.name == "XMenu")
            {
                SEzConfig.Config_Init();

                GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;

                SlotHelper.InitSessionAllSlots();
            }
            
            if (scene.name == "Main")
            {
                // creating a console input field listener to skip SlotExdender Update method key events conflict while console is active in game
                ListenerInstance = InitializeListener();
            }
        }

        private static void GameInput_OnBindingsChanged()
        {
            BZLogger.Debug("Method call: Main.GameInput_OnBindingsChanged()");

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
                BZLogger.Debug($"found constructor [{ctorName}] in class [{type}]");

                if (ctor_info.Name == ctorName)
                {
                    return ctor_info as MethodBase;
                }
            }

            BZLogger.Debug($"the required constructor [{ctorName}] in class [{type}] has not found!");

            return null;
        }
    }    
}
