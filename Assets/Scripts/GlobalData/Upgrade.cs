using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public Stat Stat;
    [CanBeNull] public Upgrade RequiredUpgrade;
    [System.NonSerialized]
    public int Level = 0;
    public List<UpgradeLevel> Levels = new List<UpgradeLevel>();
    public Sprite Sprite;
    
    public float CurrentDeltaValue => Level == 0 ? 0 : Levels[Level - 1].DeltaUpgradeValue;
    public bool Upgraded => Level != 0;
    public event Action OnUpgradeGot;
    public event Action OnLevelUp;

    // В классе Upgrade измени сигнатуру и добавь возвращаемые значения:
    public bool AddLevel()
    {
        if (Level >= Levels.Count)
        {
            Debug.LogWarning("You should not upgrade this, because there is no upgrade set for this");
            return false; // Достигнут макс. уровень
        }
    
        if (Levels[Level].Price.Any(p => p.Amount > p.Currency.Amount))
        {
            Debug.LogWarning("Not enough to buy");
            return false; // Не хватает денег
        }

        foreach (var price in Levels[Level].Price)
        {
            Debug.Log($"The price is {price.Amount} of {price.Currency.Id}");
            price.Currency.Amount -= price.Amount;
        }
    
        Level++;
        if (Level == 1)
        {
            OnUpgradeGot?.Invoke();
        }
    
        Stat.Value += CurrentDeltaValue;
        OnLevelUp?.Invoke(); // Вызываем OnLevelUp, он у тебя объявлен, но не использовался
        return true; // Покупка успешна!
    }

    public void OnEnable()
    {
        Level = 0;
    }
}
