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
    public List<float> UpgradeValues = new List<float>();
    public Sprite Sprite;
    
    public float CurrentValue => Level == 0 ? 0 : UpgradeValues[Level - 1];
    public bool Upgraded => Level != 0;
    public event Action OnUpgradeGot;

    public void AddLevel()
    {
        if ((Level + 1) > UpgradeValues.Count)
        {
            Debug.LogWarning("You should not upgrade this, because there is no upgrade set for this");
            return;
        }
        Level++;
        if (Level == 0)
        {
            OnUpgradeGot?.Invoke();
        }

        Stat.Value = CurrentValue;
    }
}
