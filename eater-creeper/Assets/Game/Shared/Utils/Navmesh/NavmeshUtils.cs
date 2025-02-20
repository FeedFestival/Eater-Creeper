using Game.Shared.Utils.World;
using UnityEngine;

namespace Game.Shared.Utils {
    public static class NavmeshUtils {

        public static bool isPosOnNavmesh(Vector3 pos) {
            var checkPos = pos;
            stayOnNavMesh(ref checkPos);
            var d = Vector3.Distance(pos, checkPos);
            return (d > 0) == false;
        }

        public static Vector3 closestEdgePoint(Vector3 pos, Vector3? defaultValue = null) {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.FindClosestEdge(pos, out hit, UnityEngine.AI.NavMesh.AllAreas)) {
                return hit.position;
            } else {
                return defaultValue.HasValue ? defaultValue.Value : pos;
            }
        }

        public static Vector2 closestEdgePointVct2(Vector3 pos, Vector2? defaultValue = null) {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.FindClosestEdge(pos, out hit, UnityEngine.AI.NavMesh.AllAreas)) {
                return WorldUtils.ToVector2(hit.position);
            } else {
                return defaultValue.HasValue
                    ? defaultValue.Value
                    : WorldUtils.ToVector2(pos);
            }
        }

        public static void stayOnNavMesh(ref Vector3 pos) {
            UnityEngine.AI.NavMeshHit hit;
            bool canWalk = UnityEngine.AI.NavMesh.SamplePosition(pos, out hit, 20, UnityEngine.AI.NavMesh.AllAreas);
            if (canWalk) {
                pos = hit.position;
            } else {
                if (UnityEngine.AI.NavMesh.FindClosestEdge(pos, out hit, UnityEngine.AI.NavMesh.AllAreas)) {
                    pos = hit.position;
                }
            }
        }

    }
}