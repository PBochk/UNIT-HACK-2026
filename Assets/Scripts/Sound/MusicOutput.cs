using UnityEngine;

public class MusicOutput : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("Sound manager not found");
            return;
        }
            
        SoundManager.Instance.OnGeneralVolumeChanged.AddListener(HandleVolumeChanged);
        SoundManager.Instance.OnMusicVolumeChanged.AddListener(HandleVolumeChanged);
    }

    private void HandleVolumeChanged()
    {
        audioSource.volume = SoundManager.Instance.GeneralVolume * SoundManager.Instance.MusicVolume;
    }
}
