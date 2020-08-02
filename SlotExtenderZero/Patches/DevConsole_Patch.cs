using Harmony;

namespace SlotExtenderZero.Patches
{
    [HarmonyPatch(typeof(DevConsole))]
    [HarmonyPatch("SetState")]
    public class DevConsole_SetState_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(DevConsole __instance, bool value)
        {
            if (Main.ListenerInstance != null)
            {
                Main.ListenerInstance.onConsoleInputFieldActive.Update(value);
            }
        }
    }
}
