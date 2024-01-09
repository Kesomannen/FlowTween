namespace FlowTween {
    
/// <summary>
/// Specifies how a tween should loop.
/// You can apply one to a tween with <c>tween.Loop(...)</c>.
/// </summary>
public enum LoopMode {
    /// <summary>
    /// Run from start to end, then stops.
    /// </summary>
    None,
    
    /// <summary>
    /// Runs from start to end, then back to the start and loops until the tween is cancelled.
    /// </summary>
    Loop,
    
    /// <summary>
    /// Goes back and forth, repeating until the tween is cancelled.
    /// </summary>
    PingPong
}

}