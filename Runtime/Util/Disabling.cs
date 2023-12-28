using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowTween
{
    public static class Disabling {
        static readonly Dictionary<GameObject, IOnDisableRoutine[]> _cache = new();
        
        public static void DisableObject(MonoBehaviour behaviour, bool cached = true) {
            var onDisables = GetOnDisables(behaviour.gameObject, cached);
            if (onDisables.Length == 0) {
                behaviour.gameObject.SetActive(false);
                return;
            }
            
            behaviour.StartCoroutine(DisableObjectRoutine(behaviour, onDisables));
        }
        
        public static IEnumerator DisableObjectRoutine(MonoBehaviour behaviour, bool cached = true) {
            var onDisables = GetOnDisables(behaviour.gameObject, cached);
            if (onDisables.Length == 0) {
                behaviour.gameObject.SetActive(false);
                yield break;
            }
            
            yield return DisableObjectRoutine(behaviour, onDisables);
        }

        static IEnumerator DisableObjectRoutine(MonoBehaviour behaviour, IReadOnlyCollection<IOnDisableRoutine> onDisables) {
            var toComplete = onDisables.Count;
            
            foreach (var onDisable in onDisables) {
                behaviour.StartCoroutine(Await(onDisable.OnDisableRoutine()));
            }
            
            yield return new WaitUntil(() => toComplete == 0);

            behaviour.gameObject.SetActive(false);
            yield break;

            IEnumerator Await(IEnumerator routine) {
                yield return routine;
                toComplete--;
            }
        }

        static IOnDisableRoutine[] GetOnDisables(GameObject gameObject, bool cached) {
            if (!gameObject.activeSelf) {
                return Array.Empty<IOnDisableRoutine>();
            }

            if (!cached) {
                return gameObject.GetComponentsInChildren<IOnDisableRoutine>();
            }

            if (!_cache.TryGetValue(gameObject, out var onDisables)) {
                return _cache[gameObject] = gameObject.GetComponentsInChildren<IOnDisableRoutine>();
            }
            
            return onDisables;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            _cache.Clear();
        }
    }
    
    public static class DisablingExtensions {
        public static void DisableObject(this MonoBehaviour behaviour, bool cached = true) {
            Disabling.DisableObject(behaviour, cached);
        }
        
        public static IEnumerator DisableObjectRoutine(this MonoBehaviour behaviour, bool cached = true) {
            return Disabling.DisableObjectRoutine(behaviour, cached);
        }
        
        public static void SetObjectEnabled(this MonoBehaviour behaviour, bool enabled, bool cached = true) {
            if (enabled) {
                behaviour.gameObject.SetActive(true);
            } else {
                behaviour.DisableObject(cached);
            }
        }
        
        public static IEnumerator SetObjectEnabledRoutine(this GameObject gameObject, bool enabled, bool cached = true) {
            if (enabled) {
                gameObject.SetActive(true);
            } else {
                yield return gameObject.DisableObjectRoutine(cached);
            }
        }

        public static void DisableObject(this GameObject gameObject, bool cached = true) {
            if (!gameObject.TryGetComponent(out MonoBehaviour behaviour)) {
                gameObject.SetActive(false);
                return;
            }
            Disabling.DisableObject(behaviour, cached);
        }
        
        public static IEnumerator DisableObjectRoutine(this GameObject gameObject, bool cached = true) {
            if (!gameObject.TryGetComponent(out MonoBehaviour behaviour)) {
                gameObject.SetActive(false);
                yield break;
            }
            yield return Disabling.DisableObjectRoutine(behaviour, cached);
        }
    }

    public interface IOnDisableRoutine {
        IEnumerator OnDisableRoutine();
    }
}