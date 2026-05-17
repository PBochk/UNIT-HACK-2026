using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] elements;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float staggerDelay = 0.1f;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        foreach (var group in elements)
        {
            if (group != null)
            {
                group.alpha = 0f;
                group.interactable = false;
                group.blocksRaycasts = false;
                group.gameObject.SetActive(false);
            }
        }

        continueButton?.onClick.AddListener(Hide);
        exitButton?.onClick.AddListener(Application.Quit);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        for (int i = 0; i < elements.Length; i++)
        {
            var group = elements[i];
            if (group == null) continue;

            group.gameObject.SetActive(true);
            
            // Плавное появление альфы
            DOTween.To(() => group.alpha, x => group.alpha = x, 1f, duration)
                .SetEase(Ease.OutQuad)
                .SetDelay(i * staggerDelay)
                .OnStart(() =>
                {
                    group.interactable = true;
                    group.blocksRaycasts = true;
                });
        }
    }

    public void Hide()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            var group = elements[i];
            if (group == null) continue;

            group.interactable = false;
            group.blocksRaycasts = false;

            DOTween.To(() => group.alpha, x => group.alpha = x, 0f, duration)
                .SetEase(Ease.InQuad)
                .SetDelay(i * staggerDelay)
                .OnComplete(() =>
                {
                    group.gameObject.SetActive(false);
                    if (i == elements.Length - 1)
                        gameObject.SetActive(false);
                });
        }
    }
}