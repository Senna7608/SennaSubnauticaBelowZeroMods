using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace BZTestMOD
{
    public static class Main
    {
        public static void Load()
        {
            try
            {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "SubnauticaBelowZero.BZTestMOD.mod");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
