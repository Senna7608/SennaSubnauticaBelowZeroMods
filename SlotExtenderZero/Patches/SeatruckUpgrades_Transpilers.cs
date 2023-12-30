using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SlotExtenderZero.Patches
{
    internal static class SeaTruckUpgrades_Transpilers
    {
        internal static IEnumerable<CodeInstruction> SlotIDs_Transpiler(IEnumerable<CodeInstruction> cins)
        {
            foreach (CodeInstruction cin in cins)
            {
                if (cin.opcode == OpCodes.Ldsfld && cin.operand == (object)AccessTools.Field(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.slotIDs)))
                {
                    cin.operand = AccessTools.Field(typeof(SlotHelper), nameof(SlotHelper.SessionSeatruckSlotIDs));                    
                }
            }

            return cins;
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades))]
        [HarmonyPatch(MethodType.StaticConstructor)]
        [HarmonyPatch(".cctor")]
        internal static class Constructor_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins)
            {
                var matcher = new CodeMatcher(cins);               

                matcher.MatchForward(false,
                    new CodeMatch(OpCodes.Ldc_I4_4),
                    new CodeMatch(OpCodes.Newarr, typeof(string))
                    );
                if (matcher.IsValid)
                {                    
                    matcher.Set(OpCodes.Ldc_I4, 13);
                }

                matcher.MatchForward(true,
                    new CodeMatch(OpCodes.Ldstr, "SeaTruckModule4"),
                    new CodeMatch(OpCodes.Stelem_Ref),
                    new CodeMatch(OpCodes.Stsfld)
                    );
                if (matcher.IsValid)
                {                    
                    matcher.Insert(
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldc_I4_4),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule5"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldc_I4_5),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule6"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldc_I4_6),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule7"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup, null),
                        new CodeInstruction(OpCodes.Ldc_I4_7),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule8"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldc_I4_8),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule9"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup, null),
                        new CodeInstruction(OpCodes.Ldc_I4, 9),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule10"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup, null),
                        new CodeInstruction(OpCodes.Ldc_I4, 10),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule11"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup, null),
                        new CodeInstruction(OpCodes.Ldc_I4, 11),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckModule12"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldc_I4, 12),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckArmLeft"),
                        new CodeInstruction(OpCodes.Stelem_Ref),
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldc_I4, 13),
                        new CodeInstruction(OpCodes.Ldstr, "SeaTruckArmRight"),
                        new CodeInstruction(OpCodes.Stelem_Ref)
                        );                    

                    return matcher.InstructionEnumeration();
                }
                
                return cins;
            }
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.LazyInitialize))]        
        internal static class LazyInitialize_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.UnlockDefaultModuleSlots))]
        internal class UnlockDefaultModuleSlots_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.TryActivateAfterBurner))]
        internal class TryActivateAfterBurner_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.UpdateModuleSlots))]
        internal class UpdateModuleSlots_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.OnUpgradeModuleChange))]        
        internal static class OnUpgradeModuleChange_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.GetSlotBinding", new Type[] { })]
        internal class GetSlotBinding_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.GetSlotBinding", new Type[] { typeof(int) })]
        internal class GetSlotBinding_int_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.GetSlotItem", new Type[] { typeof(int) })]
        internal class GetSlotItem_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.GetSlotByItem", new Type[] { typeof(InventoryItem) })]
        internal class GetSlotByItem_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.GetSlotCharge", new Type[] { typeof(int) })]
        internal class GetSlotCharge_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.SlotKeyDown", new Type[] { typeof(int) })]
        internal class SlotKeyDown_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.SlotNext")]
        internal class SlotNext_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.SlotPrevious")]
        internal class SlotPrevious_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.SlotLeftDown")]
        internal class SlotLeftDown_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.SlotLeftHeld")]
        internal class SlotLeftHeld_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.SlotLeftUp")]
        internal class SlotLeftUp_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), "IQuickSlots.GetSlotCount")]
        internal class GetSlotCount_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }

        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.GetQuickSlotType))]
        internal class GetQuickSlotType_Transpiler
        {
            [HarmonyTranspiler]
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SlotIDs_Transpiler(cins);
        }
    }    
}