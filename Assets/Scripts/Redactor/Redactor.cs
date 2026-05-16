using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class Redactor : MonoBehaviour
{
    [SerializeField] private GameObject SelectionScreen;
    [SerializeField] private List<ObstacleNodeUI> nodesUI;
    [SerializeField] private List<ObstaclePlace> places;
    // [SerializeField] private SerializedDictionary<ObstacleType, ObstacleNodeUI> obstacles;
    [SerializeField] private Button deleteObstacle;
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
        // if (place.Unlocked.Value != 1 || (int)place.Type < place.Currency.Value) return;
        // place.PlaceObstacle(obstacles[place.Type]);
    }

    private void HandleObstacleSelected(ObstacleNodeUI nodeUI)
    {
        Debug.Log("Obstacle selected");
        // if (!_currentPlace || !nodeUI.IsEnough) return;
        if (!_currentPlace) return;
        else
        {
            _currentPlace.PlaceObstacle(nodeUI.Prefab);
            nodeUI.SpendCurrency();
        }
    }

    private void HandleRemoveSelected()
    {
        if (!_currentPlace || !_currentPlace.IsPlaced) return;
        _currentPlace.RemoveObstacle();
    }
    
    private void ShowSelectionScreen()
    {
        SelectionScreen.SetActive(true); //TODO: animations
    }
    private void HideSelectionScreen()
    {
        SelectionScreen.SetActive(false);
    }
}
