using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using SlotExtenderZero.Configuration;
using BZHelper;

namespace SlotExtenderZero
{
    internal static class SlotHelper
    {
        public static string[] SessionSeatruckSlotIDs;        
        public static string[] SessionExosuitSlotIDs { get; private set; }
        public static string[] NewChipSlotIDs { get; private set; }
        public static string[] NewHoverbikeSlotIDs { get; private set; }
        //public static string[] ScannerModuleBatterySlotIDs { get; private set; }

        public static SlotPosLayout SessionSlotPosLayout = SEzConfig.SLOT_LAYOUT == SlotLayout.Grid ? new SlotPosGrid() : (SlotPosLayout)new SlotPosCircle();

        public static Vector2[] SlotPos => SessionSlotPosLayout.SlotPos;
        public static Vector2[] ArmSlotPos => SessionSlotPosLayout.ArmSlotPos;
        public static Vector2 VehicleImgPos => SessionSlotPosLayout.VehicleImgPos;

        public static Vector2[] ChipSlotPos => new Vector2[2]
        {
            new Vector2(-136.5f, 184), // chip slot 3 - left
            new Vector2(136.5f, 184)  // chip slot 4 - right
        };

        public static SlotPosLayout HoverbikeSlotPosLayout = new SlotPosHoverbike();        
          
        public static Dictionary<string, SlotData> ALLSLOTS = new Dictionary<string, SlotData>();

        public static readonly Dictionary<SlotName, string> slotStringCache = new Dictionary<SlotName, string>()
        {
            { SlotName.Chip1,  "Chip1"  },
            { SlotName.Chip2,  "Chip2"  },
            { SlotName.Chip3,  "Chip3"  },
            { SlotName.Chip4,  "Chip4"  },

            { SlotName.HoverbikeModule1,  "HoverbikeModule1"  },
            { SlotName.HoverbikeModule2,  "HoverbikeModule2"  },
            { SlotName.HoverbikeModule3,  "HoverbikeModule3"  },
            { SlotName.HoverbikeModule4,  "HoverbikeModule4"  },

            { SlotName.ExosuitModule1,  "ExosuitModule1"  },
            { SlotName.ExosuitModule2,  "ExosuitModule2"  },
            { SlotName.ExosuitModule3,  "ExosuitModule3"  },
            { SlotName.ExosuitModule4,  "ExosuitModule4"  },
            { SlotName.ExosuitModule5,  "ExosuitModule5"  },
            { SlotName.ExosuitModule6,  "ExosuitModule6"  },
            { SlotName.ExosuitModule7,  "ExosuitModule7"  },
            { SlotName.ExosuitModule8,  "ExosuitModule8"  },
            { SlotName.ExosuitModule9,  "ExosuitModule9"  },
            { SlotName.ExosuitModule10, "ExosuitModule10" },
            { SlotName.ExosuitModule11, "ExosuitModule11" },
            { SlotName.ExosuitModule12, "ExosuitModule12" },
            { SlotName.ExosuitArmLeft,  "ExosuitArmLeft"  },
            { SlotName.ExosuitArmRight, "ExosuitArmRight" },

            { SlotName.SeaTruckModule1,  "SeaTruckModule1"  },
            { SlotName.SeaTruckModule2,  "SeaTruckModule2"  },
            { SlotName.SeaTruckModule3,  "SeaTruckModule3"  },
            { SlotName.SeaTruckModule4,  "SeaTruckModule4"  },
            { SlotName.SeaTruckModule5,  "SeaTruckModule5"  },
            { SlotName.SeaTruckModule6,  "SeaTruckModule6"  },
            { SlotName.SeaTruckModule7,  "SeaTruckModule7"  },
            { SlotName.SeaTruckModule8,  "SeaTruckModule8"  },
            { SlotName.SeaTruckModule9,  "SeaTruckModule9"  },
            { SlotName.SeaTruckModule10, "SeaTruckModule10" },
            { SlotName.SeaTruckModule11, "SeaTruckModule11" },
            { SlotName.SeaTruckModule12, "SeaTruckModule12" },
            { SlotName.SeaTruckArmLeft,  "SeaTruckArmLeft"  },
            { SlotName.SeaTruckArmRight, "SeaTruckArmRight" },
            { SlotName.ScannerModuleBattery1, "ScannerModuleBattery1" },
            { SlotName.ScannerModuleBattery2, "ScannerModuleBattery2" },
            { SlotName.ScannerModuleBattery3, "ScannerModuleBattery3" },
            { SlotName.ScannerModuleBattery4, "ScannerModuleBattery4" }
        };

        public static List<SlotData> NewChipSlots = new List<SlotData>()
        {
            new SlotData(slotStringCache[SlotName.Chip3], SlotConfigID.Chip, ChipSlotPos[0], SlotType.CloneChip),
            new SlotData(slotStringCache[SlotName.Chip4], SlotConfigID.Chip, ChipSlotPos[1], SlotType.CloneChip),
        };

        public static List<SlotData> ScannerModuleBatterySlots = new List<SlotData>()
        {
            new SlotData(slotStringCache[SlotName.ScannerModuleBattery1], SlotConfigID.Battery, Vector2.zero, SlotType.CloneBattery),
            new SlotData(slotStringCache[SlotName.ScannerModuleBattery2], SlotConfigID.Battery, Vector2.zero, SlotType.CloneBattery),
            new SlotData(slotStringCache[SlotName.ScannerModuleBattery3], SlotConfigID.Battery, Vector2.zero, SlotType.CloneBattery),
            new SlotData(slotStringCache[SlotName.ScannerModuleBattery4], SlotConfigID.Battery, Vector2.zero, SlotType.CloneBattery)
        };

        public static List<SlotData> NewHoverbikeSlots = new List<SlotData>()
        {
            new SlotData(slotStringCache[SlotName.HoverbikeModule1], SlotConfigID.Slot_1, HoverbikeSlotPosLayout.SlotPos[0], SlotType.OriginalNormal),
            new SlotData(slotStringCache[SlotName.HoverbikeModule2], SlotConfigID.Slot_2, HoverbikeSlotPosLayout.SlotPos[1], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.HoverbikeModule3], SlotConfigID.Slot_3, HoverbikeSlotPosLayout.SlotPos[2], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.HoverbikeModule4], SlotConfigID.Slot_4, HoverbikeSlotPosLayout.SlotPos[3], SlotType.CloneNormal),
        };

        public static List<SlotData> SessionExosuitSlots = new List<SlotData>()
        {
            new SlotData(slotStringCache[SlotName.ExosuitArmLeft], SlotConfigID.ExosuitArmLeft, ArmSlotPos[0], SlotType.OriginalArmLeft),
            new SlotData(slotStringCache[SlotName.ExosuitArmRight], SlotConfigID.ExosuitArmRight, ArmSlotPos[1], SlotType.OriginalArmRight),
            new SlotData(slotStringCache[SlotName.ExosuitModule1], SlotConfigID.Slot_1, SlotPos[0], SlotType.OriginalNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule2], SlotConfigID.Slot_2, SlotPos[1], SlotType.OriginalNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule3], SlotConfigID.Slot_3, SlotPos[2], SlotType.OriginalNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule4], SlotConfigID.Slot_4, SlotPos[3], SlotType.OriginalNormal)
        };

        public static readonly List<SlotData> NewExosuitSlots = new List<SlotData>()
        {
            new SlotData(slotStringCache[SlotName.ExosuitModule5], SlotConfigID.Slot_5, SlotPos[4], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule6], SlotConfigID.Slot_6, SlotPos[5], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule7], SlotConfigID.Slot_7, SlotPos[6], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule8], SlotConfigID.Slot_8, SlotPos[7], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule9], SlotConfigID.Slot_9, SlotPos[8], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule10], SlotConfigID.Slot_10, SlotPos[9], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule11], SlotConfigID.Slot_11, SlotPos[10], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.ExosuitModule12], SlotConfigID.Slot_12, SlotPos[11], SlotType.CloneNormal)
        };

        public static List<SlotData> SessionSeatruckSlots = new List<SlotData>()
        {
            new SlotData(slotStringCache[SlotName.SeaTruckModule1], SlotConfigID.Slot_1, SlotPos[0], SlotType.OriginalNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule2], SlotConfigID.Slot_2, SlotPos[1], SlotType.OriginalNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule3], SlotConfigID.Slot_3, SlotPos[2], SlotType.OriginalNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule4], SlotConfigID.Slot_4, SlotPos[3], SlotType.OriginalNormal),
        };

        public static readonly List<SlotData> NewSeatruckSlots = new List<SlotData>()
        {
            new SlotData(slotStringCache[SlotName.SeaTruckModule5], SlotConfigID.Slot_5, SlotPos[4], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule6], SlotConfigID.Slot_6, SlotPos[5], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule7], SlotConfigID.Slot_7, SlotPos[6], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule8], SlotConfigID.Slot_8, SlotPos[7], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule9], SlotConfigID.Slot_9, SlotPos[8], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule10], SlotConfigID.Slot_10, SlotPos[9], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule11], SlotConfigID.Slot_11, SlotPos[10], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckModule12], SlotConfigID.Slot_12, SlotPos[11], SlotType.CloneNormal),
            new SlotData(slotStringCache[SlotName.SeaTruckArmLeft], SlotConfigID.SeaTruckArmLeft, ArmSlotPos[0], SlotType.CloneArmLeft),
            new SlotData(slotStringCache[SlotName.SeaTruckArmRight], SlotConfigID.SeaTruckArmRight, ArmSlotPos[1], SlotType.CloneArmRight)
        };

        public static void InitSlotIDs()
        {
            BZLogger.Trace("SlotHelper.InitSlotIDs()");            

            for (int i = 0; i < SEzConfig.EXTRASLOTS; i++)
            {
                if (!SessionSeatruckSlots.Contains(NewSeatruckSlots[i]))
                {
                    SessionSeatruckSlots.Add(NewSeatruckSlots[i]);
                }

                if (!SessionExosuitSlots.Contains(NewExosuitSlots[i]))
                {
                    SessionExosuitSlots.Add(NewExosuitSlots[i]);
                }
            }

            if (SEzConfig.isSeatruckArmsExists)
            {
                foreach (SlotData slotData in NewSeatruckSlots)
                {
                    SlotType slotType = slotData.SlotType;

                    if (slotType == SlotType.CloneArmLeft || slotType == SlotType.CloneArmRight)
                    {
                        if (!SessionSeatruckSlots.Contains(slotData))
                        {
                            SessionSeatruckSlots.Add(slotData);
                        }
                    }
                }
            }

            NewChipSlotIDs = new string[NewChipSlots.Count];

            for (int i = 0; i < NewChipSlotIDs.Length; i++)
            {
                NewChipSlotIDs[i] = NewChipSlots[i].SlotID;
            }

            NewHoverbikeSlotIDs = new string[NewHoverbikeSlots.Count];

            for (int i = 0; i < NewHoverbikeSlotIDs.Length; i++)
            {
                NewHoverbikeSlotIDs[i] = NewHoverbikeSlots[i].SlotID;
            }

            SessionExosuitSlotIDs = new string[SessionExosuitSlots.Count];

            for (int i = 0; i < SessionExosuitSlotIDs.Length; i++)
            {
                SessionExosuitSlotIDs[i] = SessionExosuitSlots[i].SlotID;
            }

            SessionSeatruckSlotIDs = new string[SessionSeatruckSlots.Count];

            for (int i = 0; i < SessionSeatruckSlotIDs.Length; i++)
            {
                SessionSeatruckSlotIDs[i] = SessionSeatruckSlots[i].SlotID;
            }            

            BZLogger.Log("Session slotID's created.");
        }

        public static void ExpandSlotMapping()
        {
            BZLogger.Trace("SlotHelper.ExpandSlotMapping()");

            foreach (SlotData slotData in NewChipSlots)
            {
                if (!Equipment.slotMapping.ContainsKey(slotData.SlotID))
                {
                    Equipment.slotMapping.Add(slotData.SlotID, EquipmentType.Chip);
                }
            }

            foreach (SlotData slotData in ScannerModuleBatterySlots)
            {
                if (!Equipment.slotMapping.ContainsKey(slotData.SlotID))
                {
                    Equipment.slotMapping.Add(slotData.SlotID, EquipmentType.BatteryCharger);
                }
            }

            foreach (SlotData slotData in NewHoverbikeSlots)
            {
                if (slotData.SlotType == SlotType.CloneNormal && !Equipment.slotMapping.ContainsKey(slotData.SlotID))
                {
                    Equipment.slotMapping.Add(slotData.SlotID, EquipmentType.HoverbikeModule);
                }
            }

            foreach (SlotData slotData in SessionExosuitSlots)
            {
                if (slotData.SlotType == SlotType.CloneNormal && !Equipment.slotMapping.ContainsKey(slotData.SlotID))
                {
                    Equipment.slotMapping.Add(slotData.SlotID, EquipmentType.ExosuitModule);
                }
            }

            foreach (SlotData slotData in SessionSeatruckSlots)
            {
                switch (slotData.SlotType)
                {                    
                    case SlotType.CloneNormal:
                        if (!Equipment.slotMapping.ContainsKey(slotData.SlotID))
                        {
                            Equipment.slotMapping.Add(slotData.SlotID, EquipmentType.SeaTruckModule);
                        }
                        break;

                    case SlotType.CloneArmLeft:
                    case SlotType.CloneArmRight:
                        if (!Equipment.slotMapping.ContainsKey(slotData.SlotID))
                        {
                            Equipment.slotMapping.Add(slotData.SlotID, (EquipmentType)ModdedEquipmentType.SeatruckArm);
                        }

                        break;
                }
            }

            BZLogger.Log("Equipment slot mapping Patched!");
        }

        public static bool IsExtendedSlot(string slotName)
        {
            BZLogger.Trace($"SlotHelper.IsExtendedSlot({slotName})");
            
            if (ALLSLOTS.TryGetValue(slotName, out SlotData slotData))
            {
                switch(slotData.SlotType)
                {
                    case SlotType.CloneBattery:
                    case SlotType.CloneChip:
                    case SlotType.CloneArmLeft:
                    case SlotType.CloneArmRight:
                    case SlotType.CloneNormal:
                        return true;
                }
            }           

            return false;
        }

        public static bool IsSeatruckArmSlot(string slotName)
        {
            BZLogger.Trace($"SlotHelper.IsSeatruckArmSlot({slotName})");

            return slotName.Equals(slotStringCache[SlotName.SeaTruckArmLeft]) || slotName.Equals(slotStringCache[SlotName.SeaTruckArmRight]) ? true : false;
        }

        public static void InitSessionAllSlots()
        {
            BZLogger.Trace($"SlotHelper.InitSessionAllSlots()");

            ALLSLOTS.Clear();

            foreach (SlotData slotData in SessionSeatruckSlots)
            {
                if (ALLSLOTS.ContainsKey(slotData.SlotID))
                {
                    continue;
                }

                SEzConfig.SLOTKEYBINDINGS.TryGetValue(slotData.SlotConfigID, out string result);
                slotData.KeyCodeName = result;
                ALLSLOTS.Add(slotData.SlotID, slotData);
            }

            foreach (SlotData slotData in SessionExosuitSlots)
            {
                if (ALLSLOTS.ContainsKey(slotData.SlotID))
                {
                    continue;
                }

                SEzConfig.SLOTKEYBINDINGS.TryGetValue(slotData.SlotConfigID, out string result);
                slotData.KeyCodeName = result;
                ALLSLOTS.Add(slotData.SlotID, slotData);
            }

            TraceAllSlots();
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

        [Conditional("TRACE")]
        private static void TraceAllSlots()
        {
            BZLogger.Trace("Listing Dictionary: ALLSLOTS...");

            foreach (KeyValuePair<string, SlotData> kvp in ALLSLOTS)
            {
                BZLogger.Trace(
                    $"SlotID: {kvp.Value.SlotID}" +
                    $", InternalSlotID: {kvp.Value.SlotConfigIDName}" +
                    $", SlotPOS: {kvp.Value.SlotPos}" +
                    $", SlotType: {kvp.Value.SlotType}" +
                    $", KeyCodeName: {kvp.Value.KeyCodeName}");
            }
        }
    }
}
