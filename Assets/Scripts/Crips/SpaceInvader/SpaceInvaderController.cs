using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class SpaceInvaderController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask wallLayer; // Укажите слой, на котором находятся стены!
    [SerializeField] private float wallOffset = 0.1f; // Небольшой зазор, чтобы не тереться о стену вплотную

    [Header("Combat Settings")]
    [SerializeField] private float ballPushForce = 12f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 2f;

    private float _shootTimer;
    private int _moveDirection = 1;
    
    private float _leftBound;
    private float _rightBound;

    private void Start()
    {
        _shootTimer = shootCooldown;
        DetectBounds();
    }

    private void DetectBounds()
    {
        Collider2D col = GetComponent<Collider2D>();
        // Получаем половину ширины коллайдера самого врага
        float extentsX = col != null ? col.bounds.extents.x : 0.5f;

        // Ищем стену слева
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 100f, wallLayer);
        if (hitLeft.collider != null)
        {
            _leftBound = hitLeft.point.x + extentsX + wallOffset;
        }
        else
        {
            _leftBound = transform.position.x - 5f; // Фолбэк, если стены нет
        }

        // Ищем стену справа
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 100f, wallLayer);
        if (hitRight.collider != null)
        {
            _rightBound = hitRight.point.x - extentsX - wallOffset;
        }
        else
        {
            _rightBound = transform.position.x + 5f; // Фолбэк, если стены нет
        }
    }

    private void FixedUpdate()
    {
        Move();
        HandleShooting();
    }

    private void Move()
    {
        transform.Translate(Vector3.right * _moveDirection * moveSpeed * Time.deltaTime);

        // Мягко разворачиваемся при достижении рассчитанных границ
        if (_moveDirection == 1 && transform.position.x >= _rightBound)
        {
            transform.position = new Vector3(_rightBound, transform.position.y, transform.position.z);
            _moveDirection = -1;
        }
        else if (_moveDirection == -1 && transform.position.x <= _leftBound)
        {
            transform.position = new Vector3(_leftBound, transform.position.y, transform.position.z);
            _moveDirection = 1;
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
        // Запасной физический отскок (если мяч сдвинет врага в стену)
        else if (collision.gameObject.CompareTag("Wall"))
        {
            _moveDirection *= -1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        float leftX = transform.position.x - 1f;
        float rightX = transform.position.x + 1f;

        // Отрисовка границ прямо в редакторе (до запуска игры)
        if (Application.isPlaying)
        {
            leftX = _leftBound;
            rightX = _rightBound;
        }
        else
        {
            Collider2D col = GetComponent<Collider2D>();
            float extentsX = col != null ? col.bounds.extents.x : 0.5f;

            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 100f, wallLayer);
            if (hitLeft.collider != null) leftX = hitLeft.point.x + extentsX + wallOffset;

            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 100f, wallLayer);
            if (hitRight.collider != null) rightX = hitRight.point.x - extentsX - wallOffset;
        }

        float y = transform.position.y;
        Gizmos.DrawLine(new Vector3(leftX, y, 0), new Vector3(rightX, y, 0));

        float markSize = 0.2f;
        Gizmos.DrawLine(new Vector3(leftX, y - markSize, 0), new Vector3(leftX, y + markSize, 0));
        Gizmos.DrawLine(new Vector3(rightX, y - markSize, 0), new Vector3(rightX, y + markSize, 0));
    }
}