extern alias SEZero;

using UnityEngine;
using BZHelper;
using SEZero::SlotExtenderZero.API;
using System.Collections;

namespace SeaTruckDepthUpgrades
{
    public class SeaTruckDepthManager : MonoBehaviour
    {
        private SeatruckHelper _helper;
        private SeatruckHelper helper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);
                }

                return _helper;
            }            
        }        

        public void Awake()
        {
            if (!SeaTruckUpgrades.crushDepths.ContainsKey(SeaTruckDepthMK4_Prefab.TechTypeID))
            {
                SeaTruckUpgrades.crushDepths.Add(SeaTruckDepthMK4_Prefab.TechTypeID, 1200f);
            }

            if (!SeaTruckUpgrades.crushDepths.ContainsKey(SeaTruckDepthMK5_Prefab.TechTypeID))
            {
                SeaTruckUpgrades.crushDepths.Add(SeaTruckDepthMK5_Prefab.TechTypeID, 1550f);
            }

            if (!SeaTruckUpgrades.crushDepths.ContainsKey(SeaTruckDepthMK6_Prefab.TechTypeID))
            {
                SeaTruckUpgrades.crushDepths.Add(SeaTruckDepthMK6_Prefab.TechTypeID, 1900f);
            }

            BZLogger.Log($"SeaTruck 'crushDepths' patched in 'SeaTruckDepthManager.Awake' method. ID: {gameObject.GetInstanceID()}");            
        }        

        public void WakeUp()
        {
            BZLogger.Log("Received SlotExtenderZero 'WakeUp' message.");
                        
            StartCoroutine(CheckDepthUpgradesAsync());           
        }

        public void Start()
        {
            StartCoroutine(CheckDepthUpgradesAsync());
        }

        public IEnumerator CheckDepthUpgradesAsync()
        {
            BZLogger.Log("CheckDepthUpgradesAsync has started.");            

            while (helper == null && !helper.IsReady)
            {
                yield return null;
            }

            float extraCrushDepth = 0f;

            foreach (string slot in helper.TruckSlotIDs)
            {
                TechType techType = helper.TruckEquipment.GetTechTypeInSlot(slot);

                if (SeaTruckUpgrades.crushDepths.TryGetValue(techType, out float crushDepth) && crushDepth > extraCrushDepth)
                {
                    extraCrushDepth = crushDepth;
                }
            }

            helper.TruckUpgrades.crushDamage.SetExtraCrushDepth(extraCrushDepth);

            BZLogger.Log($"CheckDepthUpgradesAsync has completed, crushdepth set to: [{extraCrushDepth + 150}] m.");

            yield break;
        }        
    }
}
