using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class StartLineController : MonoBehaviour
{
    private BoardManager _boardManager;
    private readonly HashSet<int> _crossedBallIds = new();

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        _boardManager = GetComponentInParent<BoardManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ball") || _boardManager == null) return;

        int ballId = collider.gameObject.GetInstanceID();

        if (_crossedBallIds.Add(ballId))
        {
            _boardManager.RegisterBallCrossedLine();
        }
    }
}