#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Game.Shared.Interfaces {

    public interface IEntityID: IIsGeneral {
        int ID { get; }
        void DestroyComponent();

#if UNITY_EDITOR
        GameObject go { get; }
        void SetName();
#endif
    }

    public interface IIsGeneral {
        bool IsGeneral { get; }
    }
}
