using FlowTween.Components;
using UnityEngine;

namespace FlowTween {

public static class CameraTweens {
    public static FloatTweenFactory<Camera> FieldOfView { get; } = new(c => c.fieldOfView, (c, fov) => c.fieldOfView = fov);

    public static Tween<float> TweenFOV(this Camera camera, float fov) => camera.Tween(FieldOfView, fov);
}

}