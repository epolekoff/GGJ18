﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    private const int InitialRowCount = 6;

    // Use this for initialization
    void Start () {
        PuzzleManager.Instance.InitializeNewPuzzle(InitialRowCount);
        PuzzleManager.Instance.GameActive = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// When the game button on the menu is pressed.
    /// </summary>
    public void OnGameButtonPressed()
    {
        PuzzleManager.Instance.InitializeNewPuzzle(InitialRowCount);
    }

    /// <summary>
    /// When the chat button on the menu is pressed.
    /// </summary>
    public void OnChatButtonPressed()
    {

    }

    public void GameOver()
    {
        Debug.LogError("Hit Top Of Screen! Game Over!");
        PuzzleManager.Instance.GameActive = false;
    }
}
