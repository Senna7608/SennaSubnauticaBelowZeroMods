using System.Collections.Generic;
using UnityEngine;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using System.Collections;
using UWE;
using BZHelper;
using Nautilus.Utility;

namespace ModdedArmsHelperBZ
{
    [DisallowMultipleComponent]
    internal sealed class ArmsCacheManager : MonoBehaviour
    {
        internal GameObject exosuitResource;
        internal GameObject pipeResource;
        private GameObject InactiveCache;
        private GameObject Templates;

        public GameObject HookPrefab { get; private set; }
        internal GameObject ArmSocket { get; private set; }
        internal Texture2D ArmSocket_Illum_Green { get; private set; }
        internal Texture2D ArmSocket_Illum_Red { get; private set; }
        public TorpedoType[] TorpedoTypes { get; private set; }

        private Texture2D ArmSocket_MultiColorMask;

        internal readonly Dictionary<TechType, GameObject> ModdedArmPrefabs = new Dictionary<TechType, GameObject>();

        internal readonly Dictionary<ArmTemplate, GameObject> ArmsTemplateCache = new Dictionary<ArmTemplate, GameObject>();
                
        private TaskResult<bool> textureResult = new TaskResult<bool>();
        private TaskResult<bool> pipeResult = new TaskResult<bool>();
        private TaskResult<bool> exosuitResult = new TaskResult<bool>();
        private TaskResult<bool> socketResult = new TaskResult<bool>();
        private TaskResult<bool> cacheResult = new TaskResult<bool>();
               

        internal void Awake()
        {
            InactiveCache = UnityHelper.CreateGameObject("InactiveCache", transform, false);
            Templates = UnityHelper.CreateGameObject("Templates", InactiveCache.transform, false);

            CoroutineHost.StartCoroutine(LoadTexturesAsync(textureResult));
            CoroutineHost.StartCoroutine(LoadPipeResourcesAsync(pipeResult));
            CoroutineHost.StartCoroutine(LoadExosuitResourceAsync(exosuitResult));
            CoroutineHost.StartCoroutine(InitArmSocketGraphics(socketResult));
            CoroutineHost.StartCoroutine(InitArmPrefabsCacheAsync(cacheResult));
            CoroutineHost.StartCoroutine(RegisterArmAsync());
        }

        internal IEnumerator RegisterArmAsync()
        {
            while (cacheResult.Get() != true)
            {
                yield return null;
            }

            BZLogger.Log("Processing of modded arms registration request has started...");

            foreach (KeyValuePair<CraftableModdedArm, IArmModdingRequest> kvp in ArmServices.main.waitForRegistration)
            {
                TaskResult<bool> registerResult = new TaskResult<bool>();
                CoroutineTask<bool> registerRequest = new CoroutineTask<bool>(ProcessNewArmAsync(kvp.Key, kvp.Value, registerResult), registerResult);
                yield return registerRequest;

                if (registerResult.Get() == false) continue;                                
            }

            BZLogger.Log("Processing of modded arms registration request has completed.");
            yield break;
        }

        internal IEnumerator ProcessNewArmAsync(CraftableModdedArm armPrefab, IArmModdingRequest armModdingRequest, IOut<bool> taskResult)
        {
            BZLogger.Log($"Processing data for new [{armPrefab.ArmType}], TechType: [{armPrefab.Info.TechType}], Request: [{armModdingRequest}]");

            if (ModdedArmPrefabs.ContainsKey(armPrefab.Info.TechType))
            {
                yield break;
            }

            string techName = TechTypeExtensions.AsString(armPrefab.Info.TechType);

            GameObject clonedArm = Instantiate(ArmsTemplateCache[armPrefab.ArmTemplate], InactiveCache.transform);

            LowerArmHelper graphicsHelper = clonedArm.AddComponent<LowerArmHelper>();

            ArmTag armTag = clonedArm.AddComponent<ArmTag>();
            armTag.armType = armPrefab.ArmType;
            armTag.armTemplate = armPrefab.ArmTemplate;
            armTag.techType = armPrefab.Info.TechType;

            clonedArm.name = techName;            
            clonedArm.SetActive(false);            
            
            TaskResult<bool> success = new TaskResult<bool>();
            CoroutineTask<bool> setupRequest = new CoroutineTask<bool>(armModdingRequest.SetUpArmAsync(clonedArm, graphicsHelper, success), success) ;
            yield return setupRequest;

            if (!success.Get())
            {
                BZLogger.Error($"SetUpArmAsync failed for TechType: [{techName}]!");
                taskResult.Set(false);
                yield break;
            }

            if (armPrefab.ArmType == ArmType.ExosuitArm)
            {
                armModdingRequest.GetExosuitArmHandler(clonedArm);
            }
            else
            {
                armModdingRequest.GetSeatruckArmHandler(clonedArm);
            }

            ModdedArmPrefabs.Add(armPrefab.Info.TechType, clonedArm);            

            BZLogger.Log($"Processing complete for new [{armPrefab.ArmType}], TechType ID: [{(int)armPrefab.Info.TechType}], TechType: [{techName}]");
            taskResult.Set(true);
            yield break;
        }

        internal void SyncArmTag(GameObject clone, TechType techType)
        {
            GameObject prefab = GetModdedArmPrefab(techType);
            ArmTag ArmTag_FROM = prefab.GetComponent<ArmTag>();
            ArmTag ArmTag_TO = clone.GetComponent<ArmTag>();

            ArmTag_TO.armType = ArmTag_FROM.armType;
            ArmTag_TO.armTemplate = ArmTag_FROM.armTemplate;
            ArmTag_TO.techType = ArmTag_FROM.techType;

            BZLogger.Debug($"SyncArmTag: Type: [{ArmTag_TO.armType}], Template: [{ArmTag_TO.armTemplate}], TechType: [{ArmTag_TO.techType}]");
        }

        internal ISeatruckArm SpawnArm(TechType techType, Transform parent)
        {
            BZLogger.Debug($"SpawnArm: techType: {techType}, Transform: {parent.name}");            
            
            GameObject arm = Instantiate(GetModdedArmPrefab(techType), parent, Vector3.zero, Quaternion.identity, false);
             
            SyncArmTag(arm, techType);

            arm.SetActive(true);

            return arm.GetComponent<ISeatruckArm>();
        }        

        internal GameObject GetModdedArmPrefab(TechType techType)
        {
            if (ModdedArmPrefabs.TryGetValue(techType, out GameObject armPrefab))
            {                           
                return armPrefab;
            }

            return null;
        }

        private IEnumerator LoadTexturesAsync(IOut<bool> result)
        {
            ArmSocket_MultiColorMask = ImageUtils.LoadTextureFromFile($"{ModdedArmsHelperBZ_Main.modFolder}/Assets/ArmSocket_MultiColorMask.png");
            ArmSocket_MultiColorMask.name = "ArmSocket_MultiColorMask";

            ArmSocket_Illum_Green = ImageUtils.LoadTextureFromFile($"{ModdedArmsHelperBZ_Main.modFolder}/Assets/ArmSocket_Illum_Green.png");
            ArmSocket_Illum_Green.name = "ArmSocket_Illum_Green";

            ArmSocket_Illum_Red = ImageUtils.LoadTextureFromFile($"{ModdedArmsHelperBZ_Main.modFolder}/Assets/ArmSocket_Illum_Red.png");
            ArmSocket_Illum_Red.name = "ArmSocket_Illum_Red";

            BZLogger.Log("Arm socket texture cache initialized.");

            result.Set(true);
            yield break;
        }

        private IEnumerator LoadPipeResourcesAsync(IOut<bool> result)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Tools/Pipe.prefab");

            yield return request;

            if (!request.TryGetPrefab(out GameObject prefab))
            {
                BZLogger.Error("An error occurred while loading [Pipe] prefab!");
                yield break;
            }

            pipeResource = UWE.Utils.InstantiateDeactivated(prefab, transform, Vector3.zero, Quaternion.identity);

            pipeResource.GetComponent<Rigidbody>().isKinematic = true;

            pipeResource.GetComponent<WorldForces>().enabled = false;

            Utils.ZeroTransform(pipeResource.transform);

            result.Set(true);
            yield break;
        }

        private IEnumerator LoadExosuitResourceAsync(IOut<bool> result)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Tools/Exosuit.prefab");

            yield return request;

            if (!request.TryGetPrefab(out GameObject prefab))
            {
                BZLogger.Error("An error occurred while loading [Exosuit] prefab!");
                yield break;
            }

            exosuitResource = UWE.Utils.InstantiateDeactivated(prefab, transform, Vector3.zero, Quaternion.identity);

            exosuitResource.GetComponent<Exosuit>().enabled = false;
            exosuitResource.GetComponent<Rigidbody>().isKinematic = true;
            exosuitResource.GetComponent<WorldForces>().enabled = false;
            UWE.Utils.ZeroTransform(exosuitResource.transform);

            result.Set(true);
            yield break;
        }

        private IEnumerator InitArmSocketGraphics(IOut<bool> result)
        {
            while (textureResult.Get() != true || pipeResult.Get() != true || exosuitResult.Get() != true)
            {
                yield return null;
            }

            GameObject pipes_end_cap = pipeResource.transform.Find("model/endcap/pipes_end_cap").gameObject;

            ArmSocket = Instantiate(pipes_end_cap, transform);

            ArmSocket.SetActive(false);

            ArmSocket.name = "ArmSocket";

            GameObject exoStorage = exosuitResource.transform.FindDeepChild("Exosuit_01_storage").gameObject;

            GameObject exoStorageModel = exoStorage.GetPrefabClone(transform, true, "model");

            Renderer exoStorageRenderer = exoStorageModel.GetComponent<Renderer>();

            Renderer armsocket_renderer = ArmSocket.GetComponent<Renderer>();

            exoStorageRenderer.material.CopyPropertiesFromMaterial(armsocket_renderer.material);

            Texture _MainTex = armsocket_renderer.material.GetTexture(Shader.PropertyToID("_MainTex"));

            Material material = armsocket_renderer.material;

            material.EnableKeyword("UWE_3COLOR");
            material.EnableKeyword("MARMO_EMISSION");
            material.SetTexture(Shader.PropertyToID("_MainTex"), _MainTex);

            material.SetTexture(Shader.PropertyToID("_Illum"), ArmSocket_Illum_Green);
            material.SetTexture(Shader.PropertyToID("_MultiColorMask"), ArmSocket_MultiColorMask);

            Destroy(exoStorageModel);

            BZLogger.Log($"Arm socket cache initialized. GameObject: [{ArmSocket.name}], Path: [{ArmSocket.transform.GetPath()}]");

            yield break;
        }

        private IEnumerator InitArmPrefabsCacheAsync(IOut<bool> result)
        {
            while (exosuitResult.Get() != true)
            {
                yield return null;
            }

            Exosuit exosuit = exosuitResource.GetComponent<Exosuit>();

            TorpedoTypes = (TorpedoType[])exosuit.torpedoTypes.Clone();

            GameObject clone, model;
            Transform ArmRig, elbow;

            foreach (Exosuit.ExosuitArmPrefab armPrefab in exosuit.armPrefabs)
            {
                clone = UWE.Utils.InstantiateDeactivated(armPrefab.prefab, Templates.transform, Vector3.zero, Quaternion.Euler(0, 0, 0));

                ArmRig = clone.transform.Find("exosuit_01_armRight/ArmRig");
                elbow = ArmRig.Find("clavicle/shoulder/bicepPivot/elbow/");

                UnityHelper.CreateGameObject("ModdedLowerArmContainer", elbow, Vector3.zero, Vector3.zero, Vector3.one);

                switch (armPrefab.techType)
                {
                    case TechType.ExosuitClawArmModule:
                        clone.name = "ClawArmTemplate";
                        DestroyImmediate(clone.GetComponent<ExosuitClawArm>());

                        DestroyImmediate(ArmRig.Find("exosuit_arm_torpedoLauncher_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_drill_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_hand_geo").gameObject);                        
                        DestroyImmediate(ArmRig.Find("exosuit_propulsion_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("grapplingHook").gameObject);
                        DestroyImmediate(elbow.Find("drill").gameObject);
                        DestroyImmediate(elbow.Find("grapplingBase").gameObject);
                        DestroyImmediate(elbow.Find("object").gameObject);
                        DestroyImmediate(elbow.Find("propulsion").gameObject);
                        DestroyImmediate(elbow.Find("torpedoLauncher").gameObject);

                        model = ArmRig.Find("exosuit_hand_geo").gameObject;
                        model.name = "ArmModel";

                        ArmsTemplateCache.Add(ArmTemplate.ClawArm, clone);
                        break;

                    case TechType.ExosuitDrillArmModule:
                        clone.name = "DrillArmTemplate";
                        DestroyImmediate(clone.GetComponent<ExosuitDrillArm>());

                        DestroyImmediate(ArmRig.Find("exosuit_arm_torpedoLauncher_geo").gameObject);                        
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_hand_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_hand_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_propulsion_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("grapplingHook").gameObject);                        
                        DestroyImmediate(elbow.Find("grapplingBase").gameObject);
                        DestroyImmediate(elbow.Find("object").gameObject);
                        DestroyImmediate(elbow.Find("propulsion").gameObject);
                        DestroyImmediate(elbow.Find("torpedoLauncher").gameObject);
                        DestroyImmediate(elbow.Find("wrist").gameObject);

                        model = ArmRig.Find("exosuit_drill_geo").gameObject;
                        model.name = "ArmModel";

                        ArmsTemplateCache.Add(ArmTemplate.DrillArm, clone);
                        break;

                    case TechType.ExosuitGrapplingArmModule:
                        clone.name = "GrapplingArmTemplate";

                        ExosuitGrapplingArm component = clone.GetComponent<ExosuitGrapplingArm>();

                        HookPrefab = Instantiate(component.hookPrefab, Templates.transform);
                        HookPrefab.name = "Hook";

                        DestroyImmediate(component);
                        
                        DestroyImmediate(elbow.Find("drill").gameObject);                        
                        DestroyImmediate(elbow.Find("object").gameObject);
                        DestroyImmediate(elbow.Find("propulsion").gameObject);
                        DestroyImmediate(elbow.Find("torpedoLauncher").gameObject);
                        DestroyImmediate(elbow.Find("wrist").gameObject);

                        model = ArmRig.Find("exosuit_grapplingHook_geo").gameObject;
                        model.name = "ArmModel";

                        ArmsTemplateCache.Add(ArmTemplate.GrapplingArm, clone);
                        break;

                    case TechType.ExosuitPropulsionArmModule:
                        clone.name = "PropulsionArmTemplate";
                        DestroyImmediate(clone.GetComponent<ExosuitPropulsionArm>());

                        DestroyImmediate(ArmRig.Find("exosuit_arm_torpedoLauncher_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_drill_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_hand_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_hand_geo").gameObject);                        
                        DestroyImmediate(ArmRig.Find("grapplingHook").gameObject);
                        DestroyImmediate(elbow.Find("drill").gameObject);
                        DestroyImmediate(elbow.Find("grapplingBase").gameObject);
                        DestroyImmediate(elbow.Find("object").gameObject);                        
                        DestroyImmediate(elbow.Find("torpedoLauncher").gameObject);
                        DestroyImmediate(elbow.Find("wrist").gameObject);

                        model = ArmRig.Find("exosuit_propulsion_geo").gameObject;
                        model.name = "ArmModel";

                        ArmsTemplateCache.Add(ArmTemplate.PropulsionArm, clone);
                        break;

                    case TechType.ExosuitTorpedoArmModule:
                        clone.name = "TorpedoArmTemplate";
                        DestroyImmediate(clone.GetComponent<ExosuitTorpedoArm>());
                        
                        DestroyImmediate(ArmRig.Find("exosuit_drill_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_grapplingHook_hand_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_hand_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("exosuit_propulsion_geo").gameObject);
                        DestroyImmediate(ArmRig.Find("grapplingHook").gameObject);
                        DestroyImmediate(elbow.Find("drill").gameObject);
                        DestroyImmediate(elbow.Find("grapplingBase").gameObject);
                        DestroyImmediate(elbow.Find("object").gameObject);
                        DestroyImmediate(elbow.Find("propulsion").gameObject);                        
                        DestroyImmediate(elbow.Find("wrist").gameObject);

                        model = ArmRig.Find("exosuit_arm_torpedoLauncher_geo").gameObject;
                        model.name = "ArmModel";

                        ArmsTemplateCache.Add(ArmTemplate.TorpedoArm, clone);
                        break;
                }                
            }
            
            BZLogger.Log($"Arm prefab cache initialized. GameObject: [{Templates.name}], Path: [{Templates.transform.GetPath()}]");

            ModdedArmsHelperBZ_Main.armsCacheManager = this;            
                       
            //DestroyImmediate(exosuitResource);
            result.Set(true);
            yield break;
        }
    }
}
