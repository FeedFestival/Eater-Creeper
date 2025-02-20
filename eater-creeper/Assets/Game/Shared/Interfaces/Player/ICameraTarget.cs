using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraTarget
{
    Transform Transform { get; }

    void Move(Vector2 pos, bool cancel);
}
