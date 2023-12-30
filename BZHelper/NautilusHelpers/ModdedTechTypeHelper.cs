using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BZHelper.NautilusHelpers
{
#pragma warning disable CS1591 // Missing XML documentation

    public sealed class ModdedTechTypeHelper
    {
        static ModdedTechTypeHelper()
        {
            BZLogger.Log("ModdedTechTypeHelper created.");
            _main.SearchForModdedTechTypes();
        }        

        private ModdedTechTypeHelper()
        {
        }

        private static readonly ModdedTechTypeHelper _main = new ModdedTechTypeHelper();

        public static ModdedTechTypeHelper Main => _main;

        private static readonly Dictionary<TechType, EquipmentType> _TypeDefCache = new Dictionary<TechType, EquipmentType>();
        public Dictionary<TechType, EquipmentType> TypeDefCache => _TypeDefCache;

        private static readonly Dictionary<string, TechType> _ModdedTechTypes = new Dictionary<string, TechType>();
        public Dictionary<string, TechType> ModdedTechTypes => _ModdedTechTypes;

        private static readonly Dictionary<TechType, string> _ModdedTechNames = new Dictionary<TechType, string>();        
        public Dictionary<TechType, string> ModdedTechNames => _ModdedTechNames;

        private static bool isInitialized = false;

        private void SearchForModdedTechTypes()
        {
            BZLogger.Trace("Searching for modded TechTypes has started...");

            _ModdedTechTypes.Clear();
            _ModdedTechNames.Clear();
            _TypeDefCache.Clear();

            int i = 0;

            int[] techTypeArray = (int[])Enum.GetValues(typeof(TechType));

            for (int j = 0; j < techTypeArray.Length; j++)
            {
                if (techTypeArray[j] >= 10008)
                {
                    TechType techType = (TechType)techTypeArray[j];
                    string techName = TechTypeExtensions.AsString(techType);
                                       
                    EquipmentType equipmentType = TechData.GetEquipmentType(techType);
                    _ModdedTechTypes.Add(techName, techType);
                    _TypeDefCache.Add(techType, equipmentType);
                    BZLogger.Trace($"Modded techtype found! Name: [{techName}], ID: [{(int)techType}], Type: [{equipmentType}]");
                    i++;
                }
            }

            foreach(KeyValuePair<string, TechType> kvp in _ModdedTechTypes)
            {
                _ModdedTechNames.Add(kvp.Value, kvp.Key);
            }

            isInitialized = true;

            BZLogger.Log($"Found [{i}] modded TechType(s).");
        }

        public bool IsModdedTechTypeExists(string classID)
        {
            if (!isInitialized)
                return false;

            if (_ModdedTechTypes.ContainsKey(classID))
            {
                return true;
            }

            return false;
        }

        public bool IsModdedTechTypeExists(string classID, out TechType techType)
        {
            if (!isInitialized)
            {
                techType = TechType.None;
                return false;
            }

            if (_ModdedTechTypes.ContainsKey(classID))
            {
                techType = _ModdedTechTypes[classID];
                return true;
            }

            techType = TechType.None;
            return false;
        }

        public bool IsModdedTechTypeExists(TechType techType)
        {
            if (!isInitialized)
                return false;

            if (_ModdedTechNames.ContainsKey(techType))
            {
                return true;
            }

            return false;
        }

        public bool IsModdedTechTypeExists(TechType techType, out string classID)
        {
            if (!isInitialized)
            {
                classID = "None";
                return false;
            }

            if (_ModdedTechNames.ContainsKey(techType))
            {
                classID = _ModdedTechNames[techType];
                return true;
            }

            classID = "None";
            return false;
        }
    }    
}
