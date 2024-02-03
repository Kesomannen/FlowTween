namespace FlowTween {
    
/// <summary>
/// Contains values for all of the standard easing functions.
/// You can apply one to a tween with <c>tween.Ease(...)</c>.
/// Alternatively, you can access the functions directly from <see cref="Easings"/>.
/// Visit https://easings.net/ for a visual representation of each of the types.
/// </summary>
public enum EaseType {
    Linear,
    Sine,
    Quad,
    Cubic,
    Quart,
    Quint,
    Expo,
    Circ,
    Back,
    Elastic,
    Bounce
}

}