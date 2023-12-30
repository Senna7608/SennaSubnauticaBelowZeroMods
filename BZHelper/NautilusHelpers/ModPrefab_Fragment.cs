using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UWE;
using Nautilus.Assets;
using Nautilus.Handlers;

namespace BZHelper.NautilusHelpers
{
#pragma warning disable CS1591 // Missing XML documentation

    public abstract class ModPrefab_Fragment : CustomPrefab
    {
        //protected readonly string TechTypeName;
        public readonly string VirtualPrefabFilename;
        protected readonly string FriendlyName;
        protected readonly TechType FragmentTemplate;
        protected readonly string PrefabFilePath;
        protected readonly EntitySlot.Type SlotType;
        protected readonly bool PrefabZUp;
        protected readonly LargeWorldEntity.CellLevel CellLevel;
        protected readonly Vector3 LocalScale;                
        public readonly float ScanTime;
        public readonly int TotalFragments;
        public readonly bool DestroyAfterScan;

        public GameObject GameObjectClone { get; set; } = null;
        public Sprite UnlockSprite { get; private set; } = null;

        protected ModPrefab_Fragment(
            string techTypeName,
            string friendlyName,
            TechType template,
            string prefabFilePath,
            EntitySlot.Type slotType,
            bool prefabZUp,
            LargeWorldEntity.CellLevel cellLevel,
            Vector3 localScale,            
            float scanTime = 2,
            int totalFragments = 2,
            bool destroyAfterScan = true
            )
            : base(techTypeName, friendlyName, "")
        {            
            FriendlyName = friendlyName;
            FragmentTemplate = template;
            PrefabFilePath = prefabFilePath;
            SlotType = slotType;
            PrefabZUp = prefabZUp;
            CellLevel = cellLevel;
            LocalScale = localScale;            
            ScanTime = scanTime;
            TotalFragments = totalFragments;
            DestroyAfterScan = destroyAfterScan;
            VirtualPrefabFilename = $"{techTypeName}.prefab";
        }

        public void Patch()
        {
            //TechType = TechTypeHandler.Main.AddTechType(TechTypeName, FriendlyName, string.Empty, null, false);

            CoroutineHost.StartCoroutine(PatchAsync());
        }        

        private IEnumerator PatchAsync()
        {
            BZLogger.Trace($"{Info.ClassID}: Async Patch started...");

            while (!SpriteManager.hasInitialized)
            {                
                yield return null;
            }

            InternalPatch();
            BZLogger.Log($"{Info.ClassID}: Async Patch completed.");
            yield break;
        }

        private void InternalPatch()
        {
            BZLogger.Trace($"{Info.ClassID}: Internal patch started...");

            UnlockSprite = GetUnlockSprite();

            SpriteHandler.RegisterSprite(Info.TechType, UnlockSprite);

            //PrefabHandler.RegisterPrefab(this);            

            LootDistributionData.SrcData srcData = new LootDistributionData.SrcData()
            {
                prefabPath = VirtualPrefabFilename,
                distribution = GetBiomeDatas()
            };

            LootDistributionHandler.AddLootDistributionData(Info.ClassID, srcData);

            WorldEntityInfo EntityInfo = new WorldEntityInfo()
            {
                classId = Info.ClassID,
                techType = Info.TechType,
                slotType = SlotType,
                prefabZUp = PrefabZUp,
                cellLevel = CellLevel,
                localScale = LocalScale
            };

            WorldEntityDatabaseHandler.AddCustomInfo(Info.ClassID, EntityInfo);

            SetGameObject(GetGameObjectAsync);
            Register();

            BZLogger.Log($"{Info.ClassID}: Internal Patch completed.");
        }        

        public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            BZLogger.Trace($"GetGameObjectAsync started for TechType: [{Info.ClassID}]...");

            if (GameObjectClone != null)
            {                
                gameObject.Set(GameObjectClone);
                yield break;
            }

            GameObject result;

            if (!string.IsNullOrEmpty(PrefabFilePath))
            {
                BZLogger.Trace($"Prefab filename request started for fragment: {PrefabFilePath}");

                IPrefabRequest prefabRequest = PrefabDatabase.GetPrefabForFilenameAsync(PrefabFilePath);
                yield return prefabRequest;

                if (prefabRequest.TryGetPrefab(out result))
                {                    
                    GameObjectClone = UWE.Utils.InstantiateDeactivated(result, null, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    BZLogger.Error($"Cannot find prefab in PrefabDatabase at path '{PrefabFilePath}!");
                    yield break;
                }
            }            
            else if (FragmentTemplate != TechType.None)
            {
                BZLogger.Trace($"Prefab template request started for fragment: {FragmentTemplate}");

                CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(FragmentTemplate);
                yield return request;

                result = request.GetResult();

                if (result == null)
                {
                    BZLogger.Error($"Cannot instantiate prefab from TechType '{FragmentTemplate}'!");
                    yield break;
                }

                GameObjectClone = UWE.Utils.InstantiateDeactivated(result, null, Vector3.zero, Quaternion.identity);
            }

            GameObjectClone.name = Info.ClassID;

            PrefabIdentifier prefabIdentifier = GameObjectClone.GetComponent<PrefabIdentifier>();
            prefabIdentifier.ClassId = Info.ClassID;            

            TechTag techTag = GameObjectClone.EnsureComponent<TechTag>();
            techTag.type = Info.TechType;            

            ResourceTracker resourceTracker = GameObjectClone.GetComponent<ResourceTracker>();
            resourceTracker.overrideTechType = TechType.Fragment;

            TaskResult<bool> modifyResult = new TaskResult<bool>();
            CoroutineTask<bool> modifyRequest = new CoroutineTask<bool>(ModifyGameObjectAsync(modifyResult), modifyResult);
            yield return modifyRequest;
            if (!modifyResult.Get())
            {
                BZLogger.Error("ModifyGameObjectAsync failed!");
                yield break;
            }   
     
            gameObject.Set(GameObjectClone);

            BZLogger.Trace($"GetGameObjectAsync complete for fragment: {FriendlyName}");

            yield break;
        }        

        protected abstract IEnumerator ModifyGameObjectAsync(IOut<bool> success);

        protected abstract List<LootDistributionData.BiomeData> GetBiomeDatas();

        protected abstract Sprite GetUnlockSprite();        
    }
}
