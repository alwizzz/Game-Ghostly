using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] AudioClip clickSFX;

    MusicManager musicManager;
    SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = SceneLoader.GetThisSingletonScript();
        musicManager = MusicManager.GetThisSingletonScript();

    }
    
    void PlayClickSFX()
    {
        AudioSource.PlayClipAtPoint(clickSFX, Camera.main.transform.position, 0.1f);
    }

    public void LoadNextLevel() 
    {
        PlayClickSFX();
        sceneLoader.LoadNextLevel(); 
    }

    public void LoadMainMenuScene() 
    { 
        PlayClickSFX();
        sceneLoader.LoadMainMenuScene();
    }
    public void LoadTheGameScene() 
    { 
        PlayClickSFX();
        sceneLoader.LoadTheGameScene(); 
    }

    public void LoadHowToPlayScene()
    {
        PlayClickSFX();
        sceneLoader.LoadHowToPlayScene();
    }

    public void QuitGame()
    {
        PlayClickSFX();
        Application.Quit();
    }


}
