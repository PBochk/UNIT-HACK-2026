using System;
using UnityEngine;

public sealed class PlacementManager : MonoBehaviour
{
    [field:SerializeField] public GameObject Redactor { get; private set; }
    [field:SerializeField] public GameObject UpgradeTree { get; private set; }

    public event Action OnCompleted;
}
