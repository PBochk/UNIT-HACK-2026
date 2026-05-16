using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "Scriptable Objects/Stat")]
public class Stat : ScriptableObject
{
    public string Id;
    //This one is in russian
    public string DisplayName;
    public string DisplayDescription;
    public float Value;
}