using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public sealed class BlackHoleController : MonoBehaviour
{
    [SerializeField] private float suctionForce = 30f;
    [SerializeField] private float entryImpulse = 15f;
    [SerializeField] private float redirectStrength = 1f;

    private void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball")) return;

        Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();
        if (ballRb != null)
        {
            Vector2 directionToCenter = ((Vector2)transform.position - ballRb.position).normalized;
            float currentSpeed = ballRb.linearVelocity.magnitude;
            Vector2 newDirection = Vector2.Lerp(ballRb.linearVelocity.normalized, directionToCenter, redirectStrength).normalized;
            
            ballRb.linearVelocity = newDirection * currentSpeed;
            ballRb.AddForce(directionToCenter * entryImpulse, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball")) return;

        Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();
        if (ballRb != null)
        {
            Vector2 directionToCenter = ((Vector2)transform.position - ballRb.position).normalized;
            ballRb.AddForce(directionToCenter * suctionForce * ballRb.mass, ForceMode2D.Force);
        }
    }
}