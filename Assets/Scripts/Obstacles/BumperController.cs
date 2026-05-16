using UnityEngine;

public sealed class BumperController : MonoBehaviour
{
    [SerializeField] private float bounceForce = 15f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        Vector2 direction = collision.transform.position - transform.position;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * bounceForce, ForceMode2D.Impulse);
        
    }
}