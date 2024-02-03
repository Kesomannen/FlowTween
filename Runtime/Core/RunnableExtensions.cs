using System;

namespace FlowTween {

/// <summary>
/// Provides extension methods shared by all <see cref="Runnable"/> subclasses,
/// mostly for setting properties in a builder-like fashion.
/// </summary>
public static class RunnableExtensions {
    /// <summary>
    /// Pauses the runnable.
    /// </summary>
    /// <seealso cref="Runnable.IsPaused"/>
    public static T Pause<T>(this T runnable) where T : Runnable {
        runnable.IsPaused = true;
        return runnable;
    }
    
    /// <summary>
    /// Resumes the runnable.
    /// </summary>
    /// <seealso cref="Runnable.IsPaused"/>
    public static T Resume<T>(this T runnable) where T : Runnable {
        runnable.IsPaused = false;
        return runnable;
    }
    
    /// <summary>
    /// Adds an action to the runnable's <see cref="TweenBase.CompleteAction"/>.
    /// </summary>
    public static T OnComplete<T>(this T runnable, Action action) where T : Runnable {
        runnable.CompleteAction += action;
        return runnable;
    }
    
    /// <summary>
    /// Sets the runnable's <see cref="TweenBase.Delay"/>.
    /// </summary>
    public static T SetDelay<T>(this T runnable, float delay) where T : Runnable {
        runnable.Delay = delay;
        return runnable;
    }
    
    /// <summary>
    /// Sets the runnable's <see cref="TweenBase.LoopMode"/> and <see cref="TweenBase.Loops"/>.
    /// </summary>
    public static T Loop<T>(this T runnable, LoopMode mode = LoopMode.Loop, int? loops = null) where T : Runnable {
        runnable.LoopMode = mode;
        runnable.Loops = loops;
        return runnable;
    }
}

}