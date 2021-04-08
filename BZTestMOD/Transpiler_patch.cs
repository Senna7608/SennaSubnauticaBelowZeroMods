/*
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BZTestMOD
{
    internal static class Transpiler_patch
    {
        static Quaternion[] FaceRotation = new Quaternion[]
		{
			Quaternion.Euler(0f, -90f, 0f),
			Quaternion.Euler(0f, 90f, 0f),
			Quaternion.Euler(0f, 0f, 0f),
			Quaternion.Euler(0f, -180f, 0f),
			Quaternion.Euler(-90f, 0f, 0f),
			Quaternion.Euler(90f, 0f, 0f)
		};

    private static Quaternion SetQuaternion()
        {
            Debug.Log("[BZTestMOD] SetQuaternion now called!");
            Debug.Log($"[BZTestMOD] Builder.lastRotation: {Builder.lastRotation}");
            return Quaternion.Euler(0f, 90f, 0f);
        }


        private static IEnumerable<CodeInstruction> SubstQuaternionGetter(IEnumerable<CodeInstruction> cins)
        {
            var list = cins.ToList();

            var Quaternion_identity = AccessTools.DeclaredPropertyGetter(typeof(Quaternion), "identity");

            int index = list.FindIndex(ci => ci.opcode == OpCodes.Call && Equals(ci.operand, Quaternion_identity));

            if (index > 0)
            {
                list[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Transpiler_patch), nameof(SetQuaternion)));
            }

            return list;
        }

        private static IEnumerable<CodeInstruction> SpawnPiece_SetQuaternion(IEnumerable<CodeInstruction> cins)
        {
            var list = cins.ToList();            

            int index = list.FindIndex(ci => ci.opcode == OpCodes.Ldarg_S && Equals(ci.operand, typeof(Quaternion)));

            if (index > 0)
            {
                list[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Transpiler_patch), nameof(SetQuaternion)));
            }

            return list;
        }

        [HarmonyPatch(typeof(Base))]
        [HarmonyPatch("BuildMoonpoolGeometry")]
        public class Base_BuildMoonpoolGeometry_Patch
        {            
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins) => SubstQuaternionGetter(cins);
        }

        [HarmonyPatch(typeof(Base))]
        [HarmonyPatch("SpawnPiece")]
        [HarmonyPatch(new Type[] { typeof(Base.Piece), typeof(Int3), typeof(Vector3), typeof(Quaternion), typeof(Base.Direction) })]
        public class Base_SpawnPiece_Patch
        {
            [HarmonyPrefix]
            internal static bool Prefix(Base __instance, Base.Piece piece, ref Quaternion rotation)
            {
                if (piece == Base.Piece.Moonpool)
                {
                    Debug.Log("[BZTestMOD] SpawnPiece now called!");

                    rotation = Quaternion.Euler(0f, 90f, 0f);
                }


                return true;
            }

            
        }
    }
}
*/