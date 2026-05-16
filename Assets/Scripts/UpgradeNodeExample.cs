using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Text;

public class UpgradeNodeExample : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Upgrade Upgrade;
    
    [Header("UI References")]
    [SerializeField] private RectTransform nodeRect;
    [SerializeField] private GameObject floatingTextPrefab; 
    [SerializeField] private UpgradeTooltip tooltip; // РАСКОММЕНТИРОВАНО: Ссылка на скрипт тултипа

    private Upgrade Required;
    private Vector3 originalScale;
    
    const float MouseEnterScale = 1.07f;

    public void Start()
    {
        if (nodeRect == null) nodeRect = GetComponent<RectTransform>();
        originalScale = nodeRect.localScale;
        var tooltipCanvas = FindObjectsByType
            <Canvas>(FindObjectsSortMode.None).FirstOrDefault(c => c.name == "TooltipCanvas");
        if (tooltipCanvas != null)
        {
            tooltip.transform.SetParent(tooltipCanvas.transform);
        }

        if (Upgrade.RequiredUpgrade != null)
        {
            gameObject.SetActive(false);
            Required = Upgrade.RequiredUpgrade;
            Required.OnUpgradeGot += () => gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        nodeRect.DOKill(true); 
        nodeRect.DOScale(originalScale * MouseEnterScale, 0.15f).SetEase(Ease.OutBack);

        // РАСКОММЕНТИРОВАНО: Показываем тултип
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nodeRect.DOKill(true);
        nodeRect.DOScale(originalScale, 0.15f).SetEase(Ease.InOutSine);

        // РАСКОММЕНТИРОВАНО: Прячем тултип
        if (tooltip != null) tooltip.Hide();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Upgrade.Levels.Count <= Upgrade.Level) return;

        bool success = Upgrade.AddLevel();       
        
        if (success)
        {
            PlayPurchaseAnimation();
            SpawnFloatingText("Upgraded!");
            
            ShowTooltip(); 
        }
        else
        {
            nodeRect.DOShakePosition(0.25f, strength: new Vector3(5, 0, 0), vibrato: 25);
        }
    }

    private void ShowTooltip()
    {
        if (tooltip != null)
        {
            tooltip.Show(Upgrade);
        }
    }

    private void PlayPurchaseAnimation()
    {
        nodeRect.DOKill(true);
        nodeRect.localScale = originalScale;
        nodeRect.localRotation = Quaternion.identity;

        Sequence seq = DOTween.Sequence();
        seq.Append(nodeRect.DOScale(originalScale * 1.3f, 0.1f).SetEase(Ease.OutQuad));
        seq.Join(nodeRect.DORotate(new Vector3(0, 0, Random.Range(-8f, 8f)), 0.1f)); 

        seq.Append(nodeRect.DOScale(originalScale * MouseEnterScale, 0.15f).SetEase(Ease.InQuad)); 
        seq.Join(nodeRect.DORotate(Vector3.zero, 0.15f));
    }

    private void SpawnFloatingText(string text)
    {
        if (floatingTextPrefab == null) return;

        GameObject textObj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform.parent);
        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;

        textObj.transform.DOMoveY(transform.position.y + 1f, 0.5f).SetEase(Ease.OutCubic);
        tmp.DOFade(0, 0.4f).SetDelay(0.4f).OnComplete(() => Destroy(textObj));
    }
}