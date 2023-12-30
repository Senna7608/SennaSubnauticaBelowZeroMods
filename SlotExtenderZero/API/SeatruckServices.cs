using BZHelper;
using UnityEngine;

namespace SlotExtenderZero.API
{
    public sealed class SeatruckServices
    {
        private static readonly SeatruckServices _main = new SeatruckServices();

        static SeatruckServices()
        {
            BZLogger.Log("API/SeatruckServices: SeatruckServices created.");
        }

        private SeatruckServices()
        {
        }        

        public static SeatruckServices Main => _main;

        public SeatruckHelper GetSeaTruckHelper(GameObject seaTruckMainCab)
        {
            if (seaTruckMainCab.TryGetComponent(out SeaTruckSegment seaTruckSegment))
            {
                if (seaTruckSegment.isMainCab)
                {
                    return seaTruckMainCab.EnsureComponent<SeatruckHelper>();
                }                
            }

            BZLogger.Debug("Segment is not connected to Seatruck Main Cab!");

            return null;
        }

        public SeatruckHelper GetSeaTruckHelper(SeaTruckSegment seaTruckSegment)
        {
            FindMainCab(seaTruckSegment, out GameObject seaTruckMainCab);

            if (seaTruckMainCab == null)
            {
                BZLogger.Debug("Segment is not connected to Seatruck Main Cab!");
                return null;
            }

            return seaTruckMainCab.EnsureComponent<SeatruckHelper>();
        }

        public SeaTruckTelemetry GetSeaTruckTelemetry(GameObject seaTruckMainCab)
        {
            if (seaTruckMainCab.TryGetComponent(out SeaTruckSegment seaTruckSegment))
            {
                if (seaTruckSegment.isMainCab)
                {
                    return seaTruckMainCab.EnsureComponent<SeaTruckTelemetry>();
                }
            }

            BZLogger.Debug("Segment is not connected to Seatruck Main Cab!");

            return null;
        }

        public SeaTruckTelemetry GetSeaTruckTelemetry(SeaTruckSegment seaTruckSegment)
        {
            FindMainCab(seaTruckSegment, out GameObject seaTruckMainCab);

            if (seaTruckMainCab == null)
            {
                BZLogger.Debug("Segment is not connected to Seatruck Main Cab!");
                return null;
            }

            return seaTruckMainCab.EnsureComponent<SeaTruckTelemetry>();
        }

        private void FindMainCab(SeaTruckSegment seaTruckSegment, out GameObject mainCab)
        {
            mainCab = null;
            
            while (seaTruckSegment)
            {                
                if (seaTruckSegment.isMainCab)
                {
                    mainCab = seaTruckSegment.gameObject;
                    return;
                }
                if (seaTruckSegment.isFrontConnected)
                {
                    seaTruckSegment = seaTruckSegment.frontConnection.GetConnection().truckSegment;
                }
                else
                {                    
                    seaTruckSegment = null;
                }
            }
        }
    }
}
