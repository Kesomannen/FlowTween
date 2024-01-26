using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowTween.Sequencing {

public class DynamicSequenceBuilder {
    readonly List<BranchBuilder> _branches = new();

    public static IBranch Create() {
        var builder = new DynamicSequenceBuilder();
        var branch = new BranchBuilder(builder);
        builder._branches.Add(branch);
        return branch;
    }
    
    public DynamicSequence Build(MonoBehaviour behaviour) {
        var (root, current) = _branches[0].BuildNodes();
        var created = new Dictionary<IBranch, SequenceNode> {
            { _branches[0], current }
        };
        

        return new DynamicSequence(behaviour, root);
    }
    
    class BranchBuilder : IBranch {
        readonly List<Func<IEnumerator>> _items = new();
        readonly DynamicSequenceBuilder _builder;
        readonly IBranch _parent;
    
        public BranchBuilder(DynamicSequenceBuilder builder, IBranch parent = null) {
            _builder = builder;
            _parent = parent;
        }

        public (SequenceNode Root, SequenceNode Last) BuildNodes(SequenceNode parent = null) {
            var rootNode = new ActionNode(_items[0]);
            var currentNode = rootNode;
            foreach (var func in _items.Skip(1)) {
                var newNode = new ActionNode(func);
                currentNode.Children.Add(newNode);
                currentNode = newNode;
            }
            parent?.Children.Add(rootNode);
            return (rootNode, currentNode);
        }
        
        public DynamicSequence Build(MonoBehaviour behaviour) {
            return _builder.Build(behaviour);
        }
        
        public IBranch Branch(params Action<IBranch>[] branchActions) {
            foreach (var action in branchActions) {
                var newBranch = new BranchBuilder(_builder, this);
                action(newBranch);
                _builder._branches.Add(newBranch);
            }
            return this;
        }
            
        public IBranch Invoke(Action action) {
            return Add(Callback.From(action));
        }
        
        public IBranch Delay(float waitSeconds) {
            return Add(Wait.Seconds(waitSeconds));
        }
    
        public IBranch Add(YieldInstruction yieldInstruction) {
            return Add(yieldInstruction.Yield);
        }
        
        public IBranch Add(IEnumerator enumerator) {
            return Add(() => enumerator);
        }
        
        public IBranch Add(Func<IEnumerator> func) {
            _items.Add(func);
            return this;
        }
    }

    public interface IBranch {
        IBranch Branch(params Action<IBranch>[] branchActions);
        
        IBranch Invoke(Action action);
        IBranch Delay(float waitSeconds);
        IBranch Add(IEnumerator enumerator);
        IBranch Add(YieldInstruction yieldInstruction);
        IBranch Add(Func<IEnumerator> func);
    }
}

}