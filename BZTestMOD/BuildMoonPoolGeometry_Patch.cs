/*
using BZCommon;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BZTestMOD
{
    [HarmonyPatch(typeof(Base))]
    [HarmonyPatch("BuildMoonpoolGeometry")]
    public class Base_BuildMoonpoolGeometry_Patch
    {
        private struct PieceDef
        {
            public PieceDef(GameObject prefab)
            {
                this.prefab = prefab.transform;
            }

            public Transform prefab;
        }

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

        private static List<IBaseModuleGeometry> sBaseModulesGeometry = new List<IBaseModuleGeometry>();

        [HarmonyPrefix]
        public static bool Prefix(Base __instance, Int3 cell)
        {
            Debug.Log($"[BZTestMOD] BuildMoonpoolGeometry now called!");

            Int3 @int = Base.CellSize[7];
            Int3.Bounds bounds = new Int3.Bounds(cell, cell + @int - 1);
            BaseDeconstructable parent = null;
            Vector3 position = Int3.Scale(@int - Int3.one, Base.halfCellSize);
            Quaternion identity = Quaternion.identity;
           
            Transform _SpawnPiece(Base.Piece _piece, Int3 _cell, Vector3 _position, Quaternion _rotation, Base.Direction? _faceDirection = null)
            {
                Debug.Log("[BZTestMOD] _SpawnPiece called!");

                if (_piece == Base.Piece.Invalid)
                {
                    return null;
                }
                Transform transform = __instance.GetCellObject(_cell);

                if (transform == null)
                {
                    transform = __instance.CreateCellObject(_cell);
                }

                

                PieceDef[] basePieceDef = (PieceDef[])__instance.GetPrivateField("pieces", BindingFlags.Static);

                Debug.Log($"[BZTestMOD] basePieceDef: {basePieceDef.Length}");

                GameObject gameObject = UWE.Utils.InstantiateDeactivated(basePieceDef[(int)_piece].prefab.gameObject, transform, _position, _rotation);

                if (_faceDirection != null && _piece == Base.Piece.CorridorBulkhead)
                {
                    foreach (BaseWaterTransition baseWaterTransition in gameObject.GetComponentsInChildren<BaseWaterTransition>())
                    {
                        baseWaterTransition.face.cell = _cell;
                        baseWaterTransition.face.direction = _faceDirection.Value;
                    }
                }

                gameObject.SetActive(true);

                gameObject.BroadcastMessage("OnAddedToBase", __instance, SendMessageOptions.DontRequireReceiver);

                return gameObject.transform;
            }


            Base.Piece _GetMoonpoolPiece(Base.Face _face, Base.FaceType _faceType)
            {
                Int3 _cell = _face.cell;

                __instance.NormalizeCell(cell);

                int direction = (int)_face.direction;

                if (direction >= 0 && direction < moonpoolFacePieces.GetLength(0) && _faceType >= Base.FaceType.None && (int)_faceType < moonpoolFacePieces.GetLength(1))
                {
                    return moonpoolFacePieces[direction, (int)_faceType];
                }
                return Base.Piece.Invalid;
            }

            if (__instance.GetCellMask(cell))
            {
                Transform transform = _SpawnPiece(Base.Piece.Moonpool, cell, position, identity, null);

                parent = BaseDeconstructable.MakeCellDeconstructable(transform, bounds, TechType.BaseMoonpool);
                transform.tag = "MainPieceGeometry";
            }

            for (int i = 0; i < moonpoolFaces.Length; i++)
            {
                RoomFace roomFace = moonpoolFaces[i];
                Base.Face face = new Base.Face(cell + roomFace.offset, roomFace.direction);

                if (__instance.GetFaceMask(face))
                {
                    Base.FaceType face2 = __instance.GetFace(face);
                    Base.Piece moonpoolPiece = _GetMoonpoolPiece(face, face2);

                    if (moonpoolPiece != Base.Piece.Invalid)
                    {
                        Transform transform2 = _SpawnPiece(moonpoolPiece, cell, Vector3.zero, roomFace.rotation, null);

                        transform2.localPosition = Int3.Scale(roomFace.offset, Base.cellSize) + roomFace.localOffset;

                        if (face2 != Base.FaceType.Solid)
                        {
                            TechType recipe = Base.FaceToRecipe[(int)face2];

                            BaseDeconstructable baseDeconstructable = BaseDeconstructable.MakeFaceDeconstructable(transform2, bounds, face, face2, recipe);

                            if (!__instance.isGhost)
                            {
                                transform2.GetComponentsInChildren<IBaseModuleGeometry>(true, sBaseModulesGeometry);

                                int j = 0;

                                int count = sBaseModulesGeometry.Count;

                                while (j < count)
                                {
                                    sBaseModulesGeometry[j].geometryFace = face;
                                    j++;
                                }

                                sBaseModulesGeometry.Clear();

                                if (face2 == Base.FaceType.UpgradeConsole)
                                {
                                    baseDeconstructable.LinkModule(new Base.Face?(new Base.Face(face.cell - __instance.anchor, face.direction)));
                                }
                            }
                        }
                        else if (!__instance.isGhost)
                        {
                            BaseExplicitFace.MakeFaceDeconstructable(transform2, face, parent);
                        }
                    }
                }
            }

            return false;
        }

        private static readonly RoomFace[] moonpoolFaces = new RoomFace[]
        {
            new RoomFace(1, 0, Base.Direction.South, 90f, default(Vector3)),
            new RoomFace(2, 0, Base.Direction.South, 90f, default(Vector3)),
            new RoomFace(1, 2, Base.Direction.North, 270f, default(Vector3)),
            new RoomFace(2, 2, Base.Direction.North, 270f, default(Vector3)),
            new RoomFace(3, 1, Base.Direction.East, 0f, default(Vector3)),
            new RoomFace(0, 1, Base.Direction.West, 180f, default(Vector3))
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
}
*/