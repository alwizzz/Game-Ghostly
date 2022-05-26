using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public LevelMaster levelMaster;

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        levelMaster = FindObjectOfType<LevelMaster>();
    }

    void Singleton()
    {
        var thisScriptCount = FindObjectsOfType<LevelMaster>().Length;
        if (thisScriptCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    public void LoadTransitionScene() { SceneManager.LoadScene("Transition"); }
    public void LoadGameOverScene() { SceneManager.LoadScene("GameOver"); }


    public void LoadNextLevel()
    {
        levelMaster.LevelUp();
        levelMaster.StartGame();
        SceneManager.LoadScene("TheGame");
    }

    public bool IsInTransitionScene()
    {
        var thisSceneName = SceneManager.GetActiveScene().name;
        return (thisSceneName == "Transition") ? true : false;
    }
}
