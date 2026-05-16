using System.Collections.Generic;
using UnityEngine;

public sealed class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float ballSpawnOffsetY = 1.5f;
    [SerializeField] private int totalBalls = 3;
    [SerializeField] private float ballSpawnCooldown = 3f;

    private PlungerController _plunger;
    private int _ballsLeftToSpawn;
    private float _cooldownTimer;
    private bool _isCooldownActive;

    public IReadOnlyList<GameObject> Obstacles { get; private set; }

    private void Awake()
    {
        int childCount = transform.childCount;
        List<GameObject> obstaclesList = new(childCount);

        for (var i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.TryGetComponent(out PlungerController plungerComponent))
            {
                _plunger = plungerComponent;
            }
            else
            {
                obstaclesList.Add(child.gameObject);
            }
        }

        Obstacles = obstaclesList;
    }
    
    private void Start() => StartGame();

    private void Update()
    {
        if (!_isCooldownActive) return;

        _cooldownTimer -= Time.deltaTime;
        if (!(_cooldownTimer <= 0f)) return;
        _isCooldownActive = false;
        SpawnNextBall();
    }

    public void StartGame()
    {
        _ballsLeftToSpawn = totalBalls;
        _isCooldownActive = false;
        _cooldownTimer = 0f;
        
        SpawnNextBall();
    }

    public void RegisterBallCrossedLine()
    {
        if (_ballsLeftToSpawn <= 0 || _isCooldownActive) return;
        _cooldownTimer = ballSpawnCooldown;
        _isCooldownActive = true;
    }

    private void SpawnNextBall()
    {
        if (_ballsLeftToSpawn <= 0 || ballPrefab == null || _plunger == null) return;

        Vector3 spawnPosition = _plunger.transform.position + Vector3.up * ballSpawnOffsetY;
        Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        _ballsLeftToSpawn--;
    }
}