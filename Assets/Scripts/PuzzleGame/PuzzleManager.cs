using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : Singleton<PuzzleManager> {

    public bool GameActive { get; set; }

    public PuzzleTile[,] PuzzleGrid { get; set; }


    private static Dictionary<PuzzleTileType, string> PuzzleTileResourceMap = new Dictionary<PuzzleTileType, string>()
    {
        { PuzzleTileType.Red, "PuzzleGame/PuzzleTileRed" },
        { PuzzleTileType.Blue, "PuzzleGame/PuzzleTileBlue" },
        { PuzzleTileType.Yellow, "PuzzleGame/PuzzleTileYellow" },
        { PuzzleTileType.Green, "PuzzleGame/PuzzleTileGreen" },
        { PuzzleTileType.Purple, "PuzzleGame/PuzzleTilePurple" },
    };

    private const float TileWidth = 5f;
    private const float TileHeight = 5f;
    private const float MarginX = 0.1f;
    private const float MarginY = 0.1f;

    private const int DefaultPuzzleWidth = 8;
    private const int DefaultPuzzleHeight = 12;
    private const float VerticalScrollSpeed = 100f;

    public GameObject TileContainer;

    private List<PuzzleTile> m_createdTiles = new List<PuzzleTile>();
    private Vector3 m_tileContainerInitialPosition;
    private int m_lastAddedRow;
    private int m_rowsPastTopOfScreen = 0;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        // Record the starting position of the scrolling tiles.
        m_tileContainerInitialPosition = TileContainer.transform.position;
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        ScrollTilesVertically();
    }

    /// <summary>
    /// Generate a new puzzle with some rows. This assumes the puzzle is empty.
    /// </summary>
    public void InitializeNewPuzzle(int initialRows)
    {
        int width = DefaultPuzzleWidth;
        int height = DefaultPuzzleHeight;

        // Create a new puzzle.
        PuzzleGrid = new PuzzleTile[width, 1000];
        ResetPuzzle();

        // Row 0 is the top, the bottom at the start is equal to the height. Generate 1 row past that.
        int bottomRow = height + 1;

        // Iterate through the first few initial rows.
        int rowsToAdd = initialRows;
        for(int row = bottomRow - rowsToAdd; row <= bottomRow; row ++)
        {
            GenerateRow(row, width);
        }
    }

    /// <summary>
    /// Reset the puzzle fields to their initial state.
    /// </summary>
    private void ResetPuzzle()
    {
        DestroyOldPuzzleTiles();
        TileContainer.transform.position = m_tileContainerInitialPosition;
    }

    /// <summary>
    /// Generate all of the tiles in a new row.
    /// </summary>
    private void GenerateRow(int row, int width)
    {
        // Iterate all of the tiles in this row and create them.
        for (int x = 0; x < width; x++)
        {
            PuzzleTileType tileType = ChooseRandomTile();
            PuzzleTile newTile = GenerateTile(tileType, x, row);
            PuzzleGrid[x, row] = newTile;
        }
        m_lastAddedRow = row;
    }

    /// <summary>
    /// Generate a tile from the type.
    /// </summary>
    private PuzzleTile GenerateTile(PuzzleTileType tileType, int x, int y)
    {
        GameObject resource = Resources.Load(PuzzleTileResourceMap[tileType]) as GameObject;
        GameObject tileObject = GameObject.Instantiate(resource);
        PuzzleTile puzzleTile = tileObject.GetComponent<PuzzleTile>();
        m_createdTiles.Add(puzzleTile);

        puzzleTile.X = x;
        puzzleTile.Y = y;

        puzzleTile.transform.localPosition = GetLocalPositionOfTileCoordinate(x, y);

        puzzleTile.transform.SetParent(TileContainer.transform, false);

        return puzzleTile;
    }

    /// <summary>
    /// Clean up all old tiles.
    /// </summary>
    private void DestroyOldPuzzleTiles()
    {
        foreach(var puzzleTile in m_createdTiles)
        {
            GameObject.Destroy(puzzleTile.gameObject);
        }
        m_createdTiles = new List<PuzzleTile>();
    }

    /// <summary>
    /// Get the local space position in the container for this tile-space coordinate.
    /// </summary>
    public Vector3 GetLocalPositionOfTileCoordinate(int x, int y)
    {
        return new Vector3(x * (TileWidth + MarginX), -y * (TileHeight + MarginY), 0);
    }

    /// <summary>
    /// Get a random tile.
    /// </summary>
    /// <returns></returns>
    private PuzzleTileType ChooseRandomTile()
    {
        List<PuzzleTileType> goodTiles = new List<PuzzleTileType>()
        {
            PuzzleTileType.Red,
            PuzzleTileType.Blue,
            PuzzleTileType.Yellow,
            PuzzleTileType.Green,
            PuzzleTileType.Purple
        };

        List<PuzzleTileType> badTiles = new List<PuzzleTileType>()
        {
            PuzzleTileType.Alarm,
            PuzzleTileType.Junk
        };

        // Return a random good tile.
        return goodTiles[Random.Range(0, goodTiles.Count)];
    }

    /// <summary>
    /// Scroll the tiles up the screen at a constant speed.
    /// </summary>
    private void ScrollTilesVertically()
    {
        // Don't move if the game is not active.
        if(!GameActive)
        {
            return;
        }

        // Move the tiles.
        TileContainer.transform.position += Vector3.up * VerticalScrollSpeed * Time.deltaTime;

        // Calculate how many rows of tiles have passed the top of the screen.
        int rowsPastTopOfScreen = Mathf.FloorToInt((TileContainer.transform.position.y - m_tileContainerInitialPosition.y) / ((TileHeight + MarginY) * TileContainer.transform.localScale.y));

        // If the number of rows off the top has grown, we need to add a new row at the bottom.
        if(m_rowsPastTopOfScreen != rowsPastTopOfScreen)
        {
            GenerateRow(m_lastAddedRow + 1, DefaultPuzzleWidth);
            m_rowsPastTopOfScreen = rowsPastTopOfScreen;
        }

        // Check if there are any tiles in the top row. If there are, we may lose the game.
        int topRow = m_rowsPastTopOfScreen - 1;
        for(int x = 0; x < DefaultPuzzleWidth; x++)
        {
            if(topRow >= 0 && PuzzleGrid[x, topRow] != null)
            {
                GameManager.Instance.GameOver();
                break;
            }
        }

    }
}
