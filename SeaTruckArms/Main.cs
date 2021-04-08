using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SeaTruckArms.ArmPrefabs;
using System.IO;
using System.Collections;
using UWE;
using BZCommon;
using UnityEngine;
using SeaTruckArms.API;
using System.Collections.Generic;

namespace SeaTruckArms
{
    [QModCore]
    public static class Main
    {
        internal static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static SeaTruckArms_Graphics graphics;

        internal static bool isGraphicsReady = false;
        internal static Dictionary<string, SeaTruckArmFragment> ArmFragmentPrefabs = new Dictionary<string, SeaTruckArmFragment>();

        [QModPatch]
        public static void Load()
        {           
            SeaTruckClawArmFragment_Prefab clawArmFragment = new SeaTruckClawArmFragment_Prefab();
            clawArmFragment.Patch();            
            new SeaTruckClawArm_Prefab(clawArmFragment).Patch();
                
            SeaTruckDrillArmFragment_Prefab drillArmFragment = new SeaTruckDrillArmFragment_Prefab();
            drillArmFragment.Patch();           
            new SeaTruckDrillArm_Prefab(drillArmFragment).Patch();

            SeaTruckGrapplingArmFragment_Prefab grapplingArmFragment = new SeaTruckGrapplingArmFragment_Prefab();
            grapplingArmFragment.Patch();           
            new SeaTruckGrapplingArm_Prefab(grapplingArmFragment).Patch();

            SeaTruckPropulsionArmFragment_Prefab propulsionArmFragment = new SeaTruckPropulsionArmFragment_Prefab();
            propulsionArmFragment.Patch();            
            new SeaTruckPropulsionArm_Prefab(propulsionArmFragment).Patch();

            SeaTruckTorpedoArmFragment_Prefab torpedoArmFragment = new SeaTruckTorpedoArmFragment_Prefab();
            torpedoArmFragment.Patch();            
            new SeaTruckTorpedoArm_Prefab(torpedoArmFragment).Patch();

            ArmFragmentPrefabs.Add(clawArmFragment.VirtualPrefabFilename, clawArmFragment);
            ArmFragmentPrefabs.Add(drillArmFragment.VirtualPrefabFilename, drillArmFragment);
            ArmFragmentPrefabs.Add(grapplingArmFragment.VirtualPrefabFilename, grapplingArmFragment);
            ArmFragmentPrefabs.Add(propulsionArmFragment.VirtualPrefabFilename, propulsionArmFragment);
            ArmFragmentPrefabs.Add(torpedoArmFragment.VirtualPrefabFilename, torpedoArmFragment);

            /*
            foreach (KeyValuePair<string, SeaTruckArmFragment> kvp in ArmFragmentPrefabs)
            {
                BZLogger.Debug($"prefabname: {kvp.Key}, ClassID: {kvp.Value.ClassID}, TechType: {kvp.Value.TechType}");
            }
            */

            Assembly assembly = Assembly.GetExecutingAssembly();

            Harmony.CreateAndPatchAll(assembly, $"BelowZero.{assembly.GetName().Name}.mod");            

            CoroutineHost.StartCoroutine(InitGraphicsAsync());
        }
        
        public static IEnumerator InitGraphicsAsync()
        {
            while (!uGUI.isInitialized)
            {
                BZLogger.Debug("uGUI is not ready!");
                yield return new WaitForSeconds(1);
            }

            BZLogger.Debug("API message: API initialize started.");

            graphics = new SeaTruckArms_Graphics();

            yield break;
        }
    }
}
