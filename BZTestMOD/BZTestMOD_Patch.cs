using BZCommon;
using Harmony;
using UnityEngine;

namespace BZTestMOD
{
    [HarmonyPatch(typeof(Base))]
    [HarmonyPatch("BuildMoonpoolGeometry")]
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

}
