using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour {

    public Text ScoreText;
    public Text TargetScoreText;

    public GameObject PreGameUI;
    public GameObject GameOverUI;
    public GameObject VictoryUI;
    public GameObject GameUI;

    private const string ScoreTextPrefix = "Score:";
    private const string TargetTextPrefix = "Target:";

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

    /// <summary>
    /// Target score.
    /// </summary>
    /// <param name="score"></param>
    public void UpdateTargetScore(int score)
    {
        TargetScoreText.text = TargetTextPrefix + score;
    }

    /// <summary>
    /// Pre game
    /// </summary>
    /// <param name="show"></param>
    public void ShowPreGameUI(bool show)
    {
        PreGameUI.SetActive(show);
    }

    /// <summary>
    /// Game over
    /// </summary>
    /// <param name="show"></param>
    public void ShowGameOverUI(bool show)
    {
        GameOverUI.SetActive(show);
    }

    /// <summary>
    /// Victory
    /// </summary>
    /// <param name="show"></param>
    public void ShowVictoryUI(bool show)
    {
        VictoryUI.SetActive(show);
    }

    /// <summary>
    /// Game
    /// </summary>
    /// <param name="show"></param>
    public void ShowGameUI(bool show)
    {
        GameUI.SetActive(show);
    }

    /// <summary>
    /// Press a button to start playing.
    /// </summary>
    public void OnGameStartButtonPressed()
    {
        GameManager.Instance.OnGameStartButtonPressed();
    }

    /// <summary>
    /// Menu
    /// </summary>
    public void OnReturnToMenuButtonPressed()
    {
        GameManager.Instance.ReturnToMenu();
    }
}
