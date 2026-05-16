using System.Collections.Generic;

[System.Serializable]
public struct UpgradeLevel
{
    public List<CurrencyPrice> Price;
    public float DeltaUpgradeValue;
}