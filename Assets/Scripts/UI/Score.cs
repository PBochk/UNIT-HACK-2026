using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text current;
    [SerializeField] private TMP_Text record;
    [SerializeField] private Currency score;

    private int _currentScore;
    private int _highScore;
    private void Start()
    {
        current.text = "0";
        record.text = "0";
        score.OnCurrencyValueChanged += HandleScoreChanged;
        GameManager.Instance.OnStageChanged += HandleStageChange;
    }

    private void HandleStageChange(GameStage _)
    {
        current.text = "0";
        _highScore = Mathf.Max(_currentScore, _highScore);
        record.text = _highScore.ToString();
        _currentScore = 0;
    }
    
    private void HandleScoreChanged()
    {
        current.text = score.Amount.ToString();
    }
}
