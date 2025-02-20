using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IGameScene {
        void SetPlayer(IPlayer player);
        void StartScene();
    }
}