using System.Reflection;
using System.IO;
using BepInEx;
using BepInEx.Logging;

namespace FreezeCannonTool
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInProcess("SubnauticaZero.exe")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]    
    public class FreezeCannonTool_Main : BaseUnityPlugin
    {
        private const string GUID = "com.senna.freezecannon";
        private const string MODNAME = "FreezeCannon";
        private const string VERSION = "2.0";

        internal static ManualLogSource BepinLogger;
        internal static FreezeCannonTool_Main mInstance;        
        
        public void Awake()
        {
            mInstance = this;
            BepinLogger = Logger;
            BepinLogger.LogInfo("Awake");

            FreezeCannon_Fragment fragment = new FreezeCannon_Fragment();
            fragment.Patch();
            new FreezeCannon_Prefab(fragment).Patch();                
        }       
    }

    public static class Main
    {
        public static readonly string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
