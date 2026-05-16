using UnityEngine;
using System.Collections;

public sealed class BallController : MonoBehaviour
{
    public bool IsPoweredUp { get; private set; }

    public void ActivatePowerUp(float duration)
    {
        StartCoroutine(PowerUpRoutine(duration));
    }

    private IEnumerator PowerUpRoutine(float duration)
    {
        IsPoweredUp = true;
        yield return new WaitForSeconds(duration);
        IsPoweredUp = false;
    }
}