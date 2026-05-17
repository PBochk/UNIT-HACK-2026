using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : SoundOutput
{
    [SerializeField] private Button button;

    private void Start()
    {
        Bind(button.onClick);
    }
}
