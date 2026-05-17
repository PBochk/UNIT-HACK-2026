using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreenManager : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button GoToPlace;

    public event Action OnGoToPlace;
    public event Action OnPlay;

    public void Awake()
    {
        PlayButton.onClick.AddListener(() => OnPlay?.Invoke());
        GoToPlace.onClick.AddListener(() => OnGoToPlace?.Invoke());
    }
}