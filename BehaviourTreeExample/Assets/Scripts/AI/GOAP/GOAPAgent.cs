using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public sealed class GOAPAgent : MonoBehaviour
{
    public GOAPPlanner planner;
    
    private HashSet<Action> availableActions;
    private Queue<Action> currentActions;
    
    private AgentState state = AgentState.Idle;

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
            case AgentState.Idle:
                var worldData = GOAPuser.GetWorldData();
                var goal = GOAPuser.CreateGoals();

                var plan = planner.Plan(agent, availableActions, goal, worldData);
                if (plan != null)
                {
                    currentActions = plan;
                    
                    if (currentActions.Peek().RequiresInRange())
                    {
                        state = AgentState.MoveTo;
                    }
                    else
                    {
                        state = AgentState.PerformAction;
                    }
                }
                else
                {
                    Debug.Log(GOAPuser + "<color=red>Failed to make plan</color>");
                }
                break;
            case AgentState.MoveTo:
                if (action.RequiresInRange() && action.target ==  null)
                {
                    Debug.Log("<color=orange>Action requires agent to be in range but target has not been set for: </color>" + action);
                    state = AgentState.Idle;
                    return;
                }

                var reachedDestination = GOAPuser.MoveAgent(action);
                state = reachedDestination switch
                {
                    MoveState.inRange => AgentState.PerformAction,
                    MoveState.unreachable => AgentState.Idle,
                    _ => state
                };

                break;
            case AgentState.PerformAction:
                if (currentActions.Count == 0)
                {
                    state = AgentState.Idle;
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
                        var success = action.PerformAction(gameObject);

                        if (!success)
                        {
                            state = AgentState.Idle;
                        }
                    }
                    else
                    {
                        state = AgentState.MoveTo;
                    }
                }
                else
                {
                    state = AgentState.Idle;
                }
                
                break;
            default:
                state = AgentState.Idle;
                break;
        }
    }

    private enum AgentState
    {
        Idle,
        MoveTo,
        PerformAction,
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (agent != null)
        {
            var text = currentActions.Count == 0 ? state.ToString() : currentActions.Peek().actionContext;
            Handles.Label(agent.transform.position + Vector3.up * (agent.height + 0.2f), text);
        }
    }
}
