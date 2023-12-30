using UnityEngine;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML documentation

    public class TransitDockListener : MonoBehaviour
    { 
        private SeaTruckSegment segmentBase = null;
        
        private bool _isKinematicsLocked;        
       
        public delegate void OnTransitDocked(bool isInTransition);
        public event OnTransitDocked onTransitDocked;        

        public bool IsKinematicsChanged
        {
            get => _isKinematicsLocked;

            private set
            {
                if (_isKinematicsLocked != value)
                {
                    _isKinematicsLocked = value;
                    BZLogger.Trace("TransitDockListener: onTransitDocked Event triggered!");
                    onTransitDocked?.Invoke(_isKinematicsLocked);
                }
            }
        }

        private void Awake()
        {            
            segmentBase = gameObject.GetComponent<SeaTruckSegment>();            
        }

        private void Start()
        {
            InvokeRepeating("CheckKinematicState", 0, 2);
        }

        private void CheckKinematicState()
        {
            IsKinematicsChanged = (bool)GetFirstSegment().GetPrivateField("isKinematicsLocked");
        }

        private void OnDestroy()
        {
            CancelInvoke();
        }

        private SeaTruckSegment GetFirstSegment()
        {
            SeaTruckSegment segment = segmentBase;

            while (segment)
            {
                if (!segment.isFrontConnected)
                {
                    return  segment;                    
                }                

                segment = segment.frontConnection.GetConnection().truckSegment;
            }

            return segmentBase;
        }
    }
}
