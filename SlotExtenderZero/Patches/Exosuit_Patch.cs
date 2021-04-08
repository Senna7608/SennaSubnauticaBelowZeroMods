using BZCommon;
using HarmonyLib;
using System.Reflection;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(MethodType.Constructor)]
    internal class Exosuit_Constructor_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(Exosuit __instance)
        {
            if (Main.ExosuitCtorPatched)
            {
                BZLogger.Debug("Exosuit constructor already patched. Exit method.");
                return;
            }

            __instance.SetPrivateField("_slotIDs", SlotHelper.SessionExosuitSlotIDs, BindingFlags.Static);

            BZLogger.Debug($"Exosuit constructor patched. ID: {__instance.GetInstanceID()}");

            Main.ExosuitCtorPatched = true;
        }
    }
    

    /*
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
    */

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("Awake")]
    internal class Exosuit_Awake_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(Exosuit __instance)
        {
            __instance.gameObject.EnsureComponent<SlotExtenderZero>();

            BZLogger.Debug($"MonoBehaviour component added in Exosuit.Awake -> Postfix Patch. ID: {__instance.gameObject.GetInstanceID()}");
        }
    }
}
