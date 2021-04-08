extern alias SEZero;

using UnityEngine;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArms.API.ArmHandlers
{
    public abstract class PropulsionArmHandler : MonoBehaviour
    {
        public SeaTruckHelper TruckHelper => GetComponentInParent<SeaTruckArmManager>().helper;

        public PropulsionCannon propulsionCannon => GetComponent<PropulsionCannon>();

        public bool usingTool;

        public virtual void Awake()
        {
            TruckHelper.MainCab.EnsureComponent<ImmuneToPropulsioncannon>();
            EnergyInterface energyInterface = TruckHelper.MainCab.EnsureComponent<EnergyInterface>();
            energyInterface.sources = TruckHelper.MainCab.GetComponentsInChildren<BatterySource>();
            propulsionCannon.energyInterface = energyInterface;
            propulsionCannon.shootForce = 60f;
            propulsionCannon.attractionForce = 145f;
            propulsionCannon.massScalingFactor = 0.005f;
            propulsionCannon.pickupDistance = 25f;
            propulsionCannon.maxMass = 1800f;
            propulsionCannon.maxAABBVolume = 400f;
        }
    }
}
