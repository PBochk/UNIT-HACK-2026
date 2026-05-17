using System;
using UnityEngine;
using UnityEngine.Events;

public class BallFlipperColisionSound : SoundOutput
{
    public UnityEvent OnCollision;

    private void Awake()
    {
        Bind(OnCollision);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Flipper"))
        {
            OnCollision.Invoke();            
        }
    }
}
