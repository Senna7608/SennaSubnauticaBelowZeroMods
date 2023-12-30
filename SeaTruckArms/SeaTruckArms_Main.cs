extern alias SEZero;

using BepInEx;
using BepInEx.Logging;
using SeaTruckArms.ArmPrefabs;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArms
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.slotextenderzero", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.senna.moddedarmshelperbz", BepInDependency.DependencyFlags.HardDependency)]
    public class SeaTruckArms_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.seatruckarms";
        private const string MODNAME = "SeaTruckArms";
        private const string VERSION = "2.0";        

        internal static ManualLogSource BepinLogger;
        internal static SeaTruckArms_Main mInstance;

        public void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            ModCamp.Main.Register(GUID, VERSION);            

            //SeatruckClawArm_Fragment clawArmFragment = new SeatruckClawArm_Fragment();
            //clawArmFragment.Patch();
            new SeatruckClawArm_Prefab(null).Patch();

            //SeatruckDrillArm_Fragment drillArmFragment = new SeatruckDrillArm_Fragment();
            //drillArmFragment.Patch();
            new SeatruckDrillArm_Prefab(null).Patch();

            //SeatruckGrapplingArm_Fragment grapplingArmFragment = new SeatruckGrapplingArm_Fragment();
            //grapplingArmFragment.Patch();
            new SeatruckGrapplingArm_Prefab(null).Patch();

            //SeatruckPropulsionArm_Fragment propulsionArmFragment = new SeatruckPropulsionArm_Fragment();
            //propulsionArmFragment.Patch();
            new SeatruckPropulsionArm_Prefab(null).Patch();

            //SeatruckTorpedoArm_Fragment torpedoArmFragment = new SeatruckTorpedoArm_Fragment();
            //torpedoArmFragment.Patch();
            new SeatruckTorpedoArm_Prefab(null).Patch();
        }       
    }
}
