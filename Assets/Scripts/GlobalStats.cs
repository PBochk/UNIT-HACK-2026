using System.Collections.Generic;
using UnityEngine;

public class GlobalStats : MonoBehaviour
{
    public static GlobalStats Instance { get; private set; }
    [SerializeField]
    public List<Stat> AllStats = new List<Stat>();

    public void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        Instance = this;
    }
}