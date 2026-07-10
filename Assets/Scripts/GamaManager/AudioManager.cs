using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    public AudioSource bgmSource;
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bgmSource.loop = true;
        StartBgm();
    }

    public void StartBgm()
    {
        bgmSource.Play();
    }

    public void PauseBgm()
    {
        bgmSource.Pause();
    }
    public void UnPauseBgm()
    {
        bgmSource.UnPause();
    }
    public void StopBgm()
    {
        bgmSource.Stop();
    }

}
