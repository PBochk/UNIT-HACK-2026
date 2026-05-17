using System;
using UnityEngine;
using UnityEngine.Events;

public class SoundOutput : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    
    private void Start()
    {
        SoundManager.Instance.OnGeneralVolumeChanged.AddListener(HandleVolumeChanged);
        SoundManager.Instance.OnSoundVolumeChanged.AddListener(HandleVolumeChanged);
    }

    private void HandleVolumeChanged()
    {
        audioSource.volume = SoundManager.Instance.GeneralVolume * SoundManager.Instance.SoundVolume;
    }
    
    public void Bind(UnityEvent e)
    {
        e.AddListener(PlaySound);
    }

    public void Bind(Action a)
    {
        a += PlaySound;
    }
    
    private void PlaySound()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
