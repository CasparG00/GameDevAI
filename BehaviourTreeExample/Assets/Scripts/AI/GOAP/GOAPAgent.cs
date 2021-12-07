using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class GOAPAgent : MonoBehaviour
{
    public GOAPPlanner planner;
    
    private HashSet<Action> availableActions;
    private Queue<Action> currentActions;
    
    private AgentState state = AgentState.idle;

    private NavMeshAgent agent;

    private void Start()
    {
        availableActions = new HashSet<Action>();
        currentActions = new Queue<Action>();
        planner = new GOAPPlanner();

        var actions = gameObject.GetComponents<Action>();
        foreach (var action in actions)
        {
            availableActions.Add(action);
        }

        agent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        Action action = null;
        if (currentActions.Count > 0)
        {
            action = currentActions.Peek();
        }

        switch (state)
        {
            case AgentState.idle:
                var worldData = GetComponent<IGoap>().GetWorldData();
                var goal = GetComponent<IGoap>().CreateGoals();

                var plan = planner.Plan(agent, availableActions, goal, worldData);
                if (plan != null)
                {
                    currentActions = plan;
                    
                    if (currentActions.Peek().RequiresInRange())
                    {
                        state = AgentState.moveTo;
                    }
                    else
                    {
                        state = AgentState.performAction;
                    }
                }
                else
                {
                    Debug.Log("<color=orange>Failed to make plan</color>");
                }
                break;
            case AgentState.moveTo:
                if (action.RequiresInRange() && action.target ==  null)
                {
                    state = AgentState.idle;
                    return;
                }

                if (GetComponent<IGoap>().MoveAgent(action))
                {
                    state = AgentState.performAction;
                }
                
                break;
            case AgentState.performAction:
                if (!(currentActions.Count > 0))
                {
                    Debug.Log("Completed All Actions");
                    state = AgentState.idle;
                    return;
                }
                
                action = currentActions.Peek();
                if (action.IsCompleted()) 
                {
                    currentActions.Dequeue();
                }

                if (currentActions.Count > 0)
                {
                    action = currentActions.Peek();
                    var inRange = !action.RequiresInRange() || action.IsInRange();

                    if (inRange)
                    {
                        var success = action.PerformAction(agent);

                        if (!success)
                        {
                            state = AgentState.idle;
                        }
                    }
                    else
                    {
                        state = AgentState.moveTo;
                    }
                }
                else
                {
                    state = AgentState.idle;
                }
                
                break;
            default:
                state = AgentState.idle;
                break;
        }
    }
    
    public enum AgentState
    {
        idle,
        moveTo,
        performAction,
    }
}
