using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip menuMusic;
    public AudioClip levelMusic;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        PlayMusicBasedOnScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicBasedOnScene(scene.name);
    }

    private void PlayMusicBasedOnScene(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = levelMusic;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}