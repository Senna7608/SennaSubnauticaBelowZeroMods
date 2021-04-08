using System;
using UnityEngine;
using System.Reflection;
using QModManager.API.ModLoading;
using System.IO;
using BZCommon;
using System.Collections.Generic;
using HarmonyLib;
using UWE;

namespace FreezeCannon
{
    [QModCore]
    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static AssetBundle assetBundle;

        [QModPatch]
        public static void Load()
        {
            try
            {
                assetBundle = AssetBundle.LoadFromFile($"{modFolder}/Assets/freeze_cannon.asset");

                //FreezeCannonFragmentPrefab frag = new FreezeCannonFragmentPrefab();

                //frag.Patch();

                new FreezeCannon_Prefab(null).Patch();
                

               // Assembly assembly = Assembly.GetExecutingAssembly();

                //Harmony.CreateAndPatchAll(assembly, $"BelowZero.{assembly.GetName().Name}.mod");

                //PrefabHelper.CreateMainMenuTestObject(TechType.PropulsionCannon);
                /*
                for (int i = -1; i < 32; i++)
                {
                    BZLogger.Log($"Layer [{i}], name: {LayerMask.LayerToName(i)}");
                }
                */
                
                
                
                

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
