using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonSoubd : SoundOutput
{
    public UnityEvent OnClick;
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => OnClick?.Invoke());
        Bind(OnClick);
    }
}
