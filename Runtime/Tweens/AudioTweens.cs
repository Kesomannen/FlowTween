using UnityEngine;

namespace FlowTween {
    
public static class AudioTweens {
    public static FloatTweenFactory<AudioSource> Volume { get; } = new(c => c.volume, (c, v) => c.volume = v);
    public static FloatTweenFactory<AudioSource> Pitch { get; } = new(c => c.pitch, (c, v) => c.pitch = v);
    public static FloatTweenFactory<AudioSource> PanStereo { get; } = new(c => c.panStereo, (c, v) => c.panStereo = v);

    public static Tween<float> TweenVolume(this AudioSource audioSource, float volume) => audioSource.Tween(Volume, volume);
    public static Tween<float> TweenPitch(this AudioSource audioSource, float pitch) => audioSource.Tween(Pitch, pitch);
    public static Tween<float> TweenPan(this AudioSource audioSource, float panStereo) => audioSource.Tween(PanStereo, panStereo);
}

}