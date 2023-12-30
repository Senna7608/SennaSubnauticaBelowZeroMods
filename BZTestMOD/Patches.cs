using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZTestMOD
{
    /*
        [HarmonyPatch(typeof(Drillable))]
        [HarmonyPatch("Start")]
        internal class SeamothDrillable_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(Drillable __instance)
            {
                Component seamothDrillable = __instance.GetComponent("SeamothDrillable");

                if (seamothDrillable != null) // if SeamothDrillable component is available on resource, processing the drill damage value, else do nothing.
                {
                    if (Plugin.ConfigAffectSeamothArms.Value == true && Plugin.ConfigVariableModeEnabled.Value == false)
                    {
                        // Calls the SetDrillDamage method in SeamothDrillable in a safer way with a float parameter.                        
                        seamothDrillable.SendMessage("SetDrillDamage", Plugin.ConfigAdditionalDamage, SendMessageOptions.RequireReceiver);                        
                    }
                    else if (Plugin.ConfigAffectSeamothArms.Value == true && Plugin.ConfigVariableModeEnabled.Value == true)
                    {
                        Dictionary<TechType, int> ConfigDictionary = new()
                        {
                            { TechType.DrillableSalt, Plugin.ConfigDrillableSaltDamage.Value },
                            { TechType.DrillableQuartz, Plugin.ConfigDrillableQuartzDamage.Value },
                            { TechType.DrillableCopper, Plugin.ConfigDrillableCopperDamage.Value },
                            { TechType.DrillableTitanium, Plugin.ConfigDrillableTitaniumDamage.Value },
                            { TechType.DrillableLead, Plugin.ConfigDrillableLeadDamage.Value },
                            { TechType.DrillableSilver, Plugin.ConfigDrillableSilverDamage.Value },
                            { TechType.DrillableDiamond, Plugin.ConfigDrillableDiamondDamage.Value },
                            { TechType.DrillableGold, Plugin.ConfigDrillableGoldDamage.Value },
                            { TechType.DrillableMagnetite, Plugin.ConfigDrillableMagnetiteDamage.Value },
                            { TechType.DrillableLithium, Plugin.ConfigDrillableLithiumDamage.Value },
                            { TechType.DrillableMercury, Plugin.ConfigDrillableMercuryDamage.Value },
                            { TechType.DrillableUranium, Plugin.ConfigDrillableUraniumDamage.Value },
                            { TechType.DrillableAluminiumOxide, Plugin.ConfigDrillableAluminiumOxideDamage.Value },
                            { TechType.DrillableNickel, Plugin.ConfigDrillableNickelDamage.Value },
                            { TechType.DrillableSulphur, Plugin.ConfigDrillableSulphurDamage.Value },
                            { TechType.DrillableKyanite, Plugin.ConfigDrillableKyaniteDamage.Value }
                        };
                        TechType key = __instance.GetDominantResourceType();
                        Plugin.Log("The techType is = " + key, 2);
                        var valueGet = ConfigDictionary.TryGetValue(key, out int value);
                        Plugin.Log("Value is = " + value, 2);
                        // Calls the SetDrillDamage method in SeamothDrillable in a safer way with a float parameter.
                        seamothDrillable.SendMessage("SetDrillDamage", value, SendMessageOptions.RequireReceiver);                        
                    }
                }
            }
        }

        */






    /*
    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("OnDrill")]
    internal class Drillable_Transpiler_Patch
    {
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins)
        {
            var list = cins.ToList();
            int index = list.FindIndex(ci => ci.opcode == OpCodes.Ldc_R4 && Equals(ci.operand, 5f));
            list[index] = new CodeInstruction(OpCodes.Ldc_R4, 100f);

            return list;
        }
    }


    [HarmonyPatch(typeof(Drillable), nameof(Drillable.OnDrill))]
    internal class Drillable_Transpiler_Patch
    {
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins)
        {
            var matcher = new CodeMatcher(cins);

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 5f));
            if (matcher.IsValid)
            {
                matcher.Set(OpCodes.Ldc_R4, 100f);
                return matcher.InstructionEnumeration();
            }

            Console.WriteLine($"Failed to find matching instructions for {nameof(Drillable_Transpiler_Patch)}. Either another mod has already transpiled this or the games code has changed.");
            return cins;
        }
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions)
            .MatchForward(true,
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Stloc_1));

        if (!matcher.IsValid)
        {
            Main.logSource.LogError("Cannot find patch location in BaseGhost.Finish");
            return instructions;
        }

        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, typeof(BaseGhost_Finish_Patch).GetMethod(nameof(CacheObject)))).InstructionEnumeration();
        return matcher.InstructionEnumeration();
    }

    */
}
