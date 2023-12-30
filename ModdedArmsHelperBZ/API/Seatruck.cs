extern alias SEZero;
using SEZero::SlotExtenderZero.API;
using UnityEngine;

namespace ModdedArmsHelperBZ.API
{
    [DisallowMultipleComponent]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Seatruck : MonoBehaviour
    {
        public SeatruckHelper seatruckHelper { get; private set; }
        public GameObject mainCab { get; private set; }        
        public EnergyInterface energyInterface { get; private set; }
        public WorldForces worldForces { get; private set; }
        internal SeatruckArmManager armManager { get; private set; }

        internal void Awake()
        {
            seatruckHelper = GetComponent<SeatruckHelper>();
            mainCab = seatruckHelper.MainCab;            
            armManager = GetComponent<SeatruckArmManager>();
            worldForces = GetComponent<WorldForces>();

            energyInterface = seatruckHelper.MainCab.EnsureComponent<EnergyInterface>();
            energyInterface.sources = seatruckHelper.MainCab.GetComponentsInChildren<BatterySource>();
        }    
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
