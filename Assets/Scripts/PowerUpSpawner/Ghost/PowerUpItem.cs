using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class PowerUpItem : MonoBehaviour
{
    [SerializeField] private float buffDuration = 5f;

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball")) return;
        Debug.Log(collider.name);
        BallController ballState = collider.GetComponent<BallController>();
        if (ballState != null)
        {
            ballState.ActivatePowerUp(buffDuration);
        }
        Destroy(gameObject);
    }
}