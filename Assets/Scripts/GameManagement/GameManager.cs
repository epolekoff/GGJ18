using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public PuzzleTile[,] PuzzleGrid { get; set; }

    private int m_width;
    private int m_height;

    private Dictionary<PuzzleTileType, GameObject> PuzzleTilePrefab;

    // Use this for initialization
    void Start () {
        InitializeNewPuzzle(8, 12);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializeNewPuzzle(int width, int height)
    {
        PuzzleGrid = PuzzleManager.Instance.GenerateInitialPuzzle(width, height, 3);
    }

}
