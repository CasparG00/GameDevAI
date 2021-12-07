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
    private IGoap GOAPuser;

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
        GOAPuser = GetComponent<IGoap>();
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
                var worldData = GOAPuser.GetWorldData();
                var goal = GOAPuser.CreateGoals();

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

                var reachedDestination = GOAPuser.MoveAgent(action);
                state = reachedDestination switch
                {
                    MoveState.inRange => AgentState.performAction,
                    MoveState.unreachable => AgentState.idle,
                    _ => state
                };

                break;
            case AgentState.performAction:
                if (currentActions.Count == 0)
                {
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

    private enum AgentState
    {
        idle,
        moveTo,
        performAction,
    }
}
