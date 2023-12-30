using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HarmonyLib;
using SlotExtenderZero.Configuration;
using BepInEx;
using BepInEx.Logging;
using Nautilus.Utility;
using Nautilus.Handlers;
using Nautilus.Utility.ModMessages;
using SlotExtenderZero.API;

namespace SlotExtenderZero
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class SlotExtenderZero_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.slotextenderzero";
        private const string MODNAME = "SlotExtenderZero";
        private const string VERSION = "2.0";

        internal ManualLogSource BepinLogger;
        public static SlotExtenderZero_Main mInstance;
        internal static Harmony hInstance;
        public ModInbox mInbox;

        public void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            ModCamp.Main.Register(GUID, VERSION);

            SEzConfig.Load();            

            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);            

            OptionsPanelHandler.RegisterModOptions(new SEzOptions());

            SaveUtils.RegisterOnQuitEvent(OnQuitEvent);
        }        
          
        private static void OnQuitEvent()
        {
            Main.uGUI_PrefixComplete = false;
            Main.uGUI_PostfixComplete = false;            
            Main.ChipSlotsPatched = false;            
            
            GameInput.OnBindingsChanged -= GameInput_OnBindingsChanged;
        }     

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {            
            if (scene.name == "XMenu")
            {
                SEzConfig.isSeatruckArmsExists = ModCamp.Main.IsModPresent("com.senna.seatruckarms");
                SEzConfig.isSeatruckScannerModuleExists = ModCamp.Main.IsModPresent("com.senna.seatruckscannermodule");

                SlotHelper.InitSlotIDs();
                SlotHelper.ExpandSlotMapping();
                
                SEzConfig.Init();

                GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;                            

                SlotHelper.InitSessionAllSlots();                

                hInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);               
            }
            
            if (scene.name == "Main")
            {                
                Main.ListenerInstance = InitializeListener();
            }
        }

        private static void GameInput_OnBindingsChanged()
        {            
            Main.isKeyBindigsUpdate = true;
           
            SEzConfig.SLOTKEYBINDINGS_Update();
            
            SEzConfig.SyncKeyBindings();
            
            SlotHelper.ALLSLOTS_Update();
            
            if (uGUI_SlotTextHandler.Instance != null)
            {
                uGUI_SlotTextHandler.Instance.UpdateSlotText();
            }
           
            Main.isKeyBindigsUpdate = false;
        }

        private static InputFieldListener InitializeListener()
        {
            if (Main.ListenerInstance == null)
            {
                Main.ListenerInstance = UnityEngine.Object.FindObjectOfType(typeof(InputFieldListener)) as InputFieldListener;

                if (Main.ListenerInstance == null)
                {
                    GameObject inputFieldListener = new GameObject("InputFieldListener");
                    Main.ListenerInstance = inputFieldListener.AddComponent<InputFieldListener>();
                }
            }

            return Main.ListenerInstance;
        }
    }

    internal static class Main
    {
        internal static InputFieldListener ListenerInstance { get; set; }        

        internal static bool isConsoleActive;
        internal static bool isKeyBindigsUpdate = false;

        internal static bool uGUI_PrefixComplete = false;
        internal static bool uGUI_PostfixComplete = false;        
        
        internal static bool ChipSlotsPatched = false;

        internal static bool LazyInitializePrefixPatched = false;

        internal static bool LazyInitializePostfixPatched = false;

        internal static bool QuickSlotsPatched = false;        
    }    
}
