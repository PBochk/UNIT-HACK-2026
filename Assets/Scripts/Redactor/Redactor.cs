using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class Redactor : MonoBehaviour
{
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private List<ObstacleNodeUI> nodesUI;
    [SerializeField] private List<ObstaclePlace> places;
    [SerializeField] private Button deleteObstacle;
    private Dictionary<ObstaclePlace, ObstacleNodeUI> _placesToUI = new();
    private ObstaclePlace _currentPlace;
    
    private void Awake()
    {
        foreach (var place in places)
        {
            place.OnPlaceClicked.AddListener(HandlePlaceSelected);
        }

        foreach (var nodeUI in nodesUI)
        {
            nodeUI.OnNodeSelected.AddListener(HandleObstacleSelected);
        }

        deleteObstacle.onClick.AddListener(HandleRemoveSelected);
    }

    private void HandlePlaceSelected(ObstaclePlace place)
    {
        Debug.Log("Place selected");
        _currentPlace = place;
        
    }

    private void HandleObstacleSelected(ObstacleNodeUI nodeUI)
    {
        Debug.Log("Obstacle selected");
        // if (!_currentPlace || !nodeUI.IsEnough) return;
        if (!_currentPlace) return;
        else
        {
            _currentPlace.PlaceObstacle(nodeUI.Prefab);
            _placesToUI.Add(_currentPlace, nodeUI);
            nodeUI.DecreaseAmount();
        }
    }

    private void HandleRemoveSelected()
    {
        if (!_currentPlace || !_currentPlace.IsPlaced) return;
        _currentPlace.RemoveObstacle();
        _placesToUI[_currentPlace].DecreaseAmount();
        _placesToUI.Remove(_currentPlace);
    }
    
    private void ShowSelectionScreen()
    {
        selectionScreen.SetActive(true); //TODO: animations
    }
    private void HideSelectionScreen()
    {
        selectionScreen.SetActive(false);
    }
}
