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
    [field: SerializeField] public Stat CurrentAmount { get; private set; }
    [field: SerializeField] public Stat MaxAmount { get; private set; }
    [field: SerializeField] public GameObject Prefab { get;  private set; }
    [SerializeField] private Button _button;
    public UnityEvent<ObstacleNodeUI> OnNodeSelected;
    
    public bool IsAvailable => CurrentAmount.Value < MaxAmount.Value;
    private void Start()
    {
        _button.onClick.AddListener(() => OnNodeSelected?.Invoke(this));
        UpdateText();
    }

    public void IncreaseAmount()
    {
        CurrentAmount.Value++;
        UpdateText();
    }
    
    public void DecreaseAmount()
    {
        CurrentAmount.Value--;
        UpdateText();
    }

    private void UpdateText()
    {
        priceText.text = CurrentAmount.Value.ToString();
    }
}
