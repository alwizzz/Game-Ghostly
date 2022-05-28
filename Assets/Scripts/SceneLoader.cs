using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    LevelMaster levelMaster;
    MusicManager musicManager;

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        levelMaster = LevelMaster.GetThisSingletonScript();
        musicManager = FindObjectOfType<MusicManager>();
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

    public static SceneLoader GetThisSingletonScript()
    {
        var list = FindObjectsOfType<SceneLoader>() ;
        return list[list.Length - 1];
    }

    // change SceneLoader with the class name as they both cant use GetType() method

    public void LoadTransitionScene() 
    {
        musicManager.PlayPlayingTrack();        
        SceneManager.LoadScene("Transition"); 
    }
    public void LoadGameOverScene() 
    { 
        musicManager.PlayGameOverTrack();
        SceneManager.LoadScene("GameOver");
    }
    public void LoadMainMenuScene() 
    {
        levelMaster.RestartGame();
        musicManager.PlayMainMenuTrack();
        SceneManager.LoadScene("MainMenu"); 
    }
    public void LoadTheGameScene() { 
        levelMaster.StartGame();
        levelMaster.ResetIntensity();
        musicManager.PlayPlayingTrack();
        SceneManager.LoadScene("TheGame"); 
    }




    public void LoadNextLevel()
    {
        levelMaster.LevelUp();
        LoadTheGameScene();
    }


    public bool IsInTransitionScene()
    {
        var thisSceneName = SceneManager.GetActiveScene().name;
        return (thisSceneName == "Transition") ? true : false;
    }

    public bool IsInTheGameScene()
    {
        var thisSceneName = SceneManager.GetActiveScene().name;
        return (thisSceneName == "TheGame") ? true : false;
    }

    public bool IsInGameOverScene()
    {
        var thisSceneName = SceneManager.GetActiveScene().name;
        return (thisSceneName == "GameOver") ? true : false;
    }

    public bool IsInMainMenuScene()
    {
        var thisSceneName = SceneManager.GetActiveScene().name;
        return (thisSceneName == "MainMenu") ? true : false;
    }
}
