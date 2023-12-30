using CheatManagerZero.Configuration;
using System.Collections;
using UnityEngine;

namespace CheatManagerZero
{
    public class FragmentTracker : MonoBehaviour
    {        
        public PingInstance pingInstance;
        public TechType techType = TechType.None;        
        public string techName;
        private bool isSignalReady = false;

        public void Awake()
        {
            TechTag techTag = gameObject.GetComponent<TechTag>();

            if (techTag != null)
            {
                techType = techTag.type;                
            }
            if (techTag == null)
            {
               Pickupable pickupable = gameObject.GetComponent<Pickupable>();
               techType = pickupable.GetTechType();
            }

            techName = Language.main.Get(TechTypeExtensions.AsString(techType, false));
        }

        public void Start()
        {
            pingInstance = gameObject.EnsureComponent<PingInstance>();
            pingInstance.origin = transform;
            pingInstance.displayPingInManager = false;
            pingInstance.pingType = PingType.Signal;
            pingInstance.visible = true;
            pingInstance.minDist = 5;
            pingInstance.range = 10;
            pingInstance.colorIndex = 2;

            StartCoroutine(InitializeAsync());
        }

        public void OnTrackerVisibleChange()
        {
            if (isSignalReady && pingInstance != null && pingInstance.Initialized == true)
            {
                pingInstance.SetVisible(CMZ_Config.isFragmentTrackerEnabled);                
            }
        }

        private IEnumerator InitializeAsync()
        {
            while (!pingInstance.Initialized)
            {
                yield return null;
            }

            pingInstance.SetLabel(techName);
            pingInstance.SetVisible(CMZ_Config.isFragmentTrackerEnabled);
            isSignalReady = true;
            
            yield break;
        }

        public void OnDestroy()
        {
            if (pingInstance != null)
            {
                PingManager.Unregister(pingInstance);
            }
        }        
    }
}
