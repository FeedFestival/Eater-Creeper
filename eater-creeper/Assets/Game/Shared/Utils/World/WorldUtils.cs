using System;
using UnityEngine;

namespace Game.Shared.Utils.World {
    public class WorldUtils : MonoBehaviour {

        public static Vector2 ToVector2(Vector3 pos) {
            return new Vector2(pos.x, pos.z);
        }

        public static Vector3 ToVector3(Vector2 pos, float y = 0f) {
            return new Vector3(pos.x, y, pos.y);
        }

        public static Vector2 RelativeDir(Transform t, Vector2 dir, float length) {
            return ToVector2(t.position) + (dir * length);
        }

        public static Vector3 RelativeDir(Transform t, Vector3 dir, float length) {
            return ToVector3(t.position) + (dir * length);
        }

        /*
         * Check if direction A is between directions B and C
         */
        public static bool IsDirectionBetween(Vector3 A, Vector3 B, Vector3 C) {

            // Check if A is on the same side of the plane formed by B and C
            float dotAB = Vector3.Dot(A, B);
            float dotAC = Vector3.Dot(A, C);
            float dotBC = Vector3.Dot(B, C);

            // If A is between B and C, both dotAB and dotAC should be positive, and the angle between B and C should be greater than the angle between A and both B and C
            if (dotBC > 0) {
                return dotAB > dotBC && dotAC > dotBC;
            }
            return false;
        }
    }
}