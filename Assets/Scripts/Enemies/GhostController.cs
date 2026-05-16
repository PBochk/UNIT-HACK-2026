using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class GhostController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float shiftDownDistance = 1f;
    [SerializeField] private float leftBound = -5f;
    [SerializeField] private float rightBound = 5f;

    private int _moveDirection = 1;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        MoveSnake();
    }

    private void MoveSnake()
    {
        transform.Translate(Vector3.right * _moveDirection * moveSpeed * Time.deltaTime);

        if (_moveDirection == 1 && transform.position.x >= rightBound 
            || _moveDirection == -1 && transform.position.x <= leftBound)
        {
            ShiftDownAndReverse();
        }
    }

    private void ShiftDownAndReverse()
    {
        transform.position += Vector3.down * shiftDownDistance;
        _moveDirection *= -1;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball")) return;
        BallController ballState = collider.GetComponent<BallController>();
        Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();

        if (ballState != null && ballState.IsPoweredUp)
        {
            Destroy(gameObject);
        }
        else if (ballRb != null)
        {
            ballRb.linearVelocity *= 0.5f;
        }
    }
}