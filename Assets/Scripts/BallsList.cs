using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class BallsList : MonoBehaviour
{
    [System.Serializable]
    public struct UnusualBallData
    {
        public BallController prefab;
        public Stat dropChanceStat;
    }

    [SerializeField] private BallController regularBallPrefab;
    [SerializeField] private List<UnusualBallData> unusualBalls = new();

    public BallController GetRandomBallPrefab()
    {
        if (regularBallPrefab == null) return null;

        float unusualSum = 
            unusualBalls.Where(ballData => ballData.dropChanceStat 
                != null && ballData.prefab != null).Sum(ballData => ballData.dropChanceStat.Value);

        float regularBallChance = 100f - unusualSum;
        float roll = Random.Range(0f, 100f);

        if (roll < regularBallChance)
        {
            return regularBallPrefab;
        }

        float currentThreshold = regularBallChance;
        foreach (UnusualBallData ballData in 
                 unusualBalls.Where(ballData => ballData.dropChanceStat != null && ballData.prefab != null))
        {
            currentThreshold += ballData.dropChanceStat.Value;
            if (roll <= currentThreshold)
            {
                return ballData.prefab;
            }
        }

        return regularBallPrefab;
    }
}