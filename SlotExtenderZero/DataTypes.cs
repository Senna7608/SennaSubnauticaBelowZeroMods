using UnityEngine;
using BZHelper;

namespace SlotExtenderZero
{
    public enum ModdedEquipmentType
    {
        SeatruckArm = 200
    };

    public enum SlotLayout
    {
        Grid,
        Circle
    };

    public enum SlotType
    {
        OriginalNormal,
        OriginalArmLeft,
        OriginalArmRight,
        CloneNormal,
        CloneArmLeft,
        CloneArmRight,
        Chip,
        CloneChip,
        Battery,
        CloneBattery
    };

    public enum SlotConfigID
    {        
        ExosuitArmLeft = -1,
        ExosuitArmRight,
        Slot_1,
        Slot_2,
        Slot_3,
        Slot_4,
        Slot_5,
        Slot_6,
        Slot_7,
        Slot_8,
        Slot_9,
        Slot_10,
        Slot_11,
        Slot_12,
        SeaTruckArmLeft,
        SeaTruckArmRight,
        Chip,
        Battery
    };

    public enum SlotName
    {
        Chip1,
        Chip2,
        Chip3,
        Chip4,

        HoverbikeModule1,
        HoverbikeModule2,
        HoverbikeModule3,
        HoverbikeModule4,

        ExosuitModule1,
        ExosuitModule2,
        ExosuitModule3,
        ExosuitModule4,
        ExosuitModule5,
        ExosuitModule6,
        ExosuitModule7,
        ExosuitModule8,
        ExosuitModule9,
        ExosuitModule10,
        ExosuitModule11,
        ExosuitModule12,
        ExosuitArmLeft,
        ExosuitArmRight,

        SeaTruckModule1,
        SeaTruckModule2,
        SeaTruckModule3,
        SeaTruckModule4,
        SeaTruckModule5,
        SeaTruckModule6,
        SeaTruckModule7,
        SeaTruckModule8,
        SeaTruckModule9,
        SeaTruckModule10,
        SeaTruckModule11,
        SeaTruckModule12,
        SeaTruckArmLeft,
        SeaTruckArmRight,

        ScannerModuleBattery1,
        ScannerModuleBattery2,
        ScannerModuleBattery3,
        ScannerModuleBattery4
    };

    public class SlotData
    {
        public string SlotID;
        public SlotConfigID SlotConfigID;
        public Vector2 SlotPos;
        public SlotType SlotType;

        public string KeyCodeName { get; set; }
        public string SlotConfigIDName => SlotConfigID.ToString();
        public KeyCode KeyCode => InputHelper.GetInputNameAsKeyCode(KeyCodeName);

        public SlotData(string slotID, SlotConfigID internalSlotID, Vector2 slotPOS, SlotType slotType)
        {
            SlotID = slotID;
            SlotConfigID = internalSlotID;
            SlotPos = slotPOS;
            SlotType = slotType;
        }
    }

    public abstract class SlotPosLayout
    {
        public abstract Vector2 VehicleImgPos { get; }
        public abstract Vector2[] SlotPos { get; }
        public abstract Vector2[] ArmSlotPos { get; }        

        public const float Unit = 200f;
        public const float RowStep = Unit * 2.2f / 3;
    }
}
