using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour {

    public Text ScoreText;

    private const string ScoreTextPrefix = "Score:";

    void Update()
    {
        UpdateScore(PuzzleManager.Instance.Score);
    }

    /// <summary>
    /// Update the displayed score.
    /// </summary>
    /// <param name="score"></param>
    public void UpdateScore(int score)
    {
        ScoreText.text = ScoreTextPrefix + score;
    }
}
