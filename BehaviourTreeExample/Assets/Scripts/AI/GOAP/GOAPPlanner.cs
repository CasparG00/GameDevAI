using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class GOAPPlanner
{
    public Queue<Action> Plan(NavMeshAgent _agent, IEnumerable<Action> _availableActions, Dictionary<string, object> _goal, Dictionary<string, object> _worldStates)
    {
        var usableActions = new List<Action>();

        foreach (var action in _availableActions)
        {
            action.DoReset();
            
            if (action.IsAchievable(_agent.gameObject))
            {
                usableActions.Add(action);
            }
        }

        var leaves = new List<Node>();

        // build graph
        var start = new Node(null, 0, _worldStates, null);
        var success = BuildGraph(start, leaves, usableActions, _goal);

        if (!success)
        {
            return null;
        }

        Node cheapest = null;
        foreach (var leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;
            }
            else
            {
                if (leaf.cost < cheapest.cost)
                {
                    cheapest = leaf;
                }
            }
        }

        var result = new List<Action>();

        var node = cheapest;

        while (node != null)
        {
            if (node.action != null)
            {
                result.Insert(0, node.action);
            }

            node = node.parent;
        }

        var queue = new Queue<Action>();
        foreach (var action in result)
        {
            queue.Enqueue(action);
        }

        return queue;
    }

    private bool BuildGraph(Node _parent, List<Node> _leaves, List<Action> _usableActions,
        Dictionary<string, object> _goal)
    {
        var pathFound = false;

        foreach (var action in _usableActions)
        {
            if (InState(action.preconditions, _parent.state))
            {
                var currentState = PopulateState(_parent.state, action.effects);

                var node = new Node(_parent, _parent.cost + action.cost, currentState, action);

                if (InState(_goal, currentState))
                {
                    _leaves.Add(node);
                    pathFound = true;
                }
                else
                {
                    var subset = ActionSubset(_usableActions, action);
                    var found = BuildGraph(node, _leaves, subset, _goal);
                    if (found)
                    {
                        pathFound = true;
                    }
                }
            }
        }
        
        return pathFound;
    }
    
    private bool InState(Dictionary<string, object> _test, Dictionary<string, object> _state) {
        var allMatch = true;
        foreach (var t in _test) 
        {
            var match = _state.Contains(t);
            if (!match)
                allMatch = false;
        }
        return allMatch;
    }

    private List<Action> ActionSubset(List<Action> _actions, Action _removing)
    {
        var subset = new List<Action>();

        foreach (var action in _actions)
        {
            if (!action.Equals(_removing))
            {
                subset.Add(action);
            }
        }

        return subset;
    }
    
    private Dictionary<string, object> PopulateState(Dictionary<string, object> _currentState, Dictionary<string, object> _stateChange) 
    {
        var state = _currentState.ToDictionary(_s => _s.Key, _s => _s.Value);

        foreach (var change in _stateChange) {
            state[change.Key] = change.Value;
        }
        return state;
    }


    private class Node {
        public readonly Node parent;
        public readonly float cost;
        public readonly Dictionary<string,object> state;
        public readonly Action action;

        public Node(Node _parent, float _cost, Dictionary<string,object> _state, Action _action) {
            this.parent = _parent;
            this.cost = _cost;
            this.state = _state;
            this.action = _action;
        }
    }
}
