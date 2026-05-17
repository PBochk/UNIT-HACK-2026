using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class SpaceInvaderController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.15f;

    [Header("Combat Settings")]
    [SerializeField] private float ballPushForce = 12f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 2f;

    private float _shootTimer;
    private int _moveDirection = 1;

    private Collider2D _collider;

    private void Start()
    {
        _shootTimer = shootCooldown;
        _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        Move();
        HandleShooting();
    }

    private void Move()
    {
        if (_collider == null)
            return;

        Bounds bounds = _collider.bounds;

        Vector2 rayOrigin = _moveDirection == 1
            ? new Vector2(bounds.max.x, bounds.center.y)
            : new Vector2(bounds.min.x, bounds.center.y);

        Vector2 direction = Vector2.right * _moveDirection;

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            direction,
            wallCheckDistance,
            wallLayer);

        // Стена впереди — разворачиваемся
        if (hit.collider != null)
        {
            _moveDirection *= -1;
            return;
        }

        transform.Translate(
            Vector3.right * (_moveDirection * moveSpeed * Time.fixedDeltaTime),
            Space.World);
    }

    private void HandleShooting()
    {
        _shootTimer -= Time.deltaTime;

        if (_shootTimer > 0f)
            return;

        Shoot();

        _shootTimer = shootCooldown;
    }

    private void Shoot()
    {
        if (projectilePrefab == null || shootPoint == null)
            return;

        Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ballRb =
                collision.gameObject.GetComponent<Rigidbody2D>();

            if (ballRb != null)
            {
                Vector2 pushDirection =
                    (collision.transform.position - transform.position).normalized;

                ballRb.linearVelocity = Vector2.zero;

                ballRb.AddForce(
                    pushDirection * ballPushForce,
                    ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
        else if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            // Фолбэк если всё-таки коснулся стены
            _moveDirection *= -1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col == null)
            return;

        Bounds bounds = col.bounds;

        Vector2 rayOrigin = _moveDirection == 1
            ? new Vector2(bounds.max.x, bounds.center.y)
            : new Vector2(bounds.min.x, bounds.center.y);

        Vector2 direction = Vector2.right * _moveDirection;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            rayOrigin,
            rayOrigin + direction * wallCheckDistance);
    }
}