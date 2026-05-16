using System;
using UnityEngine;

public sealed class PlacementManager : MonoBehaviour
{
    [field:SerializeField] public Redactor Redactor { get; private set; }
    [field:SerializeField] public UpgradeTree UpgradeTree { get; private set; }

    public event Action OnCompleted;
}
