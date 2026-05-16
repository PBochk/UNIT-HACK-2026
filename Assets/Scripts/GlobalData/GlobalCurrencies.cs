using System.Collections.Generic;
using UnityEngine;

public class GlobalCurrencies : MonoBehaviour
{
    public static GlobalCurrencies Instance { get; private set; }
    [SerializeField]
    public List<Currency> AllCurrencies = new List<Currency>();

    public void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        Instance = this;
    }
}