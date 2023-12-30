using BZHelper;
using System.Collections.Generic;

namespace SlotExtenderZero.API
{
    public sealed class ModCamp
    {
        private static readonly ModCamp _main = new ModCamp();

        static ModCamp()
        {
            BZLogger.Log("API/ModCamp: ModCamp created.");
        }
        private ModCamp()
        {
        }

        public static ModCamp Main => _main;

        private readonly Dictionary<string, string> registeredMods = new Dictionary<string, string>();        

        public void Register(string GUID, string VERSION)
        {
            if (!registeredMods.ContainsKey(GUID))
            {
                registeredMods.Add(GUID, VERSION);
                BZLogger.Log($"API/ModCamp: Registering Mod: [{GUID}], VERSION: [{VERSION}]");
            }
        }
        public bool IsModPresent(string GUID)
        {            
            return registeredMods.ContainsKey(GUID);
        }
        public bool GetModVersion(string GUID, out string VERSION)
        {
            return registeredMods.TryGetValue(GUID, out VERSION);
        }       
    }
}
