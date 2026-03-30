using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource systemSource;
    private List<AudioSource> activeSources;
    public static AudioManager Instance;
    
    private void Awake()
    {
        if ((Instance == null))
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
            systemSource = gameObject.GetComponent<AudioSource>();
            activeSources = new List<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //Funções de gerenciamento de audio podem ser adicionadas aqui, como tocar sons, ajustar volumes, etc.
    public void PlaySound(AudioClip clip)
    {
       
        systemSource.Stop();
        systemSource.clip = clip;
        systemSource.Play();
    }

    public void StopSound()
    {
        systemSource.Stop();
    }

    public void PauseSound()
    {
        systemSource.Pause();
    }

    public void ResumeSound()
    {
        systemSource.UnPause();
    }

    public void PlayOneShot(AudioClip clip)
    {
       systemSource.PlayOneShot(clip);
    }
    //Funçoes de gerencamento de audio 3d

    public void PlaySound(AudioClip clip, AudioSource source)
    {
        if(!activeSources.Contains(source)) activeSources.Add(source);
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void StopSound(AudioSource source)
    {
        source.Stop();
        activeSources.Remove(source);
    }

    public void PauseSound(AudioSource source)
    {
        source.Pause();
    }

    public void ResumeSound(AudioSource source)
    {
        source.UnPause();
        
    }

    public void PlayOneShot(AudioClip clip, AudioSource source)
    {
        source.PlayOneShot(clip);
    }
}



