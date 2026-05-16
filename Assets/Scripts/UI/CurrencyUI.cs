using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _amountText;
    [SerializeField]
    private Image icon;

    [SerializeField] private Currency _currency;

    public void Start()
    {
        icon.sprite = _currency.Icon;
        _currency.OnCurrencyValueChanged += () =>
        {
            UpdateText(_currency.Amount);
        };
        UpdateText(_currency.Amount);
    }

    private void UpdateText(int value)
    {
        Debug.Log($"{_currency.Id}: {_currency.Amount}");
        _amountText.text = value.ToString();
    }
}