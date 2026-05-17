using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlacementScreenManager : MonoBehaviour
{
    
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button GoToTreeButton;

    public event Action OnGoToTree;
    public event Action OnPlay;

    public void Awake()
    {
        PlayButton.onClick.AddListener(() => OnPlay?.Invoke());
        GoToTreeButton.onClick.AddListener(() => OnGoToTree?.Invoke());
    }
}