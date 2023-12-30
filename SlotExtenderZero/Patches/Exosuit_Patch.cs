using HarmonyLib;
using BZHelper;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("slotIDs", MethodType.Getter)]
    public class Exosuit_slotIDs_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref string[] __result)
        {
            __result = SlotHelper.SessionExosuitSlotIDs;
            return false;
        }
    }    

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("Awake")]
    internal class Exosuit_Awake_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.First)]
        internal static void Postfix(Exosuit __instance)
        {
            __instance.gameObject.EnsureComponent<SlotExtenderZeroControl>();

            BZLogger.Debug($"SlotExtenderZeroControl added in Exosuit.Awake->Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
}
