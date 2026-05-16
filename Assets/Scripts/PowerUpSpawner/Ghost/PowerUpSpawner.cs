using UnityEngine;

public sealed class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private float spawnCooldown = 10f;

    private Transform[] _spawnPoints;
    private GameObject[] _activePowerUps;
    private float _timer;

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

    private void Update()
    {
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