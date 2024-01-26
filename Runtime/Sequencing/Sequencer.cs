using System;
using System.Collections;
using FlowTween.Sequencing;
using UnityEngine;

namespace FlowTween.Components {

public class Sequencer : MonoBehaviour {
    [SerializeReference] SerializedNode[] _nodes;
}

[Serializable]
public abstract class SerializedNode {
    [SerializeField] Vector2 _position;
        
    public Vector2 Position {
        get => _position;
        set => _position = value;
    }

    public abstract IEnumerator Execute();
}

}