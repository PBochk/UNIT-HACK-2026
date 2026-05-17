using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DragPreview : MonoBehaviour
{
    
    [SerializeField]
    Image _image;

    [SerializeField] public Vector3 _offset = new Vector3(0f, -25f, 0f);

    public void Initialize(Sprite sprite)
    {
        _image.sprite = sprite;
    }
    
    private void Update()
    {
        //if (Mouse.current == null) return;
        //Vector2 mousePosition = Mouse.current.position.ReadValue();
        //transform.position = (Vector3)mousePosition + _offset;
    }
}