using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObstacleNodeUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text priceText;
    [field: SerializeField] public ObstacleType Type { get; private set; }
    [field: SerializeField] public Stat Unlocked { get; private set; }
    [field: SerializeField] public Currency Currency { get; private set; }
    [field: SerializeField] public GameObject Prefab { get;  private set; }
    [SerializeField] private Button _button;
    public UnityEvent<ObstacleNodeUI> OnNodeSelected;

    public int Price => (int)Type;
    public bool IsEnough => Currency.Amount >= Price;
    private void Start()
    {
        priceText.text = Price.ToString();
        _button.onClick.AddListener(() => OnNodeSelected?.Invoke(this));
    }

    public void SpendCurrency()
    {
        Currency.Amount -= Price;
    }
}
