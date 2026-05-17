using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleAsset", menuName = "Scriptable Objects/ObstacleAsset")]
public class ObstacleAsset : ScriptableObject
{
    public string Id;
    public Sprite Sprite;
    public string DisplayName;
    public string DisplayDescription;
    public GameObject ObstaclePrefab;
}