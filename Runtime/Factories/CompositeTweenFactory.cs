using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace FlowTween {

public interface ICompositeTweenFactory<T, in THolder, TPart, in TPartId> : ITweenFactory<T, THolder> {
    void SetPart(ref T composite, TPartId part, TPart value);
    TPart GetPart(T composite, TPartId part);
    TPart Lerp(TPart from, TPart to, float t);
    
    public Tween<TPart> Apply(Tween<TPart> tween, THolder holder, TPartId part) {
        return tween.OnUpdate(v => {
                var current = Get(holder);
                SetPart(ref current, part, v);
                Set(holder, current);
            })
            .Lerp(Lerp)
            .From(GetPart(Get(holder), part));
    }

    public Tween<T> Apply(Tween<T> tween, THolder holder, IReadOnlyCollection<TPartId> parts) {
        return tween.OnUpdate(v => { 
                var current = Get(holder);
                foreach (var part in parts) {
                    SetPart(ref current, part, GetPart(v, part));
                }
                Set(holder, current);
            })
            .Lerp(Lerp)
            .From(Get(holder));
    }
    
    public ITweenFactory<TPart, THolder> WithPart(TPartId part) {
        return new PartTweenFactory<T, THolder, TPart, TPartId>(this, part);
    }
}

internal class PartTweenFactory<T, THolder, TPart, TPartId> : ITweenFactory<TPart, THolder> {
    readonly ICompositeTweenFactory<T, THolder, TPart, TPartId> _factory;
    readonly TPartId _part;

    public PartTweenFactory(ICompositeTweenFactory<T, THolder, TPart, TPartId> factory, TPartId part) {
        _factory = factory;
        _part = part;
    }

    public void Set(THolder holder, TPart value) {
        var current = _factory.Get(holder);
        _factory.SetPart(ref current, _part, value);
        _factory.Set(holder, current);
    }
        
    public TPart Get(THolder holder) {
        return _factory.GetPart(_factory.Get(holder), _part);
    }
        
    public TPart Lerp(TPart from, TPart to, float t) {
        return _factory.Lerp(from, to, t);
    }
}

public static class CompositeTweenFactoryTweenExtensions {
    public static Tween<TPart> Apply<T, THolder, TPart, TPartId>(
        this Tween<TPart> tween, 
        ICompositeTweenFactory<T, THolder, TPart, TPartId> factory, 
        THolder holder, 
        TPartId part
    ) {
        return factory.Apply(tween, holder, part);
    }
    
    public static Tween<TPart> Tween<T, THolder, TPart, TPartId>(
        this THolder holder, 
        ICompositeTweenFactory<T, THolder, TPart, TPartId> factory, 
        TPartId part,
        TPart to
    ) where THolder : Object {
        return holder.Tween(to).Apply(factory, holder, part);
    }
    
    public static Tween<T> Apply<T, THolder, TPart, TPartId>(
        this Tween<T> tween, 
        ICompositeTweenFactory<T, THolder, TPart, TPartId> factory, 
        THolder holder,
        IReadOnlyCollection<TPartId> parts
    ) where THolder : Object {
        return factory.Apply(tween, holder, parts);
    }
    
    public static Tween<T> Tween<T, THolder, TPart, TPartId>(
        this THolder holder, 
        ICompositeTweenFactory<T, THolder, TPart, TPartId> factory, 
        IReadOnlyCollection<TPartId> parts,
        T to
    ) where THolder : Object {
        return holder.Tween(to).Apply(factory, holder, parts);
    }
}

}