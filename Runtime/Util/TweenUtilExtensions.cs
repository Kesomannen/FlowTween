using UnityEngine;

namespace FlowTween {

public static class TweenUtilExtensions {
    public static T CancelTweens<T>(this T obj) where T : Component {
         obj.gameObject.CancelTweens();
         return obj;
    }
    
    public static GameObject CancelTweens(this GameObject obj) {
        TweenManager.TryAccess(manager => manager.CancelObject(obj));
        return obj;
    }
    
    internal static Tween<T> Tween<T>(this Object obj) {
        return TweenManager.TryAccess(manager => manager.NewTween<T>(obj), out var tween) ? tween : new Tween<T>();
    }

    internal static Tween<T> Tween<T>(this Object obj, T to) {
        return obj.Tween<T>().To(to);
    }
}

}