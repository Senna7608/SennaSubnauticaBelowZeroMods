/*
using BZCommon;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace BZTestMOD
{
    
    [HarmonyPatch(typeof(Base))]
    [HarmonyPatch("GetMoonpoolPiece")]
    public class Base_GetMoonpoolPiece_Patch
    {

        [HarmonyPrefix]
        public static bool Prefix(Base __instance, Base.Face face, Base.FaceType faceType, ref Base.Piece __result)
        {
            //Debug.Log("[BZTestMOD] GetMoonpoolPiece now called!");
            //Debug.Log($"face.direction : {face.direction}");
            Int3 cell = face.cell;
            //this.NormalizeCell(cell);
            int direction = (int)face.direction;
            if (direction >= 0 && direction < moonpoolFacePieces.GetLength(0) && faceType >= Base.FaceType.None && (int)faceType < moonpoolFacePieces.GetLength(1))
            {
                __result = moonpoolFacePieces[direction, (int)faceType];
                Debug.Log($"face.direction : {face.direction}");
                Debug.Log($"__result : {__result}");
                return false;
            }

            __result = Base.Piece.Invalid;

            return false;
        }

        private static readonly Base.Piece[,] moonpoolFacePieces = new Base.Piece[,]
		{
			{
				Base.Piece.MoonpoolCorridorConnector,
				Base.Piece.MoonpoolCoverSide,
				Base.Piece.MoonpoolWindowSide,
				Base.Piece.MoonpoolHatch,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolReinforcementSide,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolUpgradeConsole,
				Base.Piece.MoonpoolPlanterSide,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid
    },
			{
				Base.Piece.MoonpoolCorridorConnector,
				Base.Piece.MoonpoolCoverSide,
				Base.Piece.MoonpoolWindowSide,
				Base.Piece.MoonpoolHatch,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolReinforcementSide,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolUpgradeConsole,
				Base.Piece.MoonpoolPlanterSide,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid
},
			{
				Base.Piece.MoonpoolCorridorConnectorShort,
				Base.Piece.MoonpoolCoverSideShort,
				Base.Piece.MoonpoolWindowSideShort,
				Base.Piece.MoonpoolHatchShort,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolReinforcementSideShort,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolUpgradeConsoleShort,
				Base.Piece.MoonpoolPlanterSideShort,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid
			},
			{
				Base.Piece.MoonpoolCorridorConnectorShort,
				Base.Piece.MoonpoolCoverSideShort,
				Base.Piece.MoonpoolWindowSideShort,
				Base.Piece.MoonpoolHatchShort,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolReinforcementSideShort,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.MoonpoolUpgradeConsoleShort,
				Base.Piece.MoonpoolPlanterSideShort,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid,
				Base.Piece.Invalid
			}
		};

    }
    */
    /*
    [HarmonyPatch(typeof(Base))]
    [HarmonyPatch("CanSetMoonpoolFace")]
    public class Base_CanSetMoonpoolFace_Patch
    {
        private static Base Instance;

        [HarmonyPrefix]
        public static bool Prefix(Base __instance, Base.Face face, Base.FaceType faceType, ref bool __result)
        {
            Debug.Log($"[BZTestMOD] CanSetMoonpoolFace now called!");

            Instance = __instance;

            int index = __instance.baseShape.GetIndex(face.cell);

            __result = GetFace(index, face.direction) == constructFaceTypes[(int)faceType] && moonpoolFacePieces[(int)face.direction, (int)faceType] > Base.Piece.Invalid;

            return false;
        }

        private static Base.FaceType GetFace(int index, Base.Direction direction)
        {
            return Instance.faces[GetNormalizedFaceIndex(index, direction)];
        }

        private static int GetFaceIndex(int cellIndex, Base.Direction direction)
        {
            return (int)(cellIndex * 6 + direction);
        }

        private static int GetNormalizedFaceIndex(int cellIndex, Base.Direction direction)
        {
            return GetFaceIndex(cellIndex, NormalizeFaceDirection(cellIndex, direction));
        }

        private static Base.Direction NormalizeFaceDirection(int cellIndex, Base.Direction direction)
        {
            int faceIndex = GetFaceIndex(cellIndex, direction);
            Base.FaceType faceType = Instance.faces[faceIndex];
            if ((faceType & Base.FaceType.OccupiedByOtherFace) != Base.FaceType.None)
            {
                direction = (Base.Direction)(faceType & (Base.FaceType)127);
            }
            return direction;
        }

        private static readonly Base.FaceType[] constructFaceTypes = new Base.FaceType[]
        {
            Base.FaceType.None,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.None,
            Base.FaceType.None,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.Solid,
            Base.FaceType.None,
            Base.FaceType.None,
            Base.FaceType.None,
            Base.FaceType.None,
            Base.FaceType.Solid,
            Base.FaceType.Solid
        };

        private static readonly Base.Piece[,] moonpoolFacePieces = new Base.Piece[,]
        {
            {
                Base.Piece.MoonpoolCorridorConnector,
                Base.Piece.MoonpoolCoverSide,
                Base.Piece.MoonpoolWindowSide,
                Base.Piece.MoonpoolHatch,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolReinforcementSide,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolUpgradeConsole,
                Base.Piece.MoonpoolPlanterSide,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid
    },
            {
                Base.Piece.MoonpoolCorridorConnector,
                Base.Piece.MoonpoolCoverSide,
                Base.Piece.MoonpoolWindowSide,
                Base.Piece.MoonpoolHatch,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolReinforcementSide,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolUpgradeConsole,
                Base.Piece.MoonpoolPlanterSide,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid
},
            {
                Base.Piece.MoonpoolCorridorConnectorShort,
                Base.Piece.MoonpoolCoverSideShort,
                Base.Piece.MoonpoolWindowSideShort,
                Base.Piece.MoonpoolHatchShort,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolReinforcementSideShort,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolUpgradeConsoleShort,
                Base.Piece.MoonpoolPlanterSideShort,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid
            },
            {
                Base.Piece.MoonpoolCorridorConnectorShort,
                Base.Piece.MoonpoolCoverSideShort,
                Base.Piece.MoonpoolWindowSideShort,
                Base.Piece.MoonpoolHatchShort,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolReinforcementSideShort,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.MoonpoolUpgradeConsoleShort,
                Base.Piece.MoonpoolPlanterSideShort,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid,
                Base.Piece.Invalid
            }
        };
    }
    
    

    [HarmonyPatch(typeof(Base))]
    [HarmonyPatch("GetMoonpoolFaceLocation")]
    public class Base_GetMoonpoolFaceLocation_Patch
    {

        [HarmonyPrefix]
        public static void Prefix(Base __instance)
        {
            Debug.Log($"[BZTestMOD] GetMoonpoolFaceLocation now called!");
        }
    }

    [HarmonyPatch(typeof(Base))]
    [HarmonyPatch("SpawnMoonpoolModule")]
    public class Base_SpawnMoonpoolModule_Patch
    {

        [HarmonyPrefix]
        public static void Prefix(Base __instance)
        {
            Debug.Log($"[BZTestMOD] SpawnMoonpoolModule now called!");
        }
    }




    /*
        [HarmonyPatch(typeof(Base))]
        [HarmonyPatch("GetMoonpoolFaceLocation")]
        public class Base_GetMoonpoolFaceLocation_Patch
        {        
            private struct RoomFace
            {            
                public RoomFace(int x, int z, Base.Direction direction, float yAngle, Vector3 localOffset = default(Vector3))
                {
                    this.offset = new Int3(x, 0, z);
                    this.direction = direction;
                    this.rotation = Quaternion.Euler(0f, yAngle, 0f);
                    this.localOffset = localOffset;
                }

                public Int3 offset;
                public Base.Direction direction;
                public Quaternion rotation;
                public Vector3 localOffset;
            }

            [HarmonyPrefix]
            public static bool Prefix(Base __instance, Base.Face face, out Vector3 position, out Quaternion rotation, bool __result)
            {
                    BZLogger.Log($"[BZTestMOD] Base component GetMoonpoolFaceLocation now WORKING");

                int index = __instance.baseShape.GetIndex(face.cell);
                Int3 u = __instance.NormalizeCell(face.cell);

                int faceIndex = (int)(index * 6 + face.direction);

                Base.FaceType faceType = __instance.faces[faceIndex];
                if ((byte)(faceType & Base.FaceType.OccupiedByOtherFace) != 0)
                {
                    face.direction = (Base.Direction)((byte)(faceType & (Base.FaceType)127));
                }
                for (int i = 0; i < largeRoomRotatedFaces.Length; i++)
                {
                    RoomFace roomFace = largeRoomRotatedFaces[i];
                    Int3 u2 = u + roomFace.offset;
                    if (!(u2 != face.cell) && roomFace.direction == face.direction)
                    {
                        position = __instance.GridToLocal(u + roomFace.offset) + roomFace.localOffset;
                        rotation = roomFace.rotation;
                        __result = true;
                        return false;
                    }
                }
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return true;



            }

            private static readonly RoomFace[] largeRoomRotatedFaces = new RoomFace[]
            {
                new RoomFace(1, 0, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(2, 0, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(3, 0, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(4, 0, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(0, 1, Base.Direction.West, 180f, default(Vector3)),
                new RoomFace(5, 1, Base.Direction.East, 0f, default(Vector3)),
                new RoomFace(1, 2, Base.Direction.North, 270f, default(Vector3)),
                new RoomFace(2, 2, Base.Direction.North, 270f, default(Vector3)),
                new RoomFace(3, 2, Base.Direction.North, 270f, default(Vector3)),
                new RoomFace(4, 2, Base.Direction.North, 270f, default(Vector3)),
                new RoomFace(1, 0, Base.Direction.Below, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(2, 0, Base.Direction.Below, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(3, 0, Base.Direction.Below, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(4, 0, Base.Direction.Below, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(0, 1, Base.Direction.Below, 90f, new Vector3(1.6f, 0f, 0f)),
                new RoomFace(1, 1, Base.Direction.Below, 0f, default(Vector3)),
                new RoomFace(2, 1, Base.Direction.Below, 0f, default(Vector3)),
                new RoomFace(3, 1, Base.Direction.Below, 0f, default(Vector3)),
                new RoomFace(4, 1, Base.Direction.Below, 0f, default(Vector3)),
                new RoomFace(5, 1, Base.Direction.Below, 270f, new Vector3(-1.6f, 0f, 0f)),
                new RoomFace(1, 2, Base.Direction.Below, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(2, 2, Base.Direction.Below, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(3, 2, Base.Direction.Below, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(4, 2, Base.Direction.Below, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(0, 0, Base.Direction.Above, 0f, Int3.Scale(Base.CellSize[17] - Int3.one, Base.halfCellSize)),
                new RoomFace(1, 0, Base.Direction.Above, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(2, 0, Base.Direction.Above, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(3, 0, Base.Direction.Above, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(4, 0, Base.Direction.Above, 0f, new Vector3(0f, 0f, 1.6f)),
                new RoomFace(0, 1, Base.Direction.Above, 90f, new Vector3(1.6f, 0f, 0f)),
                new RoomFace(1, 1, Base.Direction.Above, 0f, default(Vector3)),
                new RoomFace(2, 1, Base.Direction.Above, 0f, default(Vector3)),
                new RoomFace(3, 1, Base.Direction.Above, 0f, default(Vector3)),
                new RoomFace(4, 1, Base.Direction.Above, 0f, default(Vector3)),
                new RoomFace(5, 1, Base.Direction.Above, 270f, new Vector3(-1.6f, 0f, 0f)),
                new RoomFace(1, 2, Base.Direction.Above, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(2, 2, Base.Direction.Above, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(3, 2, Base.Direction.Above, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(4, 2, Base.Direction.Above, 180f, new Vector3(0f, 0f, -1.6f)),
                new RoomFace(1, 1, Base.Direction.East, 0f, default(Vector3)),
                new RoomFace(1, 1, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(1, 1, Base.Direction.West, 180f, default(Vector3)),
                new RoomFace(1, 1, Base.Direction.North, 270f, default(Vector3)),
                new RoomFace(2, 1, Base.Direction.East, 0f, default(Vector3)),
                new RoomFace(2, 1, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(2, 1, Base.Direction.West, 180f, default(Vector3)),
                new RoomFace(2, 1, Base.Direction.North, 270f, default(Vector3)),
                new RoomFace(3, 1, Base.Direction.East, 0f, default(Vector3)),
                new RoomFace(3, 1, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(3, 1, Base.Direction.West, 180f, default(Vector3)),
                new RoomFace(3, 1, Base.Direction.North, 270f, default(Vector3)),
                new RoomFace(4, 1, Base.Direction.East, 0f, default(Vector3)),
                new RoomFace(4, 1, Base.Direction.South, 90f, default(Vector3)),
                new RoomFace(4, 1, Base.Direction.West, 180f, default(Vector3)),
                new RoomFace(4, 1, Base.Direction.North, 270f, default(Vector3))
            };

        }
    


    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch("CreateGhost")]
    public class Builder_CreateGhost_Patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            try
            {
                GameObject prefab = (GameObject)typeof(Builder).GetField("prefab", BindingFlags.NonPublic | BindingFlags.Static).GetValue(typeof(Builder));

                if (prefab.name.Equals("BaseMoonpool"))
                {
                    if (prefab.TryGetComponent(out Constructable constructable))
                    {
                        constructable.forceUpright = false;
                        constructable.rotationEnabled = true;
                    }
                }

                //Debug.Log($"[SNTestMOD] prefab.name = {prefab.name}");

            }
            catch
            {
                Debug.Log("[SNTestMOD] Builder CreateGhost patch failed to get prefab gameobject!");
            }

        }
    }


    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch("Update")]
    public class Builder_Update_Patch
    {

        [HarmonyPostfix]
        public static void Postfix()
        {
            try
            {
                GameObject prefab = (GameObject)typeof(Builder).GetField("prefab", BindingFlags.NonPublic | BindingFlags.Static).GetValue(typeof(Builder));

                if (prefab.name.Equals("BaseMoonpool"))
                {

                    //Debug.Log($"[SNTestMOD] canPlace = {Builder.canPlace}");
                }

            }
            catch
            {
                Debug.Log("[SNTestMOD] Builder Update patch failed to get prefab gameobject!");
            }

        }

    }

}
*/