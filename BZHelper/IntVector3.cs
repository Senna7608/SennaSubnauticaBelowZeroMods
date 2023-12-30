using System;
using UnityEngine;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML documentation

    public struct IntVector3 : IEquatable<IntVector3>
    {
        public int x;
        public int y;
        public int z;

        public int magnitude
        {
            get
            {
                return (int)Mathf.Sqrt(x * x + y * y + z * z);
            }
        }

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public IntVector3(Vector3 vector3)
        {
            x = (int)vector3.x;
            y = (int)vector3.y;
            z = (int)vector3.z;
        }

        public IntVector3(string stringXYZ, char splitChar = ' ')
        {
            string[] numbers = stringXYZ.Split(splitChar);
            
            int.TryParse(numbers[0], out x);
            int.TryParse(numbers[1], out y);
            int.TryParse(numbers[2], out z);
        }

        public static implicit operator IntVector3(Vector3 v3)
        {
            return new IntVector3((int)v3.x, (int)v3.y, (int)v3.z);
        }

        public static implicit operator Vector3(IntVector3 iv3)
        {
            return new Vector3(iv3.x, iv3.y, iv3.z);
        }        

        public static int Distance(IntVector3 iv3A, IntVector3 iv3B)
        {
            return (iv3A - iv3B).magnitude;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static IntVector3 operator +(IntVector3 iv3A, IntVector3 iv3B)
        {
            return new IntVector3(iv3A.x + iv3B.x, iv3A.y + iv3B.y, iv3A.z + iv3B.z);
        }

        public static IntVector3 operator -(IntVector3 iv3A, IntVector3 iv3B)
        {
            return new IntVector3(iv3A.x - iv3B.x, iv3A.y - iv3B.y, iv3A.z - iv3B.z);
        }

        public static IntVector3 operator *(IntVector3 iv3A, IntVector3 iv3B)
        {
            return new IntVector3(iv3A.x * iv3B.x, iv3A.y * iv3B.y, iv3A.z * iv3B.z);
        }

        public static IntVector3 operator *(IntVector3 iv3A, int iv3B)
        {
            return new IntVector3(iv3A.x * iv3B, iv3A.y * iv3B, iv3A.z * iv3B);
        }        

        public static bool operator ==(IntVector3 iv3A, IntVector3 iv3B)
        {
            if (ReferenceEquals(iv3A, iv3B))
            {
                return true;
            }

            return iv3A == null || iv3B == null ? false : iv3A.x == iv3B.x && iv3A.y == iv3B.y && iv3A.z == iv3B.z;
        }

        public static bool operator !=(IntVector3 iv3A, IntVector3 iv3B)
        {
            return !(iv3A == iv3B);
        }

        public bool Equals(IntVector3 other)
        {
            if (this == null)
            {
                return false;
            }

            return ReferenceEquals(this, other) ? true : x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public bool Equals(Vector3 v3)
        {
            IntVector3 iv3 = new IntVector3(v3);

            if (this == null)
            {
                return false;
            }

            return ReferenceEquals(this, iv3) ? true : x.Equals(iv3.x) && y.Equals(iv3.y) && z.Equals(iv3.z);
        }

        public override bool Equals(object obj)
        {
            if (this == null)
            {
                return false;
            }

            return ReferenceEquals(this, obj) ? true : obj.GetType() == GetType() && Equals((IntVector3)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{x} {y} {z}";
        }
    }
}
