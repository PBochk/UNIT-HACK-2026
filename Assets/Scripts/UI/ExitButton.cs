using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    private void Awake()
    {
        var b = GetComponent<Button>();
        b.onClick.AddListener(Application.Quit);
    }
}
