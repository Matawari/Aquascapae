using System.Collections.Generic;

// SequenceNode.cs
public class SequenceNode : BehaviorTreeNode
{
    private List<BehaviorTreeNode> children;

    public SequenceNode(params BehaviorTreeNode[] nodes)
    {
        children = new List<BehaviorTreeNode>(nodes);
    }

    public override bool Execute()
    {
        foreach (var child in children)
        {
            if (!child.Execute())
            {
                return false;
            }
        }
        return true;
    }
}