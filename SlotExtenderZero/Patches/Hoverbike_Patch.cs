using HarmonyLib;
using BZHelper;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(Hoverbike))]
    [HarmonyPatch("Awake")]
    internal class Hoverbike_Awake_Patch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        internal static void Prefix(Hoverbike __instance)
        {               
            __instance.SetPrivateField("slotIDs", SlotHelper.NewHoverbikeSlotIDs);

            BZLogger.Debug($"Hoverbike slotIDs patched. ID: {__instance.GetInstanceID()}");            
        }
    }
}
