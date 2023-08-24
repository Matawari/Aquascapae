using System.Collections.Generic;

public class SelectorNode : BehaviorTreeNode
{
    private List<BehaviorTreeNode> children;

    public SelectorNode(params BehaviorTreeNode[] nodes)
    {
        children = new List<BehaviorTreeNode>(nodes);
    }

    public override bool Execute()
    {
        foreach (var child in children)
        {
            if (child.Execute())
            {
                return true;
            }
        }
        return false;
    }
}