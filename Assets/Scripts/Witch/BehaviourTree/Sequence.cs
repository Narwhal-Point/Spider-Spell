using System.Collections.Generic;

namespace Witch.BehaviourTree
{
    public class Sequence : Node
    {
        private Node _currentNode;
        private int _currentNodeIndex = 0;

        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            // If there's no current node, get the first one
            if (_currentNode == null)
            {
                _currentNode = Children[_currentNodeIndex];
            }

            // Evaluate the current node
            switch (_currentNode.Evaluate())
            {
                case NodeState.Failure:
                    State = NodeState.Failure;
                    _currentNode = null;
                    _currentNodeIndex = 0;
                    return State;
                case NodeState.Success:
                    // Move on to the next node
                    _currentNodeIndex++;
                    if (_currentNodeIndex >= Children.Count)
                    {
                        // All nodes have been successful
                        State = NodeState.Success;
                        _currentNode = null;
                        _currentNodeIndex = 0;
                    }
                    else
                    {
                        // Get the next node
                        _currentNode = Children[_currentNodeIndex];
                        State = NodeState.Running;
                    }
                    return State;
                case NodeState.Running:
                    State = NodeState.Running;
                    return State;
                default:
                    State = NodeState.Failure;
                    _currentNode = null;
                    _currentNodeIndex = 0;
                    return State;
            }
        }

    }

}