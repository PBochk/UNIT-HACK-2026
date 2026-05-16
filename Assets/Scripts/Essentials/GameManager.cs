using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    [SerializeField] private BoardManager[] boards;
    [SerializeField] private PlacementManager placementManager;

    public BoardManager CurrentBoard  { get; private set; }

    public PlacementManager PlacementManager { get;private set; }
    public event Action<GameStage> OnStageChanged;

    public int Round { get; private set; } = 1;
    public int BoardsCount => boards.Length;
    public int CurrentBoardIndex { get; private set; } = 0;
    public GameStage Stage { get; private set; } = GameStage.Battle;

    private readonly List<int> _roundsToChangeBoard = new () {3, 6, 9};

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
        (PlacementManager = Instantiate(placementManager))
            .OnCompleted += OnPlacementCompleted;
    }

    private void Start()
    {
        SetStage(GameStage.Battle);
    }
    
    private void OnPlacementCompleted()
    {
        StartNextBattle();
    }

    private void OnBattleCompleted()
    {
        SetStage(GameStage.Placement);
    }

    private void StartNextBattle()
    {
       
        if (_roundsToChangeBoard.Contains(++Round))
            ChangeBoard();
        SetStage(GameStage.Battle);
    }

    private void ChangeBoard()
    {
        if (CurrentBoardIndex + 1 >= boards.Length)
        {
            return;
        }

        if (CurrentBoard != null)
        {
            CurrentBoard.OnBattleCompleted -= OnBattleCompleted;
            Destroy(CurrentBoard.gameObject);
        }

        CurrentBoardIndex++;
        CreateBoard();
    }

    private void CreateBoard()
    {
        if (boards.Length == 0 || boards[CurrentBoardIndex] == null) return;

        CurrentBoard = Instantiate(boards[CurrentBoardIndex]);
        CurrentBoard.OnBattleCompleted += OnBattleCompleted;
    }

    private void SetStage(GameStage newStage)
    {
        Stage = newStage;
        OnStageChanged?.Invoke(Stage);
    }
}