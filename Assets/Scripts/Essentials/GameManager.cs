using System;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    [SerializeField] private BoardManager[] boards;

    public event Action<GameStage> OnStageChanged;

    public int Round { get; private set; } = 1;
    public int BoardsCount => boards.Length;
    public int CurrentBoardIndex { get; private set; } = 0;
    public GameStage Stage { get; private set; } = GameStage.Battle;

    private BoardManager _activeBoardInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CreateBoard();
    }

    private void Start()
    {
        SetStage(GameStage.Battle);
    }

    private void OnBattleCompleted()
    {
        SetStage(GameStage.Placement);
    }

    public void StartNextBattle()
    {
        Round++;
        SetStage(GameStage.Battle);
    }

    public void ChangeBoard()
    {
        if (CurrentBoardIndex + 1 >= boards.Length)
        {
            return;
        }

        if (_activeBoardInstance != null)
        {
            _activeBoardInstance.OnBattleCompleted -= OnBattleCompleted;
            Destroy(_activeBoardInstance.gameObject);
        }

        CurrentBoardIndex++;
        CreateBoard();
    }

    private void CreateBoard()
    {
        if (boards.Length == 0 || boards[CurrentBoardIndex] == null) return;

        _activeBoardInstance = Instantiate(boards[CurrentBoardIndex]);
        _activeBoardInstance.OnBattleCompleted += OnBattleCompleted;
    }

    private void SetStage(GameStage newStage)
    {
        Stage = newStage;
        OnStageChanged?.Invoke(Stage);
    }
}