extern alias SEZero;

using UnityEngine;
using System.Collections.Generic;
using SEZero::SlotExtenderZero.API;
using System.Collections;
using UWE;
using BZHelper;

namespace SeaTruckStorage
{
    public class SeaTruckStorageManager : MonoBehaviour
    {
        public SeatruckHelper helper;
        //public ObjectHelper objectHelper = new ObjectHelper();
       
        public GameObject StorageRoot;
        public GameObject StorageLeft;
        public GameObject StorageRight;

        public SeaTruckStorageInput StorageInputLeft;
        public SeaTruckStorageInput StorageInputRight;        
        
        private readonly Dictionary<SeaTruckStorageInput, int> StorageInputs = new Dictionary<SeaTruckStorageInput, int>();
                
        private bool isGraphicsReady = false;

        private Vector3 LeftBasePos = new Vector3(-1.27f, -1.41f, 1.15f);
        private Vector3 LeftBaseRot = new Vector3(359, 273, 359);
        private Vector3 LeftTransitPos = new Vector3(-2.70f, -0.68f, -0.36f);
        private Vector3 LeftTransitRot = new Vector3(0f, 180f, 0f);

        private Vector3 RightBasePos = new Vector3(1.27f, -1.41f, 1.15f);
        private Vector3 RightBaseRot = new Vector3(359, 87, 1);
        private Vector3 RightTransitPos = new Vector3(-1.80f, -0.68f, -0.36f);
        private Vector3 RightTransitRot = new Vector3(0f, 180f, 360f);

        private IEnumerator LoadExosuitResourcesAsync()
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Tools/Exosuit.prefab");

            yield return request;

            if (!request.TryGetPrefab(out GameObject prefab))
            {
                BZLogger.Error("Cannot load Exosuit prefab!");
                yield break;
            }

            BZLogger.Trace("Exosuit prefab loaded!");

            GameObject exosuitResource = UWE.Utils.InstantiateDeactivated(prefab, transform, Vector3.zero, Quaternion.identity);

            exosuitResource.GetComponent<Exosuit>().enabled = false;
            exosuitResource.GetComponent<Rigidbody>().isKinematic = true;
            exosuitResource.GetComponent<WorldForces>().enabled = false;
            UWE.Utils.ZeroTransform(exosuitResource.transform);

            GameObject exoStorage = UnityHelper.FindDeepChild(exosuitResource.transform, "Exosuit_01_storage");

            GameObject leftStorageModel = exoStorage.GetPrefabClone(StorageLeft.transform, true, "model");
            leftStorageModel.SetActive(false);

            GameObject rightStorageModel = leftStorageModel.GetPrefabClone(StorageRight.transform, true, "model");
            rightStorageModel.SetActive(false);

            BoxCollider colliderLeft = StorageLeft.AddComponent<BoxCollider>();
            colliderLeft.enabled = false;
            colliderLeft.size = new Vector3(1.0f, 0.4f, 0.8f);
            colliderLeft.center = new Vector3(0f, 0.27f, 0f);
            colliderLeft.isTrigger = true;

            BoxCollider colliderRight = StorageRight.AddComponent<BoxCollider>();
            colliderRight.enabled = false;
            colliderRight.size = new Vector3(1.0f, 0.4f, 0.8f);
            colliderRight.center = new Vector3(0f, 0.27f, 0f);
            colliderRight.isTrigger = true;

            ColorizationHelper.AddRendererToSkyApplier(gameObject, leftStorageModel, Skies.Auto);
            ColorizationHelper.AddRendererToColorCustomizer(gameObject, leftStorageModel, false, new int[] { 0 });
            
            ColorizationHelper.AddRendererToColorCustomizer(gameObject, rightStorageModel, false, new int[] { 0 });
            ColorizationHelper.AddRendererToSkyApplier(gameObject, rightStorageModel, Skies.Auto);
                        
            Destroy(exosuitResource);

            isGraphicsReady = true;

            yield break;
        }

        private void Awake()
        {
            StorageRoot = UnityHelper.CreateGameObject("StorageRoot", transform);
            StorageLeft = UnityHelper.CreateGameObject("StorageLeft", StorageRoot.transform, LeftBasePos, LeftBaseRot, new Vector3(0.80f, 0.86f, 0.47f), 13);
            StorageRight = UnityHelper.CreateGameObject("StorageRight", StorageRoot.transform, RightBasePos, RightBaseRot, new Vector3(-0.80f, 0.86f, 0.47f), 13);
            
            helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);
            helper.onDockedChanged += OnDockedChanged;

            StartCoroutine(LoadExosuitResourcesAsync());                                  
        }

        public void Start()
        {
            StartCoroutine(PreStart());                      
        }

        private IEnumerator PreStart()
        {
            while (!isGraphicsReady)
            {
                yield return null;
            }

            helper.TruckEquipment.isAllowedToRemove += IsAllowedToRemove;

            helper.TruckEquipment.onEquip += OnEquip;
            helper.TruckEquipment.onUnequip += OnUnequip;

            StorageInputLeft = StorageLeft.AddComponent<SeaTruckStorageInput>();
            StorageInputRight = StorageRight.AddComponent<SeaTruckStorageInput>();

            StorageInputs.Add(StorageInputLeft, -1);
            StorageInputs.Add(StorageInputRight, -1);
            
            CheckStorageSlots();           

            yield break;
        }

        private void OnDockedChanged(bool isDocked, bool isInTransition)
        {
            if (isDocked && isInTransition)
            {
                StorageLeft.transform.localPosition = LeftTransitPos;
                StorageLeft.transform.localRotation = Quaternion.Euler(LeftTransitRot);

                StorageRight.transform.localPosition = RightTransitPos;
                StorageRight.transform.localRotation = Quaternion.Euler(RightTransitRot);
            }
            else
            {
                StorageLeft.transform.localPosition = LeftBasePos;
                StorageLeft.transform.localRotation = Quaternion.Euler(LeftBaseRot);

                StorageRight.transform.localPosition = RightBasePos;
                StorageRight.transform.localRotation = Quaternion.Euler(RightBaseRot);
            }
        }

        private void OnDestroy()
        {
            BZLogger.Trace("Removing unused handlers...");

            helper.TruckEquipment.isAllowedToRemove -= IsAllowedToRemove;
            helper.TruckEquipment.onEquip -= OnEquip;
            helper.TruckEquipment.onUnequip -= OnUnequip;
            helper.onDockedChanged -= OnDockedChanged;
        }

        private void CheckStorageSlots()
        {
            foreach (string slot in helper.TruckSlotIDs)
            {
                if (helper.TruckEquipment.GetTechTypeInSlot(slot) == SeaTruckStorage_Prefab.TechTypeID)
                {
                    int slotID = helper.GetSlotIndex(slot);

                    if (StorageInputs.ContainsValue(slotID))
                    {
                        continue;
                    }

                    if (GetStorageInput(-1, out SeaTruckStorageInput storageInput))
                    {                        
                        storageInput.slotID = slotID;
                        storageInput.SetEnabled(true);
                        StorageInputs[storageInput] = slotID;
                    }
                }
            }
        }
        
        private bool GetStorageInput(int slotID, out SeaTruckStorageInput seaTruckStorageInput)
        {
            foreach (KeyValuePair<SeaTruckStorageInput, int> kvp in StorageInputs)
            {
                if (kvp.Value == slotID)
                {
                    seaTruckStorageInput = kvp.Key;
                    return true;
                }
            }

            seaTruckStorageInput = null;
            return false;
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            if (item.item.GetTechType() == SeaTruckStorage_Prefab.TechTypeID)
            {
                if (GetStorageInput( -1, out SeaTruckStorageInput storageInput))
                {                    
                    int slotID = helper.GetSlotIndex(slot);
                    storageInput.slotID = slotID;
                    storageInput.SetEnabled(true);
                    StorageInputs[storageInput] = slotID;
                }                
            }
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            if (item.item.GetTechType() == SeaTruckStorage_Prefab.TechTypeID)
            {
                int slotID = helper.GetSlotIndex(slot);

                if (GetStorageInput(slotID, out SeaTruckStorageInput storageInput))
                {
                    storageInput.slotID = -1;
                    storageInput.SetEnabled(false);
                    StorageInputs[storageInput] = -1;
                    CheckStorageSlots();
                }                
            }
        }        

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            TechType techType = pickupable.GetTechType();

            if (techType == SeaTruckStorage_Prefab.TechTypeID)
            {                
                SeamothStorageContainer component = pickupable.GetComponent<SeamothStorageContainer>();

                if (component != null)
                {
                    bool flag = component.container.count == 0;

                    if (verbose && !flag)
                    {
                        ErrorMessage.AddDebug(Language.main.Get("DeconstructNonEmptyStorageContainerError"));
                    }
                    return flag;
                }

                Debug.LogError("No SeamothStorageContainer found on SeaTruckStorageModule item");
            }

            return true;
        }        
    }

}
