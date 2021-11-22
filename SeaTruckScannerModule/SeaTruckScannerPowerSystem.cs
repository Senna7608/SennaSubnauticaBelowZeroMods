using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UWE;
using System.IO;
using BZCommon.ConfigurationParser;
using BZCommon;

namespace SeaTruckScannerModule
{
    public class SeaTruckScannerPowerSystem : MonoBehaviour, IProtoTreeEventListener
    {        
        public struct SlotDefinition
        {
            public string id;
            public GameObject battery;
            public GameObject batteryIon;
            public Image bar;
            public TextMeshProUGUI text;
        }
               
        private ChildObjectIdentifier equipmentRoot = null;

        public ChildObjectIdentifier EquipmentRoot
        {
            get
            {
                if (equipmentRoot == null)
                {
                    equipmentRoot = transform.parent.parent.Find("EquipmentRoot").GetComponent<ChildObjectIdentifier>();                    
                }

                return equipmentRoot;
            }
        }

        public List<SlotDefinition> slotDefinitions;
        
        public GameObject ui;
        public PowerRelay powerRelay;
        
        public Color colorEmpty = new Color(1f, 0f, 0f, 1f);
        public Color colorHalf = new Color(1f, 1f, 0f, 1f);
        public Color colorFull = new Color(0f, 1f, 0f, 1f);

        private bool isCabinPowered = false;
        private bool isMainPowered = false;
                
        public Dictionary<string, string> serializedSlots;
                
        protected Equipment equipment;
        protected Dictionary<string, SlotDefinition> slots;
        protected Dictionary<string, IBattery> batteries;

        public delegate void OnPowerLevelChanged(bool isCabinPowered, bool isMainPowered);
        public event OnPowerLevelChanged onPowerLevelChanged;

        private static readonly HashSet<TechType> compatibleTech = new HashSet<TechType>
        {
            TechType.Battery,            
            TechType.PrecursorIonBattery
        };       

        private string labelInteract = string.Empty;
        private string labelStorage = string.Empty;
        private IList<UniqueIdentifier> childUIDs = new List<UniqueIdentifier>();
        

        private void PreInit()
        {
            if (slotDefinitions == null)
            {
                slotDefinitions = new List<SlotDefinition>();                

                Transform slots = transform.Find("model/battery_charging_station_base_jnt/battery_charging_station_door_jnt");
                Transform powered = transform.Find("UI/Powered");

                for (int i = 1; i < 5; i++)
                {
                    slotDefinitions.Add(new SlotDefinition()
                    {
                        id = $"ScannerModuleBattery{i}",
                        battery = slots.Find($"battery_charging_station_slot{i}_jnt/battery{i}").gameObject,
                        batteryIon = slots.Find($"battery_charging_station_slot{i}_jnt/precursorionbattery{i}").gameObject,
                        bar = powered.Find($"Battery{i}/Bar").gameObject.GetComponent<Image>(),
                        text = powered.Find($"Battery{i}/Text").gameObject.GetComponent<TextMeshProUGUI>()
                    });
                }                
            }

            labelStorage = Language.main.Get("SeatruckScanner_PowerSystemLabel");
            labelInteract = Language.main.Get("SeatruckScanner_PowerSystemInteract");
        }
        
        private void Awake()
        {
            PreInit();            

            if (powerRelay == null)
            {
                powerRelay = transform.GetComponentInParent<PowerRelay>();
            }

            if (ui == null)
            {
                ui = transform.Find("UI").gameObject;
            }

            GenericHandTarget genericHandTarget = transform.Find("Trigger").GetComponent<GenericHandTarget>();

            genericHandTarget.onHandHover.RemoveAllListeners();
            genericHandTarget.onHandHover.AddListener((data) => OnHandHover(data));
            genericHandTarget.onHandClick.RemoveAllListeners();
            genericHandTarget.onHandClick.AddListener((data) => OnHandClick(data));            

            Initialize();
        }

        public void OnHandHover(HandTargetEventData data)
        {
            HandReticle main = HandReticle.main;

            main.SetIcon(HandReticle.IconType.Hand, 1f);
            main.SetText(HandReticle.TextType.Hand, labelInteract, true, GameInput.Button.LeftHand);                      
        }

        public void OnHandClick(HandTargetEventData data)
        {
            PDA pda = Player.main.GetPDA();

            if (!pda.isInUse)
            {
                Inventory.main.SetUsedStorage(equipment, false);
                pda.Open(PDATab.Inventory, transform, null);
            }
        }

        private void Initialize()
        {
            if (equipment == null)
            {
                equipment = new Equipment(gameObject, EquipmentRoot.transform);
                equipment.SetLabel(labelStorage);
                equipment.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
                equipment.onEquip += OnEquip;
                equipment.onUnequip += OnUnequip;
                batteries = new Dictionary<string, IBattery>();
                slots = new Dictionary<string, SlotDefinition>();
                int i = 0;
                int count = slotDefinitions.Count;

                while (i < count)
                {
                    SlotDefinition slotDefinition = slotDefinitions[i];
                    string id = slotDefinition.id;
                    if (!string.IsNullOrEmpty(id) && !batteries.ContainsKey(id))
                    {
                        batteries[id] = null;
                        slots[id] = slotDefinition;

                        Image bar = slotDefinition.bar;

                        if (bar != null)
                        {
                            bar.material = new Material(bar.material);
                        }
                    }

                    i++;
                }

                UnlockDefaultEquipmentSlots();                                           

                UpdateVisuals();                
            }            
        }

        public void Start()
        {            
            if (serializedSlots != null)
            {
                Dictionary<string, InventoryItem> items = StorageHelper.ScanItems(EquipmentRoot.transform);
                equipment.RestoreEquipment(serializedSlots, items);
                serializedSlots = null;
                UnlockDefaultEquipmentSlots();
            }

            powerRelay.powerDownEvent.AddHandler(this, new Event<PowerRelay>.HandleFunction(OnPowerChanged));
            powerRelay.powerUpEvent.AddHandler(this, new Event<PowerRelay>.HandleFunction(OnPowerChanged));            
        }

        public void Update()
        {
            if (GameModeUtils.RequiresPower())
            {
                bool flag = CheckMainPowerLevel();

                if (isMainPowered != flag)
                {
                    isMainPowered = flag;

                    onPowerLevelChanged?.Invoke(isCabinPowered, isMainPowered);
                }               
            }
            else if (!isMainPowered)            
            {
                isMainPowered = true;
                isCabinPowered = powerRelay.IsPowered();
                onPowerLevelChanged?.Invoke(isCabinPowered, isMainPowered);
            }
        }

        private void OnPowerChanged(PowerRelay powerRelay)
        {
            if (powerRelay.IsPowered())
            {
                isCabinPowered = true;
                ui.SetActive(true);
            }
            else
            {
                isCabinPowered = false;
                ui.SetActive(false);
            }

            onPowerLevelChanged?.Invoke(isCabinPowered, isMainPowered);
        }
        
        private bool CheckMainPowerLevel()
        {            
            foreach (KeyValuePair<string, IBattery> kvp in batteries)
            {
                if (kvp.Value != null && kvp.Value.charge > 0f)
                {
                    return true;
                }
            }

            return false;            
        }

        public float TotalCanProvide(out int sourceCount)
        {
            float totalPower = 0f;
            sourceCount = 0;

            foreach (KeyValuePair<string, IBattery> keyValuePair in batteries)
            {
                if (keyValuePair.Value != null && keyValuePair.Value.charge > 0f)
                {
                    totalPower += keyValuePair.Value.charge;
                    sourceCount++;
                }
            }            
            
            return totalPower;
        }

        public void ConsumePower(float amount)
        {
            if (GameModeUtils.RequiresPower())
            {
                float totalPower = TotalCanProvide(out int sourceCount);

                if (sourceCount > 0)
                {
                    amount = (amount > totalPower) ? totalPower : amount;
                    amount = amount / sourceCount;

                    BZLogger.Debug($"consume power: amount: {amount}, sources: {sourceCount}");

                    foreach (KeyValuePair<string, IBattery> kvp in batteries)
                    {
                        if (kvp.Value != null && kvp.Value.charge > 0f)
                        {
                            kvp.Value.charge += -Mathf.Min(amount, kvp.Value.charge);                            

                            if (slots.TryGetValue(kvp.Key, out SlotDefinition definition))
                            {
                                UpdateVisuals(definition, kvp.Value.charge / kvp.Value.capacity, equipment.GetItemInSlot(kvp.Key).item.GetTechType());
                            }
                        }                        
                    }                    
                }                
            }            
        } 
        
        private void UnlockDefaultEquipmentSlots()
        {
            equipment.AddSlots(slots.Keys);
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            TechType techType = pickupable.GetTechType();

            if (compatibleTech.Contains(techType))
            {
                return true;
            }

            EquipmentType equipmentType = TechData.GetEquipmentType(techType);

            if (equipmentType == EquipmentType.BatteryCharger || equipmentType == EquipmentType.PowerCellCharger)
            {
                return true;
            }

            if (verbose)
            {
                ErrorMessage.AddMessage(Language.main.Get("BatteryChargerIncompatibleItem"));
            }

            return false;
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            if (item != null)
            {
                if (item.item != null)
                {
                    IBattery newBattery = item.item.GetComponent<IBattery>();

                    if (newBattery != null && batteries.ContainsKey(slot))
                    {
                        batteries[slot] = newBattery;                        
                    }
                }
            }

            if (slots.TryGetValue(slot, out SlotDefinition definition) && item != null)
            {
                if (item.item != null)
                {
                    IBattery battery = item.item.GetComponent<IBattery>();

                    if (battery != null)
                    {
                        UpdateVisuals(definition, battery.charge / battery.capacity, item.item.GetTechType());
                    }
                }
            }
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            if (batteries.ContainsKey(slot))
            {
                batteries[slot] = null;
            }

            if (slots.TryGetValue(slot, out SlotDefinition definition))
            {
                UpdateVisuals(definition, -1f, TechType.None);
            }
        }

        private void UpdateVisuals()
        {
            foreach (KeyValuePair<string, SlotDefinition> kvp in slots)
            {
                InventoryItem itemInSlot = equipment.GetItemInSlot(kvp.Key);
                float percent = -1f;
                TechType batteryTechType = TechType.None;

                if (itemInSlot != null)
                {
                    Pickupable item = itemInSlot.item;

                    if (item != null)
                    {
                        IBattery battery = item.GetComponent<IBattery>();

                        if (battery != null)
                        {
                            percent = battery.charge / battery.capacity;                            
                        }
                    }
                }

                UpdateVisuals(kvp.Value, percent, batteryTechType);
            }            
        }

        private void UpdateVisuals(SlotDefinition definition, float percent, TechType batteryTechType)
        {
            definition.battery.SetActive(false);
            definition.batteryIon.SetActive(false);

            (batteryTechType == TechType.PrecursorIonBattery ? definition.batteryIon : definition.battery).SetActive(percent >= 0f);
            
            TextMeshProUGUI text = definition.text;

            if (text != null)
            {
                text.text = ((percent >= 0f) ? string.Format("{0:P0}", percent) : Language.main.Get("ChargerSlotEmpty"));
            }
            
            Image bar = definition.bar;

            if (bar != null)
            {
                Material material = bar.material;

                if (percent >= 0f)
                {
                    Color value = (percent < 0.5f) ? Color.Lerp(colorEmpty, colorHalf, 2f * percent) : Color.Lerp(colorHalf, colorFull, 2f * percent - 1f);
                    material.SetColor(ShaderPropertyID._Color, value);
                    material.SetFloat(ShaderPropertyID._Amount, percent);
                    return;
                }

                material.SetColor(ShaderPropertyID._Color, colorEmpty);
                material.SetFloat(ShaderPropertyID._Amount, 0f);
            }
        }
           
        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            PreInit();
            Initialize();
            serializedSlots = equipment.SaveEquipment();

            string FILENAME = $"{Main.modFolder}/SaveGame/{EquipmentRoot.Id}.sav";

            List<SaveData> saveDatas = new List<SaveData>();

            foreach (KeyValuePair<string, string> kvp in serializedSlots)
            {
                saveDatas.Add(new SaveData("BATTERIES", kvp.Key, kvp.Value));
            }            
                        
            ParserHelper.CreateSaveGameFile(FILENAME, "SeatruckScannerModule", Main.VERSION, saveDatas);            
        }
        
        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            string FILENAME = $"{Main.modFolder}/SaveGame/{EquipmentRoot.Id}.sav";

            if (File.Exists(FILENAME))
            {
                if (serializedSlots == null)
                {
                    serializedSlots = new Dictionary<string, string>();
                }
                else
                {
                    serializedSlots.Clear();
                }

                serializedSlots = ParserHelper.GetAllKVPFromSection(FILENAME, "BATTERIES");
            }                       

            PreInit();
            Initialize();

            if (serializedSlots != null)
            {                
                StorageHelper.TransferEquipment(equipmentRoot.gameObject, serializedSlots, equipment);
                serializedSlots = null;
            }
            else
            {
                ClearEquipment();
            }                       
        }
          
        private void ClearEquipment()
        {
            foreach(Battery battery in EquipmentRoot.GetComponentsInChildren<Battery>(true))
            {
                BZLogger.Warn($"Removing unassigned Battery: {battery.GetComponent<PrefabIdentifier>().Id}");
                DestroyImmediate(battery.gameObject);
            }
        }        
    }
}
