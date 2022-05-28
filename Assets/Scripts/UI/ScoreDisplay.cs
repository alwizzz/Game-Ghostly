using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreDisplay : MonoBehaviour
{
    public int currentScore = 0;

    Text thisText;
    LevelMaster levelMaster;

    private void Awake()
    {
        thisText = GetComponent<Text>();
        levelMaster = LevelMaster.GetThisSingletonScript();
    }

    private void Start()
    {
        currentScore = levelMaster.currentScore;
    }

    private void Update()
    {
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
