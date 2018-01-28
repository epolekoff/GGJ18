using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IStateMachineEntity {

    public GameCanvas GameCanvas;
    public GameObject HomeScreen;

    private const int InitialRowCount = 6;

    FiniteStateMachine m_stateMachine;
    private int m_currentLevel = 0;

    // Use this for initialization
    void Start ()
    {
        GameCanvas.gameObject.SetActive(false);
        HomeScreen.gameObject.SetActive(false);

        m_stateMachine = new FiniteStateMachine(new MenuState(), this);

        PuzzleManager.Instance.GameActive = true;
    }
	
	// Update is called once per frame
	void Update () {
        m_stateMachine.Update();
    }

    public FiniteStateMachine GetStateMachine(int number = 0)
    {
        return m_stateMachine;
    }

    /// <summary>
    /// When the game button on the menu is pressed.
    /// </summary>
    public void OnGameButtonPressed()
    {
        m_stateMachine.ChangeState(new PreGameState());
    }

    public void OnGameStartButtonPressed()
    {
        m_stateMachine.ChangeState(new GameState());
        PuzzleManager.Instance.InitializeNewPuzzle(InitialRowCount, LevelConfig.GetLevel(m_currentLevel));
    }

    public void ReturnToMenu()
    {
        m_stateMachine.ChangeState(new MenuState());
        PuzzleManager.Instance.CleanUpCurrentPuzzle();
    }

    /// <summary>
    /// When the chat button on the menu is pressed.
    /// </summary>
    public void OnChatButtonPressed()
    {

    }

    /// <summary>
    /// lose
    /// </summary>
    public void GameOver()
    {
        Debug.LogError("Hit Top Of Screen! Game Over!");
        PuzzleManager.Instance.GameActive = false;
        m_stateMachine.ChangeState(new GameOverState());
    }

    /// <summary>
    /// win
    /// </summary>
    public void GameWin()
    {
        Debug.LogError("You win!");
        PuzzleManager.Instance.GameActive = false;
        m_currentLevel++;
        m_stateMachine.ChangeState(new VictoryState());
    }
}
