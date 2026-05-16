using UnityEngine;

public sealed class BumperController : MonoBehaviour
{
    [Header("Bounciness Settings")]
    [SerializeField] private float bounceForce = 15f;
    
    [Range(0f, 360f)] 
    [SerializeField] private float bounceAngle = 90f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            Vector2 launchDirection = GetVectorFromAngle(bounceAngle);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(launchDirection * bounceForce, ForceMode2D.Impulse);
        }
    }

    private Vector2 GetVectorFromAngle(float angleInDegrees)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var direction = (Vector3)GetVectorFromAngle(bounceAngle);
        Vector3 startPosition = transform.position;
        
        Vector3 endPosition = startPosition + direction * (bounceForce * 0.1f);
        Gizmos.DrawLine(startPosition, endPosition);
        
        Gizmos.DrawWireSphere(endPosition, 0.1f);
    }
}