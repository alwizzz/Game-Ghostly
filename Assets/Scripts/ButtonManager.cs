using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public SceneLoader sceneLoader;

    private void Start()
    {
        var sceneLoaders = FindObjectsOfType<SceneLoader>();
        sceneLoader = sceneLoaders[1]; // quick solution to get singleton object on DontDestroyOnLoad Scene
    }

    public void LoadNextLevel() { sceneLoader.LoadNextLevel(); }

}
