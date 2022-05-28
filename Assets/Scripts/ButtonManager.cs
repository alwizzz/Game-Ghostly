using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = SceneLoader.GetThisSingletonScript();
    }

    public void LoadNextLevel() { sceneLoader.LoadNextLevel(); }

    public void LoadMainMenuScene() { sceneLoader.LoadMainMenuScene(); }
    public void LoadTheGameScene() { sceneLoader.LoadTheGameScene(); }


}
