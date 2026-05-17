using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    [SerializeField] private BoardManager[] boards;
    [SerializeField] private PlacementManager placementManager;
    [SerializeField] private ObstaclePlacement placement;

    public BoardManager CurrentBoard  { get; private set; }

    public PlacementManager PlacementManager { get;private set; }
    public event Action<GameStage> OnStageChanged;

    public int Round { get; private set; } = 1;
    public int BoardsCount => boards.Length;
    public int CurrentBoardIndex { get; private set; } = 0;
    public GameStage Stage { get; private set; } = GameStage.Battle;

    private readonly List<int> _roundsToChangeBoard = new () {3, 6, 9};

    private GameObject[] _spawnPoint;

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
        
        //(PlacementManager = placementManager).Redactor.GetComponentInChildren<Button>().onClick.AddListener(OnPlacementCompleted);
        CurrentBoard.OnBattleCompleted += OnBattleCompleted;
        
    }

    //private void Start()
    //{
        //SetStage(GameStage.Battle);
    //}
    
    private void OnPlacementCompleted()
    {
        StartNextBattle();
    }

    private void OnBattleCompleted()
    {
        SetStage(GameStage.Placement);
        InputManager.Instance.DisableAllActions();
        SpaceInvaderController[] invaders = FindObjectsByType<SpaceInvaderController>(FindObjectsSortMode.None);
        foreach (var invader in invaders)
        {
            invader.SetPaused(true);
        }
        
        PowerUpSpawner[] spawners = FindObjectsByType<PowerUpSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            spawner.SetPaused(true);
        }
        
        GhostController[] ghosts = FindObjectsByType<GhostController>(FindObjectsSortMode.None);
        foreach (var ghost in ghosts)
        {
            ghost.SetPaused(true);
        }
        
    }

    public void StartNextBattle()
    {
       
        if (_roundsToChangeBoard.Contains(++Round))
            ChangeBoard();
        SetStage(GameStage.Battle);

        var i = 0;
        foreach (var spawnPoint in _spawnPoint)
        {
            var asset = placement.Obstacles[0];
            Instantiate(asset.ObstaclePrefab,  spawnPoint.transform.position, spawnPoint.transform.rotation);
            i++;
        }
        

        InputManager.Instance.EnableAllActions();
        SpaceInvaderController[] invaders = FindObjectsByType<SpaceInvaderController>(FindObjectsSortMode.None);
        foreach (var invader in invaders)
        {
            invader.SetPaused(false);
        }
        
        PowerUpSpawner[] spawners = FindObjectsByType<PowerUpSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            spawner.SetPaused(false);
        }
        
        GhostController[] ghosts = FindObjectsByType<GhostController>(FindObjectsSortMode.None);
        foreach (var ghost in ghosts)
        {
            ghost.SetPaused(false);
        }
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
        Debug.Log("newStage: " + newStage);
        Stage = newStage;
        OnStageChanged?.Invoke(Stage);
    }
}