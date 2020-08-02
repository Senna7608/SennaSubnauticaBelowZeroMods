using System;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace BZTestMOD
{
    public static class Main
    {
        public static void Load()
        {
            try
            {                

                HarmonyInstance.Create("SubnauticaBelowZero.BZTestMOD.mod").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
