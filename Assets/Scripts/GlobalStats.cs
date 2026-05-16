using System.Collections.Generic;
using UnityEngine;

public class GlobalStats : MonoBehaviour
{
    public static GlobalStats Instance { get; private set; }
    
    [SerializeField]
    public List<Stat> AllStats = new();

    public void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        Instance = this;
    }
}