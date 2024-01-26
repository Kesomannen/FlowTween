using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowTween.Sequencing {

public static class Wait {
    static readonly Dictionary<float, WaitForSeconds> _cache = new();

    public static WaitForSeconds Get(float duration) {
        if (!_cache.TryGetValue(duration, out var wait)) {
            wait = _cache[duration] = new WaitForSeconds(duration);
        }

        return wait;
    }
}

}