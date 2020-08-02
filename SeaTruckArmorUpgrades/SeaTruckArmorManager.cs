using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UWE;
using BZCommon;

namespace SeaTruckArmorUpgrades
{
    public class SeaTruckArmorManager : MonoBehaviour
    {
        private SeaTruckHelper helper;

        public Dictionary<TechType, float> ArmorRatings => armorRatings;

        private bool isFirstCheckComplete = false;

        public float originalDamage;

        public float DamageReductionMultiplier = 1f;

        private static readonly Dictionary<TechType, float> armorRatings = new Dictionary<TechType, float>
        {
            {
                SeaTruckArmorMK1.TechTypeID,
                125f
            },
            {
                SeaTruckArmorMK2.TechTypeID,
                150f
            },
            {
                SeaTruckArmorMK3.TechTypeID,
                175f
            },

        };

        public void Awake()
        {
            helper = new SeaTruckHelper(gameObject, false, false, true);

            DebugDamage();
        }

        [Conditional("DEBUG")]
        public void DebugDamage()
        {
            helper.OnDamageReceived.AddHandler(this, new Event<float>.HandleFunction(OnDamageReceived));
        }

        [Conditional("DEBUG")]
        private void OnDestroy()
        {
            BZLogger.Debug("SeaTruckArmorManager", "Removing unused handlers...");

            helper.OnDamageReceived.RemoveHandler(this,OnDamageReceived);
        }
        
        private void OnDamageReceived(float damage)
        {
            print($"[SeaTruckArmorManager] Damage reduced by: {originalDamage - damage}");           
        }

        public void Update()
        {
            if (!isFirstCheckComplete && helper != null)
            {
                CheckSlotsForArmorUpgrades();
                isFirstCheckComplete = true;
            }
        }

        public void CheckSlotsForArmorUpgrades()
        {
            float armorClass = 100f;

            for (int i = 0; i < helper.slotIDs.Length; i++)
            {
                string slot = helper.slotIDs[i];
                TechType techType = helper.modules.GetTechTypeInSlot(slot);

                if (armorRatings.TryGetValue(techType, out float armorRating) && armorRating > armorClass)
                {
                    armorClass = armorRating;
                }                
            }
            
            switch (armorClass)
            {
                case 125:
                    DamageReductionMultiplier = 0.75f;
                    break;
                case 150:
                    DamageReductionMultiplier = 0.5f;
                    break;
                case 175:
                    DamageReductionMultiplier = 0.25f;
                    break;
                default:
                    DamageReductionMultiplier = 1f;
                    break;
            }            

            ErrorMessage.AddDebug($"Seatruck hull strength now: {armorClass} %");
        }
    }
}
