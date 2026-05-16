using UnityEngine;

public class ProjectileController : MonoBehaviour
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
        if (!collider.CompareTag("Ball")) return;
        
        Rigidbody2D ballRigidbody = collider.GetComponent<Rigidbody2D>();
        if (ballRigidbody != null)
        {
            ballRigidbody.linearVelocity = new Vector2(ballRigidbody.linearVelocity.x, 0f);
            ballRigidbody.AddForce(Vector2.down * pushForce, ForceMode2D.Impulse);
        }
            
        Destroy(gameObject);
    }
}