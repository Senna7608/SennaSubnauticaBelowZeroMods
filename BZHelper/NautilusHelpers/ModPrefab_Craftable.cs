using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UWE;

namespace BZHelper.NautilusHelpers
{
    internal abstract class ModPrefab_Craftable : CustomPrefab
    {      
        protected readonly string FriendlyName;
        public GameObject GameObjectClone { get; set; } = null;

        protected readonly TechType PrefabTemplate;
        protected readonly string GameResourceFileName;

        protected readonly TechType RequiredForUnlock;
        protected readonly TechGroup GroupForPDA;
        protected readonly TechCategory CategoryForPDA;
        protected readonly EquipmentType TypeForEquipment;
        protected readonly QuickSlotType TypeForQuickslot;
        protected readonly CraftData.BackgroundType BackgroundType;
        protected readonly Vector2int ItemSize;        
        protected readonly ModPrefab_Fragment _Fragment;

        private bool isEncyExists = false;
        
        protected ModPrefab_Craftable(
            string techTypeName,            
            string friendlyName,
            string description,
            TechType template,
            string gamerResourceFileName,
            TechType requiredAnalysis,
            TechGroup groupForPDA,
            TechCategory categoryForPDA,
            EquipmentType equipmentType,
            QuickSlotType quickSlotType,
            CraftData.BackgroundType backgroundType,
            Vector2int itemSize,            
            ModPrefab_Fragment fragment
            )
            : base(techTypeName, friendlyName, description)
        {                     
            FriendlyName = friendlyName;
            PrefabTemplate = template;
            GameResourceFileName = gamerResourceFileName;
            RequiredForUnlock = requiredAnalysis;
            GroupForPDA = groupForPDA;
            CategoryForPDA = categoryForPDA;
            TypeForEquipment = equipmentType;
            TypeForQuickslot = quickSlotType;
            BackgroundType = backgroundType;
            ItemSize = itemSize;            
            _Fragment = fragment;            
        }        

        public void Patch()
        {
            PrePatch();
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

            Sprite sprite = GetItemSprite();
            SpriteHandler.RegisterSprite(Info.TechType, sprite);            
            CraftDataHandler.SetRecipeData(Info.TechType, GetRecipe());

            if (ItemSize.x > 0 && ItemSize.y > 0)
            {
                CraftDataHandler.SetItemSize(Info.TechType, ItemSize);
            }

            CraftDataHandler.AddToGroup(GroupForPDA, CategoryForPDA, Info.TechType);            
            CraftDataHandler.SetEquipmentType(Info.TechType, TypeForEquipment);
            CraftDataHandler.SetQuickSlotType(Info.TechType, TypeForQuickslot);
            CraftDataHandler.SetBackgroundType(Info.TechType, BackgroundType);

            EncyData encyData = GetEncyclopediaData();

            if (encyData != null)
            {
                isEncyExists = true;

                PDAEncyclopedia.EntryData entryData = new PDAEncyclopedia.EntryData()
                {
                    key = Info.ClassID,
                    path = EncyHelper.GetEncyPath(encyData.node),
                    nodes = EncyHelper.GetEncyNodes(encyData.node),
                    kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
                    unlocked = false,
                    popup = _Fragment != null ? _Fragment.UnlockSprite : sprite,
                    image = encyData.image,
                    audio = null,
                    hidden = false
                };

                PDAHandler.AddEncyclopediaEntry(entryData);

                LanguageHandler.SetLanguageLine($"Ency_{Info.ClassID}", encyData.title);
                LanguageHandler.SetLanguageLine($"EncyDesc_{Info.ClassID}", encyData.description);
            }

            if (RequiredForUnlock == TechType.None && _Fragment != null)
            {
                PDAScanner.EntryData scannerEntryData = new PDAScanner.EntryData()
                {
                    key = _Fragment.Info.TechType,
                    blueprint = Info.TechType,
                    destroyAfterScan = _Fragment.DestroyAfterScan,
                    encyclopedia = isEncyExists ? Info.ClassID : null,
                    isFragment = true,
                    locked = false,
                    scanTime = _Fragment.ScanTime,
                    totalFragments = _Fragment.TotalFragments,
                    unlockStoryGoal = false
                };

                PDAHandler.AddCustomScannerEntry(scannerEntryData);                                
            }            

            KnownTech.AnalysisTech analysisTech = new KnownTech.AnalysisTech()
            {
                techType = _Fragment != null ? _Fragment.Info.TechType : RequiredForUnlock,
                unlockMessage = Language.main.Get("EncyNotificationEntryUnlocked"),
                unlockSound = KnownTechHandler.DefaultUnlockData.BlueprintUnlockSound,
                unlockPopup = _Fragment != null ? _Fragment.UnlockSprite : sprite,
                unlockTechTypes = new List<TechType>() { Info.TechType }
            };

            //KnownTechHandler.AddRequirementForUnlock(Info.TechType, RequiredForUnlock);
            KnownTechHandler.SetAnalysisTechEntry(analysisTech);
            
            TabNode NewTabNode = GetTabNodeData();

            if (NewTabNode != null)
            {
                CraftTreeHandler.AddTabNode(NewTabNode.craftTree, NewTabNode.uniqueName, NewTabNode.displayName, NewTabNode.sprite);
            }

            foreach (CraftTreeType craftTreeType in GetCraftTreeTypesData().TreeTypes)
            {
                CraftTreeHandler.AddCraftingNode(craftTreeType.TreeType, Info.TechType, craftTreeType.StepsToTab);
            }

            SetGameObject(GetGameObjectAsync);
            Register();
            PostPatch();
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

            if (PrefabTemplate != TechType.None)
            {
                CoroutineTask<GameObject> getPrefabrequest = CraftData.GetPrefabForTechTypeAsync(PrefabTemplate);
                yield return getPrefabrequest;

                GameObject result = getPrefabrequest.GetResult();

                if (result == null)
                {
                    BZLogger.Error($"Cannot instantiate prefab from CraftData for: [{Info.ClassID}]");
                    yield break;
                }

                GameObjectClone = UWE.Utils.InstantiateDeactivated(result, null, Vector3.zero, Quaternion.identity);

                BZLogger.Trace($"CraftData.GetPrefabForTechTypeAsync comlpeted for: [{Info.ClassID}]");
            }
            else if (GameResourceFileName != null)
            {
                AsyncOperationHandle<GameObject> loadRequest = AddressablesUtility.LoadAsync<GameObject>(GameResourceFileName);

                yield return loadRequest;

                if (loadRequest.Status == AsyncOperationStatus.Failed)
                {
                    BZLogger.Error($"GameObject cannot be loaded from this location: [{GameResourceFileName}]");
                    yield break;
                }

                GameObject loadPrefab = loadRequest.Result;

                GameObjectClone = UWE.Utils.InstantiateDeactivated(loadPrefab, null, Vector3.zero, Quaternion.identity);

                BZLogger.Trace($"{Info.ClassID}: AddressablesUtility.LoadAsync comlpeted.");
            }

            TaskResult<bool> modifyResult = new TaskResult<bool>();
            CoroutineTask<bool> modifyrequest = new CoroutineTask<bool>(ModifyGameObjectAsync(modifyResult), modifyResult);
            yield return modifyrequest;

            if (!modifyResult.Get())
            {
                BZLogger.Error($"ModifyGameObjectAsync failed for TechType: [{Info.ClassID}]");
                yield break;
            }

            GameObjectClone.name = Info.ClassID;
            gameObject.Set(GameObjectClone);

            BZLogger.Trace($"GetGameObjectAsync completed for TechType: [{Info.ClassID}]");

            yield break;
        }

        protected abstract void PrePatch();
        protected abstract void PostPatch();

        protected abstract RecipeData GetRecipe();

        protected abstract EncyData GetEncyclopediaData();

        protected abstract CrafTreeTypesData GetCraftTreeTypesData();

        protected abstract TabNode GetTabNodeData();

        protected abstract IEnumerator ModifyGameObjectAsync(IOut<bool> success);      

        protected abstract Sprite GetItemSprite();
    }    
}
