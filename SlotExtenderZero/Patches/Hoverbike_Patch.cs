using BZCommon;
using HarmonyLib;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(Hoverbike))]
    [HarmonyPatch("Awake")]
    internal class Hoverbike_Awake_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(Hoverbike __instance)
        {               
            __instance.SetPrivateField("slotIDs", SlotHelper.NewHoverbikeSlotIDs);

            BZLogger.Debug($"Hoverbike slotIDs patched. ID: {__instance.GetInstanceID()}");            
        }
    }
    
    
    /*
    [HarmonyPatch(typeof(Hoverbike))]
    [HarmonyPatch("Awake")]
    public class Hoverbike_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Hoverbike __instance)
        {
            __instance.gameObject.EnsureComponent<SlotExtenderZero>();

            BZLogger.Debug("SlotExtenderZero", $"MonoBehaviour component added in Hoverbike.Awake -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
    */
}
