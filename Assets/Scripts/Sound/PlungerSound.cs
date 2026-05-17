using UnityEngine;
using UnityEngine.Events;

public class PlungerSound : SoundOutput
{
    [SerializeField] private PlungerController plunger;
    private void Awake()
    {
        Bind(plunger.OnLaunch);
    }
}
