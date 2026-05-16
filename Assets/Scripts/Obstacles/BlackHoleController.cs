using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public sealed class BlackHoleController : MonoBehaviour
{
    [SerializeField] private float suctionForce = 30f;
    [SerializeField] private float entryImpulse = 15f;
    [SerializeField] private float redirectStrength = 1f;
    [SerializeField] private float suctionDuration = 1.5f;
    [SerializeField] private float cooldownDuration = 5f;

    private float _activeTimer;
    private float _cooldownTimer;

    private void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (_activeTimer > 0f)
        {
            _activeTimer -= Time.deltaTime;
            if (_activeTimer <= 0f)
            {
                _cooldownTimer = cooldownDuration;
            }
        }
        else if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_cooldownTimer > 0f || _activeTimer > 0f) return;
        if (!collider.CompareTag("Ball")) return;

        Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        Vector2 directionToCenter = ((Vector2)transform.position - ballRb.position).normalized;
        float currentSpeed = ballRb.linearVelocity.magnitude;
        Vector2 newDirection = Vector2.Lerp(ballRb.linearVelocity.normalized, directionToCenter, redirectStrength).normalized;
            
        ballRb.linearVelocity = newDirection * currentSpeed;
        ballRb.AddForce(directionToCenter * entryImpulse, ForceMode2D.Impulse);

        _activeTimer = suctionDuration;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (_activeTimer <= 0f) return;
        if (!collider.CompareTag("Ball")) return;

        Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();
        if (ballRb != null)
        {
            Vector2 directionToCenter = ((Vector2)transform.position - ballRb.position).normalized;
            ballRb.AddForce(directionToCenter * suctionForce * ballRb.mass, ForceMode2D.Force);
        }
    }
}