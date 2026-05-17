using UnityEngine;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{
    [SerializeField] private Slider general;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sound;
    [SerializeField] private Image screen;
    [SerializeField] private Button button;
    private void Start()
    {
        var manager = SoundManager.Instance;
        general.onValueChanged.AddListener(manager.SetGeneralVolume);
        sound.onValueChanged.AddListener(manager.SetSoundVolume);
        music.onValueChanged.AddListener(manager.SetMusicVolume);
        button.onClick.AddListener(SwitchVisibility);
    }

    private void SwitchVisibility()
    {
        screen.gameObject.SetActive(!screen.gameObject.activeSelf);
    }
}
