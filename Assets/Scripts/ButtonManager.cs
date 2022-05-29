using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    [SerializeField] AudioClip clickSFX;

    SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = SceneLoader.GetThisSingletonScript();
    }

    void PlayClickSFX()
    {
        AudioSource.PlayClipAtPoint(clickSFX, Camera.main.transform.position, 0.1f);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(ButtonCooldown());
        PlayClickSFX();
        sceneLoader.LoadNextLevel();
    }

    public void LoadMainMenuScene()
    {
        StartCoroutine(ButtonCooldown());
        PlayClickSFX();
        sceneLoader.LoadMainMenuScene();
    }
    public void LoadTheGameScene()
    {
        StartCoroutine(ButtonCooldown());
        PlayClickSFX();
        sceneLoader.LoadTheGameScene();
    }

    public void LoadHowToPlayScene()
    {
        StartCoroutine(ButtonCooldown());
        PlayClickSFX();
        sceneLoader.LoadHowToPlayScene();
    }

    public void QuitGame()
    {
        //StartCoroutine(ButtonCooldown());
        PlayClickSFX();
        Application.Quit();
    }

    IEnumerator ButtonCooldown()
    {
        var buttonWhoTrigger = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        buttonWhoTrigger.enabled = false;

        yield return new WaitForSeconds(1f);
        buttonWhoTrigger.enabled = true;
    }


}
