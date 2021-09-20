/*
using HarmonyLib;

namespace Fahrenheit
{
    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("OnEquip")]
    public class Inventory_OnEquip_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Inventory __instance, string slot, InventoryItem item)
        {
            if (item.item.GetTechType() == FahrenheitChip_Prefab.TechTypeID)
            {                
                Main.FahrenheitEnabled = true;
                Main.NightVisionEnabled = true;

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("OnUnequip")]
    public class Inventory_OnUnequip_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Inventory __instance, string slot, InventoryItem item)
        {
            if (item.item.GetTechType() == FahrenheitChip_Prefab.TechTypeID)
            {
                if (__instance.equipment.GetCount(FahrenheitChip_Prefab.TechTypeID) < 1)
                {
                    Main.FahrenheitEnabled = false;
                    Main.NightVisionEnabled = false;

                    return false;
                }

                return false;
            }

            return true;
        }
    }


    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Start")]
    public class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<NightVisionManager>();
        }
    }

}
*/