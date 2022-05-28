using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip mainMenuTrack;
    [SerializeField] AudioClip playingTrack;
    [SerializeField] AudioClip playingIntenseTrack;
    [SerializeField] AudioClip gameOverTrack;


    AudioSource thisAudioSource;
    SceneLoader sceneLoader;
    private void Awake()
    {
        Singleton();
        thisAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        sceneLoader = SceneLoader.GetThisSingletonScript();
        PlayCurrentSceneTrack();
    }

    void Singleton()
    {
        var thisScriptCount = FindObjectsOfType(GetType()).Length;
        if (thisScriptCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public static MusicManager GetThisSingletonScript()
    {
        var list = FindObjectsOfType<MusicManager>();
        return list[list.Length - 1];
    }

    public void PlayMainMenuTrack() {
        thisAudioSource.clip = mainMenuTrack;
        thisAudioSource.Play();
    }

    public void PlayPlayingTrack()
    {
        if(thisAudioSource.clip == playingTrack) { return; } //do nothing if already playing the track
        thisAudioSource.clip = playingTrack;
        thisAudioSource.Play();
    }

    public void PlayPlayingIntenseTrack()
    {
        thisAudioSource.clip = playingIntenseTrack;
        thisAudioSource.Play();
    }

    public void PlayGameOverTrack()
    {
        thisAudioSource.clip = gameOverTrack;
        thisAudioSource.Play();
    }

    void PlayCurrentSceneTrack()
    {
        if (sceneLoader.IsInMainMenuScene()) { PlayMainMenuTrack(); }
        else if (sceneLoader.IsInTheGameScene()) { PlayPlayingTrack(); }
        else if (sceneLoader.IsInGameOverScene()) { PlayGameOverTrack(); }
    }
}
