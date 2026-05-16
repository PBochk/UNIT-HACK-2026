using System;
using System.Collections;
using UnityEngine;

public sealed class ScorableTarget : MonoBehaviour
{
    [SerializeField] private Currency scoreValue;
    [SerializeField] private float cooldownDuration = 2f;
    [SerializeField] private int scores;

    public event Action<Currency> OnScoreAwarded;

    private bool _canTrigger = true;
    private Color _activeColor;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;
        if (_canTrigger)
        {
            TriggerTarget();
        }
    }

    private void TriggerTarget()
    {
        _canTrigger = false;
        scoreValue.Amount +=  scores;
        Debug.Log(scoreValue.Amount);
        OnScoreAwarded?.Invoke(scoreValue);

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownDuration);
        
        _canTrigger = true;
    }
}