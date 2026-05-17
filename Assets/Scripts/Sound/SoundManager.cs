using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public float GeneralVolume { get; private set; } = 1f;
    public float SoundVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public UnityEvent OnGeneralVolumeChanged;
    public UnityEvent OnSoundVolumeChanged;
    public UnityEvent OnMusicVolumeChanged;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void SetGeneralVolume(float value)
    {
        GeneralVolume = Mathf.Clamp(value, 0f, 1f);
        OnGeneralVolumeChanged.Invoke();
    }

    public void SetSoundVolume(float value)
    {
        SoundVolume = Mathf.Clamp(value, 0f, 1f);
        OnSoundVolumeChanged.Invoke();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp(value, 0f, 1f);
        OnMusicVolumeChanged.Invoke();
    }
}
