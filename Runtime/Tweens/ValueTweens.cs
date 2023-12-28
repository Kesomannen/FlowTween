using System;
using Object = UnityEngine.Object;

namespace FlowTween {

public static class ValueTweens {
    public static FloatTweenFactory<Object> Value { get; } = new(_ => 0, (_, _) => { });
    
    public static Tween<float> TweenValue(this Object obj, float from, float to) => obj.Tween(Value, to).From(from);
    public static Tween<float> TweenValue(this Object obj, float from, float to, float duration) => obj.Tween(Value, to).From(from).SetDuration(duration);
    
    public static Tween<float> Delay(this Object obj, float duration) => obj.Tween(Value, 0).SetDuration(duration);
    public static Tween<float> DelayedCall(this Object obj, float duration, Action callback) => obj.Delay(duration).OnComplete(callback);
}

}