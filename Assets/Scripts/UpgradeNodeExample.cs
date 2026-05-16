using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro; // Для TextMeshPro, если используешь его для всплывающего текста
using System.Text;

public class UpgradeNodeExample : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Upgrade Upgrade;
    
    [Header("UI References")]
    [SerializeField] private RectTransform nodeRect;
    [SerializeField] private GameObject floatingTextPrefab; // Префаб с TextMeshPro для отлетающего текста
    //[SerializeField] private Tooltip tooltip; // Ссылка на твой скрипт тултипа (см. ниже)

    private Upgrade Required;
    private Vector3 originalScale;
    
    const float MouseEnterScale = 1.07f;

    public void Start()
    {
        if (nodeRect == null) nodeRect = GetComponent<RectTransform>();
        originalScale = nodeRect.localScale;

        if (Upgrade.RequiredUpgrade != null)
        {
            gameObject.SetActive(false);
            Required = Upgrade.RequiredUpgrade;
            Required.OnUpgradeGot += () => gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Анимация наведения: немного увеличиваем
        nodeRect.DOKill(true); // Убиваем старые анимации, чтобы не было конфликтов
        nodeRect.DOScale(originalScale * MouseEnterScale, 0.15f).SetEase(Ease.OutBack);

        // Собираем текст для тултипа
        // ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Возвращаем размер обратно
        nodeRect.DOKill(true);
        nodeRect.DOScale(originalScale, 0.15f).SetEase(Ease.InOutSine);

        // if (tooltip != null) tooltip.Hide();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Upgrade.Levels.Count <= Upgrade.Level) return;

        // Пытаемся купить
        bool success = Upgrade.AddLevel();       
        
        if (success)
        {
            PlayPurchaseAnimation();
            SpawnFloatingText("Upgraded!");
            
            // Обновляем тултип, так как цена/уровень изменились
            // ShowTooltip(); 
        }
        else
        {
            // Опционально: анимация ошибки (тряска)
            nodeRect.DOShakePosition(0.25f, strength: new Vector3(5, 0, 0), vibrato: 25);
        }
    }

    private void PlayPurchaseAnimation()
    {
        nodeRect.DOKill(true);
        nodeRect.localScale = originalScale;
        nodeRect.localRotation = Quaternion.identity;

        // Делаем Sequence в стиле Balatro: резкий рывок размера и поворот, затем возврат к hover-состоянию
        Sequence seq = DOTween.Sequence();
        seq.Append(nodeRect.DOScale(originalScale * 1.3f, 0.1f).SetEase(Ease.OutQuad));
        seq.Join(nodeRect.DORotate(new Vector3(0, 0, Random.Range(-8f, 8f)), 0.1f)); // Случайный угол

        seq.Append(nodeRect.DOScale(originalScale * MouseEnterScale, 0.15f).SetEase(Ease.InQuad)); // Возврат к скейлу наведения
        seq.Join(nodeRect.DORotate(Vector3.zero, 0.15f));
    }

    private void SpawnFloatingText(string text)
    {
        if (floatingTextPrefab == null) return;

        // Создаем текст поверх ноды
        GameObject textObj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform.parent);
        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;

        // Анимация вылетающего текста: летит вверх и растворяется
        textObj.transform.DOMoveY(transform.position.y + 50f, 1f).SetEase(Ease.OutCubic);
        tmp.DOFade(0, 1f).SetDelay(0.5f).OnComplete(() => Destroy(textObj));
    }

    // private void ShowTooltip()
    // {
    //     if (tooltip == null) return;
    //
    //     string upgradeName = Upgrade.name;
    //     string priceText = "Max Level";
    //
    //     // Формируем строку цены
    //     if (Upgrade.Level < Upgrade.Levels.Count)
    //     {
    //         StringBuilder sb = new StringBuilder("Price: ");
    //         foreach (var price in Upgrade.Levels[Upgrade.Level].Price)
    //         {
    //             sb.Append($"{price.Amount} {price.Currency.Id} ");
    //         }
    //         priceText = sb.ToString();
    //     }
    //
    //     tooltip.Show(upgradeName, priceText, transform.position);
    // }
}