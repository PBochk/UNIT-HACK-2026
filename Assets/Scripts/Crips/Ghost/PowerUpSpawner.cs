using UnityEngine;

public sealed class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private float spawnCooldown = 10f;

    private Transform[] _spawnPoints;
    private GameObject[] _activePowerUps;
    private float _timer;
    
    // Флаг паузы
    private bool _isPaused;

    private void Start()
    {
        int childCount = transform.childCount;
        _spawnPoints = new Transform[childCount];
        
        for (int i = 0; i < childCount; i++)
        {
            _spawnPoints[i] = transform.GetChild(i);
        }

        _activePowerUps = new GameObject[_spawnPoints.Length];
        _timer = spawnCooldown;
    }

    // --- ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ ПАУЗЫ ---
    public void SetPaused(bool isPaused)
    {
        _isPaused = isPaused;
    }

    private void Update()
    {
        // Если на паузе — таймер не тикает, спавн не происходит
        if (_isPaused) return;

        _timer -= Time.deltaTime;
        if (!(_timer <= 0f)) return;
        
        TrySpawn();
        _timer = spawnCooldown;
    }

    private void TrySpawn()
    {
        if (powerUpPrefab == null || _spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, _spawnPoints.Length);
        
        if (_activePowerUps[randomIndex] == null)
        {
            _activePowerUps[randomIndex] = Instantiate(powerUpPrefab, _spawnPoints[randomIndex].position, Quaternion.identity);
        }
    }
}