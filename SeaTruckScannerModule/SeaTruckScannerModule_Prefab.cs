using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using BZCommon;
using BZCommon.Helpers;
using BZCommon.Helpers.SMLHelpers;
using UWE;

namespace SeaTruckScannerModule
{
    internal class SeaTruckScannerModule_Prefab : ModPrefab_Vehicle
    {
        public static TechType TechTypeID { get; private set; }

        private GameObject antenna, scannerUI, scannerUIRoot, powerSystem;
        public static Sprite scannerUI_background;
        public static Texture2D powerUI_background;

        internal SeaTruckScannerModule_Prefab(ModPrefab_Fragment fragment)
            : base(
                  techTypeName: "SeaTruckScannerModule",
                  friendlyName: "Seatruck Scanner Module",
                  description: "It locates resources and wrecks within a range of 150 m.",
                  template: TechType.SeaTruckFabricatorModule,
                  requiredAnalysis: TechType.None,
                  groupForPDA: TechGroup.Constructor,
                  categoryForPDA: TechCategory.Constructor,
                  fragment: fragment
                  )
        {
        }

        protected override void PrePatch()
        {
            TechTypeID = TechType;

            scannerUI_background = ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/scannerUI_background.png");
            powerUI_background = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/powerUI_background.png");            
        
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_PowerSystemLabel", "Scanner main power system");
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_PowerSystemInteract", "Access to the power supply system");
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_CabinUnpoweredText", "Status: Cabin is unpowered!");
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_MainUnpoweredText", "Status: Scanner is unpowered!");
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_FullUnpoweredText", "Status: Cabin and Scanner is unpowered!");
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_ScannerReadyText", "Status: Standby");
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_PilotScannerModule", "Pilot Scanner Module");
            LanguageHandler.Main.SetLanguageLine("SeatruckScanner_PowerSwitch", "Power Switch");
        }

        protected override void PostPatch()
        {
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Magnetite, 3)
                })
            };
        }

        protected override CrafTreeTypesData GetCraftTreeTypesData()
        {
            return new CrafTreeTypesData()
            {
                TreeTypes = new List<CraftTreeType>(new CraftTreeType[1]
                {
                    new CraftTreeType(CraftTree.Type.Constructor, new string[] { "Modules" } )
                })
            };
        }        

        protected override Sprite GetItemSprite()
        {            
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/Seatruck_ScannerModule_icon.png");
        }

        protected override EncyData GetEncyclopediaData()
        {
            Texture2D image = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/Seatruck_ScannerModule_ency.png");
            
            return new EncyData()
            {
                title = FriendlyName,
                description = 
                "The Scanner module can detect resources and wrecks within a range of 150 m.\n\n" +
                " - The module has its own wall-mounted power supply for the scanner, so it does not consume the power of the Seatruck.\n" +
                "Power consumption of the module:\n" +
                " - Standby: 0 energy/sec\n" +
                " - On (scanning resources within range): 0.1 energy/sec\n" +
                " - During scan (desired resource scan): 1 energy/sec",
                node = EncyNode.Vehicles,
                image = image
            };

        }


        protected override TabNode GetTabNodeData() => null;
        protected override void ModifyGameObject() { return; }

        /*
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            AsyncOperationHandle<GameObject> seatruckFabricatorModuleRequest = AddressablesUtility.LoadAsync<GameObject>("WorldEntities/Tools/SeaTruckFabricatorModule.prefab");
            yield return seatruckFabricatorModuleRequest;
            GameObject seatruckFabricatorModulePrefab = seatruckFabricatorModuleRequest.Result;

            _GameObject = UWE.Utils.InstantiateDeactivated(seatruckFabricatorModulePrefab);

            _GameObject.SetActive(true);

            gameObject.Set(_GameObject);
        }
        */

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {            
            AsyncOperationHandle<GameObject> loadRequest_01 = AddressablesUtility.LoadAsync<GameObject>("WorldEntities/Tools/SeaTruckFabricatorModule.prefab");
            
            yield return loadRequest_01;

            if (loadRequest_01.Status == AsyncOperationStatus.Failed)
            {
                BZLogger.Error("Cannot find GameObject in Resources folder at path 'WorldEntities/Tools/SeaTruckFabricatorModule.prefab'");
                yield break;
            }

            GameObject seatruckFabricatorModulePrefab = loadRequest_01.Result;
           
            GameObjectClone = UWE.Utils.InstantiateDeactivated(seatruckFabricatorModulePrefab, null, Vector3.zero, Quaternion.identity);

            SetupScannerModule();

            /*
            IAssetBundleWrapperCreateRequest bundleRequest = AssetBundleManager.LoadBundleAsync("basegeneratorpieces");
            yield return bundleRequest;
            IAssetBundleWrapper bundle = bundleRequest.assetBundle;
            IAssetBundleWrapperRequest loadRequest_02 = bundle.LoadAssetAsync<GameObject>("Assets/Prefabs/Base/GeneratorPieces/BaseMapRoom.prefab");
            yield return loadRequest_02;            
            GameObject maproom = loadRequest_02.asset as GameObject;
            */
            AsyncOperationHandle<GameObject> loadRequest_02 = AddressablesUtility.LoadAsync<GameObject>("Assets/Prefabs/Base/GeneratorPieces/BaseMapRoom.prefab");

            yield return loadRequest_02;

            if (loadRequest_02.Status == AsyncOperationStatus.Failed)
            {
                BZLogger.Error("Cannot find Prefab at path 'Assets/Prefabs/Base/GeneratorPieces/BaseMapRoom.prefab'");
                yield break;
            }

            GameObject maproom = loadRequest_02.Result;

            if (!maproom)
            {
                BZLogger.Debug("Failed to load MapRoom prefab!");
                yield break;
            }

            antenna = UWE.Utils.InstantiateDeactivated(maproom.transform.Find("Map_room_antenna").gameObject, GameObjectClone.transform, Vector3.zero, Quaternion.identity);
            
            AsyncOperationHandle<GameObject> loadRequest_03 = AddressablesUtility.LoadAsync<GameObject>("Submarine/Build/BatteryCharger.prefab");
            yield return loadRequest_03;
            GameObject batteryChargerPrefab = loadRequest_03.Result;

            GameObject powerSystemRoot = new GameObject("powerSystemRoot");
            powerSystemRoot.transform.SetParent(GameObjectClone.transform);
            powerSystemRoot.transform.localPosition = new Vector3(-1.06f, -0.20f, 1.66f);
            powerSystemRoot.transform.localRotation = Quaternion.Euler(0, 270, 180);
            powerSystemRoot.transform.localScale = new Vector3(1.78f, 1.78f, 1.0f);

            powerSystem = UWE.Utils.InstantiateDeactivated(batteryChargerPrefab, powerSystemRoot.transform, Vector3.zero, Quaternion.identity);
            
            AsyncOperationHandle<GameObject> loadRequest_04 = AddressablesUtility.LoadAsync<GameObject>("Submarine/Build/MapRoomFunctionality.prefab");
            yield return loadRequest_04;
            GameObject mapRoomFunctionalityPrefab = loadRequest_04.Result;

            scannerUIRoot = new GameObject("scannerRoot");
            scannerUIRoot.transform.SetParent(GameObjectClone.transform, false);
            scannerUIRoot.transform.localScale = new Vector3(1.14f, 1.14f, 1.14f);
            scannerUIRoot.transform.localPosition = new Vector3(1.37f, 0.07f, 1.60f);
            scannerUIRoot.transform.localRotation = Quaternion.Euler(0, 90, 0);

            scannerUI = UWE.Utils.InstantiateDeactivated(mapRoomFunctionalityPrefab.transform.Find("screen/scannerUI").gameObject, scannerUIRoot.transform, Vector3.zero, Quaternion.identity);

            SetupAntenna();
            SetupScannerUI();
            SetupPowerSystem();

            GameObjectClone.AddComponent<SeaTruckSegmentListener>();            
            
            gameObject.Set(GameObjectClone);            

            yield break;
        }
        

        private void SetupScannerModule()
        {
            GameObject FabricatorSpawn = GameObjectClone.transform.Find("FabricatorSpawn").gameObject;
            UnityEngine.Object.DestroyImmediate(FabricatorSpawn);
            
            GameInfoIcon gameInfoIcon = GameObjectClone.GetComponent<GameInfoIcon>();
            gameInfoIcon.techType = TechType;

            SeaTruckMotor seaTruckMotor = GameObjectClone.GetComponent<SeaTruckMotor>();
            seaTruckMotor.mouseOver = Language.main.Get("SeatruckScanner_PilotScannerModule");           
        }


        private void SetupAntenna()
        {
            antenna.name = "antenna";
            antenna.transform.localPosition = new Vector3(0f, -0.19f, 1.34f);
            antenna.transform.localScale = new Vector3(0.44f, 0.44f, 0.44f);

            UnityEngine.Object.DestroyImmediate(antenna.transform.Find("root/wRod_jnt").gameObject);
            UnityEngine.Object.DestroyImmediate(antenna.transform.Find("root/bulb").gameObject);

            GameObject scanner_root = antenna.transform.Find("root/scanner_root").gameObject;
            scanner_root.transform.localScale = new Vector3(1f, 1.70f, 1f);

            GameObject scanner_headPivot = scanner_root.transform.Find("scanner_headPivot").gameObject;
            scanner_headPivot.transform.localScale = new Vector3(1.2f, 0.70f, 0.70f);

            BoxCollider bc = scanner_headPivot.AddComponent<BoxCollider>();

            bc.center = new Vector3(0f, 0.36f, -0.05f);
            bc.size = new Vector3(3.73f, 0.33f, 1.68f);            

            GameObject antennaModel = antenna.transform.Find("BaseMapRoomExteriorRadar_geo").gameObject;
            antennaModel.name = "antennaModel";

            ColorizationHelper.AddRendererToColorCustomizer(GameObjectClone, antennaModel, false, new int[] { 0 });
            ColorizationHelper.AddRendererToSkyApplier(GameObjectClone, antennaModel, Skies.Auto);            

            antenna.SetActive(true);
        }


        private void SetupPowerSystem()
        {
            powerSystem.name = "powerSystem";
            UnityEngine.Object.DestroyImmediate(powerSystem.GetComponent<BatteryCharger>());
            UnityEngine.Object.DestroyImmediate(powerSystem.GetComponent<PrefabIdentifier>());            
            UnityEngine.Object.DestroyImmediate(powerSystem.GetComponent<TechTag>());
            UnityEngine.Object.DestroyImmediate(powerSystem.GetComponent<Constructable>());
            UnityEngine.Object.DestroyImmediate(powerSystem.GetComponent<FMOD_StudioEventEmitter>());
            UnityEngine.Object.DestroyImmediate(powerSystem.GetComponent<ConstructableBounds>());
            UnityEngine.Object.DestroyImmediate(powerSystem.GetComponent<PowerConsumer>());
            UnityEngine.Object.DestroyImmediate(powerSystem.transform.Find("Collider").gameObject);

            GameObject EquipmentRoot = powerSystem.transform.Find("EquipmentRoot").gameObject;
            EquipmentRoot.transform.SetParent(GameObjectClone.transform, false);

            Transform model = powerSystem.transform.Find("model");

            UnityEngine.Object.DestroyImmediate(model.GetComponent<Animator>());
            UnityEngine.Object.DestroyImmediate(model.GetComponent<BaseModuleLighting>());
            
            GameObject cover = model.Find("battery_charging_station_base/battery_charging_station_cover").gameObject;
            cover.transform.SetParent(model, false);

            UnityEngine.Object.DestroyImmediate(model.Find("battery_charging_station_base").gameObject);
            
            SkyApplier skyApplier = model.gameObject.GetComponent<SkyApplier>();            

            LightingController controller = GameObjectClone.GetComponent<LightingController>();

            controller.RegisterSkyApplier(skyApplier);            
            
            GameObject UI = powerSystem.transform.Find("UI").gameObject;

            UI.transform.localPosition = new Vector3(0f, 0f, 0.28f);
            UI.transform.localRotation = Quaternion.Euler(0, 0, 180);                      

            GameObject backgroundLeft = UI.transform.transform.Find("Background").gameObject;
            backgroundLeft.name = "backgroundLeft";

            RectTransform leftRt = backgroundLeft.transform as RectTransform;
            leftRt.anchoredPosition = new Vector2(-495f, 0f);
            leftRt.localScale = new Vector3(0.70f, 0.70f, 0.1f);            

            GameObject backgroundRight = UWE.Utils.Instantiate(backgroundLeft, UI.transform, Vector3.zero, Quaternion.identity);
            backgroundRight.name = "backgroundRight";

            RectTransform rightRt = backgroundRight.transform as RectTransform;
            rightRt.anchoredPosition = new Vector2(500f, 0f);
            rightRt.localScale = new Vector3(0.70f, 0.70f, 0.1f);
            rightRt.SetSiblingIndex(0);

            RawImage imageLeft = backgroundLeft.GetComponent<RawImage>();
            RawImage imageRight = backgroundRight.GetComponent<RawImage>();

            imageLeft.texture = powerUI_background;
            imageRight.texture = powerUI_background;

            Transform Powered = UI.transform.Find("Powered");
            
            RectTransform battery1 = Powered.Find("Battery1") as RectTransform;
            battery1.anchoredPosition = new Vector2(-545f, 15f);

            RectTransform battery1_text = battery1.Find("Text") as RectTransform;
            battery1_text.anchoredPosition = new Vector2(0f, 95f);
            battery1_text.localScale = new Vector3(1.50f, 1.50f, 0.1f);

            RectTransform battery2 = Powered.Find("Battery2") as RectTransform;
            battery2.anchoredPosition = new Vector2(-435f, 15f);

            RectTransform battery2_text = battery2.Find("Text") as RectTransform;
            battery2_text.anchoredPosition = new Vector2(0f, 95f);
            battery2_text.localScale = new Vector3(1.50f, 1.50f, 0.1f);

            RectTransform battery3 = Powered.Find("Battery3") as RectTransform;
            battery3.anchoredPosition = new Vector2(450f, 15f);

            RectTransform battery3_text = battery3.Find("Text") as RectTransform;
            battery3_text.anchoredPosition = new Vector2(0f, 95f);
            battery3_text.localScale = new Vector3(1.50f, 1.50f, 0.1f);

            RectTransform battery4 = Powered.Find("Battery4") as RectTransform;
            battery4.anchoredPosition = new Vector2(560f, 15f);

            RectTransform battery4_text = battery4.Find("Text") as RectTransform;
            battery4_text.anchoredPosition = new Vector2(0f, 95f);
            battery4_text.localScale = new Vector3(1.50f, 1.50f, 0.1f);

            Powered.gameObject.SetActive(true);

            BoxCollider Trigger = powerSystem.transform.Find("Trigger").GetComponent<BoxCollider>();
            Trigger.center = new Vector3(0f, 0f, 0.32f);
            Trigger.size = new Vector3(0.35f, 0.35f, 0.35f);

            UI.SetActive(true);            
            
            powerSystem.AddComponent<SeaTruckScannerPowerSystem>();

            powerSystem.SetActive(true);
        }

        private void SetupScannerUI()
        {
            scannerUI.name = "scannerUI";

            UnityEngine.Object.DestroyImmediate(scannerUI.GetComponent<uGUI_MapRoomScanner>());

            GameObject scanner_cullable = scannerUI.transform.Find("scanner_cullable").gameObject;

            GameObject background = scanner_cullable.transform.Find("background").gameObject;
            RectTransform rt_background = background.GetComponent<RectTransform>();
            
            rt_background.anchoredPosition = new Vector2(-89.81f, 0f);
            rt_background.localScale = new Vector3(0.21f, 0.10f, 0.1f);
            
            Image imageB = background.GetComponent<Image>();
            imageB.sprite = scannerUI_background;         
            
            GameObject powerBtn = new GameObject("powerBtn", new Type[] { typeof(RectTransform) });            

            RectTransform rt = powerBtn.GetComponent<RectTransform>();
            rt.SetParent(scanner_cullable.transform, false);
            rt.localScale = new Vector3(0.15f, 0.15f, 0.1f);
            rt.localPosition = new Vector3(-52.0f, -36.0f, 0.03f);
            rt.localRotation = Quaternion.Euler(0, 0, 0);

            BoxCollider bc = powerBtn.AddComponent<BoxCollider>();            
            bc.size = new Vector3(100f, 100f, 2f);
            
            GenericHandTarget handTarget = powerBtn.AddComponent<GenericHandTarget>();

            handTarget.onHandHover = new HandTargetEvent();
            handTarget.onHandClick = new HandTargetEvent();

            GameObject powerButtON = scanner_cullable.transform.Find("foreground").gameObject;
            powerButtON.name = "powerButtON";
            powerButtON.transform.SetParent(powerBtn.transform, false);
            powerButtON.transform.localPosition = Vector3.zero;
            powerButtON.transform.localScale = new Vector3(1f, 1f, 0.1f);

            Image imageON = powerButtON.GetComponent<Image>();
            imageON.sprite = ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/powerButton_on.png");

            powerButtON.SetActive(false);
            
            GameObject powerButtOFF = UWE.Utils.Instantiate(powerButtON, powerBtn.transform, Vector3.zero, Quaternion.identity);
            powerButtOFF.name = "powerButtOFF";
            powerButtOFF.transform.localPosition = Vector3.zero;
            powerButtOFF.transform.localScale = new Vector3(1f, 1f, 0.1f);

            Image imageOFF = powerButtOFF.GetComponent<Image>();
            imageOFF.sprite = ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/powerButton_off.png");

            powerButtOFF.SetActive(true);

            BoxCollider collider = scannerUI.GetComponent<BoxCollider>();
            collider.center = new Vector3(-5.0f, -7f, -0.50f);
            collider.size = new Vector3(154f, 120f, 1.10f);
            //collider.center = new Vector3(0f, -141f, -1.27f);
            //collider.size = new Vector3(218f, 26f, 1.40f);            

            GameObject Unpowered = powerSystem.transform.Find("UI/Unpowered").gameObject;

            Unpowered.transform.SetParent(scanner_cullable.transform, false);

            RectTransform rt_text = Unpowered.transform.Find("Text") as RectTransform;
            rt_text.sizeDelta = new Vector2(-40f, 40f);
            
            scannerUIRoot.AddComponent<SeaTruckScannerModuleManager>();
            scannerUI.AddComponent<uGUI_SeaTruckScanner>();            
            
            scannerUI.SetActive(true);            
        }
    }
}
