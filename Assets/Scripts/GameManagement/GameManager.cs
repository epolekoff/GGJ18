using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    private Dictionary<PuzzleTileType, GameObject> PuzzleTilePrefab;

    // Use this for initialization
    void Start () {
        PuzzleManager.Instance.InitializeNewPuzzle(3);
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
        PuzzleManager.Instance.InitializeNewPuzzle(3);
    }

    /// <summary>
    /// When the chat button on the menu is pressed.
    /// </summary>
    public void OnChatButtonPressed()
    {

    }

    public void GameOver()
    {
        Debug.LogError("Hit Top Of Screen!");
        PuzzleManager.Instance.GameActive = false;
    }
}
