using System.Collections.Generic;

namespace Witch.BehaviourTree
{
    public class Selector : Node
    {
        int childRunningIndex = 0;
        int skipped = 0;
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node child in Children)
            {
                if (skipped < childRunningIndex)
                {
                    skipped += 1;
                    continue;
                }
                switch (child.Evaluate())
                {
                    case NodeState.Failure:
                        childRunningIndex += 1;
                        skipped += 1;
                        continue;
                    case NodeState.Success:
                        State = NodeState.Success;
                        childRunningIndex = 0;
                        skipped = 0;
                        return State;
                    case NodeState.Running:
                        State = NodeState.Running;
                        skipped = 0;
                        return State;
                    default:
                        State = NodeState.Failure;
                        return State;
                }
            }
            State = NodeState.Failure;
            childRunningIndex = 0;
            skipped = 0;
            return State;
        }
    }

}