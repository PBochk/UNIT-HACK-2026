using UnityEngine;
using UnityEngine.Events;

public class OnCollisionSound : SoundOutput
{
    public UnityEvent OnCollision;
    private void Awake()
    {
        Bind(OnCollision);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnCollision.Invoke();
        Debug.Log("OnCollision Enter");
    }
    
}
