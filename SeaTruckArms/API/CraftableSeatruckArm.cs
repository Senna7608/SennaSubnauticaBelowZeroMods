using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using BZCommon;
using BZCommon.Helpers.SMLHelpers;
using System.Collections;
using UWE;

namespace SeaTruckArms.API
{
    /// <summary>
    /// The abstract class to inherit when you want to add new Seatruck arm into the game.    
    /// </summary>
    public abstract class CraftableSeaTruckArm : ModPrefab
    {
        private readonly Dictionary<ArmTemplate, TechType> ArmTypes = new Dictionary<ArmTemplate, TechType>()
        {
            { ArmTemplate.ClawArm, TechType.ExosuitDrillArmModule },
            { ArmTemplate.DrillArm, TechType.ExosuitDrillArmModule },
            { ArmTemplate.GrapplingArm, TechType.ExosuitGrapplingArmModule },
            { ArmTemplate.PropulsionArm, TechType.ExosuitPropulsionArmModule },
            { ArmTemplate.TorpedoArm, TechType.ExosuitTorpedoArmModule },
        };

        protected readonly string TechTypeName;
        protected readonly string FriendlyName;
        protected readonly string Description;        
        protected readonly TechType PrefabForClone;
        protected readonly TechType RequiredForUnlock;
        protected readonly SeaTruckArmFragment _Fragment;

        private bool isEncyExists = false;

        /// <summary>
        /// The cloned arm template gameobject. Initialized internally.
        /// But can be manipulated in "ModifyGameObject" method.
        /// </summary>
        public GameObject PrefabClone { get; private set; }
        /// <summary>
		/// The arm type template. Initialized internally.        
		/// </summary>
        public ArmTemplate ArmTemplate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SeamothArms.API.CraftableSeamothArm"/> class, the basic class for any arm that can be crafted at a Vehicle Upgrade Console.
        /// </summary>
        /// <param name="techTypeName">The main internal identifier for this item. Your item's <see cref="T:TechType"/> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item. Typically seen in the PDA, inventory, or crafting screens.</param>
        /// <param name="iconFileName">The file name for this item's icon without ".png" extension. If this parameter null, the object receives the <see cref="T:SeamothArms.API.ArmTemplate"/> icon.</param>
        /// <param name="armTemplate">The template <see cref="T:SeamothArms.API.ArmTemplate"/> for cloning.</param>
        /// <param name="requiredForUnlock">The required <see cref="T:TechType"/> that must first be scanned or picked up to unlock the blueprint for this item.If You use fragment set this to TechType.None</param>
        /// <param name="fragment">The techtype fragment <see cref="T:SeamothArms.API.SeaTruckArmFragment"/>.</param>
        protected CraftableSeaTruckArm(
            string techTypeName,
            string friendlyName,
            string description,            
            ArmTemplate armTemplate,
            TechType requiredForUnlock,
            SeaTruckArmFragment fragment
            )
            : base(techTypeName, $"{techTypeName}.Prefab")
        {
            TechTypeName = techTypeName;
            FriendlyName = friendlyName;
            Description = description;            
            PrefabForClone = ArmTypes[armTemplate];
            RequiredForUnlock = requiredForUnlock;
            _Fragment = fragment;
            ArmTemplate = armTemplate;

            //IngameMenuHandler.Main.RegisterOnQuitEvent(OnQuitEvent);
        }

        /*
        private void OnQuitEvent()
        {
            Patch();
        }
        */

        public void Patch()
        {
            TechType = TechTypeHandler.Main.AddTechType(TechTypeName, FriendlyName, Description, null, false);            

            PrePatch();

            CoroutineHost.StartCoroutine(PatchAsync());
        }        

        private IEnumerator PatchAsync()
        {
            while (!SpriteManager.hasInitialized)
            {
                BZLogger.Debug($"{TechTypeName} Spritemanager is not ready!");
                yield return null;
            }

            BZLogger.Debug($"{TechTypeName} Async patch started.");

            Sprite sprite = GetItemSprite();
            SpriteHandler.Main.RegisterSprite(TechType, sprite);

            PrefabHandler.Main.RegisterPrefab(this);
            CraftDataHandler.Main.SetTechData(TechType, GetRecipe());            
            CraftDataHandler.Main.SetItemSize(TechType, new Vector2int(1, 1));
            CraftDataHandler.Main.AddToGroup(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, TechType);
            CraftDataHandler.Main.SetEquipmentType(TechType, (EquipmentType)200);
            CraftDataHandler.Main.SetQuickSlotType(TechType, QuickSlotType.Selectable);
            CraftDataHandler.Main.SetBackgroundType(TechType, CraftData.BackgroundType.ExosuitArm);

            EncyData encyData = GetEncyclopediaData();

            if (encyData != null)
            {
                isEncyExists = true;

                PDAEncyclopedia.EntryData entryData = new PDAEncyclopedia.EntryData()
                {
                    key = ClassID,
                    path = EncyHelper.GetEncyPath(encyData.node),
                    nodes = EncyHelper.GetEncyNodes(encyData.node),
                    kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
                    unlocked = false,
                    popup = _Fragment != null ? _Fragment.UnlockSprite : sprite,
                    image = encyData.image,
                    audio = null,
                    hidden = false
                };

                PDAEncyclopediaHandler.Main.AddCustomEntry(entryData);

                LanguageHandler.Main.SetLanguageLine($"Ency_{ClassID}", encyData.title);
                LanguageHandler.Main.SetLanguageLine($"EncyDesc_{ClassID}", encyData.description);
            }

            if (RequiredForUnlock == TechType.None && _Fragment != null)
            {
                PDAScanner.EntryData scannerEntryData = new PDAScanner.EntryData()
                {
                    key = _Fragment.TechType,
                    blueprint = TechType,
                    destroyAfterScan = _Fragment.DestroyAfterScan,
                    encyclopedia = isEncyExists ? ClassID : null,
                    isFragment = true,
                    locked = false,
                    scanTime = _Fragment.ScanTime,
                    totalFragments = _Fragment.TotalFragments,
                    unlockStoryGoal = false
                };

                PDAHandler.Main.AddCustomScannerEntry(scannerEntryData);

                KnownTechHandler.Main.SetAnalysisTechEntry(TechType, new TechType[1] { TechType }, _Fragment.UnlockSprite);                
            }
            else
            {
                KnownTechHandler.Main.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { TechType }, $"{FriendlyName} blueprint discovered!");
            }

            CraftTreeHandler.Main.AddTabNode(CraftTree.Type.Workbench, "SeaTruckWBUpgrades", "Seatruck Upgrades", SpriteManager.Get(TechType.SeaTruck));

            CraftTreeHandler.Main.AddCraftingNode(CraftTree.Type.Fabricator, TechType, new string[] { "Upgrades", "SeatruckUpgrades" });
            CraftTreeHandler.Main.AddCraftingNode(CraftTree.Type.Workbench, TechType, new string[] { "SeaTruckWBUpgrades" });
            CraftTreeHandler.Main.AddCraftingNode(CraftTree.Type.SeaTruckFabricator, TechType, new string[] { "Upgrades" });
            CraftTreeHandler.Main.AddCraftingNode(CraftTree.Type.SeamothUpgrades, TechType, new string[] { "SeaTruckUpgrade" });

            PostPatch();

            yield break;
        }
        
        /// <summary>
        /// INTERNAL ARM SPAWNING METHOD, DON'T OVERRIDE! Use ModifyGameObject() instead!
        /// Initializes the visual appearance of the arm in the open world or in the inventory.
        /// </summary>
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(PrefabForClone);
            yield return request;

            GameObject result = request.GetResult();

            if (result == null)
            {
                BZLogger.Warn("API message: Cannot instantiate prefab from TechType!");
                yield break;
            }

            PrefabClone = Object.Instantiate(result);
            
            PrefabClone.name = TechTypeName;

            if (ArmTemplate == ArmTemplate.ClawArm)
            {
                OverrideClawArm();
            }

            PrefabClone.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            PostModify();

            gameObject.Set(PrefabClone);

            yield break;
        }       

        private void OverrideClawArm()
        {
            Main.graphics.ArmsCache.TryGetValue(ArmTemplate.ClawArm, out GameObject armPrefab);

            SkinnedMeshRenderer smr = armPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            Mesh clawMesh = smr.sharedMesh;

            MeshFilter mf = PrefabClone.GetComponentInChildren<MeshFilter>();
            mf.sharedMesh = Object.Instantiate(clawMesh);
            mf.sharedMesh.name = "seatruck_hand_geo";

            MeshRenderer mr = PrefabClone.GetComponentInChildren<MeshRenderer>();
            mr.materials = (Material[])smr.materials.Clone();

            PrefabClone.transform.Find("model/exosuit_rig_armLeft:exosuit_drill_geo").gameObject.name = "seatruck_claw_arm_geo";

            Object.Destroy(PrefabClone.GetComponentInChildren<CapsuleCollider>());

            BoxCollider bc_1 = PrefabClone.FindChild("collider").AddComponent<BoxCollider>();

            bc_1.size = new Vector3(1.29f, 0.33f, 0.42f);
            bc_1.center = new Vector3(-0.53f, 0f, 0.04f);

            GameObject collider2 = new GameObject("collider2");
            collider2.transform.SetParent(PrefabClone.transform, false);
            collider2.transform.localPosition = new Vector3(-1.88f, 0.07f, 0.50f);
            collider2.transform.localRotation = Quaternion.Euler(0, 34, 0);

            BoxCollider bc_2 = collider2.AddComponent<BoxCollider>();
            bc_2.size = new Vector3(1.06f, 0.23f, 0.31f);
            bc_2.center = new Vector3(0, -0.08f, 0);            
        }

        //private Sprite GetResourceIcon(TechType techType)
        //{
        //    return SpriteManager.Get(techType);            
        //}

        /// <summary>
		/// This provides the <see cref="T:SMLHelper.V2.Crafting.TechData" /> instance used to designate how this arm is crafted.
		/// </summary>
        protected abstract RecipeData GetRecipe();

        protected abstract EncyData GetEncyclopediaData();

        /// <summary>
        /// Use this if You want anything else before base patch method (after TechTypeID set).
        /// </summary>
        protected abstract void PrePatch();

        /// <summary>
		/// Use this if You want anything else after base patch method.
		/// </summary>
        protected abstract void PostPatch();

        /// <summary>
		/// Use this if You want modify the arm gameobject.
		/// </summary>
        protected abstract void PostModify();

        protected abstract Sprite GetItemSprite();
    }
}
