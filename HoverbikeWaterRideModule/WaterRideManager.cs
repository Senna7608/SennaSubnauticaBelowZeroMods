using UnityEngine;
using BZCommon;
using System.Collections.Generic;

namespace HoverbikeWaterRideModule
{
    public class WaterRideManager : MonoBehaviour
    {
        private Hoverbike hoverbike;
        private Equipment modules;
        private List<string> slotNames = new List<string>();

        public void Awake()
        {
            hoverbike = GetComponent<Hoverbike>();
            modules = hoverbike.modules;

            BZLogger.Debug("WaterRideManager", "Adding slot listener handlers...");

            modules.onEquip += OnEquip;
            modules.onUnequip += OnUnEquip;
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            CheckModule();
        }

        private void OnUnEquip(string slot, InventoryItem item)
        {
            CheckModule();
        }

        private void EnableModule()
        {

            hoverbike.forceLandMode = true;
            //hoverbike.waterLevelOffset = 2f;
            //hoverbike.waterDampening = 1f;
        }

        private void DisableModule()
        {
            hoverbike.forceLandMode = false;
            //hoverbike.waterLevelOffset = 0f;
            //hoverbike.waterDampening = 10f;
        }

        private void CheckModule()
        {
            slotNames.Clear();

            modules.GetSlots(EquipmentType.HoverbikeModule, slotNames);

            foreach (string slot in slotNames)
            {
                if (modules.GetTechTypeInSlot(slot) == WaterRideModule_Prefab.TechTypeID)
                {
                    EnableModule();
                    return;
                }

                DisableModule();
            }

        }

        private void Update()
        {
            if (hoverbike.forceLandMode)
            {
                if (transform.position.y < 0)
                {
                    hoverbike.gravity = 0f;
                }
                else
                {
                    hoverbike.gravity = 12f;
                }
            }
        }

        private void OnDestroy()
        {
            BZLogger.Debug("WaterRideManager", "Removing unused handlers...");

            modules.onEquip -= OnEquip;
            modules.onUnequip -= OnUnEquip;
        }

          
    }
}
