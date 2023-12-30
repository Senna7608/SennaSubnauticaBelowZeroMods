extern alias SeZero;

using BepInEx;
using BepInEx.Logging;
using FreezeCannonArm.Prefabs;
using Nautilus.Utility;
using SeZero::SlotExtenderZero.API;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace FreezeCannonArm
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.slotextenderzero", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.moddedarmshelperbz", BepInDependency.DependencyFlags.HardDependency)]
    public class FreezeCannonArm_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.freezecannonarm";
        private const string MODNAME = "FreezeCannonArm";
        private const string VERSION = "2.0";

        internal static ManualLogSource BepinLogger;
        internal static FreezeCannonArm_Main mInstance;        
        
                
        public void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            ModCamp.Main.Register(GUID, VERSION);

            Main.illumTex = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/FreezeCannonArm_illum.png");

            SeatruckFCA_Fragment seatruckFCA_Fragment = new SeatruckFCA_Fragment();
            seatruckFCA_Fragment.Patch();
            new SeatruckFCA_Prefab(seatruckFCA_Fragment).Patch();

            ExosuitFCA_Fragment exosuitFCA_Fragment = new ExosuitFCA_Fragment();
            exosuitFCA_Fragment.Patch();
            new ExosuitFCA_Prefab(exosuitFCA_Fragment).Patch();
        }
    }

    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static Texture2D illumTex;
    }
}
