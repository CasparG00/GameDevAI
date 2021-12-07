using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly Dictionary<string, int> items = new Dictionary<string, int>();

    public void Add(string _id, int _amount)
    {
        _id = _id.ToLower();
        if (items.ContainsKey(_id))
        {
            items[_id] += _amount;
        }
        else
        {
            items.Add(_id, _amount);
        }
    }

    public void Remove(string _id, int _amount)
    {
        _id = _id.ToLower();
        
        if (items[_id] > _amount) 
        {
            items[_id] -= _amount;
        }
        else
        {
            items[_id] = 0;
        }
    }

    public int GetAmount(string _id)
    {
        _id = _id.ToLower();
        return items.ContainsKey(_id) ? items[_id] : 0;
    }
}