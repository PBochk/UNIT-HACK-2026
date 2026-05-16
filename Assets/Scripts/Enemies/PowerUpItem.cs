using UnityEngine;

public sealed class PowerUpItem : MonoBehaviour
{
    [SerializeField] private float buffDuration = 5f;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball")) return;
        BallController ballState = collider.GetComponent<BallController>();
        if (ballState != null)
        {
            ballState.ActivatePowerUp(buffDuration);
        }
        Destroy(gameObject);
    }
}