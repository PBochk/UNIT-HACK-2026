using UnityEngine;

public sealed class ProjectileController : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float pushForce = 15f;
    [SerializeField] private float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collider.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = new Vector2(ballRb.linearVelocity.x, 0f);
                ballRb.AddForce(Vector2.down * pushForce, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
        }
        else if (collider.CompareTag("Wall"))
        {
            Debug.LogError(gameObject.name);
            Destroy(gameObject);
        }
    }
}