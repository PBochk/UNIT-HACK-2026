using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Currency", menuName = "Scriptable Objects/Currency")]
public class Currency : ScriptableObject
{
    [SerializeField]
    private int _amount;
    public int Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            OnCurrencyValueChanged?.Invoke();
        }
    }
    public string Id;
    public string DisplayName;
    public Sprite Icon;

    public event Action OnCurrencyValueChanged;
}