extern alias SEZero;

using UnityEngine;
using System.Collections.Generic;
using SEZero::SlotExtenderZero.API;
using BZCommon;

namespace SeaTruckSpeedUpgrades
{
    public class SeaTruckSpeedManager : MonoBehaviour
    {
        private SeaTruckHelper helper;

        public Dictionary<TechType, float> Accelerations => accelerations;        

        private static readonly Dictionary<TechType, float> accelerations = new Dictionary<TechType, float>
        {
            {
                SeaTruckSpeedMK1_Prefab.TechTypeID,
                1.5f
            },
            {
                SeaTruckSpeedMK2_Prefab.TechTypeID,
                2f
            },
            {
                SeaTruckSpeedMK3_Prefab.TechTypeID,
                2.5f
            },

        };

        public void Awake()
        {
            helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);

            helper.onUpgradeModuleEquip += OnUpgradeModuleChanged;
            helper.onUpgradeModuleUnEquip += OnUpgradeModuleChanged;
        }

        private void OnUpgradeModuleChanged(int slotID, TechType techType)
        {
            if (accelerations.ContainsKey(techType))
            {
                CheckSlotsForSpeedUpgrades();
            }
        }

        private void OnDestroy()
        {
            BZLogger.Debug("Removing unused handlers...");

            helper.onUpgradeModuleEquip -= OnUpgradeModuleChanged;
            helper.onUpgradeModuleUnEquip -= OnUpgradeModuleChanged;
        }

        public void WakeUp()
        {
            BZLogger.Debug("Received SlotExtenderZero 'WakeUp' message.");

            CheckSlotsForSpeedUpgrades();
        }

        public void CheckSlotsForSpeedUpgrades()
        {
            float extraAcceleration = 1f;

            foreach (string slot in helper.TruckSlotIDs)
            {
                TechType techType = helper.TruckEquipment.GetTechTypeInSlot(slot);

                if (accelerations.TryGetValue(techType, out float _acceleration) && _acceleration > extraAcceleration)
                {
                    extraAcceleration = _acceleration;
                }                
            }

            helper.TruckMotor.acceleration = 17.5f * extraAcceleration;

            ErrorMessage.AddDebug($"Seatruck speed now: {extraAcceleration * 100} %");
        }        
    }
}
