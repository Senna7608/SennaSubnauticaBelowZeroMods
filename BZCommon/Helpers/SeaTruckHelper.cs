using SMLHelper.V2.Handlers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UWE;

namespace BZCommon
{
    public class SeaTruckHelper
    {
        private class SeaTruckSlotListener : MonoBehaviour
        {
            public SeaTruckHelper thisHelper = null;

            private void Update()
            {
                if (thisHelper == null || !thisHelper.IsPiloted())
                    return;

                thisHelper.ActiveSlot = thisHelper.GetActiveSlotID();
            }
        }

        private class SeaTruckDockingListener : MonoBehaviour
        {
            public SeaTruckHelper thisHelper = null;

            private void Update()
            {
                if (thisHelper == null || !thisHelper.IsPiloted())
                {
                    return;
                }

                thisHelper.IsDocked = thisHelper.thisDockable.isDocked;
            }
        }

        private class SeaTruckDamageListener : MonoBehaviour
        {
            public SeaTruckHelper thisHelper = null;

            private void Update()
            {
                if (thisHelper == null || !thisHelper.IsPiloted())
                    return;

                thisHelper.Damage = thisHelper.thisDamageInfo.damage;
            }
        }

        private class SeaTruckPilotingListener : MonoBehaviour
        {
            public SeaTruckHelper thisHelper = null;

            public void OnPilotBegin()
            {
                thisHelper.onPilotingBegin?.Invoke();
            }

            public void OnPilotEnd()
            {
                thisHelper.onPilotingEnd?.Invoke();
            }
        }

        public GameObject MainCab { get; private set; }

        public SeaTruckAnimation thisAnimation;
        public SeaTruckAquarium thisAquarium;
        public SeaTruckConnection thisConnection;
        public SeaTruckDockingBay thisDockingBay;
        public SeaTruckEffects thisEffects;
        public SeaTruckLights thisLights;
        public SeaTruckSegment thisSegment;
        public SeaTruckConnectingDoor thisConnectingDoor;
        public SeaTruckMotor thisMotor;
        public SeaTruckTeleporter thisTeleporter;
        public PingInstance thisPingInstance;
        public Dockable thisDockable;
        public ColorNameControl thisColorNameControl;
        public LiveMixin thisLiveMixin;
        private DamageInfo thisDamageInfo;
        public DealDamageOnImpact thisDealDamageOnImpact;
        
        public WorldForces thisWorldForces;
        public GameObject thisInputStackDummy;
        public Int2 thisLeverDirection;
        public float thisAnimAccel;

        public SeaTruckUpgrades thisUpgrades;
        public IQuickSlots thisQuickSlots;

        public float[] quickSlotTimeUsed;
        public float[] quickSlotCooldown;
        public float[] quickSlotCharge;

        public string[] slotIDs;
        public Dictionary<string, int> slotIndexes;
        public Dictionary<TechType, float> crushDepths;
        public Equipment modules;

        public Event<int> OnActiveSlotChanged;
        public Event<bool> OnDockedChanged;
        public Event<float> OnDamageReceived;

        public PowerRelay powerRelay;

        private List<IItemsContainer> containers = new List<IItemsContainer>();
        private List<GameObject> handTargets = new List<GameObject>();

        public bool isReady = false;

        private int _activeSlot;

        private bool _isDocked;

        private float _damage;

        private readonly bool[] listenersEnabled = new bool[3];

        public uGUI_SeaTruckHUD thisHUD;

        public delegate void OnPilotingBegin();
        public delegate void OnPilotingEnd();
        public OnPilotingBegin onPilotingBegin;
        public OnPilotingEnd onPilotingEnd;

        public int ActiveSlot
        {
            get => _activeSlot;

            set
            {
                if (_activeSlot != value)
                {
                    _activeSlot = value;
                    OnActiveSlotChanged.Trigger(_activeSlot);
                }
            }
        }

        public bool IsDocked
        {
            get => _isDocked;

            set
            {
                if (_isDocked != value)
                {
                    _isDocked = value;
                    OnDockedChanged.Trigger(_isDocked);
                }
            }
        }

        public float Damage
        {
            get => _damage;

            set
            {
                if (_damage != value)
                {
                    _damage = value;
                    OnDamageReceived.Trigger(_damage);
                }
            }
        }

        public SeaTruckHelper
            (
            GameObject Seatruck,
            bool slotListener,
            bool dockListener,
            bool damageListener
            )
        {

            BZLogger.Debug("SeatruckHelper", $"constructor on this Seatruck has started. ID: [{Seatruck.GetInstanceID()}]");

            MainCab = Seatruck;
            listenersEnabled[0] = slotListener;
            listenersEnabled[1] = dockListener;
            listenersEnabled[2] = damageListener;

            Init();

            BZLogger.Debug("SeatruckHelper", $"constructor on this Seatruck has finished. ID: [{Seatruck.GetInstanceID()}]");
        }

        private void Init()
        {
            thisUpgrades = MainCab.GetComponent<SeaTruckUpgrades>();
            thisAnimation = MainCab.GetComponent<SeaTruckAnimation>();
            thisAquarium = MainCab.GetComponent<SeaTruckAquarium>();
            thisConnection = MainCab.GetComponent<SeaTruckConnection>();
            thisDockingBay = MainCab.GetComponent<SeaTruckDockingBay>();
            thisEffects = MainCab.GetComponent<SeaTruckEffects>();
            thisLights = MainCab.GetComponent<SeaTruckLights>();
            thisSegment = MainCab.GetComponent<SeaTruckSegment>();
            thisConnectingDoor = MainCab.GetComponent<SeaTruckConnectingDoor>();
            thisMotor = MainCab.GetComponent<SeaTruckMotor>();
            thisTeleporter = MainCab.GetComponent<SeaTruckTeleporter>();
            thisPingInstance = MainCab.GetComponent<PingInstance>();
            thisDockable = MainCab.GetComponent<Dockable>();
            thisColorNameControl = MainCab.GetComponent<ColorNameControl>();
            thisLiveMixin = thisSegment.liveMixin;
            thisDamageInfo = thisLiveMixin.GetPrivateField("damageInfo") as DamageInfo;
            thisDealDamageOnImpact = MainCab.GetComponent<DealDamageOnImpact>();

            thisWorldForces = MainCab.GetComponent<WorldForces>();
            thisInputStackDummy = thisMotor.GetPrivateField("inputStackDummy") as GameObject;
            thisLeverDirection = (Int2)thisMotor.GetPrivateProperty("leverDirection", BindingFlags.SetProperty);
            thisAnimAccel = (float)thisMotor.GetPrivateField("animAccel", BindingFlags.SetField);

            slotIDs = thisUpgrades.GetPrivateField("slotIDs", BindingFlags.Static) as string[];
            slotIndexes = thisUpgrades.GetPrivateField("slotIndexes") as Dictionary<string, int>;
            crushDepths = thisUpgrades.GetPrivateField("crushDepths", BindingFlags.Static) as Dictionary<TechType, float>;

            quickSlotTimeUsed = thisUpgrades.GetPrivateField("quickSlotTimeUsed", BindingFlags.SetField) as float[];
            quickSlotCooldown = thisUpgrades.GetPrivateField("quickSlotCooldown", BindingFlags.SetField) as float[];
            quickSlotCharge = thisUpgrades.GetPrivateField("quickSlotCharge", BindingFlags.SetField) as float[];

            thisQuickSlots = MainCab.GetComponent<IQuickSlots>();

            powerRelay = thisUpgrades.relay;

            modules = thisUpgrades.modules;

            thisHUD = uGUI.main.GetComponentInChildren<uGUI_SeaTruckHUD>();

            if (listenersEnabled[0])
            {
                OnActiveSlotChanged = new Event<int>();
                SeaTruckSlotListener thisSlotListener = MainCab.AddComponent<SeaTruckSlotListener>();
                thisSlotListener.thisHelper = this;

                BZLogger.Debug("SeatruckHelper", $"Slot Listener component added to this Seatruck. ID: [{MainCab.GetInstanceID()}]");                
            }

            if (listenersEnabled[1])
            {
                OnDockedChanged = new Event<bool>();
                SeaTruckDockingListener thisDockListener = MainCab.AddComponent<SeaTruckDockingListener>();
                thisDockListener.thisHelper = this;

                BZLogger.Debug("SeatruckHelper", $"Docking Listener component added to this Seatruck. ID: [{MainCab.GetInstanceID()}]");
            }

            if (listenersEnabled[2])
            {
                OnDamageReceived = new Event<float>();
                SeaTruckDamageListener thisDamageListener = MainCab.AddComponent<SeaTruckDamageListener>();
                thisDamageListener.thisHelper = this;

                BZLogger.Debug("SeatruckHelper", $"Damage Listener component added to this Seatruck. ID: [{MainCab.GetInstanceID()}]");
            }

            isReady = true;

            SeaTruckPilotingListener truckPilotingListener = MainCab.AddComponent<SeaTruckPilotingListener>();
            truckPilotingListener.thisHelper = this;

            DebugSlots();
        }
        
        public bool IsPowered()
        {
            return !thisMotor.requiresPower || (thisMotor.relay && thisMotor.relay.IsPowered());
        }

        public int GetActiveSlotID()
        {
            return thisQuickSlots.GetActiveSlotID();
        }

        public float GetSlotProgress(int slotID)
        {
            return thisQuickSlots.GetSlotProgress(slotID);
        }

        public int GetSlotIndex(string slot)
        {
            if (slotIndexes.TryGetValue(slot, out int result))
            {
                return result;
            }

            return -1;
        }

        public bool IsPiloted()
        {
            return thisMotor.IsPiloted();
        }        

        public float GetWeight()
        {
            return thisSegment.GetWeight() + thisSegment.GetAttachedWeight() * (thisMotor.horsePowerUpgrade ? 0.65f : 0.8f);
        }

        public InventoryItem GetSlotItem(int slotID)
        {
            return thisQuickSlots.GetSlotItem(slotID);
        }

        public TechType GetSlotBinding(int slotID)
        {
            return thisQuickSlots.GetSlotBinding(slotID);
        }

        public int GetSlotCount()
        {
            return thisQuickSlots.GetSlotCount();
        }

        float GetSlotCharge(int slotID)
        {
            return thisQuickSlots.GetSlotCharge(slotID);
        }

        public QuickSlotType GetQuickSlotType(int slotID, out TechType techType)
        {
            if (slotID >= 0 && slotID < slotIDs.Length)
            {
                techType = modules.GetTechTypeInSlot(slotIDs[slotID]);

                if (techType != TechType.None)
                {
                    return TechData.GetSlotType(techType);
                }
            }

            techType = TechType.None;

            return QuickSlotType.None;
        }

        public ItemsContainer GetSeamothStorageInSlot(int slotID, TechType techType)
        {
            InventoryItem slotItem = GetSlotItem(slotID);

            if (slotItem == null)
            {
                return null;
            }

            Pickupable item = slotItem.item;

            if (item.GetTechType() != techType)
            {
                return null;
            }

            if (item.TryGetComponent(out SeamothStorageContainer component))
            {
                DebugStorageContainer(slotID, component);

                return component.container;
            }

            return null;            
        }

        public ItemsContainer GetSeaTruckStorageInSlot(int slotID)
        {
            InventoryItem slotItem = GetSlotItem(slotID);

            if (slotItem == null)
            {
                return null;
            }
            
            if (slotItem.item.TryGetComponent(out SeamothStorageContainer component))
            {
                DebugStorageContainer(slotID, component);

                return component.container;
            }

            return null;
        }

        public bool TryOpenSeaTruckStorageContainer(int slotID)
        {
            ItemsContainer container = GetSeaTruckStorageInSlot(slotID);

            if (container != null)
            {
                PDA pda = Player.main.GetPDA();
                Inventory.main.SetUsedStorage(container, false);
                pda.Open(PDATab.Inventory, null, null, -1f);
                return true;
            }

            return false;
        }

        private void GetAllStorages()
        {
            containers.Clear();

            if (!TechTypeHandler.TryGetModdedTechType("SeaTruckStorage", out TechType techType))
                return;

            foreach (string slot in slotIDs)
            {
                if (modules.GetTechTypeInSlot(slot) == techType)
                {
                    InventoryItem item = modules.GetItemInSlot(slot);                    

                    if (item.item.TryGetComponent(out SeamothStorageContainer component))
                    {
                        containers.Add(component.container);
                    }
                }
            }
        }

        public bool HasRoomForItem(Pickupable pickupable)
        {
            GetAllStorages();

            foreach (ItemsContainer container in containers)
            {
                if (container.HasRoomFor(pickupable))
                {
                    return true;
                }
            }

            return false;
        }

        public ItemsContainer GetRoomForItem(Pickupable pickupable)
        {
            GetAllStorages();

            foreach (ItemsContainer container in containers)
            {
                if (container.HasRoomFor(pickupable))
                {
                    return container;
                }
            }

            return null;
        }
        
        public bool IsValidSeaTruckStorageContainer(int slotID)
        {
            try
            {
                GameObject storageLeft = MainCab.transform.Find("StorageRoot/StorageLeft").gameObject;

                if (storageLeft)
                {
                    Component component = storageLeft.GetComponent("SeaTruckStorage.SeaTruckStorageInput");

                    int leftSlotID = (int)component.GetPublicField("slotID");

                    if (leftSlotID == slotID)
                        return true;
                }
            }
            catch
            {
                return false;
            }

            try
            {
                GameObject storageRight = MainCab.transform.Find("StorageRoot/StorageRight").gameObject;

                if (storageRight)
                {
                    Component component = storageRight.GetComponent("SeaTruckStorage.SeaTruckStorageInput");

                    int rightSlotID = (int)component.GetPublicField("slotID");

                    if (rightSlotID == slotID)
                        return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public bool IsSeatruckChained()
        {
            return (thisSegment.rearConnection != null && thisSegment.rearConnection.occupied) ? true : false;            
        }

        public bool IsDockingModulePresent()
        {
            if (!IsSeatruckChained())
            {
                return false;
            }

            List<SeaTruckSegment> chain = new List<SeaTruckSegment>();

            thisSegment.GetTruckChain(chain);
                        
            foreach (SeaTruckSegment segment in chain)
            {
                if (segment.name.Equals("SeaTruckDockingModule(Clone)"))
                {
                    return true;
                }                   
            }

            return false;
        }


        public float GetSeatruckZShift()
        {
            float zShift = 0;

            if (!IsSeatruckChained())
            {
                return zShift;
            }

            List<SeaTruckSegment> chain = new List<SeaTruckSegment>();

            thisSegment.GetTruckChain(chain);

            SeaTruckSegment lastSegment = chain.GetLast();                      

            if (lastSegment != thisSegment)
            {
                foreach (SeaTruckSegment segment in chain)
                {
                    if (segment == thisSegment)
                    {
                        continue;
                    }

                    float shift = Mathf.Abs(segment.transform.localPosition.z);

                    if (segment.name.Equals("SeaTruckDockingModule(Clone)"))
                    {
                        zShift += shift - 1.32f;                        
                    }
                    else
                    {
                        zShift += shift;
                    }
                }                

                zShift = zShift * -1f;                
            }            

            return zShift;
        }

        public List<GameObject> GetWheelTriggers()
        {
            handTargets.Clear();

            if (!IsSeatruckChained())
            {
                return handTargets;
            }

            List<SeaTruckSegment> chain = new List<SeaTruckSegment>();

            thisSegment.GetTruckChain(chain);
                        
            foreach (SeaTruckSegment segment in chain)
            {
                if (segment == thisSegment)
                {
                    continue;
                }

                GenericHandTarget handtarget = segment.GetComponentInChildren<GenericHandTarget>(true);

                if (handtarget != null)
                {
                    handTargets.Add(handtarget.gameObject);
                }                    
            }            

            DebugTriggers();

            return handTargets;
        }


        [Conditional("DEBUG")]
        void DebugSlots()
        {
            BZLogger.Debug("SeatruckHelper", $"Upgrade slots check started on this Seatruck. ID: [{MainCab.GetInstanceID()}]");

            foreach (string slot in slotIDs)
            {
                BZLogger.Debug("SeatruckHelper", $"Found slot: [{slot}]");
            }

            BZLogger.Debug("SeatruckHelper", $"Upgrade slots check finished on this Seatruck. ID: [{MainCab.GetInstanceID()}]");
        }

        [Conditional("DEBUG")]
        void DebugStorageContainer(int slotID, SeamothStorageContainer container)
        {
            BZLogger.Debug("SeatruckHelper", $"Seamoth storage container found on slot [{slotID}], name [{container.name}]");

            foreach (TechType techtype in container.allowedTech)
            {
                BZLogger.Debug("SeatruckHelper", $"allowedTech: {techtype}");
            }
        }


        [Conditional("DEBUG")]
        void DebugTriggers()
        {
            BZLogger.Debug("SeatruckHelper", "Debug handTargets:");

            foreach (GameObject trigger in handTargets)
            {
                BZLogger.Log($"handtarget name: {trigger.name}");
            }
        }

    }
}
