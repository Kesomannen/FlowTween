using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowTween.Sequencing {

public class DynamicSequenceBuilder : List<Func<IEnumerator>> {
    public void Add(Action action) {
        Add(Event.Create(action));
    }
    
    public DynamicSequence Build(MonoBehaviour behaviour) {
        var rootNode = new ActionNode(this[0]);
        var currentNode = rootNode;
        foreach (var func in this.Skip(1)) {
            var newNode = new ActionNode(func);
            currentNode.Children.Add(newNode);
            currentNode = newNode;
        }
        var sequence = new DynamicSequence(behaviour, rootNode);
        return sequence;
    }
}

}