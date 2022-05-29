using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreDisplay : MonoBehaviour
{
    public int currentScore = 0;

    Text thisText;
    public LevelMaster levelMaster;

    private void Awake()
    {
        thisText = GetComponent<Text>();
    }

    private void Start()
    {
        levelMaster = LevelMaster.GetThisSingletonScript();
        currentScore = levelMaster.currentScore;
    }

    private void Update()
    {
        // if (levelMaster != null) { levelMaster = LevelMaster.GetThisSingletonScript(); }
        UpdateText();
    }

    void UpdateText()
    {
        thisText.text = "Score: " + currentScore.ToString();
    }

    public void IncrementScore() { 
        currentScore++;
        levelMaster.UpdateCurrentScore(currentScore);
    }

}
