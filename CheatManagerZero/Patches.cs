using HarmonyLib;
using BZHelper;

namespace CheatManagerZero
{
    internal class Patches
    {
        [HarmonyPatch(typeof(Exosuit))]
        [HarmonyPatch("Awake")]
        internal class Exosuit_Awake_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(Exosuit __instance)
            {
                __instance.gameObject.EnsureComponent<ExosuitOverDrive>();
            }
        }

        /*
        [HarmonyPatch(typeof(Hoverbike))]
        [HarmonyPatch("Awake")]
        internal class Hoverbike_Start_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(Hoverbike __instance)
            {
                __instance.gameObject.EnsureComponent<HoverbikeOverDrive>();
            }
        }
        */

        [HarmonyPatch(typeof(PlayerController))]
        [HarmonyPatch("SetMotorMode")]
        internal class PlayerController_SetMotorMode_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(PlayerController __instance, Player.MotorMode newMotorMode)
            {
                if (Main.Instance != null)
                {
                    Main.Instance.onPlayerMotorModeChanged.Trigger(newMotorMode);                    
                }
            }
        }

        [HarmonyPatch(typeof(DevConsole))]
        [HarmonyPatch("Submit")]
        internal class DevConsole_Submit_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(DevConsole __instance, string value, bool __result)
            {
                if (__result)
                {
                    if (Main.Instance != null)
                    {
                        lock (value)
                        {
                            Main.Instance.onConsoleCommandEntered.Trigger(value);
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(FiltrationMachine))]
        [HarmonyPatch("OnConsoleCommand_filterfast")]
        internal class FiltrationMachine_OnConsoleCommand_filterfast_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(FiltrationMachine __instance)
            {
                if (Main.Instance != null)
                {
                    Main.Instance.onFilterFastChanged.Trigger((bool)__instance.GetPrivateField("fastFiltering"));
                }
            }
        }

        [HarmonyPatch(typeof(Seaglide))]
        [HarmonyPatch("Start")]
        internal class Seaglide_Start_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(Seaglide __instance)
            {
                __instance.gameObject.EnsureComponent<SeaglideOverDrive>();
            }
        }
        
        /*
        [HarmonyPatch(typeof(ResourceTracker))]
        [HarmonyPatch("Register")]
        public class ResourceTracker_Register_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ResourceTracker __instance)
            {
                if (__instance.overrideTechType == TechType.Fragment)
                {
                    if (__instance.rb != null)
                    {
                        FragmentTracker fragTracker = __instance.rb.gameObject.AddComponent<FragmentTracker>();
                        Main.trackerDb.Add(fragTracker);
                        BZLogger.Log($"Fragment gameobject added to database: [{__instance.rb.gameObject.name}]");
                    }                    
                }
            }
        }
        */
    }
}
