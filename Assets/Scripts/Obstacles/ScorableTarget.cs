using System;
using System.Collections;
using UnityEngine;

public sealed class ScorableTarget : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private Currency scoreValue;
    [SerializeField] private int scores;

    [Header("Timer Settings")]
    [SerializeField] private float cooldownDuration = 0.5f;

    public int Scores => scores;
    public event Action<Currency> OnScoreAwarded;

    private bool _canTrigger = true;
    private Coroutine _cooldownRoutine;

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
        scoreValue.Amount += scores;
        Debug.Log($"Очки начислены! Текущий баланс: {scoreValue.Amount}");
        OnScoreAwarded?.Invoke(scoreValue);

        if (_cooldownRoutine != null)
        {
            StopCoroutine(_cooldownRoutine);
        }
        
        _cooldownRoutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        // Просто ждем завершения времени кулдауна без изменения визуала
        yield return new WaitForSeconds(cooldownDuration);
        
        _canTrigger = true;
        _cooldownRoutine = null;
    }
}