extern alias SEZero;
using System.Collections.Generic;
using BZHelper;
using ModdedArmsHelperBZ.API.Interfaces;
using SEZero::SlotExtenderZero.API;
using UnityEngine;
#pragma warning disable IDE1006 //Naming styles

namespace ModdedArmsHelperBZ.API
{
    /// <summary>
    /// The defining helper class for all new modded arms. This class is a Singleton.
    /// </summary>
    public sealed class ArmServices
    {
        static ArmServices()
        {
            BZLogger.Log("ArmServices created.");
        }

        private ArmServices()
        {            
        }

        private static readonly ArmServices _main = new ArmServices();

        /// <summary>
        /// A property that can be used to access this class.
        /// </summary>
        public static ArmServices main => _main;

        internal Exosuit ExosuitResource
        {
            get
            {
                return ModdedArmsHelperBZ_Main.armsCacheManager.exosuitResource.GetComponent<Exosuit>();
            }
        }

        internal Dictionary<CraftableModdedArm, IArmModdingRequest> waitForRegistration = new Dictionary<CraftableModdedArm, IArmModdingRequest>();       
                
        internal void RegisterArm(CraftableModdedArm armPrefab, IArmModdingRequest armModdingRequest)
        {
            BZLogger.Log($"Received registration request from TechType: [{armPrefab.Info.TechType}]");

            if (!waitForRegistration.ContainsKey(armPrefab))
            {
                waitForRegistration.Add(armPrefab, armModdingRequest);               
            }
        }

#pragma warning disable CS1591 // Missing XML documentation

        public SeatruckHelper GetHelper(Seatruck seatruck)
        {
            return seatruck.seatruckHelper;
        }

#pragma warning restore CS1591 // Missing XML documentation

        /// <summary>
        /// This method examines whether there is enough space for the <see cref="Pickupable"/> item in the <see cref="Seatruck"/> storage modules.
        /// </summary>
        /// <returns>
        /// <c>true</c> if any of <see cref="Seatruck"/> storage module have an enough space for the <see cref="Pickupable"/> item.<br/>Otherwise, returns <c>false</c>. 
        /// </returns>
        public bool HasRoomForItem(Seatruck seatruck, Pickupable pickupable)
        {
            return seatruck.seatruckHelper.HasRoomForItem(pickupable);
        }

        /// <summary>
        /// This method is examine if any of the <see cref="Seatruck"/> storage module that can fits the <see cref="Pickupable"/> item.
        /// </summary>
        /// <returns>
        /// an <see cref="ItemsContainer"/> if any of <see cref="Seatruck"/> storage module have an enough space for the <see cref="Pickupable"/> item.<br/>Otherwise, returns <c>null</c>. 
        /// </returns>
        public ItemsContainer GetRoomForItem(Seatruck seatruck, Pickupable pickupable)
        {
            return seatruck.seatruckHelper.GetRoomForItem(pickupable);
        }

        /// <summary>
        /// This method is examine of the the current mounted left arm <see cref="TechType"/> of the <see cref="Seatruck"/>.
        /// </summary>
        /// <returns>
        /// If any arm mounted in the left arm socket, returns the arm <see cref="TechType"/>.<br/>Otherwise, returns <see cref="TechType.None"/>. 
        /// </returns>
        public TechType GetLeftArmType(Seatruck seatruck)
        {
            return seatruck.armManager.currentLeftArmType;
        }

        /// <summary>
        /// This method is examine of the current mounted right arm <see cref="TechType"/> of the <see cref="Seatruck"/>.
        /// </summary>
        /// <returns>
        /// If any arm mounted in the right arm socket, returns the arm <see cref="TechType"/>.<br/>Otherwise, returns <see cref="TechType.None"/>. 
        /// </returns>
        public TechType GetRightArmType(Seatruck seatruck)
        {
            return seatruck.armManager.currentRightArmType;
        }

        /// <summary>
        /// This method is examine of the <see cref="Seatruck"/> left arm slot.
        /// </summary>
        /// <returns>
        /// If any arm mounted in the left arm slot, returns the arm slot ID.<br/>Otherwise, returns <c>-1</c>. 
        /// </returns>
        public int GetLeftArmSlotID(Seatruck seatruck)
        {
            return seatruck.seatruckHelper.GetSlotIndex("SeaTruckArmLeft");
        }

        /// <summary>
        /// This method is examine of this <see cref="Seatruck"/> right arm slot.
        /// </summary>
        /// <returns>
        /// If any arm mounted in the right arm slot, returns the arm slot ID.<br/>Otherwise, returns <c>-1</c>. 
        /// </returns>
        public int GetRightArmSlotID(Seatruck seatruck)
        {
            return seatruck.seatruckHelper.GetSlotIndex("SeaTruckArmRight");
        }

        /// <summary>
        /// This method is examine if any arm selected in this <see cref="Seatruck"/> quickslot bar.
        /// </summary>
        /// <returns>
        /// If any arm selected in the quickslot bar, returns <c>true</c>.<br/>Otherwise, returns <c>false</c>. 
        /// </returns>
        public bool IsArmSlotSelected(Seatruck seatruck)
        {
            return seatruck.armManager.IsArmSlotSelected;
        }

        /// <summary>
        /// This method is examine if any arm attached to any of this <see cref="Seatruck"/> arm slots.
        /// </summary>
        /// <returns>
        /// If any arm attached to any of this <see cref="Seatruck"/> arm slots, returns <c>true</c>.<br/>Otherwise, returns <c>false</c>. 
        /// </returns>
        public bool IsAnyArmAttached(Seatruck seatruck)
        {
            return seatruck.armManager.IsAnyArmAttached;
        }

#pragma warning disable CS1591 // Missing XML documentation

        public bool IsSeatruckArm(TechType techType)
        {
            if (ModdedArmsHelperBZ_Main.armsCacheManager.ModdedArmPrefabs.TryGetValue(techType, out GameObject prefabResult))
            {
                return prefabResult.GetComponent<ArmTag>().armType == ArmType.SeatruckArm;
            }

            return false;
        }

        public bool IsSeatruckArm(TechType techType, out TechType techTypeResult)
        {
            if (ModdedArmsHelperBZ_Main.armsCacheManager.ModdedArmPrefabs.TryGetValue(techType, out GameObject prefabResult))
            {
                if (prefabResult.GetComponent<ArmTag>().armType == ArmType.SeatruckArm)
                {
                    techTypeResult = techType;
                    return true;
                }
            }

            techTypeResult = TechType.None;
            return false;
        }

#pragma warning restore CS1591 // Missing XML documentation

        /// <summary>
        /// This method is examine for the selected arm <see cref="TechType"/> on this <see cref="Seatruck"/>.
        /// </summary>
        /// <returns>
        /// If any arm selected in the quickslot bar, returns the arm <see cref="TechType"/>.<br/>Otherwise, returns <see cref="TechType.None"/>. 
        /// </returns>
        public TechType GetSelectedArmTechType(Seatruck seatruck)
        {
            return seatruck.armManager.GetSelectedArmTechType();
        }

        /// <summary>
        /// This method is examine of the current active target on crosshair on this <see cref="Seatruck"/>.
        /// </summary>
        /// <returns>
        /// If current active target not null, returns the target <see cref="GameObject"/>.<br/>Otherwise, returns <c>null</c>. 
        /// </returns>
        public GameObject GetActiveTarget(Seatruck seatruck)
        {
            return seatruck.armManager.GetActiveTarget();
        }
    }
}
