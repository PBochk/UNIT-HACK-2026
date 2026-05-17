using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstaclePlacement", menuName = "Scriptable Objects/ObstaclePlacement")]
public class ObstaclePlacement : ScriptableObject
{
    [System.NonSerialized] public List<ObstacleAsset> Obstacles = new();
    [SerializeField] private List<ObstacleAsset> _obstacles = new();

    public void Initialize(int capacity)
    {
        Obstacles = new List<ObstacleAsset>(new ObstacleAsset[capacity]);
        
        for (int i = 0; i < capacity; i++)
        {
            if (i < _obstacles.Count)
                Obstacles[i] = _obstacles[i];
        }
    }
}