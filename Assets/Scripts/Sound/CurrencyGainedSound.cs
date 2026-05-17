using UnityEngine;
using UnityEngine.Events;

public class CurrencyGainedSound : SoundOutput
{
    [SerializeField] private Currency currency;
    public UnityEvent OnCurrencyGained;
    private void Awake()
    {
        currency.OnCurrencyValueChanged += () => OnCurrencyGained.Invoke();
        Bind(OnCurrencyGained);
    }
}
