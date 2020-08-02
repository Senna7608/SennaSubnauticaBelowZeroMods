using UnityEngine;
using BZCommon;
using System.Collections.Generic;
using BZCommon.Helpers;

namespace SeaTruckStorage
{
    public class SeaTruckStorageManager : MonoBehaviour
    {
        public SeaTruckHelper helper;
        public ObjectHelper objectHelper = new ObjectHelper();
        public ColorizationHelper colorizationHelper = new ColorizationHelper();
        public GameObject StorageRoot;
        public GameObject StorageLeft;
        public GameObject StorageRight;

        public SeaTruckStorageInput StorageInputLeft;
        public SeaTruckStorageInput StorageInputRight;        
        
        private readonly Dictionary<SeaTruckStorageInput, int> StorageInputs = new Dictionary<SeaTruckStorageInput, int>();
        
        private void Awake()
        {
            StorageRoot = objectHelper.CreateGameObject("StorageRoot", transform);
            StorageLeft = objectHelper.CreateGameObject("StorageLeft", StorageRoot.transform, new Vector3(-1.27f, -1.41f, 1.15f), new Vector3(359, 273, 359), new Vector3(0.80f, 0.86f, 0.47f));
            StorageRight = objectHelper.CreateGameObject("StorageRight", StorageRoot.transform, new Vector3(1.27f, -1.41f, 1.15f), new Vector3(359, 87, 1), new Vector3(-0.80f, 0.86f, 0.47f));

            var exosuit = Resources.Load<GameObject>("worldentities/tools/exosuit");

            GameObject exoStorage = objectHelper.FindDeepChild(exosuit.transform, "Exosuit_01_storage");

            objectHelper.GetPrefabClone(ref exoStorage, StorageLeft.transform, true, "model", out GameObject leftStorageModel);
            leftStorageModel.SetActive(false);            

            objectHelper.GetPrefabClone(ref leftStorageModel, StorageRight.transform, true, "model", out GameObject rightStorageModel);
            rightStorageModel.SetActive(false);

            BoxCollider colliderLeft = StorageLeft.AddComponent<BoxCollider>();
            colliderLeft.enabled = false;
            colliderLeft.size = new Vector3(1.0f, 0.4f, 0.8f);

            BoxCollider colliderRight = StorageRight.AddComponent<BoxCollider>();
            colliderRight.enabled = false;
            colliderRight.size = new Vector3(1.0f, 0.4f, 0.8f);
                        
            colorizationHelper.AddColorCustomizerToGameObject(StorageRoot);            
        }

        public void Start()
        {            
            helper = new SeaTruckHelper(gameObject, false, false, false);

            helper.modules.isAllowedToRemove += IsAllowedToRemove;

            helper.modules.onEquip += OnEquip;
            helper.modules.onUnequip += OnUnequip;

            StorageInputLeft = StorageLeft.AddComponent<SeaTruckStorageInput>();
            StorageInputRight = StorageRight.AddComponent<SeaTruckStorageInput>();

            StorageInputs.Add(StorageInputLeft, -1);
            StorageInputs.Add(StorageInputRight, -1);

            CheckStorageSlots();            
        }

        private void OnDestroy()
        {
            BZLogger.Debug("SeaTruckStorageManager", "Removing unused handlers...");

            helper.modules.isAllowedToRemove -= IsAllowedToRemove;
            helper.modules.onEquip -= OnEquip;
            helper.modules.onUnequip -= OnUnequip;
        }


        private void CheckStorageSlots()
        {
            foreach (string slot in helper.slotIDs)
            {
                if (helper.modules.GetTechTypeInSlot(slot) == SeaTruckStorage.TechTypeID)
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
            if (item.item.GetTechType() == SeaTruckStorage.TechTypeID)
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
            if (item.item.GetTechType() == SeaTruckStorage.TechTypeID)
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

            if (techType == SeaTruckStorage.TechTypeID)
            {                
                SeamothStorageContainer component = pickupable.GetComponent<SeamothStorageContainer>();

                if (component != null)
                {
                    bool flag = component.container.count == 0;

                    if (verbose && !flag)
                    {
                        ErrorMessage.AddDebug(Language.main.Get("SeamothStorageNotEmpty"));
                    }
                    return flag;
                }

                Debug.LogError("No SeamothStorageContainer found on SeaTruckStorageModule item");
            }

            return true;
        }        
    }

}
