using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Required for DOTween

public class GameChanger : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;
    [SerializeField] private PlacementScreenManager PlacementScreenManager;
    [SerializeField] private UpgradeScreenManager UpgradeScreenManager;
    [SerializeField] private Canvas PlacementScreen;
    [SerializeField] private Canvas UpgradeScreen;
    [SerializeField] private Image Fader;
    [SerializeField] private float fadeDuration = 0.5f; // Added for easy tweaking

    private bool _busyAnimating = false;

    public void Awake()
    {
        GameManager.OnStageChanged += stage =>
        {
            if (stage == GameStage.Placement && !_busyAnimating)
                ChangeScreen(ScreenKind.Tree);
        };

        PlacementScreenManager.OnPlay += () =>
        {
            if(!_busyAnimating)
                ChangeScreen(ScreenKind.Game);
        };
        
        PlacementScreenManager.OnGoToTree += () =>
        {
            if(!_busyAnimating)
                ChangeScreen(ScreenKind.Tree);
        };
        
        // NOTE: Double-check these two! They both route to Screen.Tree in your original code.
        UpgradeScreenManager.OnPlay += () =>
        {
            if(!_busyAnimating)
                ChangeScreen(ScreenKind.Game); 
        };
        
        UpgradeScreenManager.OnGoToPlace += () =>
        {
            if(!_busyAnimating)
                ChangeScreen(ScreenKind.Place); // Did you mean Screen.Place here?
        };
    }

    public void ChangeScreen(ScreenKind screen)
    {
        if (_busyAnimating) return;
        _busyAnimating = true;

        // Ensure the fader is active and starts fully transparent
        Fader.gameObject.SetActive(true);
        CanvasGroup faderCanvasGroup = Fader.GetComponent<CanvasGroup>();
        
        // We use a DOTween Sequence to chain the steps perfectly
        Sequence fadeSequence = DOTween.Sequence();

        // 1 -> Fade In (Alpha to 1)
        fadeSequence.Append(Fader.DOFade(1f, fadeDuration));

        // 2 & 3 -> Disable all screens & Enable the required screen midway through the fade
        fadeSequence.AppendCallback(() =>
        {
            // Toggle Canvases using .enabled to avoid performance hits from SetActive(false)
            PlacementScreen.enabled = false;
            UpgradeScreen.enabled = false;

            switch (screen)
            {
                case ScreenKind.Place:
                    PlacementScreen.enabled = true;
                    break;
                case ScreenKind.Tree:
                    UpgradeScreen.enabled = true;
                    break;
                case ScreenKind.Game:
                    GameManager.StartNextBattle();
                    break;
            }
        });

        // 4 -> Fade Out (Alpha to 0)
        fadeSequence.Append(Fader.DOFade(0f, fadeDuration));

        // Clean up when everything is done
        fadeSequence.OnComplete(() =>
        {
            Fader.gameObject.SetActive(false);
            _busyAnimating = false;
        });
    }
}