using UnityEngine;
using BZCommon;
using System.Collections.Generic;

namespace SeaTruckDepthUpgrades
{
    public class SeaTruckDepthManager : MonoBehaviour
    {
        private SeaTruckHelper helper;

        public Dictionary<TechType, float> CrushDepths => crushDepths;

        private bool isFirstCheckComplete = false;
        
        private static readonly Dictionary<TechType, float> crushDepths = new Dictionary<TechType, float>
        {
            {
                TechType.SeaTruckUpgradeHull1,
                150f
            },
            {
                TechType.SeaTruckUpgradeHull2,
                350f
            },
            {
                TechType.SeaTruckUpgradeHull3,
                750f
            },
            {
                SeaTruckDepthMK4.TechTypeID,
                950f
            },
            {
                SeaTruckDepthMK5.TechTypeID,
                1350f
            },
            {
                SeaTruckDepthMK6.TechTypeID,
                1750f
            }
        };

        public void Awake()
        {            
            helper = new SeaTruckHelper(gameObject, false, false, false);
        }

        public void Update()
        {
            if (!isFirstCheckComplete && helper != null)
            {
                CheckSlotsForDepthUpgrades();
                isFirstCheckComplete = true;
            }
        }

        public void CheckSlotsForDepthUpgrades()
        {
            float extraCrushDepth = 0f;

            foreach (string slot in helper.slotIDs)
            {
                TechType techType = helper.modules.GetTechTypeInSlot(slot);

                if (crushDepths.TryGetValue(techType, out float crushDepth) && crushDepth > extraCrushDepth)
                {
                    extraCrushDepth = crushDepth;
                }
            }

            helper.thisUpgrades.crushDamage.SetExtraCrushDepth(extraCrushDepth);
        }        
    }
}
