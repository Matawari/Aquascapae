using System;


// ConditionNode.cs
public class ConditionNode : BehaviorTreeNode
{
    private Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override bool Execute()
    {
        return condition.Invoke();
    }

}