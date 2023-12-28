using UnityEngine;

namespace FlowTween
{
    public class Disabler : MonoBehaviour {
        [SerializeField] bool _cached = true;
        
        public void DisableObject(MonoBehaviour behaviour) {
            Disabling.DisableObject(behaviour, _cached);
        }
    }
}