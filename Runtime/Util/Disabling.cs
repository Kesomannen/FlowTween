using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowTween {
    /// <summary>
    /// Utility class for allowing MonoBehaviours to have coroutines as
    /// OnDisable methods. This is extremely useful when you want to
    /// have a tween play when the object is disabled, since Unity
    /// doesn't allow you to re-enable an object from OnDisable.
    /// </summary>
    /// <seealso cref="IOnDisableRoutine"/>
    public static class Disabling {
        static readonly Dictionary<GameObject, IOnDisableRoutine[]> _cache = new();
        
        /// <summary>
        /// Disables the given MonoBehaviour's game object after all
        /// <see cref="IOnDisableRoutine.OnDisableRoutine"/> in any of its children have finished.
        /// </summary>
        /// <param name="behaviour">The root behaviour. All of the coroutines will be ran on this.</param>
        /// <param name="cached">
        /// Whether or not to cache the <see cref="IOnDisableRoutine"/> components. This avoids a
        /// <c>GetComponentInChildren</c> call, but in dynamic hierarchies can cause issues.
        /// </param>
        public static void DisableObject(MonoBehaviour behaviour, bool cached = true) {
            var onDisables = GetOnDisables(behaviour.gameObject, cached);
            if (onDisables.Length == 0) {
                behaviour.gameObject.SetActive(false);
                return;
            }
            
            behaviour.StartCoroutine(DisableObjectRoutine(behaviour, onDisables));
        }
        
        /// <inheritdoc cref="DisableObject(UnityEngine.MonoBehaviour,bool)"/>
        /// <returns>
        /// An IEnumerator that can be yielded from to execute some code after the disabling is finished.
        /// </returns>
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
        /// <inheritdoc cref="Disabling.DisableObject"/>
        public static void DisableObject(this MonoBehaviour behaviour, bool cached = true) {
            Disabling.DisableObject(behaviour, cached);
        }
        
        /// <inheritdoc cref="Disabling.DisableObjectRoutine"/>
        public static IEnumerator DisableObjectRoutine(this MonoBehaviour behaviour, bool cached = true) {
            return Disabling.DisableObjectRoutine(behaviour, cached);
        }
        
        /// <summary>
        /// Sets the enabled state of the given MonoBehaviour's game object.
        /// If enabled is false, the object is disabled using <see cref="Disabling.DisableObject"/>.
        /// </summary>
        public static void SetObjectEnabled(this MonoBehaviour behaviour, bool enabled, bool cached = true) {
            if (enabled) {
                behaviour.gameObject.SetActive(true);
            } else {
                behaviour.DisableObject(cached);
            }
        }
        
        /// <inheritdoc cref="SetObjectEnabled(UnityEngine.MonoBehaviour,bool,bool)"/>
        /// <returns>
        /// An IEnumerator that can be yielded from to execute some code after the disabling is finished.
        /// If enabled is true, the IEnumerator will immediately return.
        /// </returns>
        public static IEnumerator SetObjectEnabledRoutine(this GameObject gameObject, bool enabled, bool cached = true) {
            if (enabled) {
                gameObject.SetActive(true);
            } else {
                yield return gameObject.DisableObjectRoutine(cached);
            }
        }
        
        /// <summary>
        /// Disables the given game object after all
        /// <see cref="IOnDisableRoutine.OnDisableRoutine"/> in any of its children have finished.
        /// </summary>
        /// <param name="gameObject">
        /// The game object to disable. If it does not have any MonoBehaviours attached to it, the
        /// disabling will be immediate and no coroutines will be ran.
        /// </param>
        /// <param name="cached">
        /// Whether or not to cache the <see cref="IOnDisableRoutine"/> components. This avoids a
        /// <c>GetComponentInChildren</c> call, but in dynamic hierarchies can cause issues.
        /// </param>
        public static void DisableObject(this GameObject gameObject, bool cached = true) {
            if (!gameObject.TryGetComponent(out MonoBehaviour behaviour)) {
                gameObject.SetActive(false);
                return;
            }
            Disabling.DisableObject(behaviour, cached);
        }
        
        /// <inheritdoc cref="DisableObject(UnityEngine.MonoBehaviour,bool)"/>
        /// <returns>
        /// An IEnumerator that can be yielded from to execute some code after the disabling is finished.
        /// </returns>
        public static IEnumerator DisableObjectRoutine(this GameObject gameObject, bool cached = true) {
            if (!gameObject.TryGetComponent(out MonoBehaviour behaviour)) {
                gameObject.SetActive(false);
                yield break;
            }
            yield return Disabling.DisableObjectRoutine(behaviour, cached);
        }
    }

    /// <summary>
    /// Allows a MonoBehaviour to have a coroutine as its OnDisable method.
    /// Only works if the MonoBehaviour or its game object are disabled using
    /// <see cref="Disabling"/>.
    /// </summary>
    public interface IOnDisableRoutine {
        /// <summary>
        /// The coroutine to run when the MonoBehaviour is disabled.
        /// After all <c>OnDisableRoutine</c>s have finished in the hierarchy being disabled
        /// will the game object be disabled.
        /// </summary>
        IEnumerator OnDisableRoutine();
    }
    
    /// <summary>
    /// Allows a MonoBehaviour to detect when its game object is being
    /// disabled using <see cref="Disabling"/>.
    /// </summary>
    /// <seealso cref="IOnDisablingStarted"/>
    public interface IOnDisablingStarted : IOnDisableRoutine {
        /// <summary>
        /// Called when the MonoBehaviour's game object starts being disabled by <see cref="Disabling"/>.
        /// </summary>
        void OnDisablingStarted();
        
        IEnumerator IOnDisableRoutine.OnDisableRoutine() {
            OnDisablingStarted();
            yield break;
        }
    }
}