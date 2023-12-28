using System;
using Object = UnityEngine.Object;

namespace FlowTween {

public interface ITweenFactory<T, in THolder> {
    void Set(THolder holder, T value);
    T Get(THolder holder);
    T Lerp(T from, T to, float t);
    
    public Tween<T> Apply(Tween<T> tween, THolder holder) {
        return tween.Lerp(Lerp)
            .OnUpdate(v => Set(holder, v))
            .From(Get(holder));
    }
}

public class TweenFactory<T, THolder> : ITweenFactory<T, THolder> {
    readonly Func<THolder, T> _getter;
    readonly Action<THolder, T> _setter;
    readonly Func<T, T, float, T> _lerp;

    public TweenFactory(
        Func<THolder, T> getter,
        Action<THolder, T> setter, 
        Func<T, T, float, T> lerp
    ) {
        _getter = getter;
        _setter = setter;
        _lerp = lerp;
    }

    public T Get(THolder holder) => _getter(holder);
    public void Set(THolder holder, T value) => _setter(holder, value);
    public T Lerp(T from, T to, float t) => _lerp(from, to, t);
}

public static class TweenFactoryTweenExtensions {
    public static Tween<T> Apply<T, THolder>(this Tween<T> tween, ITweenFactory<T, THolder> factory, THolder holder) {
        return factory.Apply(tween, holder);
    }
    
    public static Tween<T> Tween<T, THolder>(this THolder holder, ITweenFactory<T, THolder> factory, T to) where THolder : Object {
        return holder.Tween(to).Apply(factory, holder);
    }
}

}