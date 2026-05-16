using UnityEngine;

public sealed class CreepController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float ballPushForce = 12f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 2f;

    private Vector3 _startPosition;
    private float _shootTimer;
    private int _moveDirection = 1;

    private void Start()
    {
        _startPosition = transform.position;
        _shootTimer = shootCooldown;
    }

    private void Update()
    {
        Move();
        HandleShooting();
    }

    private void Move()
    {
        transform.Translate(Vector3.right * _moveDirection * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - _startPosition.x) >= moveDistance)
        {
            _moveDirection *= -1;
        }
    }

    private void HandleShooting()
    {
        _shootTimer -= Time.deltaTime;
        if (!(_shootTimer <= 0f)) return;
        Shoot();
        _shootTimer = shootCooldown;
    }

    private void Shoot()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                ballRb.linearVelocity = Vector2.zero;
                ballRb.AddForce(pushDirection * ballPushForce, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            _moveDirection *= -1;
        }
    }
}