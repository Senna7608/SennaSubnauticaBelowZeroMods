using UnityEngine;
using BZCommon;
using System.Collections.Generic;

namespace SeaTruckSpeedUpgrades
{
    public class SeaTruckSpeedManager : MonoBehaviour
    {
        private SeaTruckHelper helper;

        public Dictionary<TechType, float> Accelerations => accelerations;

        private bool isFirstCheckComplete = false;

        private static readonly Dictionary<TechType, float> accelerations = new Dictionary<TechType, float>
        {
            {
                SeaTruckSpeedMK1.TechTypeID,
                1.5f
            },
            {
                SeaTruckSpeedMK2.TechTypeID,
                2f
            },
            {
                SeaTruckSpeedMK3.TechTypeID,
                2.5f
            },

        };

        public void Awake()
        {            
            helper = new SeaTruckHelper(gameObject, false, false, false);
        }

        public void Update()
        {
            if (!isFirstCheckComplete && helper != null)
            {
                CheckSlotsForSpeedUpgrades();
                isFirstCheckComplete = true;
            }
        }

        public void CheckSlotsForSpeedUpgrades()
        {
            float extraAcceleration = 1f;

            foreach (string slot in helper.slotIDs)
            {
                TechType techType = helper.modules.GetTechTypeInSlot(slot);

                if (accelerations.TryGetValue(techType, out float _acceleration) && _acceleration > extraAcceleration)
                {
                    extraAcceleration = _acceleration;
                }                
            }

            helper.thisMotor.acceleration = 17.5f * extraAcceleration;

            ErrorMessage.AddDebug($"Seatruck speed now: {extraAcceleration * 100} %");
        }        
    }
}
