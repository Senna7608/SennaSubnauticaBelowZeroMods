using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.ArmControls;
using SeaTruckArms.ArmPrefabs;
using BZCommon.Helpers;
using BZCommon;
using System.Reflection;
using System.IO;
using SMLHelper.V2.Utility;

namespace SeaTruckArms
{
    public class SeatruckArms_Graphics
    {
        public GameObject GraphicsRoot { get; private set; }
        public ObjectHelper objectHelper { get; private set; }

        public Exosuit ExosuitResource { get; private set; }
        public GameObject ArmSocket { get; private set; }
        public Texture2D ArmSocket_Illum_Green { get; private set; }
        public Texture2D ArmSocket_Illum_Red { get; private set; }
        public TorpedoType[] TorpedoTypes { get; private set; }

        private Texture2D ArmSocket_MultiColorMask;                
        private ColorizationHelper colorizationHelper;
        
        private readonly Dictionary<ArmTemplate, GameObject> ArmsCache = new Dictionary<ArmTemplate, GameObject>();

        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public SeatruckArms_Graphics()
        {            
            GraphicsRoot = new GameObject("SeatruckArms_Graphics");            
            GraphicsRoot.AddComponent<Indestructible>();

            objectHelper = new ObjectHelper();
            colorizationHelper = new ColorizationHelper();

            ExosuitResource = Resources.Load<GameObject>("worldentities/tools/exosuit").GetComponent<Exosuit>();

            if (ExosuitResource == null)
            {
                BZLogger.Error("SeatruckArms", "Failed to load resource: [Exosuit]");
            }
            else
            {
                BZLogger.Log("SeatruckArms", "Resource loaded: [Exosuit]");
            }

            InitializeTextures();

            InitializeArmSocketGraphics();

            InitializeArmsGraphics();

            BZLogger.Log("SeaTruckArms", $"Seatruck Arms Graphics initialized. Name: {GraphicsRoot.name}, ID: {GraphicsRoot.GetInstanceID()}");

            Main.isGraphicsReady = true;
        }

        public ISeaTruckArm SpawnArm(TechType techType, Transform parent)
        {
            GameObject arm = Object.Instantiate(GetSeaTruckArmPrefab(techType));
            arm.transform.parent = parent.transform;
            arm.transform.localRotation = Quaternion.identity;
            arm.transform.localPosition = Vector3.zero;
            arm.SetActive(true);
            return arm.GetComponent<ISeaTruckArm>();
        }

        public GameObject GetSeaTruckArmPrefab(TechType techType)
        {
            if (techType == SeaTruckDrillArmPrefab.TechTypeID)
            {
                return ArmsCache[ArmTemplate.DrillArm];
            }
            else if (techType == SeaTruckClawArmPrefab.TechTypeID)
            {
                return ArmsCache[ArmTemplate.ClawArm];
            }
            else if (techType == SeaTruckGrapplingArmPrefab.TechTypeID)
            {
                return ArmsCache[ArmTemplate.GrapplingArm];
            }
            else if (techType == SeaTruckTorpedoArmPrefab.TechTypeID)
            {
                return ArmsCache[ArmTemplate.TorpedoArm];
            }

            return null;
        }

        private void InitializeTextures()
        {
            ArmSocket_MultiColorMask = ImageUtils.LoadTextureFromFile($"{modFolder}/Assets/ArmSocket_MultiColorMask.png");
            ArmSocket_MultiColorMask.name = "ArmSocket_MultiColorMask";

            ArmSocket_Illum_Green = ImageUtils.LoadTextureFromFile($"{modFolder}/Assets/ArmSocket_Illum_Green.png");
            ArmSocket_Illum_Green.name = "ArmSocket_Illum_Green";

            ArmSocket_Illum_Red = ImageUtils.LoadTextureFromFile($"{modFolder}/Assets/ArmSocket_Illum_Red.png");
            ArmSocket_Illum_Red.name = "ArmSocket_Illum_Red";

            BZLogger.Log("SeaTruckArms", "Texture cache initialized.");
        }

        private void InitializeArmSocketGraphics()
        {
            var resource = Resources.Load<GameObject>("worldentities/tools/pipe");
            
            GameObject pipes_end_cap = objectHelper.FindDeepChild(resource, "pipes_end_cap");

            ArmSocket = Object.Instantiate(pipes_end_cap, GraphicsRoot.transform);

            ArmSocket.SetActive(false);

            ArmSocket.name = "ArmSocket";

            GameObject exoStorage = objectHelper.FindDeepChild(ExosuitResource.transform, "Exosuit_01_storage");

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

            colorizationHelper.AddColorCustomizerToGameObject(ArmSocket);

            Object.Destroy(exoStorageModel);

            BZLogger.Log("SeaTruckArms", "Arm socket graphics cache initialized.");
        }


        private void InitializeArmsGraphics()
        {
            TorpedoTypes = (TorpedoType[])ExosuitResource.torpedoTypes.Clone();

            for (int i = 0; i < ExosuitResource.armPrefabs.Length; i++)
            {
                GameObject prefab = objectHelper.GetPrefabClone(ExosuitResource.armPrefabs[i].prefab, GraphicsRoot.transform, false);

                prefab.FindChild("exosuit_01_armRight").name = "seatruck_armRight";

                Object.DestroyImmediate(prefab.GetComponent<ColorCustomizer>());

                switch (ExosuitResource.armPrefabs[i].techType)
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
                        drillGeo.name = "seatruck_drill_geo";

                        prefab.AddComponent<SeaTruckDrillArmControl>();
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
                        handGeo.name = "seatruck_hand_geo";

                        prefab.AddComponent<SeaTruckClawArmControl>();
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
                        grapplingGeo.name = "seatruck_grapplingHook_geo";

                        prefab.AddComponent<SeaTruckGrapplingArmControl>();

                        ArmsCache.Add(ArmTemplate.GrapplingArm, prefab);
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
                        torpedoLauncherGeo.name = "seatruck_arm_torpedoLauncher_geo";

                        prefab.AddComponent<SeaTruckTorpedoArmControl>();

                        ArmsCache.Add(ArmTemplate.TorpedoArm, prefab);
                        break;
                }
            }

            Object.DestroyImmediate(GraphicsRoot.FindChild("ExosuitPropulsionArm(Clone)"));

            colorizationHelper.AddColorCustomizerToGameObject(GraphicsRoot);

            BZLogger.Log("SeaTruckArms", "Arms graphics cache initialized.");
        }
    }
}
