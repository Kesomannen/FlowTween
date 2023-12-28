using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlowTween.Components {

[Serializable]
public abstract class FromToTweenerTargetData<T> : ITweenerTargetData<T> {
    [SerializeField] T _start;
    [SerializeField] bool _startRelative = true;
    [SerializeField] T _end;
    [SerializeField] bool _endRelative = true;
    
    protected abstract T Add(T a, T b);
    
    public T GetStartValue(T current) {
        return GetValue(current, _start, _startRelative);
    }

    public T GetEndValue(T current) {
        return GetValue(current, _end, _endRelative);
    }
    
    T GetValue(T current, T value, bool relative) {
        return relative ? Add(current, value) : value;
    }
}

[Serializable]
public class Vector3TweenerTargetData : FromToTweenerTargetData<Vector3> {
    protected override Vector3 Add(Vector3 a, Vector3 b) => a + b;
}

[Serializable]
public class Vector2TweenerTargetData : FromToTweenerTargetData<Vector2> {
    protected override Vector2 Add(Vector2 a, Vector2 b) => a + b;
}

[Serializable]
public class FloatTweenerTargetData : FromToTweenerTargetData<float> {
    protected override float Add(float a, float b) => a + b;
}

[Serializable]
public class ColorTweenerTargetData : FromToTweenerTargetData<Color> {
    protected override Color Add(Color a, Color b) => a * b;
}

}