using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.ArmPrefabs;
using BZCommon.Helpers;
using BZCommon;
using SMLHelper.V2.Utility;
using SeaTruckArms.API;
using SeaTruckArms.API.Interfaces;
using SeaTruckArms.ArmHandlerRequesters;
using System.Collections;
using UWE;

namespace SeaTruckArms
{
    internal partial class SeaTruckArms_Graphics
    {
        public GameObject GraphicsRoot { get; private set; }

        public ObjectHelper objectHelper => ArmServices.main.objectHelper;
        public GameObject exosuitResource;
        public GameObject pipeResource;

        public GameObject ArmSocket { get; private set; }
        public Texture2D ArmSocket_Illum_Green { get; private set; }
        public Texture2D ArmSocket_Illum_Red { get; private set; }
        public TorpedoType[] TorpedoTypes { get; private set; }

        private Texture2D ArmSocket_MultiColorMask;

        internal readonly Dictionary<ArmTemplate, GameObject> ArmsCache = new Dictionary<ArmTemplate, GameObject>();

        internal readonly Dictionary<TechType, ArmTemplate> ArmTechTypes = new Dictionary<TechType, ArmTemplate>();

        internal readonly Dictionary<TechType, ISeaTruckArmHandlerRequest> ArmHandlers = new Dictionary<TechType, ISeaTruckArmHandlerRequest>();

        protected internal SeaTruckArms_Graphics()
        {            
            GraphicsRoot = new GameObject("SeaTruckArmsRoot");

            BZLogger.Log($"API message: Graphics root GameObject created: {GraphicsRoot.name}, ID: {GraphicsRoot.GetInstanceID()}");

            //SceneManager.MoveGameObjectToScene(GraphicsRoot, SceneManager.GetSceneByName("StartScreen"));

            GraphicsRoot.AddComponent<Indestructible>();

            CoroutineHost.StartCoroutine(LoadPipeResourcesAsync());

            CoroutineHost.StartCoroutine(LoadExosuitResourcesAsync());            

            RegisterBaseArms();

            RegisterBaseArmHandlers();

            InitializeTextures();

            CoroutineHost.StartCoroutine(InitializeArmSocketGraphics());

            CoroutineHost.StartCoroutine(InitializeArmsGraphics());

            GraphicsRoot.AddComponent<ArmRegistrationListener>();            
        }        

        internal void RegisterBaseArms()
        {
            ArmTechTypes.Clear();

            ArmTechTypes.Add(SeaTruckClawArm_Prefab.TechTypeID, ArmTemplate.ClawArm);
            ArmTechTypes.Add(SeaTruckDrillArm_Prefab.TechTypeID, ArmTemplate.DrillArm);
            ArmTechTypes.Add(SeaTruckGrapplingArm_Prefab.TechTypeID, ArmTemplate.GrapplingArm);
            ArmTechTypes.Add(SeaTruckPropulsionArm_Prefab.TechTypeID, ArmTemplate.PropulsionArm);
            ArmTechTypes.Add(SeaTruckTorpedoArm_Prefab.TechTypeID, ArmTemplate.TorpedoArm);
                        
            BZLogger.Log("API message: Base arm techtypes registered.");
        }

        internal void RegisterBaseArmHandlers()
        {
            ArmHandlers.Clear();

            ArmHandlers.Add(SeaTruckClawArm_Prefab.TechTypeID, new ClawArmHandlerRequest());
            ArmHandlers.Add(SeaTruckDrillArm_Prefab.TechTypeID, new DrillArmHandlerRequest());
            ArmHandlers.Add(SeaTruckGrapplingArm_Prefab.TechTypeID, new GrapplingArmHandlerRequest());
            ArmHandlers.Add(SeaTruckPropulsionArm_Prefab.TechTypeID, new PropulsionArmHandlerRequest());
            ArmHandlers.Add(SeaTruckTorpedoArm_Prefab.TechTypeID, new TorpedoArmHandlerRequest());

            BZLogger.Log("API message: Base arm handlers registered.");
        }

        public void RegisterNewArm(CraftableSeaTruckArm armPrefab, ISeaTruckArmHandlerRequest armHandlerRequest)
        {
            if (ArmTechTypes.ContainsKey(armPrefab.TechType))
            {
                return;
            }

            ArmTechTypes.Add(armPrefab.TechType, armPrefab.ArmTemplate);
            ArmHandlers.Add(armPrefab.TechType, armHandlerRequest);

            string techName = TechTypeExtensions.AsString(armPrefab.TechType);

            BZLogger.Log($"API message: New arm techtype registered, ID:[{(int)armPrefab.TechType}], name:[{techName}]");
        }

        public ISeaTruckArmHandler SpawnArm(TechType techType, Transform parent)
        {
            GameObject arm = Object.Instantiate(GetSeaTruckArmPrefab(techType));

            arm.transform.parent = parent.transform;
            arm.transform.localRotation = Quaternion.identity;
            arm.transform.localPosition = Vector3.zero;            
            arm.SetActive(true);

            return GetSeaTruckArmHandler(techType, arm);
        }

        private ISeaTruckArmHandler GetSeaTruckArmHandler(TechType techType, GameObject arm)
        {
            if (ArmHandlers.TryGetValue(techType, out ISeaTruckArmHandlerRequest armHandler))
            {
                return armHandler.GetHandler(ref arm);
            }

            return null;
        }

        public GameObject GetSeaTruckArmPrefab(TechType techType)
        {
            if (ArmTechTypes.TryGetValue(techType, out ArmTemplate template))
            {
                return ArmsCache[template];
            }

            return null;
        }

        private void InitializeTextures()
        {
            ArmSocket_MultiColorMask = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/ArmSocket_MultiColorMask.png");
            ArmSocket_MultiColorMask.name = "ArmSocket_MultiColorMask";

            ArmSocket_Illum_Green = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/ArmSocket_Illum_Green.png");
            ArmSocket_Illum_Green.name = "ArmSocket_Illum_Green";

            ArmSocket_Illum_Red = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/ArmSocket_Illum_Red.png");
            ArmSocket_Illum_Red.name = "ArmSocket_Illum_Red";

            BZLogger.Log("API message: Texture cache initialized.");
        }

        private IEnumerator InitializeArmSocketGraphics()
        {
            while (pipeResource == null || exosuitResource == null)
            {
                yield return null;
            }

            GameObject pipes_end_cap = pipeResource.transform.Find("model/endcap/pipes_end_cap").gameObject;            

            ArmSocket = Object.Instantiate(pipes_end_cap, GraphicsRoot.transform);

            ArmSocket.SetActive(false);

            ArmSocket.name = "ArmSocket";            

            //GameObject exoStorage = exosuitResource.transform.Find("exosuit_01/root/geoChildren/upgrade_geoHldr/Exosuit_01_storage").gameObject;

            GameObject exoStorage = objectHelper.FindDeepChild(exosuitResource.transform, "Exosuit_01_storage").gameObject;

            objectHelper.GetPrefabClone(ref exoStorage, GraphicsRoot.transform, true, "model", out GameObject exoStorageModel);
            
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

            Object.Destroy(exoStorageModel);

            BZLogger.Log("API message: Arm socket graphics cache initialized.");
            
            yield break;
        }


        private IEnumerator InitializeArmsGraphics()
        {
            while (exosuitResource == null)
            {
                yield return null;
            }

            Exosuit exosuit = exosuitResource.GetComponent<Exosuit>();

            TorpedoTypes = (TorpedoType[])exosuit.torpedoTypes.Clone();
            
            for (int i = 0; i < exosuit.armPrefabs.Length; i++)
            {
                GameObject prefab = objectHelper.GetPrefabClone(exosuit.armPrefabs[i].prefab, GraphicsRoot.transform, false);                

                prefab.FindChild("exosuit_01_armRight").name = "seatruck_armRight";

                Object.DestroyImmediate(prefab.GetComponent<SkyApplier>());

                switch (exosuit.armPrefabs[i].techType)
                {
                    case TechType.ExosuitDrillArmModule:

                        prefab.name = "SeaTruckDrillArm";

                        Object.DestroyImmediate(prefab.GetComponent<ExosuitDrillArm>());
                        
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_arm_torpedoLauncher_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_propulsion_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingHook"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingBase"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "propulsion"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "object"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "torpedoLauncher"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "wrist"));

                        GameObject drillGeo = objectHelper.FindDeepChild(prefab, "exosuit_drill_geo");

                        drillGeo.name = "ArmModel";
                        
                        ArmsCache.Add(ArmTemplate.DrillArm, prefab);
                        break;

                    case TechType.ExosuitClawArmModule:

                        prefab.name = "SeaTruckClawArm";

                        Object.DestroyImmediate(prefab.GetComponent<ExosuitClawArm>());
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_arm_torpedoLauncher_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_drill_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_propulsion_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingHook"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingBase"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "propulsion"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "object"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "torpedoLauncher"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "drill"));

                        GameObject handGeo = objectHelper.FindDeepChild(prefab, "exosuit_hand_geo");
                        handGeo.name = "ArmModel";
                        
                        ArmsCache.Add(ArmTemplate.ClawArm, prefab);
                        break;

                    case TechType.ExosuitGrapplingArmModule:

                        prefab.name = "SeaTruckGrapplingArm";

                        GameObject grapplingBase = objectHelper.FindDeepChild(prefab, "grapplingBase");
                        GameObject hook = objectHelper.FindDeepChild(prefab, "hook");

                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_arm_torpedoLauncher_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_drill_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_propulsion_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "drill"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "object"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "propulsion"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "torpedoLauncher"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "wrist"));

                        GameObject grapplingGeo = objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_geo");
                        grapplingGeo.name = "ArmModel";                        

                        ArmsCache.Add(ArmTemplate.GrapplingArm, prefab);
                        break;

                    case TechType.ExosuitPropulsionArmModule:

                        prefab.name = "SeaTruckPropulsionArm";

                        Object.DestroyImmediate(prefab.GetComponent<ExosuitPropulsionArm>());

                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_arm_torpedoLauncher_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_drill_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingHook"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingBase"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "wrist"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "object"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "torpedoLauncher"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "drill"));

                        GameObject propulsionGeo = objectHelper.FindDeepChild(prefab, "exosuit_propulsion_geo");
                        propulsionGeo.name = "ArmModel";

                        //ColorizationHelper.AddRendererToColorCustomizer(prefab, propulsionGeo, true, new int[] { 0, 1 });

                        ArmsCache.Add(ArmTemplate.PropulsionArm, prefab);
                        break;

                    case TechType.ExosuitTorpedoArmModule:

                        prefab.name = "SeaTruckTorpedoArm";

                        Object.DestroyImmediate(prefab.GetComponent<ExosuitTorpedoArm>());

                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_propulsion_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_drill_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_grapplingHook_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "exosuit_hand_geo"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingHook"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "grapplingBase"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "wrist"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "object"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "propulsion"));
                        Object.DestroyImmediate(objectHelper.FindDeepChild(prefab, "drill"));

                        GameObject torpedoLauncherGeo = objectHelper.FindDeepChild(prefab, "exosuit_arm_torpedoLauncher_geo");
                        torpedoLauncherGeo.name = "ArmModel";

                        //ColorCustomizer torpedoColorCustomizer = prefab.GetComponent<ColorCustomizer>();
                        //torpedoColorCustomizer.isBase = false;                        

                        //ColorizationHelper.AddRendererToColorCustomizer(prefab, torpedoLauncherGeo, true, new int[] { 0, 1 });

                        //SkyApplier torpedoSkyApplier = prefab.GetComponent<SkyApplier>();
                        //torpedoSkyApplier.renderers = new Renderer[1] { torpedoColorCustomizer.colorDatas[0].renderer };

                        ArmsCache.Add(ArmTemplate.TorpedoArm, prefab);
                        break;
                }
            }            

            BZLogger.Log("API message: Arms graphics cache initialized.");

            yield break;
        }        
    }
}
