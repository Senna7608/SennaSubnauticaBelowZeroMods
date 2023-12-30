using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UWE;
using BZHelper;

namespace SeaTruckScannerModule
{
    public class SeaTruckScannerModuleManager : MonoBehaviour
    {
        public TransitDockListener transitDockListener = null;        

        public bool isInTransit;
        public GameObject powerSystemRoot;
        public GameObject powerBtn, powerButtON, powerButtOFF;
        public GameObject list;
        public GameObject scanning;
        public GameObject uiUnpowered;
        public TextMeshProUGUI uiUnpoweredText;
        public Animator antennaAnimator;

        public SeaTruckScannerPowerSystem mainPowerSystem;
        public uGUI_SeaTruckScanner mainUI;

        public int numNodesScanned;        
        public TechType typeToScan;                      

        private const float scanningRange = 150f;        

        private const float baseScanTime = 7f;

        private const float powerPerSecond = 0.2f;

        private const float idlePowerPerSecond = 0.05f;        

        private readonly List<ResourceTrackerDatabase.ResourceInfo> resourceNodes = new List<ResourceTrackerDatabase.ResourceInfo>();        

        private double timeLastScan;
        private bool scanActive;
        private bool prevScanActive;
        private float prevScanInterval;
        private float timeLastPowerDrain;

        public bool powered = false;
        public bool powerButtonState = false;

        private string unPoweredText = string.Empty;
        private string cabinUnpoweredText = string.Empty;
        private string mainUnpoweredText = string.Empty;
        private string fullUnpoweredText = string.Empty;
        private string scannerReadyText = string.Empty;        
        private string powerSwitchOnlineText = string.Empty;
        private string powerSwitchDockedText = string.Empty;

        public FMOD_CustomEmitter powerButtonSound;
        public FMODAsset powerButtonON, powerButtonOff;
        private Vector3 startPosition = Vector3.zero;

        public List<IPowerInterface> powerInterfaces;

        private bool onScanning = false;
        public bool OnScanning
        {
            get => onScanning;

            set
            {
                if (onScanning != value)
                {
                    onScanning = value;

                    antennaAnimator.SetBool("scanning", onScanning);
                }
            }
        }

        private bool onScannerOn = true;
        public bool OnScannerOn 
        {
            get => onScannerOn;

            set
            {
                if (onScannerOn != value)
                {
                    onScannerOn = value;

                    antennaAnimator.SetBool("powered", onScannerOn);

                    if (onScannerOn)
                    {
                        mainUI.InvokeRepeating("UpdateAvailableTechTypes", 1f, 15f);
                    }
                    else
                    {
                        mainUI.CancelInvoke("UpdateAvailableTechTypes");
                    }
                }
            } 
        }

        private void Awake()
        {
            powerSystemRoot = transform.parent.Find("powerSystemRoot").gameObject;
            mainPowerSystem = powerSystemRoot.GetComponentInChildren<SeaTruckScannerPowerSystem>(true);

            list = transform.Find("scannerUI/scanner_cullable/list").gameObject;
            scanning = transform.Find("scannerUI/scanner_cullable/scanning").gameObject;
            uiUnpowered = transform.Find("scannerUI/scanner_cullable/Unpowered").gameObject;
            uiUnpoweredText = uiUnpowered.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            uiUnpoweredText.fontSizeMin = 8f;
            uiUnpoweredText.fontSizeMax = 10f;
           
            powerBtn = transform.Find("scannerUI/scanner_cullable/powerBtn").gameObject;
            powerButtON = powerBtn.transform.Find("powerButtON").gameObject;
            powerButtOFF = powerBtn.transform.Find("powerButtOFF").gameObject;

            GenericHandTarget handTarget = powerBtn.GetComponent<GenericHandTarget>();
            
            handTarget.onHandHover.AddListener((data) => OnModHandHover(data));
            handTarget.onHandClick.AddListener((data) => OnModHandClick(data));

            antennaAnimator = transform.parent.Find("antenna").gameObject.GetComponent<Animator>();            
              
            powerButtonON = ScriptableObject.CreateInstance<FMODAsset>();
            powerButtonON.name = "seatruckScanner_powerButton_on";
            powerButtonON.id = "slider_click_on";
            powerButtonON.path = "event:/bz/ui/options_screen/slider_click_on";

            powerButtonOff = ScriptableObject.CreateInstance<FMODAsset>();
            powerButtonOff.name = "seatruckScanner_powerButton_off";
            powerButtonOff.id = "slider_click_off";
            powerButtonOff.path = "event:/bz/ui/options_screen/slider_click_off";

            powerButtonSound = powerBtn.AddComponent<FMOD_CustomEmitter>();

            transitDockListener = UWE.Utils.GetEntityRoot(gameObject).GetComponent<TransitDockListener>();
            transitDockListener.onTransitDocked += OnTransitDocked;
        }

        private void Start()
        {            
            mainPowerSystem.onPowerLevelChanged += OnPowerLevelChanged;
            mainUI = transform.GetComponentInChildren<uGUI_SeaTruckScanner>();

            ResourceTrackerDatabase.onResourceDiscovered += OnResourceDiscovered;
            ResourceTrackerDatabase.onResourceRemoved += OnResourceRemoved;

            unPoweredText = Language.main.Get("unpowered");
            cabinUnpoweredText = Language.main.Get("SeatruckScanner_CabinUnpoweredText");
            mainUnpoweredText = Language.main.Get("SeatruckScanner_MainUnpoweredText");
            fullUnpoweredText = Language.main.Get("SeatruckScanner_FullUnpoweredText");
            scannerReadyText = Language.main.Get("SeatruckScanner_ScannerReadyText");
            powerSwitchOnlineText = Language.main.Get("SeatruckScanner_PowerSwitch");
            powerSwitchDockedText = Language.main.Get("SeatruckScanner_PowerSwitchDocked");

            SeaTruckScannerModule_Main.scannerModules.Add(this);            

            list.SetActive(false);
            uiUnpoweredText.text = unPoweredText;
            uiUnpowered.SetActive(true);
            OnScannerOn = false;                      
        }        

        private void OnPowerLevelChanged(bool isCabinPowered, bool isMainPowered)
        {
            BZLogger.Debug($"OnPowerLevelChanged called with {isCabinPowered} and {isMainPowered} parameter!");

            if (!isCabinPowered && !isMainPowered)
            {                
                uiUnpoweredText.text = fullUnpoweredText;
                powered = false;                
            }
            else if (!isCabinPowered && isMainPowered)
            {
                uiUnpoweredText.text = cabinUnpoweredText;
                powered = false;
            }
            else if (isCabinPowered && !isMainPowered)
            {
                uiUnpoweredText.text = mainUnpoweredText;
                powered = false;
            }
            else if (isCabinPowered && isMainPowered)
            {
                uiUnpoweredText.text = scannerReadyText;
                powered = true;
                timeLastScan = 0.0;
            }

            if (scanActive && !powered)
            {                
                mainUI.OnCancelScan();
            }

            if (scanActive && !isInTransit)
            {
                mainUI.OnCancelScan();
            }

            if (list.activeSelf && !powered)
            {
                list.SetActive(powered);
                OnScannerOn = false;
            }

            if (!powered)
            {                
                powerButtonState = powered;                
                powerButtON.SetActive(powered);
                powerButtOFF.SetActive(!powered);                
            }

            uiUnpowered.SetActive(true);
        }

        public void OnModHandHover(HandTargetEventData data)
        {
            HandReticle main = HandReticle.main;
            
            if (isInTransit)
            {
                main.SetIcon(HandReticle.IconType.HandDeny, 1f);
                main.SetText(HandReticle.TextType.Hand, powerSwitchDockedText, true, GameInput.Button.LeftHand);
                return;
            }

            main.SetIcon(HandReticle.IconType.Hand, 1f);

            if (powered)
            {
                main.SetText(HandReticle.TextType.Hand, powerSwitchOnlineText, true, GameInput.Button.LeftHand);
            }
            else
            {
                main.SetText(HandReticle.TextType.Hand, unPoweredText, true, GameInput.Button.LeftHand);
            }
        }

        public void OnModHandClick(HandTargetEventData data)
        {
            if (!powered)
                return;

            if (isInTransit)
                return;
            
            powerButtonState = !powerButtonState;

            if (powerButtonState)
            {
                powerButtonSound.SetAsset(powerButtonON);
                powerButtonSound.Play();
                powerButtON.SetActive(true);
                powerButtOFF.SetActive(false);
                uiUnpowered.SetActive(false);
                list.SetActive(true);                                
            }
            else
            {                
                powerButtonSound.SetAsset(powerButtonOff);
                powerButtonSound.Play();
                OnScannerOn = false;

                if (scanActive)
                {
                    mainUI.OnCancelScan();                    
                }

                if (list.activeSelf)
                {
                    list.SetActive(false);
                }                
                
                uiUnpoweredText.text = scannerReadyText;
                uiUnpowered.SetActive(true);
                powerButtON.SetActive(false);
                powerButtOFF.SetActive(true);
            }            
        }        

        private void Update()
        {
            if (powerButtonState && powered && !isInTransit)
            {
                if (!OnScannerOn)
                    OnScannerOn = true;

                UpdateScanning();
            }

            if (isInTransit)
            {
                powerButtonSound.SetAsset(powerButtonOff);
                
                OnScannerOn = false;

                if (scanActive)
                {
                    mainUI.OnCancelScan();
                }

                if (list.activeSelf)
                {
                    list.SetActive(false);
                }

                uiUnpoweredText.text = scannerReadyText;
                uiUnpowered.SetActive(true);
                powerButtON.SetActive(false);
                powerButtOFF.SetActive(true);
            }

        }        

        public void OnResourceDiscovered(ResourceTrackerDatabase.ResourceInfo info)
        {
            if (typeToScan == info.techType && (transform.position - info.position).sqrMagnitude <= Mathf.Sqrt(scanningRange))
            {
                resourceNodes.Add(info);
            }
        }

        public void OnResourceRemoved(ResourceTrackerDatabase.ResourceInfo info)
        {
            if (typeToScan == info.techType)
            {
                resourceNodes.Remove(info);
            }
        }

        public TechType GetActiveTechType()
        {
            return typeToScan;
        }           

        public float GetScanRange()
        {
            return scanningRange;
        }

        public float GetScanInterval()
        {
            return baseScanTime;
        }

        private void ObtainResourceNodes(TechType typeToScan)
        {
            resourceNodes.Clear();
            
            Vector3 scannerPos = transform.position;

            ICollection<ResourceTrackerDatabase.ResourceInfo> nodes = ResourceTrackerDatabase.GetNodes(typeToScan);

            if (nodes != null)
            {
                float scanRange = GetScanRange();
                float sqrRange = scanRange * scanRange;

                foreach (ResourceTrackerDatabase.ResourceInfo resourceInfo in nodes)
                {
                    if ((scannerPos - resourceInfo.position).sqrMagnitude <= sqrRange)
                    {                        
                        resourceNodes.Add(resourceInfo);                        
                    }
                }
            }
            
            resourceNodes.Sort(delegate (ResourceTrackerDatabase.ResourceInfo a, ResourceTrackerDatabase.ResourceInfo b)
            {
                float sqrMagnitude = (a.position - scannerPos).sqrMagnitude;
                float sqrMagnitude2 = (b.position - scannerPos).sqrMagnitude;
                return sqrMagnitude.CompareTo(sqrMagnitude2);
            });
            
        }

        public void StartScanning(TechType newTypeToScan)
        {
            startPosition = transform.position;
            typeToScan = newTypeToScan;
            ObtainResourceNodes(typeToScan);            
            scanActive = (typeToScan > TechType.None);
            numNodesScanned = 0;
            timeLastScan = 0.0;
        }

        public IList<ResourceTrackerDatabase.ResourceInfo> GetNodes()
        {
            return resourceNodes;
        }

        public void GetDiscoveredNodes(ICollection<ResourceTrackerDatabase.ResourceInfo> outNodes)
        {
            int num = Mathf.Min(numNodesScanned, resourceNodes.Count);

            for (int i = 0; i < num; i++)
            {
                outNodes.Add(resourceNodes[i]);
            }
        }
        
        private void UpdateBlips()
        {
            if (scanActive)
            {
                int num = Mathf.Min(numNodesScanned + 1, resourceNodes.Count);

                if (num != numNodesScanned)
                {
                    numNodesScanned = num;
                }                
            }
        }        

        private void UpdateScanning()
        {
            DayNightCycle main = DayNightCycle.main;

            if (!main)
            {
                return;
            }

            double timePassed = main.timePassed;

            if (timeLastScan + (double)GetScanInterval() <= timePassed && powered)
            {
                timeLastScan = timePassed;

                if (Vector3.Distance(startPosition, transform.position) > 75f)
                {                    
                    ObtainResourceNodes(typeToScan);
                    startPosition = transform.position;
                }

                UpdateBlips();                
            }
            
            float scanInterval = GetScanInterval();

            if (scanActive != prevScanActive || scanInterval != prevScanInterval)
            {                
                prevScanActive = scanActive;
                prevScanInterval = scanInterval;                
            }

            if (powered && timeLastPowerDrain + 1f < Time.time)
            {
                mainPowerSystem.ConsumePower(scanActive ? powerPerSecond : idlePowerPerSecond);                
                timeLastPowerDrain = Time.time;
            }            
        }        

        private void OnTransitDocked(bool isInTransition)
        {
            BZLogger.Debug($"OnTransitDocked called with {isInTransition} parameter!");

            isInTransit = isInTransition;

            if (isInTransition)
            {
                powerInterfaces = mainPowerSystem.powerRelay.GetPowerSources();

                foreach (IPowerInterface powerInterface in powerInterfaces)
                {
                    GameObject _base = powerInterface.GetGameObject();

                    BZLogger.Debug($"powerinterface: {_base.name}");

                    if (_base.TryGetComponent(out Base baseComponent))
                    {
                        mainPowerSystem.InvokeRepeating("ChargePower", 0, 2f);
                    }
                }                
            }
            else
            {
                mainPowerSystem.CancelInvoke("ChargePower");
                //powerInterfaces.Clear();
            }
        }

        private void OnDestroy()
        {
            if (GameApplication.isQuitting)
            {
                return;
            }
            
            ResourceTrackerDatabase.onResourceDiscovered -= OnResourceDiscovered;
            ResourceTrackerDatabase.onResourceRemoved -= OnResourceRemoved;
            mainPowerSystem.onPowerLevelChanged -= OnPowerLevelChanged;
            
            SeaTruckScannerModule_Main.scannerModules.Remove(this);
        }        
    }
}
