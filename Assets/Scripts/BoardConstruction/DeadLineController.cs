using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class DeadLine : MonoBehaviour
{
    private BoardManager _boardManager;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        _boardManager = GetComponentInParent<BoardManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball")) return;

        if (_boardManager != null)
        {
            _boardManager.RegisterBallDestroyed();
        }

        Destroy(collider.gameObject);
    }
}