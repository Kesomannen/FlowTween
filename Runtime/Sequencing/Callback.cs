using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowTween.Sequencing {

public static class Callback {
    public static IEnumerator Run(Action action) {
        action?.Invoke();
        yield break;
    }
    
    public static Func<IEnumerator> From(Action action) {
        return () => Run(action);
    }
}

}