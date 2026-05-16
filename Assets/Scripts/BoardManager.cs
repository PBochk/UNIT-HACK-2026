using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class BoardManager : MonoBehaviour
{
    public IReadOnlyList<GameObject> Obstacles { get; private set; }
    private void Start()
    {
        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle").ToList();
        Obstacles = obstacles;
    }
}
