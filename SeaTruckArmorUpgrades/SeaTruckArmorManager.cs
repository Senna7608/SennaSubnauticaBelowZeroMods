extern alias SEZero;

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using BZHelper;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArmorUpgrades
{
    public class SeaTruckArmorManager : MonoBehaviour
    {
        private SeatruckHelper helper;

        public Dictionary<TechType, float> ArmorRatings => armorRatings;        

        public float originalDamage;

        public float DamageReductionMultiplier = 1f;

        private static readonly Dictionary<TechType, float> armorRatings = new Dictionary<TechType, float>
        {
            {
                SeaTruckArmorMK1_Prefab.TechTypeID,
                125f
            },
            {
                SeaTruckArmorMK2_Prefab.TechTypeID,
                150f
            },
            {
                SeaTruckArmorMK3_Prefab.TechTypeID,
                175f
            },

        };

        public void Awake()
        {
            helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);            

            helper.onUpgradeModuleEquip += OnUpgradeModuleChanged;
            helper.onUpgradeModuleUnEquip += OnUpgradeModuleChanged;

            AddDamageListener();
        }        

        private void OnUpgradeModuleChanged(int slotID, TechType techType)
        {
            if (armorRatings.ContainsKey(techType))
            {
                CheckSlotsForArmorUpgrades();
            }
        }

        [Conditional("DEBUG")]
        public void AddDamageListener()
        {
            BZLogger.Debug("Adding damage listener handler");
            helper.onDamageReceived += OnDamageReceived;
        }
        
        private void OnDestroy()
        {
            helper.onUpgradeModuleEquip -= OnUpgradeModuleChanged;
            helper.onUpgradeModuleUnEquip -= OnUpgradeModuleChanged;

            RemoveDamageListener();
        }

        [Conditional("DEBUG")]
        private void RemoveDamageListener()
        {
            helper.onDamageReceived -= OnDamageReceived;
        }

        private void OnDamageReceived(float damage)
        {
            print($"[SeaTruckArmorManager] Damage reduced by: {originalDamage - damage}");           
        }        

        public void WakeUp()
        {
            BZLogger.Log("Received SlotExtenderZero 'WakeUp' message.");

            CheckSlotsForArmorUpgrades();                      
        }

        public void CheckSlotsForArmorUpgrades()
        {
            float armorClass = 100f;

            for (int i = 0; i < helper.TruckSlotIDs.Length; i++)
            {
                string slot = helper.TruckSlotIDs[i];
                TechType techType = helper.TruckEquipment.GetTechTypeInSlot(slot);

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
