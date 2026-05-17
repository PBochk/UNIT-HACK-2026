using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    [System.NonSerialized] public Dictionary<ObstacleAsset, int> AmountHeld = new();
    [SerializeField] private SerializedDictionary<ObstacleAsset, int> _amountHeld;

    private void OnEnable()
    {
        AmountHeld = new Dictionary<ObstacleAsset, int>(_amountHeld);
    }

    public bool ContainsAtLeastOne(ObstacleAsset obstacle)
    {
        return obstacle != null && AmountHeld.TryGetValue(obstacle, out int count) && count > 0;
    }

    public void Add(ObstacleAsset obstacle)
    {
        if (obstacle == null) return;
        
        if (!AmountHeld.ContainsKey(obstacle))
            AmountHeld[obstacle] = 0;
            
        AmountHeld[obstacle]++;
    }

    public bool TryRemove(ObstacleAsset obstacle)
    {
        if (!ContainsAtLeastOne(obstacle)) return false;
        
        AmountHeld[obstacle]--;
        return true;
    }
}