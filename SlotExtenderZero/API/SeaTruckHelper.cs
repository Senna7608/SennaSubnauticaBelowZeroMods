using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using System.Linq;
using BZHelper;
using BZHelper.NautilusHelpers;

namespace SlotExtenderZero.API
{
    public enum SeatruckModule
    {
        MainCab,
        AquariumModule,
        DockingModule,
        FabricatorModule,
        TeleportationModule,
        SleeperModule,
        StorageModule,
        ScannerModule
        //PlanterModule
    }

    [DisallowMultipleComponent]
    public class SeatruckHelper : MonoBehaviour
    {
        public GameObject MainCab { get; private set; }
        
        public SeaTruckConnection TruckConnection { get; private set; }
        public SeaTruckDockingBay TruckDockingBay { get; private set; }
        public SeaTruckEffects TruckEffects { get; private set; }
        public SeaTruckLights TruckLights { get; private set; }
        public SeaTruckSegment TruckSegment { get; private set; }
        public SeaTruckConnectingDoor TruckConnectingDoor { get; private set; }
        public SeaTruckMotor TruckMotor { get; private set; }        
        public SeaTruckUpgrades TruckUpgrades { get; private set; }

        public LiveMixin TruckLiveMixin { get; private set; }
        public PowerRelay TruckPowerRelay { get; private set; }
        public Equipment TruckEquipment { get; private set; }
        public WorldForces TruckWorldForces { get; private set; }
        public Dockable TruckDockable { get; private set; }
        public PingInstance TruckPingInstance { get; private set; }

        public LightingController TruckLightingController { get; private set; }
        public SkyApplier TruckGlassApplier { get; private set; }
        public SkyApplier TruckInteriorApplier { get; private set; }
        public SkyApplier TruckOuterApplier { get; private set; }

        public IQuickSlots TruckQuickSlots { get; private set; }

        public ColorNameControl TruckColorNameControl { get; private set; }        
        public DealDamageOnImpact TruckDealDamageOnImpact { get; private set; }

        public uGUI_SeaTruckHUD TruckHUD { get; private set; }

        public bool TruckSegmentIsDirty { get; private set; }

        private GameObject _inputStackDummy = null;

        public GameObject TruckInputStackDummy
        {
            get
            {
                if (_inputStackDummy == null)
                {
                    _inputStackDummy = MainCab.transform.Find("inputStackDummy").gameObject;
                }

                return _inputStackDummy;
            }
        }

        public Int2 TruckLeverDirection { get; set; }
        public float TruckAnimAccel { get; set; }
        //public float[] TruckQuickSlotTimeUsed { get; set; }
        //public float[] TruckQuickSlotCooldown { get; set; }
        //public float[] TruckQuickSlotCharge { get; set; }

        public string[] TruckSlotIDs { get; private set; }
        public Dictionary<string, int> TruckSlotIndexes { get; private set; }
        public Dictionary<TechType, float> TruckCrushDepths { get; private set; }        
        
        internal bool isReady = false;
        public bool IsReady
        {
            get
            {
                return isReady;
            }
        }


        private List<IItemsContainer> containers = new List<IItemsContainer>();
        private List<GameObject> handTargets = new List<GameObject>();
        private List<SeaTruckSegment> chain = new List<SeaTruckSegment>();

        private static readonly Dictionary<SeatruckModule, string> moduleStringCache = new Dictionary<SeatruckModule, string>()
        {
            { SeatruckModule.MainCab, "SeaTruck(Clone)" },
            { SeatruckModule.AquariumModule, "SeaTruckAquariumModule(Clone)" },
            { SeatruckModule.DockingModule, "SeaTruckDockingModule(Clone)" },
            { SeatruckModule.FabricatorModule, "SeaTruckFabricatorModule(Clone)" },
            { SeatruckModule.SleeperModule, "SeaTruckSleeperModule(Clone)" },
            { SeatruckModule.StorageModule, "SeaTruckStorageModule(Clone)" },
            { SeatruckModule.TeleportationModule, "SeaTruckTeleportationModule(Clone)" },
            { SeatruckModule.ScannerModule, "SeaTruckScannerModule(Clone)" }
        };

        private DamageInfo damageInfo;

        public delegate void OnActiveSlotChanged(int slotID);
        public event OnActiveSlotChanged onActiveSlotChanged;

        public delegate void OnDockedChanged(bool isDocked, bool isInTransition);
        public event OnDockedChanged onDockedChanged;

        public delegate void OnDamageReceived(float damage);
        public event OnDamageReceived onDamageReceived;        

        public delegate void OnPilotingBegin();
        public event OnPilotingBegin onPilotingBegin;

        public delegate void OnPilotingEnd();        
        public event OnPilotingEnd onPilotingEnd;

        public delegate void OnPlayerEntered();
        public event OnPlayerEntered onPlayerEntered;

        public delegate void OnPlayerExited();
        public event OnPlayerExited onPlayerExited;

        public delegate void OnUpgradeModuleEquip(int slotID, TechType techType);
        public event OnUpgradeModuleEquip onUpgradeModuleEquip;

        public delegate void OnUpgradeModuleUnEquip(int slotID, TechType techType);        
        public event OnUpgradeModuleUnEquip onUpgradeModuleUnEquip;

        private int _activeSlot;

        public int ActiveSlot
        {
            get => _activeSlot;

            private set
            {
                if (_activeSlot != value)
                {
                    _activeSlot = value;
                    onActiveSlotChanged?.Invoke(_activeSlot);                    
                }
            }
        }

        private bool _isDocked;

        public bool IsDocked
        {
            get => _isDocked;

            private set
            {
                if (_isDocked != value)
                {
                    _isDocked = value;
                    BZLogger.Trace("API/Helper: OnDockedChanged Event triggered!");
                    onDockedChanged?.Invoke(_isDocked, TruckDockable.isInTransition);                    
                }
            }
        }

        private float _damage;

        public float Damage
        {
            get => TruckLiveMixin.damageInfo.damage;
             
            private set
            {
                if (_damage != value)
                {
                    _damage = value;
                    onDamageReceived?.Invoke(_damage);                    
                }
            }            
        }

        public string TruckName => TruckPingInstance?.GetLabel();

        public SeaTruckAnimation TruckAnimation => TruckSegment?.seatruckanimation;

        public GameObject TruckAquariumModule => GetTruckModule(SeatruckModule.AquariumModule);
        public GameObject TruckDockingModule => GetTruckModule(SeatruckModule.DockingModule);
        public GameObject TruckFabricatorModule => GetTruckModule(SeatruckModule.FabricatorModule);
        public GameObject TruckSleeperModule => GetTruckModule(SeatruckModule.SleeperModule);
        public GameObject TruckStorageModule => GetTruckModule(SeatruckModule.StorageModule);
        public GameObject TruckTeleportationModule => GetTruckModule(SeatruckModule.TeleportationModule);

        private void Awake()            
        {
            MainCab = gameObject;

            BZLogger.Trace($"API/Helper: Awake started, ID: [{MainCab.GetInstanceID()}]");            
            
            TruckUpgrades = MainCab.GetComponent<SeaTruckUpgrades>();            
            TruckConnection = MainCab.GetComponent<SeaTruckConnection>();
            TruckDockingBay = MainCab.GetComponent<SeaTruckDockingBay>();
            TruckEffects = MainCab.GetComponent<SeaTruckEffects>();
            TruckLights = MainCab.GetComponent<SeaTruckLights>();
            TruckSegment = MainCab.GetComponent<SeaTruckSegment>();
            TruckConnectingDoor = MainCab.GetComponent<SeaTruckConnectingDoor>();
            TruckMotor = MainCab.GetComponent<SeaTruckMotor>();

            List<SkyApplier> skyAppliers = new List<SkyApplier>();
            MainCab.GetComponents(skyAppliers);

            foreach (SkyApplier skyApplier in skyAppliers)
            {
                switch(skyApplier.anchorSky)
                {
                    case Skies.Auto:
                        TruckOuterApplier = skyApplier;
                        break;
                    case Skies.Custom:
                        TruckInteriorApplier = skyApplier;
                        break;
                    case Skies.BaseGlass:
                        TruckGlassApplier = skyApplier;
                        break;
                }
            }

            TruckLightingController = MainCab.GetComponent<LightingController>();
            TruckPingInstance = MainCab.GetComponent<PingInstance>();
            TruckDockable = MainCab.GetComponent<Dockable>();
            TruckColorNameControl = MainCab.GetComponent<ColorNameControl>();
            TruckLiveMixin = TruckSegment.liveMixin;

            //damageInfo = TruckLiveMixin.GetPrivateField("damageInfo") as DamageInfo;
            damageInfo = TruckLiveMixin.damageInfo;

            TruckDealDamageOnImpact = MainCab.GetComponent<DealDamageOnImpact>();

            //TruckSegmentIsDirty = (bool)TruckSegment.GetPrivateField("isDirty");
            TruckSegmentIsDirty = TruckSegment.isDirty;

            TruckWorldForces = MainCab.GetComponent<WorldForces>();

            //TruckLeverDirection = (Int2)TruckMotor.GetPrivateProperty("leverDirection", BindingFlags.SetProperty);
            TruckLeverDirection = TruckMotor.leverDirection;

            //TruckAnimAccel = (float)TruckMotor.GetPrivateField("animAccel", BindingFlags.SetField);
            TruckAnimAccel = TruckMotor.animAccel;

            //TruckSlotIDs = TruckUpgrades.GetPrivateField("slotIDs", BindingFlags.Static | BindingFlags.Public) as string[];            
            TruckSlotIDs = SeaTruckUpgrades.slotIDs;

            //TruckSlotIndexes = TruckUpgrades.GetPrivateField("slotIndexes",  BindingFlags.Public) as Dictionary<string, int>;
            TruckSlotIndexes = TruckUpgrades.slotIndexes;

            //TruckCrushDepths = TruckUpgrades.GetPrivateField("crushDepths", BindingFlags.Static | BindingFlags.Public) as Dictionary<TechType, float>;
            TruckCrushDepths = SeaTruckUpgrades.crushDepths;

            //TruckQuickSlotTimeUsed = TruckUpgrades.GetPrivateField("quickSlotTimeUsed", BindingFlags.SetField) as float[];
            //TruckQuickSlotTimeUsed = TruckUpgrades.quickSlotTimeUsed;

            //TruckQuickSlotCooldown = TruckUpgrades.GetPrivateField("quickSlotCooldown", BindingFlags.SetField) as float[];
            //TruckQuickSlotCooldown = TruckUpgrades.quickSlotCooldown;

            //TruckQuickSlotCharge = TruckUpgrades.GetPrivateField("quickSlotCharge", BindingFlags.SetField) as float[];
            //TruckQuickSlotCharge = TruckUpgrades.quickSlotCharge;

            TruckQuickSlots = MainCab.GetComponent<IQuickSlots>();

            TruckPowerRelay = TruckUpgrades.relay;

            TruckEquipment = TruckUpgrades.modules;

            TruckHUD = uGUI.main.GetComponentInChildren<uGUI_SeaTruckHUD>();

            TruckEquipment.onEquip += OnEquip;
            TruckEquipment.onUnequip += OnUnEquip;

            isReady = true;

            TraceSlots();

            TraceQuickSlots();

            TraceCrushDepths();

            BZLogger.Trace($"API/Helper: Awake finished, ID: [{MainCab.GetInstanceID()}]");

            BZLogger.Log($"API/Helper: SeaTruckHelper is ready for this SeaTruck, ID: [{MainCab.GetInstanceID()}]");
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            onUpgradeModuleEquip?.Invoke(GetSlotIndex(slot), item.item.GetTechType());
        }

        private void OnUnEquip(string slot, InventoryItem item)
        {
            onUpgradeModuleUnEquip?.Invoke(GetSlotIndex(slot), item.item.GetTechType());
        }

        private void Update()
        {
            if (!isReady)
            {
                return;
            }

            ActiveSlot = GetActiveSlotID();

            IsDocked = TruckDockable.isDocked;

            Damage = damageInfo.damage;            
        }

        private void OnPilotBegin()
        {
            onPilotingBegin?.Invoke();
        }

        private void OnPilotEnd()
        {
            onPilotingEnd?.Invoke();
        }

        private void OnPlayerEnter()
        {
            onPlayerEntered?.Invoke();
        }

        private void OnPlayerExit()
        {
            onPlayerExited?.Invoke();
        }

        public bool IsPowered()
        {
            return !TruckMotor.requiresPower || (TruckMotor.relay && TruckMotor.relay.IsPowered());
        }

        public int GetActiveSlotID()
        {
            return TruckQuickSlots.GetActiveSlotID();
        }

        public float GetSlotProgress(int slotID)
        {
            return TruckQuickSlots.GetSlotProgress(slotID);
        }

        public int GetSlotIndex(string slot)
        {
            if (TruckSlotIndexes.TryGetValue(slot, out int result))
            {
                return result;
            }

            return -1;
        }

        public bool IsPiloted()
        {
            return TruckMotor.IsPiloted();
        }        

        public float GetWeight()
        {
            return TruckSegment.GetWeight() + TruckSegment.GetAttachedWeight() * (TruckMotor.horsePowerUpgrade ? 0.65f : 0.8f);
        }

        public InventoryItem GetSlotItem(int slotID)
        {
            return TruckQuickSlots.GetSlotItem(slotID);
        }

        public TechType GetSlotBinding(int slotID)
        {
            return TruckQuickSlots.GetSlotBinding(slotID);
        }

        public int GetSlotCount()
        {
            return TruckQuickSlots.GetSlotCount();
        }

        public float GetSlotCharge(int slotID)
        {
            return TruckQuickSlots.GetSlotCharge(slotID);
        }

        public QuickSlotType GetQuickSlotType(int slotID, out TechType techType)
        {
            if (slotID >= 0 && slotID < TruckSlotIDs.Length)
            {
                techType = TruckEquipment.GetTechTypeInSlot(TruckSlotIDs[slotID]);

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
            BZLogger.Trace($"helper.GetSeamothStorageInSlot: slotID: {slotID} techType: {techType}");

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
                TraceStorageContainer(slotID, component);

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
                TraceStorageContainer(slotID, component);

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
                pda.Open(PDATab.Inventory, null, null);
                return true;
            }

            return false;
        }

        private void GetAllStorages()
        {
            containers.Clear();

            //if (!TechTypeHandler.TryGetModdedTechType("SeaTruckStorage", out TechType techType))
            //return;
            if (!ModdedTechTypeHelper.Main.IsModdedTechTypeExists("SeaTruckStorage", out TechType techType))
                return;

            foreach (string slot in TruckSlotIDs)
            {
                if (TruckEquipment.GetTechTypeInSlot(slot) == techType)
                {
                    InventoryItem item = TruckEquipment.GetItemInSlot(slot);                    

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

                if (storageLeft != null)
                {
                    Component component = storageLeft.GetComponent("SeaTruckStorage.SeaTruckStorageInput");

                    int leftSlotID = (int)component.GetPrivateField("slotID", BindingFlags.Public);

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

                if (storageRight != null)
                {
                    Component component = storageRight.GetComponent("SeaTruckStorage.SeaTruckStorageInput");

                    int rightSlotID = (int)component.GetPrivateField("slotID", BindingFlags.Public);

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
            return TruckSegment.rearConnection != null && TruckSegment.rearConnection.occupied;            
        }

        public bool IsDockingModulePresent()
        {
            if (!IsSeatruckChained())
            {
                return false;
            }

            chain.Clear();

            TruckSegment.GetTruckChain(chain);
                        
            foreach (SeaTruckSegment segment in chain)
            {
                if (segment.name.Equals(moduleStringCache[SeatruckModule.DockingModule]))
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

            chain.Clear();

            TruckSegment.GetTruckChain(chain);
            
            foreach (SeaTruckSegment segment in chain)
            {
                if (segment == TruckSegment)
                {
                    continue;
                }

                float shift = Mathf.Abs(segment.transform.localPosition.z);

                if (segment.name.Equals(moduleStringCache[SeatruckModule.DockingModule]))
                {
                    zShift += shift - 1.32f;                        
                }
                else
                {
                    zShift += shift;
                }
            }                                

            return zShift * -1f;
        }

        public GameObject GetTruckModule(SeatruckModule module)
        {
            chain.Clear();

            TruckSegment.GetTruckChain(chain);

            moduleStringCache.TryGetValue(module, out string moduleString);

            foreach (SeaTruckSegment segment in chain)
            {
                if (segment.name.Equals(moduleString))
                {
                    return segment.gameObject;
                }
            }

            return null;
        }

        public List<GameObject> GetWheelTriggers()
        {
            handTargets.Clear();

            if (!IsSeatruckChained())
            {
                return handTargets;
            }

            chain.Clear();

            TruckSegment.GetTruckChain(chain);
                        
            foreach (SeaTruckSegment segment in chain)
            {
                if (segment == TruckSegment)
                {
                    continue;
                }

                GenericHandTarget handtarget = segment.GetComponentInChildren<GenericHandTarget>(true);

                if (handtarget != null)
                {
                    handTargets.Add(handtarget.gameObject);
                }                    
            }            

            TraceTriggers();

            return handTargets;
        }

        public void RegisterRendererToSkyApplier(SkyApplier truckSkyApplier, GameObject objectContainRenderer)
        {            
            Renderer renderer = objectContainRenderer.GetComponent<Renderer>();

            if (renderer == null)
            {
                throw new System.ArgumentException($"GameObject [{objectContainRenderer.name}] does not have Renderer component!");
            }           

            List<Renderer> renderers = truckSkyApplier.renderers.ToList();

            if (renderers.Contains(renderer))
            {
                return;
            }

            renderers.Add(renderer);

            truckSkyApplier.renderers = renderers.ToArray();

            truckSkyApplier.UpdateSkyIfNecessary();
        }

        public void UnregisterRendererToSkyApplier(SkyApplier truckSkyApplier, GameObject objectContainRenderer)
        {
            Renderer renderer = objectContainRenderer.GetComponent<Renderer>();

            if (renderer == null)
            {
                throw new System.ArgumentException($"GameObject [{objectContainRenderer.name}] does not have Renderer component!");
            }

            List<Renderer> renderers = truckSkyApplier.renderers.ToList();

            if (!renderers.Contains(renderer))
            {
                return;
            }

            renderers.Remove(renderer);

            truckSkyApplier.renderers = renderers.ToArray();
        }

        [Conditional("TRACE")]
        void TraceSlots()
        {
            BZLogger.Trace($"API/Helper: Tracing Upgrade slots (Seatruck), ID: [{MainCab.GetInstanceID()}]");

            foreach (string slot in TruckSlotIDs)
            {
                BZLogger.Trace($"API/Helper: value(string): [{slot}]");
            }

            BZLogger.Trace("Trace completed (Upgrade slots).");
        }

        [Conditional("TRACE")]
        void TraceStorageContainer(int slotID, SeamothStorageContainer container)
        {
            BZLogger.Trace("API/Helper: Tracing Seamoth storage container(s):");

            BZLogger.Trace($"API/Helper: Seamoth storage container found on slot [{slotID}], name [{container.name}]");

            BZLogger.Trace($"Tracing container [{container.name}] allowedTech.");

            foreach (TechType techtype in container.allowedTech)
            {
                BZLogger.Trace($"API/Helper: value(TechType): {techtype}");
            }

            BZLogger.Trace("Trace completed (storage container -> allowedTech).");

            BZLogger.Trace("Trace completed (storage container).");
        }


        [Conditional("TRACE")]
        void TraceTriggers()
        {
            BZLogger.Trace("API/Helper: Tracing handTargets:");

            foreach (GameObject trigger in handTargets)
            {
                BZLogger.Trace($"API/Helper: value(GameObject) name: {trigger.name}");
            }

            BZLogger.Trace("Trace completed.");
        }

        [Conditional("TRACE")]
        void TraceQuickSlots()
        {            
            BZLogger.Trace("API/Helper: Tracing QuickSlots:");
            BZLogger.Trace($"TruckQuickSlots count(int): {TruckQuickSlots.GetSlotCount()}");
            BZLogger.Trace($"TruckQuickSlotTimeUsed length(int): {TruckUpgrades.quickSlotTimeUsed.Length}");
            BZLogger.Trace($"TruckQuickSlotCooldown length(int): {TruckUpgrades.quickSlotCooldown.Length}");
            BZLogger.Trace($"TruckQuickSlotCharge length(int): {TruckUpgrades.quickSlotCharge.Length}");
            BZLogger.Trace("Trace completed.");
        }

        [Conditional("TRACE")]
        void TraceCrushDepths()
        {
            BZLogger.Trace($"API/Helper: Tracing [{TruckName}] crushdepths...");

            foreach (KeyValuePair<TechType, float> kvp in TruckCrushDepths)
            {
                BZLogger.Trace($"API/Helper: key(TechType): {kvp.Key} value(float): [{kvp.Value}]");
            }

            BZLogger.Trace("Trace completed.");
        }
    }
}
