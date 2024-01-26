using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowTween.Sequencing {

public static class Wait {
    static readonly Dictionary<float, WaitForSeconds> _cache = new();

    public static WaitForSeconds Seconds(float seconds) {
        if (!_cache.TryGetValue(seconds, out var wait)) {
            wait = _cache[seconds] = new WaitForSeconds(seconds);
        }

        return wait;
    }
}

}