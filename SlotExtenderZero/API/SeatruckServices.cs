using BZCommon;
using UnityEngine;

namespace SlotExtenderZero.API
{
    public sealed class SeatruckServices
    {
        static SeatruckServices()
        {
            BZLogger.Log("API message: SeatruckServices created.");
        }

        private SeatruckServices() { }

        private static readonly SeatruckServices main = new SeatruckServices();

        public static SeatruckServices Main => main;

        public SeaTruckHelper GetSeaTruckHelper(GameObject seatruckMainCab)
        {
            return seatruckMainCab.EnsureComponent<SeaTruckHelper>();
        }
    }
}
