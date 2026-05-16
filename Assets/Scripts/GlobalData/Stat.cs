using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "Scriptable Objects/Stat")]
public class Stat : ScriptableObject
{
    public string Id;
    //This one is in russian
    public string DisplayName;
    public string DisplayDescription;
    public float DefaultValue = 1;

    [System.NonSerialized]
    private float _value;
    public float Value
    {
        get => _value;
        set => _value = value;
    }

    public event Action OnValueChanged;

    public void OnEnable()
    {
        _value = DefaultValue;
    }
}
    