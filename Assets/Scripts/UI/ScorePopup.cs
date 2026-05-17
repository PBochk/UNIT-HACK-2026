using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private ScorableTarget target;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Canvas canvas;
    private void Start()
    {
        target.OnScoreAwarded += ShowText;
    }

    private void ShowText(Currency _)
    {
        if (floatingTextPrefab == null || canvas == null) return;
        
        var text = target.Scores.ToString();
        var textObj = Instantiate(floatingTextPrefab, canvas.transform);
        var rect = textObj.GetComponent<RectTransform>();
        var tmp = textObj.GetComponent<TMP_Text>();
        
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
        tmp.text = text;
        tmp.alpha = 1f;

        textObj.transform.DOMoveY(rect.anchoredPosition.y + 1f, 0.5f).SetEase(Ease.OutCubic);
        tmp.DOFade(0, 0.4f).SetDelay(0.4f).OnComplete(() => Destroy(textObj));
    }
}
