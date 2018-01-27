using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : Singleton<PuzzleManager> {

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

    public GameObject TileContainer;

    /// <summary>
    /// Generate a new puzzle with some rows. This assumes the puzzle is empty.
    /// </summary>
    public PuzzleTile[,] GenerateInitialPuzzle(int width, int height, int initialRows)
    {
        PuzzleTile[,] puzzle = new PuzzleTile[width, 1000];

        // Row 0 is the top, the bottom at the start is equal to the height
        int bottomRow = height;

        // Iterate through the first few initial rows.
        for(int row = bottomRow; row > bottomRow - initialRows; row -= 1)
        {
            // Iterate all of the tiles in this row and create them.
            for (int x = 0; x < width; x++)
            {
                PuzzleTileType tileType = ChooseRandomTile();
                PuzzleTile newTile = GenerateTile(tileType, x, row);
                puzzle[x, row] = newTile;
            }
        }

        return puzzle;
    }

    /// <summary>
    /// Generate a tile from the type.
    /// </summary>
    private PuzzleTile GenerateTile(PuzzleTileType tileType, int x, int y)
    {
        GameObject resource = Resources.Load(PuzzleTileResourceMap[tileType]) as GameObject;
        GameObject tileObject = GameObject.Instantiate(resource);
        PuzzleTile puzzleTile = tileObject.GetComponent<PuzzleTile>();

        puzzleTile.X = x;
        puzzleTile.Y = y;

        puzzleTile.transform.localPosition = GetLocalPositionOfTileCoordinate(x, y);

        puzzleTile.transform.parent = TileContainer.transform;

        return puzzleTile;
    }

    /// <summary>
    /// Get the local space position in the container for this tile-space coordinate.
    /// </summary>
    public Vector3 GetLocalPositionOfTileCoordinate(int x, int y)
    {
        return new Vector3(x * (TileWidth + MarginX), y * (TileHeight + MarginY), 0);
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
}
