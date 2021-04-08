using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZTestMOD
{
    [HarmonyPatch(typeof(PDAScanner))]
    [HarmonyPatch("Initialize")]
    public class PDAScanner_Initialize_Patch
    {

        [HarmonyPostfix]
        public static void Postfix()
        {
            

        }

    }
}
