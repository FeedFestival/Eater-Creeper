using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Utils {
    public static class MathUtils {


    }

    public static class Percent {

        public static float Find(float _percent, float _of) {
            return (_of / 100f) * _percent;
        }
        public static float What(float _is, float _of) {
            return (_is * 100f) / _of;
        }

    }
}
