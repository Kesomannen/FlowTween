using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowTween.Sequencing {

public class DynamicSequence {
    public SequenceNode RootNode { get; }
    public MonoBehaviour Behaviour { get; }
    
    public bool IsRunning { get; private set; }

    Dictionary<SequenceNode, NodeState> _nodeStates;

    public DynamicSequence(MonoBehaviour behaviour, SequenceNode rootNode) {
        Behaviour = behaviour;
        RootNode = rootNode;
    }

    public Coroutine Run() {
        if (IsRunning) {
            throw new InvalidOperationException("Sequence is already running");
        }
        return Behaviour.StartCoroutine(RunInternal());
    }

    IEnumerator RunInternal() {
        _nodeStates = new Dictionary<SequenceNode, NodeState>();
        IsRunning = true;
        yield return RunNode(RootNode);
        IsRunning = false;
    }

    IEnumerator RunNode(SequenceNode node) {
        if (_nodeStates.TryGetValue(node, out _)) {
            yield break;
        }
        
        _nodeStates[node] = NodeState.Running;

        switch (node) {
            case ActionNode actionNode:
                yield return actionNode.Behaviour();
                break;
            
            case JoinNode joinNode:
                while (joinNode.Parents.Any(n => !_nodeStates.TryGetValue(n, out var state) || 
                                                 state != NodeState.Completed)
                ) {
                    yield return null;
                }
                break;
            
            default: throw new ArgumentOutOfRangeException(nameof(node));
        }
        
        _nodeStates[node] = NodeState.Completed;
        foreach (var child in node.Children) {
            yield return RunNode(child);
        }
    }

    enum NodeState {
        Running,
        Completed
    }
}

public abstract class SequenceNode {
    public readonly List<SequenceNode> Children = new();
}

public class ActionNode : SequenceNode {
    public readonly Func<IEnumerator> Behaviour;

    public ActionNode(Func<IEnumerator> behaviour) {
        Behaviour = behaviour;
    }

    public ActionNode(IEnumerator enumerator) {
        Behaviour = () => enumerator;
    }
}

public class JoinNode : SequenceNode {
    public readonly List<SequenceNode> Parents = new();
}

}