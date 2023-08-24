using System;


// ActionNode.cs
public class ActionNode : BehaviorTreeNode
{
    private Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }

    public override bool Execute()
    {
        action.Invoke();
        return true;
    }

}