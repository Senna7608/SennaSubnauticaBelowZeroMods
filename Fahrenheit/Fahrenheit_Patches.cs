using BZCommon;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Fahrenheit
{
    [HarmonyPatch(typeof(uGUI_SeaTruckHUD))]
    [HarmonyPatch("Update")]
    public class uGUI_SeaTruckHUD_Update_Patch
    {
        private static Color textColor = new Color32(255, 220, 0, 255);
        private static GameObject TemperatureSuffix = null;
        private static TextMeshProUGUI textTemperatureSuffix = null;

        [HarmonyPostfix]
        public static void Postfix(uGUI_SeaTruckHUD __instance)
        {
            if (Player.main != null && Player.main.IsPilotingSeatruck())
            {
                if (TemperatureSuffix == null)
                {
                    TemperatureSuffix = __instance.gameObject.transform.Find("Content/Seatruck/Indicators/Temperature/TemperatureValue/TemperatureSuffix").gameObject;
                    textTemperatureSuffix = TemperatureSuffix.GetComponent<TextMeshProUGUI>();
                    textTemperatureSuffix.color = textColor;
                }

                int celsius = (int)__instance.GetPrivateField("lastTemperature");

                if (Main.FahrenheitEnabled)
                {
                    int fahrenheit = Mathf.CeilToInt(celsius * 1.8f + 32);

                    __instance.textTemperature.text = IntStringCache.GetStringForInt(fahrenheit);                    

                    textTemperatureSuffix.text = "\u00b0F";
                }
                else
                {
                    __instance.textTemperature.text = IntStringCache.GetStringForInt(celsius);
                    textTemperatureSuffix.text = "\u00b0C";
                }
            }
        }
    }

    [HarmonyPatch(typeof(uGUI_ExosuitHUD))]
    [HarmonyPatch("Update")]
    public class uGUI_ExosuitHUD_Update_Patch
    {
        private static Color textColor = new Color32(255, 220, 0, 255);       

        [HarmonyPostfix]
        public static void Postfix(uGUI_ExosuitHUD __instance)
        {
            if (Player.main != null && Player.main.inExosuit)
            {
                int celsius = (int)__instance.GetPrivateField("lastTemperature");
                
                if (Main.FahrenheitEnabled)
                {
                    int fahrenheit = Mathf.CeilToInt(celsius * 1.8f + 32);
                    __instance.textTemperature.text = IntStringCache.GetStringForInt(fahrenheit);
                    __instance.textTemperatureSuffix.text = "\u00b0F";
                }
                else
                {
                    __instance.textTemperature.text = IntStringCache.GetStringForInt(celsius);
                    __instance.textTemperatureSuffix.text = "\u00b0C";
                }

                __instance.textTemperatureSuffix.color = textColor;                   
            }
        }
    }

    [HarmonyPatch(typeof(ThermalPlant))]
    [HarmonyPatch("UpdateUI")]
    public class ThermalPlant_UpdateUI_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThermalPlant __instance)
        {            
            if (Main.FahrenheitEnabled)
            {
                int fahrenheit = Mathf.CeilToInt(__instance.temperature * 1.8f + 32);
                __instance.temperatureText.text = $"{IntStringCache.GetStringForInt(fahrenheit)}\u00b0F";
            }
            else
            {
                __instance.temperatureText.text = $"{IntStringCache.GetStringForInt(Mathf.CeilToInt(__instance.temperature))}\u00b0C";
            }            
        }
    }
}
