using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections;
using UnityEngine;
using UWE;

namespace BZHelper.NautilusHelpers
{
    internal abstract class ModPrefab_Vehicle : CustomPrefab
    {               
        protected readonly string FriendlyName;
        protected readonly string Description;
        public GameObject GameObjectClone { get; set; } = null;

        protected readonly TechType PrefabTemplate;        
        
        protected readonly TechType RequiredForUnlock;
        protected readonly TechGroup GroupForPDA;
        protected readonly TechCategory CategoryForPDA;        
        protected readonly ModPrefab_Fragment _Fragment;

        private bool isEncyExists = false;        

        protected ModPrefab_Vehicle(
            string techTypeName,            
            string friendlyName,
            string description,
            TechType template,                    
            TechType requiredAnalysis,
            TechGroup groupForPDA,
            TechCategory categoryForPDA,            
            ModPrefab_Fragment fragment
            )
            : base(techTypeName, friendlyName, description)
        {                       
            FriendlyName = friendlyName;
            Description = description;
            PrefabTemplate = template;                        
            RequiredForUnlock = requiredAnalysis;
            GroupForPDA = groupForPDA;
            CategoryForPDA = categoryForPDA;            
            _Fragment = fragment;            
        }        

        public void Patch()
        {
            //TechType = TechTypeHandler.Main.AddTechType(TechTypeName, FriendlyName, Description, null, false);

            PrePatch();           
            
            CoroutineHost.StartCoroutine(PatchAsync());
        }

        private IEnumerator PatchAsync()
        {
            BZLogger.Debug($"{Info.ClassID}: Async Patch started...");

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
            BZLogger.Debug($"{Info.ClassID}: Internal patch started...");
            
            Sprite sprite = GetItemSprite();
            SpriteHandler.RegisterSprite(Info.TechType, sprite);

            
            CraftDataHandler.SetRecipeData(Info.TechType, GetRecipe());               
            CraftDataHandler.AddToGroup(GroupForPDA, CategoryForPDA, Info.TechType);            

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

                KnownTechHandler.SetAnalysisTechEntry(Info.TechType, new TechType[1] { Info.TechType }, _Fragment.UnlockSprite);                
            }
            else
            {
                KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { Info.TechType }, $"{FriendlyName} blueprint discovered!");
            }

            TabNode NewTabNode = GetTabNodeData();

            if (NewTabNode != null)
            {
                CraftTreeHandler.AddTabNode(NewTabNode.craftTree, NewTabNode.uniqueName, NewTabNode.displayName, NewTabNode.sprite);
            }

            foreach (CraftTreeType craftTreeType in GetCraftTreeTypesData().TreeTypes)
            {
                CraftTreeHandler.AddCraftingNode(craftTreeType.TreeType, Info.TechType, craftTreeType.StepsToTab);
            }

            PostPatch();

            SetGameObject(GetGameObjectAsync);
            Register();

            BZLogger.Log($"{Info.ClassID}: Internal Patch completed.");
        }              

        public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(PrefabTemplate);
            yield return request;

            GameObject result = request.GetResult();

            if (result == null)
            {
                BZLogger.Warn($"{Info.ClassID} : Cannot instantiate prefab from TechType!");
                yield break;
            }

            GameObjectClone = Object.Instantiate(result);

            TaskResult<bool> modifyResult = new TaskResult<bool>();
            CoroutineTask<bool> modifyrequest = new CoroutineTask<bool>(ModifyGameObjectAsync(modifyResult), modifyResult);
            yield return modifyrequest;

            if (!modifyrequest.GetResult())
            {
                BZLogger.Error($"ModifyGameObjectAsync failed for TechType: [{Info.ClassID}]");
                yield break;
            }

            gameObject.Set(GameObjectClone);

            yield break;
        }        

        protected abstract RecipeData GetRecipe();

        protected abstract EncyData GetEncyclopediaData();

        protected abstract CrafTreeTypesData GetCraftTreeTypesData();

        protected abstract TabNode GetTabNodeData();

        protected abstract IEnumerator ModifyGameObjectAsync(IOut<bool> success);

        protected abstract void PrePatch();

        protected abstract void PostPatch();        

        protected abstract Sprite GetItemSprite();
    }    
}
