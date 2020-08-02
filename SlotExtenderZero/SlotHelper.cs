using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using BZCommon;
using SlotExtenderZero.Configuration;

namespace SlotExtenderZero
{
    public static class SlotHelper
    {
        public static string[] SessionSeatruckSlotIDs { get; private set; }
        public static string[] SessionExosuitSlotIDs { get; private set; }

        public static SlotPosLayout SessionSlotPosLayout = SEzConfig.SLOT_LAYOUT == SlotLayout.Grid ? new SlotPosGrid() : (SlotPosLayout)new SlotPosCircle();

        public static Vector2[] SlotPos => SessionSlotPosLayout.SlotPos;
        public static Vector2[] ArmSlotPos => SessionSlotPosLayout.ArmSlotPos;
        public static Vector2 VehicleImgPos => SessionSlotPosLayout.VehicleImgPos;

        public static Dictionary<string, SlotData> ALLSLOTS = new Dictionary<string, SlotData>();

        public static List<SlotData> SessionSeatruckSlots = new List<SlotData>()
        {
            new SlotData("SeaTruckModule1", SlotConfigID.Slot_1, SlotPos[0], SlotType.OriginalNormal),
            new SlotData("SeaTruckModule2", SlotConfigID.Slot_2, SlotPos[1], SlotType.OriginalNormal),
            new SlotData("SeaTruckModule3", SlotConfigID.Slot_3, SlotPos[2], SlotType.OriginalNormal),
            new SlotData("SeaTruckModule4", SlotConfigID.Slot_4, SlotPos[3], SlotType.OriginalNormal),
        };

        public static readonly List<SlotData> NewSeatruckSlots = new List<SlotData>()
        {
            new SlotData("SeaTruckModule5", SlotConfigID.Slot_5, SlotPos[4], SlotType.CloneNormal),
            new SlotData("SeaTruckModule6", SlotConfigID.Slot_6, SlotPos[5], SlotType.CloneNormal),
            new SlotData("SeaTruckModule7", SlotConfigID.Slot_7, SlotPos[6], SlotType.CloneNormal),
            new SlotData("SeaTruckModule8", SlotConfigID.Slot_8, SlotPos[7], SlotType.CloneNormal),
            new SlotData("SeaTruckModule9", SlotConfigID.Slot_9, SlotPos[8], SlotType.CloneNormal),
            new SlotData("SeaTruckModule10", SlotConfigID.Slot_10, SlotPos[9], SlotType.CloneNormal),
            new SlotData("SeaTruckModule11", SlotConfigID.Slot_11, SlotPos[10], SlotType.CloneNormal),
            new SlotData("SeaTruckModule12", SlotConfigID.Slot_12, SlotPos[11], SlotType.CloneNormal),
            new SlotData("SeaTruckArmLeft", SlotConfigID.SeaTruckArmLeft, ArmSlotPos[0], SlotType.CloneArmLeft),
            new SlotData("SeaTruckArmRight", SlotConfigID.SeaTruckArmRight, ArmSlotPos[1], SlotType.CloneArmRight)
        };

        public static List<SlotData> SessionExosuitSlots = new List<SlotData>()
        {
            new SlotData("ExosuitArmLeft", SlotConfigID.ExosuitArmLeft, ArmSlotPos[0], SlotType.OriginalArmLeft),
            new SlotData("ExosuitArmRight", SlotConfigID.ExosuitArmRight, ArmSlotPos[1], SlotType.OriginalArmRight),
            new SlotData("ExosuitModule1", SlotConfigID.Slot_1, SlotPos[0], SlotType.OriginalNormal),
            new SlotData("ExosuitModule2", SlotConfigID.Slot_2, SlotPos[1], SlotType.OriginalNormal),
            new SlotData("ExosuitModule3", SlotConfigID.Slot_3, SlotPos[2], SlotType.OriginalNormal),
            new SlotData("ExosuitModule4", SlotConfigID.Slot_4, SlotPos[3], SlotType.OriginalNormal)
        };

        public static readonly List<SlotData> NewExosuitSlots = new List<SlotData>()
        {
            new SlotData("ExosuitModule5", SlotConfigID.Slot_5, SlotPos[4], SlotType.CloneNormal),
            new SlotData("ExosuitModule6", SlotConfigID.Slot_6, SlotPos[5], SlotType.CloneNormal),
            new SlotData("ExosuitModule7", SlotConfigID.Slot_7, SlotPos[6], SlotType.CloneNormal),
            new SlotData("ExosuitModule8", SlotConfigID.Slot_8, SlotPos[7], SlotType.CloneNormal),
            new SlotData("ExosuitModule9", SlotConfigID.Slot_9, SlotPos[8], SlotType.CloneNormal),
            new SlotData("ExosuitModule10", SlotConfigID.Slot_10, SlotPos[9], SlotType.CloneNormal),
            new SlotData("ExosuitModule11", SlotConfigID.Slot_11, SlotPos[10], SlotType.CloneNormal),
            new SlotData("ExosuitModule12", SlotConfigID.Slot_12, SlotPos[11], SlotType.CloneNormal)
        };

        public static IEnumerable<string> SessionNewSeatruckSlotIDs
        {
            get
            {
                foreach (SlotData slotData in SessionSeatruckSlots)
                {
                    if (slotData.SlotType != SlotType.OriginalNormal)
                    {
                        yield return slotData.SlotID;
                    }
                }
            }
        }

        public static IEnumerable<string> SessionNewExosuitSlotIDs
        {
            get
            {
                foreach (SlotData slotData in SessionExosuitSlots)
                {
                    if (slotData.SlotType == SlotType.CloneNormal)
                    {
                        yield return slotData.SlotID;
                    }
                }
            }
        }

        public static void InitSlotIDs()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SlotHelper.InitSlotIDs()");

            for (int i = 0; i < SEzConfig.EXTRASLOTS; i++)
            {
                SessionSeatruckSlots.Add(NewSeatruckSlots[i]);
                SessionExosuitSlots.Add(NewExosuitSlots[i]);
            }

            if (SEzConfig.isSeatruckArmsExists)
            {
                foreach (SlotData slotData in NewSeatruckSlots)
                {
                    SlotType slotType = slotData.SlotType;

                    if (slotType == SlotType.CloneArmLeft || slotType == SlotType.CloneArmRight)
                    {
                        SessionSeatruckSlots.Add(slotData);
                    }
                }
            }

            SessionSeatruckSlotIDs = new string[SessionSeatruckSlots.Count];

            for (int i = 0; i < SessionSeatruckSlotIDs.Length; i++)
            {
                SessionSeatruckSlotIDs[i] = SessionSeatruckSlots[i].SlotID;
            }

            SessionExosuitSlotIDs = new string[SessionExosuitSlots.Count];

            for (int i = 0; i < SessionExosuitSlotIDs.Length; i++)
            {
                SessionExosuitSlotIDs[i] = SessionExosuitSlots[i].SlotID;
            }

            BZLogger.Log("SlotExtenderZero", "Session slotID's created.");
        }

        public static void ExpandSlotMapping()
        {
            BZLogger.Debug("SlotExtenderZero", "Method call: SlotHelper.ExpandSlotMapping()");

            foreach (SlotData slotData in SessionSeatruckSlots)
            {
                switch (slotData.SlotType)
                {
                    case SlotType.CloneNormal:
                        Equipment.slotMapping.Add(slotData.SlotID, EquipmentType.SeaTruckModule);
                        break;

                    case SlotType.CloneArmLeft:
                    case SlotType.CloneArmRight:
                        Equipment.slotMapping.Add(slotData.SlotID, (EquipmentType)ModdedEquipmentType.SeatruckArm);
                        break;
                }
            }

            foreach (SlotData slotData in SessionExosuitSlots)
            {
                if (slotData.SlotType == SlotType.CloneNormal)
                {
                    Equipment.slotMapping.Add(slotData.SlotID, EquipmentType.ExosuitModule);
                }
            }

            BZLogger.Log("SlotExtenderZero", "Equipment slot mapping Patched!");
        }

        public static bool IsExtendedSeatruckSlot(string slotName)
        {
            BZLogger.Debug("SlotExtenderZero", $"Method call: SlotHelper.IsExtendedSeatruckSlot({slotName})");

            foreach (string slot in SessionNewSeatruckSlotIDs)
            {
                if (slotName.Equals(slot))
                    return true;
            }

            return false;
        }

        public static bool IsSeatruckArmSlot(string slotName)
        {
            BZLogger.Debug("SlotExtenderZero", $"Method call: SlotHelper.IsSeatruckArmSlot({slotName})");

            return slotName.Equals("SeaTruckArmLeft") || slotName.Equals("SeaTruckArmRight") ? true : false;
        }

        public static void InitSessionAllSlots()
        {
            BZLogger.Debug("SlotExtenderZero", $"Method call: SlotHelper.InitSessionAllSlots()");

            ALLSLOTS.Clear();

            foreach (SlotData slotData in SessionSeatruckSlots)
            {
                SEzConfig.SLOTKEYBINDINGS.TryGetValue(slotData.SlotConfigID, out string result);
                slotData.KeyCodeName = result;
                ALLSLOTS.Add(slotData.SlotID, slotData);
            }

            foreach (SlotData slotData in SessionExosuitSlots)
            {
                SEzConfig.SLOTKEYBINDINGS.TryGetValue(slotData.SlotConfigID, out string result);
                slotData.KeyCodeName = result;
                ALLSLOTS.Add(slotData.SlotID, slotData);
            }

            DebugAllSlots();
        }


        public static int GetSeatruckSlotInt(SlotConfigID slotID)
        {
            for (int i = 0; i < SessionSeatruckSlots.Count; i++)
            {
                if (SessionSeatruckSlots[i].SlotConfigID == slotID)
                {
                    return i;
                }
            }

            return -1;
        }


        public static void ALLSLOTS_Update()
        {
            foreach (KeyValuePair<string, SlotData> kvp in ALLSLOTS)
            {
                SEzConfig.SLOTKEYBINDINGS.TryGetValue(kvp.Value.SlotConfigID, out string result);
                kvp.Value.KeyCodeName = result;
            }
        }

        [Conditional("DEBUG")]
        private static void DebugAllSlots()
        {
            BZLogger.Debug("SlotExtenderZero", "Listing Dictionary: ALLSLOTS...\n");

            foreach (KeyValuePair<string, SlotData> kvp in ALLSLOTS)
            {
                BZLogger.Log(
                    $"SlotID: {kvp.Value.SlotID}\n" +
                    $"InternalSlotID: {kvp.Value.SlotConfigIDName}\n" +
                    $"SlotPOS: {kvp.Value.SlotPos}\n" +
                    $"SlotType: {kvp.Value.SlotType}\n" +
                    $"KeyCodeName: {kvp.Value.KeyCodeName}\n");
            }
        }
    }
}
