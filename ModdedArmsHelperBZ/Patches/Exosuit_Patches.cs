using UnityEngine;
using HarmonyLib;
using ModdedArmsHelperBZ.API.Interfaces;
using BZHelper;

namespace ModdedArmsHelperBZ.Patches
{
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("GetArmPrefab")]
    internal class Exosuit_GetArmPrefab_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Exosuit __instance, TechType techType, ref GameObject __result)
        {
            if (ModdedArmsHelperBZ_Main.armsCacheManager != null && ModdedArmsHelperBZ_Main.armsCacheManager.ModdedArmPrefabs.ContainsKey(techType))
            {
                __result = ModdedArmsHelperBZ_Main.armsCacheManager.GetModdedArmPrefab(techType);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("SpawnArm")]
    internal class Exosuit_SpawnArm_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Exosuit __instance, TechType techType, Transform parent, ref IExosuitArm __result)
        {
            if (ModdedArmsHelperBZ_Main.armsCacheManager != null && ModdedArmsHelperBZ_Main.armsCacheManager.ModdedArmPrefabs.ContainsKey(techType))
            {
                ModdedArmsHelperBZ_Main.armsCacheManager.SyncArmTag(__result.GetGameObject(), techType);
                __result.GetGameObject().SetActive(true);                
            }            
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Exosuit_OnUpgradeModuleChange_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Exosuit __instance, int slotID, TechType techType, bool added)
        {
            if (ModdedArmsHelperBZ_Main.armsCacheManager != null && ModdedArmsHelperBZ_Main.armsCacheManager.ModdedArmPrefabs.ContainsKey(techType))
            {
                __instance.MarkArmsDirty();
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("UpdateColliders")]
    internal class Exosuit_UpdateColliders_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Exosuit __instance)
        {
            __instance.BroadcastMessage("ResetColors", SendMessageOptions.DontRequireReceiver);
        }
    }

    /*
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("UpdateUIText")]
    internal class Exosuit_UpdateUIText_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Exosuit __instance, ref bool hasPropCannon)
        {            
           IExosuitArm exosuitArmLeft = __instance.leftArmAttach.GetComponentInChildren<IExosuitArm>();

            if (exosuitArmLeft.GetGameObject().TryGetComponent(out ArmTag armTagLeft))
            {
                if (armTagLeft.armTemplate == ArmTemplate.PropulsionArm)
                {
                    hasPropCannon = true;
                    return true;
                }
            }

            IExosuitArm exosuitArmRight = __instance.rightArmAttach.GetComponentInChildren<IExosuitArm>();

            if (exosuitArmRight.GetGameObject().TryGetComponent(out ArmTag armTagRight))
            {
                if (armTagRight.armTemplate == ArmTemplate.PropulsionArm)
                {
                    hasPropCannon = true;
                    return true;
                }
            }

            return true;
        }
    }
    */

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("UpdateUIText")]
    internal class Exosuit_UpdateUIText_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Exosuit __instance, ref bool hasPropCannon)
        {
            IExosuitModdedArm exosuitArmLeft = __instance.leftArmAttach.GetComponentInChildren<IExosuitModdedArm>();

            if (exosuitArmLeft != null && exosuitArmLeft.GetCustomUseText(out string uiStringLeft))
            {
                HandReticle.main.SetTextRaw(HandReticle.TextType.Use, uiStringLeft);
                HandReticle.main.SetTextRaw(HandReticle.TextType.UseSubscript, __instance.GetPrivateField("uiStringPrimary") as string);
            }

            IExosuitModdedArm exosuitArmRight = __instance.leftArmAttach.GetComponentInChildren<IExosuitModdedArm>();

            if (exosuitArmRight != null && exosuitArmRight.GetCustomUseText(out string uiStringRight))
            {
                HandReticle.main.SetTextRaw(HandReticle.TextType.Use, uiStringRight);
                HandReticle.main.SetTextRaw(HandReticle.TextType.UseSubscript, __instance.GetPrivateField("uiStringPrimary") as string);
            }

        }
    }    
}
