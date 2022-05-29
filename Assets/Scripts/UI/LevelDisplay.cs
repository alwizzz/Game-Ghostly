using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelDisplay : MonoBehaviour
{
    [SerializeField] bool isFadingOut = true;
    [SerializeField] float lingerDuration = 2f;
    [SerializeField] float fadeOutDuration = 2f;

    Text thisText;
    Color currentColor;
    LevelMaster levelMaster;

    private void Start()
    {
        levelMaster = FindObjectOfType<LevelMaster>();
        thisText = GetComponent<Text>();
        currentColor = thisText.color;       
        thisText.text = "Level " + levelMaster.currentLevel.ToString();

        if (isFadingOut)
        {
            StartCoroutine(LingerThenFadeOut());
        }
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
