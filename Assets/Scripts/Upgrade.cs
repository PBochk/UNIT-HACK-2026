using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public Stat Stat;
    [CanBeNull] public Upgrade RequiredUpgrade;
    public int Level = 0;
    public List<float> DeltaUpgradeValues = new List<float>();
    public Sprite Sprite;
    
    public float CurrentDeltaValue => Level == 0 ? 0 : DeltaUpgradeValues[Level - 1];
    public bool Upgraded => Level != 0;
    public event Action OnUpgradeGot;

    public void AddLevel()
    {
        if ((Level + 1) > DeltaUpgradeValues.Count)
        {
            Debug.LogWarning("You should not upgrade this, because there is no upgrade set for this");
            return;
        }
        Level++;
        if (Level == 1)
        {
            OnUpgradeGot?.Invoke();
        }
        
        Stat.Value += DeltaUpgradeValues[Level - 1];
    }

    public void Awake()
    {
        Level = 0;
    }
}
