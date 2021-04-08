extern alias SEZero;

using UnityEngine;
using BZCommon;
using System.Collections.Generic;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckDepthUpgrades
{
    public class SeaTruckDepthManager : MonoBehaviour
    {
        private SeaTruckHelper helper;

        public Dictionary<TechType, float> CrushDepths => crushDepths;        
        
        private static readonly Dictionary<TechType, float> crushDepths = new Dictionary<TechType, float>
        {
            {
                TechType.SeaTruckUpgradeHull1,
                150f
            },
            {
                TechType.SeaTruckUpgradeHull2,
                500f
            },
            {
                TechType.SeaTruckUpgradeHull3,
                850f
            },
            {
                SeaTruckDepthMK4_Prefab.TechTypeID,
                1200f
            },
            {
                SeaTruckDepthMK5_Prefab.TechTypeID,
                1550f
            },
            {
                SeaTruckDepthMK6_Prefab.TechTypeID,
                1900f
            }
        };

        public void Awake()
        {
            helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);
            
            BZLogger.Debug("Adding slot listener handlers...");

            helper.onUpgradeModuleEquip += OnUpgradeModuleChanged;
            helper.onUpgradeModuleUnEquip += OnUpgradeModuleChanged;
        }       

        private void OnUpgradeModuleChanged(int slotID, TechType techType)
        {
            if (crushDepths.ContainsKey(techType))
            {
                CheckSlotsForDepthUpgrades();
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

            CheckSlotsForDepthUpgrades();
        }

        public void CheckSlotsForDepthUpgrades()
        {
            float extraCrushDepth = 0f;

            foreach (string slot in helper.TruckSlotIDs)
            {
                TechType techType = helper.TruckEquipment.GetTechTypeInSlot(slot);

                if (crushDepths.TryGetValue(techType, out float crushDepth) && crushDepth > extraCrushDepth)
                {
                    extraCrushDepth = crushDepth;
                }
            }

            helper.TruckUpgrades.crushDamage.SetExtraCrushDepth(extraCrushDepth);
        }        
    }
}
