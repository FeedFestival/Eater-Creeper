#if UNITY_EDITOR
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Game.Shared.Core {
    [CustomEditor(typeof(EntityID))]
    public class EntityIDInspector : Editor {
        private string _cachedName;
        public override void OnInspectorGUI() {
            var script = (EntityID)target;

            DrawDefaultInspector();

            ShowNameOptions(script, ref _cachedName);
        }

        public static void ShowNameOptions(IEntityID entityIdScript, ref string cachedName) {
            GUILayout.Space(5);

            if (cachedName == null) {
                cachedName = EntityDefinition.DecodeId(entityIdScript.ID);
            }
            var needsNameAdjustment = entityIdScript.go.name != cachedName;
            if (!needsNameAdjustment) {
                return;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Name")) {
                entityIdScript.SetName();
            }
            GUILayout.EndHorizontal();
        }
    }

    [CustomEditor(typeof(UnitID))]
    public class UnitIDInspector : Editor {
        private string _cachedName;
        public override void OnInspectorGUI() {
            var script = (UnitID)target;

            DrawDefaultInspector();

            EntityIDInspector.ShowNameOptions(script, ref _cachedName);
        }
    }

    [CustomEditor(typeof(InteractableID))]
    public class InteractableIDInspector : Editor {
        private string _cachedName;
        public override void OnInspectorGUI() {
            var script = (InteractableID)target;

            DrawDefaultInspector();

            EntityIDInspector.ShowNameOptions(script, ref _cachedName);
        }
    }

    [CustomEditor(typeof(ZoneID))]
    public class ZoneIDInspector : Editor {
        private string _cachedName;
        public override void OnInspectorGUI() {
            var script = (ZoneID)target;

            DrawDefaultInspector();

            EntityIDInspector.ShowNameOptions(script, ref _cachedName);
        }
    }

    [CustomEditor(typeof(ItemID))]
    public class ItemIDInspector : Editor {
        private string _cachedName;
        public override void OnInspectorGUI() {
            var script = (ItemID)target;

            DrawDefaultInspector();

            EntityIDInspector.ShowNameOptions(script, ref _cachedName);
        }
    }
}
#endif
