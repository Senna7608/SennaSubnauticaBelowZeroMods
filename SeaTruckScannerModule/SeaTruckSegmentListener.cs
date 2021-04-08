using UnityEngine;

namespace SeaTruckScannerModule
{
    public class SeaTruckSegmentListener : MonoBehaviour
    {
        public delegate void OnPlayerEntered();
        public event OnPlayerEntered onPlayerEntered;

        public delegate void OnPlayerExited();
        public event OnPlayerExited onPlayerExited;

        private void OnPlayerEnter()
        {
            onPlayerEntered?.Invoke();
        }

        private void OnPlayerExit()
        {
            onPlayerExited?.Invoke();
        }
    }
}
