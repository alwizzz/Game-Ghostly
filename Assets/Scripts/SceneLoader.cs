using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    LevelMaster levelMaster;
    MusicManager musicManager;
    delegate void MusicToPlay();

    Animator animator;

    private void Awake()
    {
        Singleton();
        animator = GetComponent<Animator>();
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

    IEnumerator TransitionAndLoad(string sceneName, MusicToPlay method)
    {
        animator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(transitionDuration);

        method();
        SceneManager.LoadScene(sceneName);
        animator.SetTrigger("FadeOut");
    }

    public void LoadTransitionScene() 
    {
        StartCoroutine(TransitionAndLoad("Transition", musicManager.PlayPlayingTrack));
        //musicManager.PlayPlayingTrack();
    }
    public void LoadGameOverScene() 
    { 
        StartCoroutine(TransitionAndLoad("GameOver", musicManager.PlayGameOverTrack));
        //musicManager.PlayGameOverTrack();

    }
    public void LoadMainMenuScene() 
    {
        levelMaster.RestartGame();
        StartCoroutine(TransitionAndLoad("MainMenu", musicManager.PlayMainMenuTrack));
        //musicManager.PlayMainMenuTrack();

    }
    public void LoadTheGameScene() { 
        levelMaster.StartGame();
        levelMaster.ResetIntensity();
        StartCoroutine(TransitionAndLoad("TheGame", musicManager.PlayPlayingTrack));
        //musicManager.PlayPlayingTrack();

    }

    public void LoadHowToPlayScene()
    {
        StartCoroutine(TransitionAndLoad("HowToPlay", musicManager.PlayMainMenuTrack));
        //musicManager.PlayMainMenuTrack();
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

    public bool IsInHowToPlayScene()
    {
        var thisSceneName = SceneManager.GetActiveScene().name;
        return (thisSceneName == "HowToPlay") ? true : false;
    }
}
