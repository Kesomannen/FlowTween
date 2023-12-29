using UnityEngine;

namespace FlowTween
{
    /// <summary>
    /// Utility component for disabling objects using <see cref="Disabling"/> with UnityEvents.
    /// </summary>
    public class Disabler : MonoBehaviour {
        [Tooltip("Whether or not to cache the IOnDisableRoutine components. " +
                 "This avoids a GetComponentInChildren call, but in dynamic hierarchies can cause issues.")]
        public bool Cached = true;
        
        public void DisableObject(MonoBehaviour behaviour) {
            Disabling.DisableObject(behaviour, Cached);
        }
        
        public void DisableThisObject() {
            Disabling.DisableObject(this, Cached);
        }
    }
}