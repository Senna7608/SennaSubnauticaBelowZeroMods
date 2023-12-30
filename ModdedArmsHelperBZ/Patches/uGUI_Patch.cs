using BZHelper;
using HarmonyLib;
using UnityEngine;

namespace ModdedArmsHelperBZ.Patches
{
    [HarmonyPatch(typeof(uGUI_MainMenu))]
    [HarmonyPatch("Awake")]
    internal class uGUI_MainMenu_Awake_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_MainMenu __instance)
        {
            if (ModdedArmsHelperBZ_Main.helperRoot != null)
            {
                Object.DestroyImmediate(ModdedArmsHelperBZ_Main.helperRoot);
            }

            ModdedArmsHelperBZ_Main.helperRoot = new GameObject("ModdedArmsHelperBZ");

            ModdedArmsHelperBZ_Main.helperRoot.transform.SetParent(uGUI.main.transform, false);

            BZLogger.Debug($"GameObject: [{ModdedArmsHelperBZ_Main.helperRoot.name}] created. Parent: [{uGUI.main.gameObject.name}]");

            if (ModdedArmsHelperBZ_Main.cacheRoot != null)
            {
                Object.DestroyImmediate(ModdedArmsHelperBZ_Main.cacheRoot);
            }

            ModdedArmsHelperBZ_Main.cacheRoot = new GameObject("CacheRoot");

            ModdedArmsHelperBZ_Main.cacheRoot.transform.SetParent(ModdedArmsHelperBZ_Main.helperRoot.transform, false);

            ModdedArmsHelperBZ_Main.cacheRoot.AddComponent<ArmsCacheManager>();

            BZLogger.Debug($"GameObject: [{ModdedArmsHelperBZ_Main.cacheRoot.name}] created. Parent: [{ModdedArmsHelperBZ_Main.helperRoot.name}]");            
        }
    }
}
