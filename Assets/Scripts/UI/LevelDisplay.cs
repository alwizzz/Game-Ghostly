using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelDisplay : MonoBehaviour
{
    [SerializeField] float lingerDuration = 2f;
    [SerializeField] float fadeOutDuration = 2f;
    float fadeOutInterval = 0.1f;
    float fadeOutSpeed;

    Text thisText;
    Color currentColor;
    LevelMaster levelMaster;

    private void Start()
    {
        levelMaster = FindObjectOfType<LevelMaster>();
        thisText = GetComponent<Text>();
        currentColor = thisText.color;       
        fadeOutSpeed = fadeOutDuration / fadeOutInterval;


        thisText.text = "Level " + levelMaster.currentLevel.ToString();
        StartCoroutine(LingerThenFadeOut());
    }

    IEnumerator LingerThenFadeOut()
    {
        yield return new WaitForSeconds(lingerDuration);

        for (float t = 0.01f; t < fadeOutDuration; t += Time.deltaTime)
        {
            thisText.color = Color.Lerp(currentColor, Color.clear, Mathf.Min(1, t / fadeOutDuration));
            yield return null;
        }

    }


}
