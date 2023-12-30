using BZHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SlotExtenderZero
{
    internal class SeatruckPatch : MonoBehaviour
    {
        private SeaTruckUpgrades truckUpgrades;

        private void Awake()
        {
            BZLogger.Debug("SeatruckPatch.Awake");

            StartCoroutine(AwakeAsync());           
        }

        private IEnumerator AwakeAsync()
        {
            BZLogger.Log("SeatruckPatch.AwakeAsync has sarted...");

            truckUpgrades = GetComponent<SeaTruckUpgrades>();

            while (truckUpgrades == null)
            {
                yield return null;
            }

            DebugEquipment();            

            int slotIDsLength = SlotHelper.SessionSeatruckSlotIDs.Length;

            if (truckUpgrades.quickSlotTimeUsed.Length != slotIDsLength)
            {
                BZLogger.Trace($"SeaTruckUpgrades.quickSlotTimeUsed.Length does not match the slotIDs array length: [{truckUpgrades.quickSlotTimeUsed.Length}]");
                BZLogger.Trace($"Resizing Array -> SeaTruckUpgrades.quickSlotTimeUsed to length: [{slotIDsLength}]");

                Array.Resize(ref truckUpgrades.quickSlotTimeUsed, slotIDsLength);
                BZLogger.Log($"SeaTruckUpgrades.quickSlotTimeUsed array resized. Actual length is: {truckUpgrades.quickSlotTimeUsed.Length}");
            }

            if (truckUpgrades.quickSlotCooldown.Length != slotIDsLength)
            {
                BZLogger.Trace($"SeaTruckUpgrades.quickSlotCooldown.Length does not match the slotIDs array length: [{truckUpgrades.quickSlotCooldown.Length}]");
                BZLogger.Trace($"Resizing Array -> SeaTruckUpgrades.quickSlotCooldown to length: [{slotIDsLength}]");

                Array.Resize(ref truckUpgrades.quickSlotCooldown, slotIDsLength);
                BZLogger.Log($"SeaTruckUpgrades.quickSlotCooldown array resized. Actual length is: {truckUpgrades.quickSlotCooldown.Length}");
            }

            if (truckUpgrades.quickSlotCharge.Length != slotIDsLength)
            {
                BZLogger.Trace($"SeaTruckUpgrades.quickSlotCharge.Length does not match the slotIDs array length: [{truckUpgrades.quickSlotCharge.Length}]");
                BZLogger.Trace($"Resizing Array -> SeaTruckUpgrades.quickSlotCharge to length: [{slotIDsLength}]");

                Array.Resize(ref truckUpgrades.quickSlotCharge, slotIDsLength);
                BZLogger.Log($"SeaTruckUpgrades.quickSlotCharge array resized. Actual length is: {truckUpgrades.quickSlotCharge.Length}");
            }

            if (truckUpgrades.slotIndexes.Count != slotIDsLength)
            {
                BZLogger.Trace($"SeaTruckUpgrades.slotIndexes.Count does not match the slotIDs array length: [{truckUpgrades.slotIndexes.Count}]");

                truckUpgrades.slotIndexes.Clear();

                foreach (string text in SlotHelper.SessionSeatruckSlotIDs)
                {
                    int index = Array.IndexOf(SlotHelper.SessionSeatruckSlotIDs, text);
                    truckUpgrades.slotIndexes.Add(text, index);
                }

                BZLogger.Log($"SeaTruckUpgrades.slotIndexes dictionary reinitialized. New size is: {truckUpgrades.slotIndexes.Count}");
            }

            gameObject.EnsureComponent<SlotExtenderZeroControl>();

            BZLogger.Log("SeatruckPatch.AwakeAsync has completed.");

            yield break;
        }


        [Conditional("DEBUG")]
        private void DebugEquipment()
        {
            BZLogger.Debug("Listing Seatruck equipment modules dict...");

            foreach (KeyValuePair<string, InventoryItem> kvp in truckUpgrades.modules.equipment)
            {
                BZLogger.Debug($"Slot: {kvp.Key}, TechType: {kvp.Value?.techType}");
            }
        }
    }
}
