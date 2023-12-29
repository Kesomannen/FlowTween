using UnityEngine;

namespace FlowTween {

public static class TweenUtilExtensions {
    /// <summary>
    /// Cancels all tweens on this component's game object.
    /// </summary>
    /// <param name="callOnComplete">Whether or not to call the tweens' <see cref="TweenBase.CompleteAction"/></param>
    /// <seealso cref="TweenManager.CancelObject"/>
    public static T CancelTweens<T>(this T obj, bool callOnComplete = false) where T : Component {
         obj.gameObject.CancelTweens(callOnComplete);
         return obj;
    }
    
    /// <summary>
    /// Cancels all tweens on this game object.
    /// </summary>
    /// <param name="callOnComplete">Whether or not to call the tweens' <see cref="TweenBase.CompleteAction"/></param>
    /// <seealso cref="TweenManager.CancelObject"/>
    public static GameObject CancelTweens(this GameObject obj, bool callOnComplete = false) {
        TweenManager.TryAccess(manager => manager.CancelObject(obj, callOnComplete));
        return obj;
    }
    
    /// <summary>
    /// Creates a new tween on this object.
    /// If at runtime and <see cref="TweenManager"/> is enabled, the tween is created
    /// using <see cref="TweenManager.NewTween{T}"/>, otherwise the default constructor is used.
    /// </summary>
    public static Tween<T> Tween<T>(this Object obj) {
        return TweenManager.TryAccess(manager => manager.NewTween<T>(obj), out var tween) ? tween : new Tween<T>();
    }

    /// <summary>
    /// Creates a new tween on this object and sets the <see cref="Tween{T}.End"/> value.
    /// If at runtime and <see cref="TweenManager"/> is enabled, the tween is created
    /// using <see cref="TweenManager.NewTween{T}"/>, otherwise the default constructor is used.
    /// </summary>
    internal static Tween<T> Tween<T>(this Object obj, T to) {
        return obj.Tween<T>().To(to);
    }
}

}