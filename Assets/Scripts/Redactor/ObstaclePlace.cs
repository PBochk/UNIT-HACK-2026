using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObstaclePlace : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image placeholder;
    public bool IsPlaced { get; private set; }
    private GameObject _obstacle;
    public UnityEvent<ObstaclePlace> OnPlaceClicked;

    private void Start()
    {
        button.onClick.AddListener(HandlePlaceClicked);
    }

    private void HandlePlaceClicked()
    {
        Debug.Log("Place clicked");
        OnPlaceClicked.Invoke(this);
    }
    
    public void PlaceObstacle(GameObject obstacle)
    {
        _obstacle = Instantiate(obstacle, transform.position, Quaternion.identity);
        placeholder.enabled = false;
        IsPlaced = true;
    }

    public void RemoveObstacle()
    {
        placeholder.enabled = true;
        Destroy(_obstacle);
        IsPlaced = false;
    }
}
