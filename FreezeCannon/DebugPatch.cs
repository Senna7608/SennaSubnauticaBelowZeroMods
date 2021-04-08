using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BZCommon;
using System.Reflection;
using UnityEngine;

namespace FreezeCannon
{
    /*
    [HarmonyPatch(typeof(LootDistributionData))]
    [HarmonyPatch("Initialize")]
    public class LootDistributionData_Initialize_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(LootDistributionData __instance)
        {
            foreach (KeyValuePair<string, LootDistributionData.SrcData> source in __instance.srcDistribution)
            {                
                BZLogger.Log($"\nprefabPath: {source.Value.prefabPath}");

                foreach (LootDistributionData.BiomeData biomeData in source.Value.distribution)
                {
                    BZLogger.Log($"biome: {biomeData.biome}, count: {biomeData.count}, probability: {biomeData.probability}");                    
                }
            }
        }
    }
    */
    /*
    [HarmonyPatch(typeof(PDAScanner))]
    [HarmonyPatch("Initialize")]        
    public class PDAScanner_Initialize_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            FieldInfo fieldInfo = AccessTools.Field(typeof(PDAScanner), "mapping");

            Dictionary<TechType, PDAScanner.EntryData> mapping = (Dictionary<TechType, PDAScanner.EntryData>)fieldInfo.GetValue(typeof(PDAScanner));

            foreach (KeyValuePair<TechType, PDAScanner.EntryData> entry in mapping)
            {
                BZLogger.Log("\nPDAScanner:");
                BZLogger.Log($"TechType: {entry.Key.ToString()}");

                BZLogger.Log($"blueprint: {entry.Value.blueprint}");
                BZLogger.Log($"destroyAfterScan: {entry.Value.destroyAfterScan}");
                BZLogger.Log($"encyclopedia: {entry.Value.encyclopedia}");
                BZLogger.Log($"isFragment: {entry.Value.isFragment}");
                BZLogger.Log($"locked: {entry.Value.locked}");
                BZLogger.Log($"scanTime: {entry.Value.scanTime}");
                BZLogger.Log($"totalFragments: {entry.Value.totalFragments}");
                BZLogger.Log($"unlockStoryGoal: {entry.Value.unlockStoryGoal}");
                BZLogger.Log($"StoryGoal: {entry.Value.StoryGoal}");
            }
        }
    }
    */
    /*
    [HarmonyPatch(typeof(SpriteManager))]
    [HarmonyPatch("Get")]
    [HarmonyPatch(new Type[] { typeof(TechType) })]
    public class SpriteManager_Get_Patch
    {
        static bool isPatched = false;

        [HarmonyPostfix]
        public static void Postfix(TechType techType)
        {
            if (isPatched)
                return;

            if (!SpriteManager.hasInitialized)
                return;

            FieldInfo fieldInfo = AccessTools.Field(typeof(SpriteManager), "atlases");

            Dictionary<string, Dictionary<string, Sprite>> atlases = (Dictionary<string, Dictionary<string, Sprite>>)fieldInfo.GetValue(typeof(SpriteManager));

            foreach (KeyValuePair<string, Dictionary<string, Sprite>> atlas in atlases)
            {                
                BZLogger.Log($"atlas: {atlas.Key}");

                foreach (KeyValuePair<string, Sprite> sprite in atlas.Value)
                {
                    BZLogger.Log($"sprite: {sprite.Key}");
                }
            }

            isPatched = true;
        }
    }
    */
    /*
    [HarmonyPatch(typeof(PDAEncyclopedia))]
    [HarmonyPatch("Initialize")]
    public class PDAEncyclopedia_Initialize_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            FieldInfo fieldInfo = AccessTools.Field(typeof(PDAEncyclopedia), "mapping");

            Dictionary<string, PDAEncyclopedia.EntryData> entries = (Dictionary<string, PDAEncyclopedia.EntryData>)fieldInfo.GetValue(typeof(PDAEncyclopedia));

            foreach (KeyValuePair<string, PDAEncyclopedia.EntryData> entry in entries)
            {
                BZLogger.Log("\nPDAEncyclopedia:");
                BZLogger.Log($"key: {entry.Value.key}");
                BZLogger.Log($"path: {entry.Value.path}");

                foreach (string node in entry.Value.nodes)
                {
                    BZLogger.Log($"node: {node}");
                }

                BZLogger.Log($"kind: {entry.Value.kind}");
                BZLogger.Log($"unlocked: {entry.Value.unlocked}");
                BZLogger.Log($"popup: {entry.Value.popup}");
                BZLogger.Log($"image: {entry.Value.image}");
                BZLogger.Log($"sound: {entry.Value.sound}");
                BZLogger.Log($"audio: {entry.Value.audio}");
                BZLogger.Log($"hidden: {entry.Value.hidden}");
            }
        }
    }
    */

    /*
    [HarmonyPatch(typeof(KnownTech))]
    [HarmonyPatch("Initialize")]
    public class KnownTech_Initialize_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            FieldInfo fieldInfo = AccessTools.Field(typeof(KnownTech), "analysisTech");

            List<KnownTech.AnalysisTech> analysisTech = (List<KnownTech.AnalysisTech>)fieldInfo.GetValue(typeof(KnownTech));

            foreach (KnownTech.AnalysisTech entry in analysisTech)
            {
                BZLogger.Log("\nKnownTech:");
                BZLogger.Log($"techType: {entry.techType.ToString()}");
                BZLogger.Log($"unlockMessage: {entry.unlockMessage}");

                try
                {
                    BZLogger.Log($"unlockSound: {entry.unlockSound.path}");
                }
                catch
                {
                    BZLogger.Log("unlockSound: null");
                    continue;
                }
                

                try
                {
                    BZLogger.Log($"unlockPopup: {entry.unlockPopup.name}");
                }
                catch
                {
                    BZLogger.Log("unlockPopup: null");
                    continue;
                }
            }
        }
    }
    */
}