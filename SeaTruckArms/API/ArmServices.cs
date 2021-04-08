extern alias SEZero;

using System.Collections.Generic;
using UnityEngine;
using BZCommon;
using BZCommon.Helpers;
using SeaTruckArms.API.Interfaces;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArms.API
{
    public sealed class ArmServices
    {
        static ArmServices()
        {
            BZLogger.Debug("API message: ArmServices created.");
        }

        private ArmServices()
        {
        }
        private static readonly ArmServices _main = new ArmServices();

        public static ArmServices main => _main;

        public Exosuit ExosuitResource
        {
            get
            {
                return Main.graphics.exosuitResource.GetComponent<Exosuit>();
            }
        }

        private ObjectHelper _objectHelper;

        public ObjectHelper objectHelper
        {
            get
            {
                if (_objectHelper == null)
                {
                    _objectHelper = new ObjectHelper();
                }

                return _objectHelper;
            }
        }

        internal Dictionary<CraftableSeaTruckArm, ISeaTruckArmHandlerRequest> waitForRegistration = new Dictionary<CraftableSeaTruckArm, ISeaTruckArmHandlerRequest>();

        internal bool isWaitForRegistration = false;        

        public void RegisterArm(CraftableSeaTruckArm armPrefab, ISeaTruckArmHandlerRequest armHandlerRequest)
        {
            if (!waitForRegistration.ContainsKey(armPrefab))
            {
                waitForRegistration.Add(armPrefab, armHandlerRequest);
            }

            isWaitForRegistration = true;
        }

        public SeaTruckHelper GetHelper(GameObject seatruckMainCab)
        {
            return SeatruckServices.Main.GetSeaTruckHelper(seatruckMainCab);
        }

        public bool HasRoomForItem(GameObject seatruckMainCab, Pickupable pickupable)
        {
            return GetHelper(seatruckMainCab).HasRoomForItem(pickupable);
        }

        public ItemsContainer GetRoomForItem(GameObject seatruckMainCab, Pickupable pickupable)
        {
            return GetHelper(seatruckMainCab).GetRoomForItem(pickupable);
        }

        public TechType GetLeftArmType(GameObject seatruckMainCab)
        {
            return seatruckMainCab.GetComponent<SeaTruckArmManager>().currentLeftArmType;
        }

        public TechType GetRightArmType(GameObject seatruckMainCab)
        {
            return seatruckMainCab.GetComponent<SeaTruckArmManager>().currentRightArmType;
        }

        public int GetLeftArmSlotID(GameObject seatruckMainCab)
        {
            return GetHelper(seatruckMainCab).GetSlotIndex("SeamothArmLeft");
        }

        public int GetRightArmSlotID(GameObject seatruckMainCab)
        {
            return GetHelper(seatruckMainCab).GetSlotIndex("SeamothArmRight");
        }

        public bool IsArmSlotSelected(GameObject seatruckMainCab)
        {
            return seatruckMainCab.GetComponent<SeaTruckArmManager>().IsArmSlotSelected;
        }

        public bool IsAnyArmAttached(GameObject seatruckMainCab)
        {
            return seatruckMainCab.GetComponent<SeaTruckArmManager>().IsAnyArmAttached;
        }

        public bool IsSeaTruckArm(TechType techType)
        {
            return Main.graphics.ArmTechTypes.ContainsKey(techType) ? true : false;
        }

        public bool IsSeaTruckArm(TechType techType, out TechType result)
        {
            if (Main.graphics.ArmTechTypes.ContainsKey(techType))
            {
                result = techType;
                return true;
            }

            result = TechType.None;
            return false;
        }

        public TechType GetSelectedArmTechType(GameObject seatruckMainCab)
        {
            return seatruckMainCab.GetComponent<SeaTruckArmManager>().GetSelectedArmTechType();
        }

        public GameObject GetActiveTarget(GameObject seatruckMainCab)
        {
            return seatruckMainCab.GetComponent<SeaTruckArmManager>().GetActiveTarget();
        }
    }
}
