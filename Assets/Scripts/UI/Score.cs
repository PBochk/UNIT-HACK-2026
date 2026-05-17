using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text current;
    [SerializeField] private TMP_Text record;
    [SerializeField] private Currency score;
    [SerializeField] private VictoryScreen victoryScreen;
    
    
    private int _currentScore;
    private int _highScore;
    private int _prev;
    private bool win = false;
    
    private const int Goal = 1000;
    private void Start()
    {
        current.text = "0";
        record.text = "0";
        score.OnCurrencyValueChanged += HandleScoreChanged;
        GameManager.Instance.OnStageChanged += HandleStageChange;
    }

    private void Update()
    {
        if (_currentScore >= 1 && !win)
        {
            victoryScreen.Show();
            win = true;
        }
    }
    
    private void HandleStageChange(GameStage _)
    {
        current.text = "0";
        _highScore = Mathf.Max(_currentScore, _highScore);
        if (_highScore >= Goal)
        {
            victoryScreen.Show();
        }
        record.text = _highScore.ToString();
        _currentScore = 0;
    }
    
    private void HandleScoreChanged()
    {
        _currentScore = score.Amount - _prev;
        _prev = _currentScore;
        current.text = score.Amount.ToString();
    }
}
