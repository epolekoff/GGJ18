﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuzzleManager : Singleton<PuzzleManager> {

    public delegate void OnTileFallComplete(PuzzleTile tile);

    public bool GameActive { get; set; }

    public PuzzleTile[,] PuzzleGrid { get; set; }


    private static Dictionary<PuzzleTileType, string> PuzzleTileResourceMap = new Dictionary<PuzzleTileType, string>()
    {
        { PuzzleTileType.Red, "PuzzleGame/PuzzleTileRed" },
        { PuzzleTileType.Blue, "PuzzleGame/PuzzleTileBlue" },
        { PuzzleTileType.Yellow, "PuzzleGame/PuzzleTileYellow" },
        { PuzzleTileType.Green, "PuzzleGame/PuzzleTileGreen" },
        { PuzzleTileType.Purple, "PuzzleGame/PuzzleTilePurple" },
        { PuzzleTileType.Junk, "PuzzleGame/PuzzleTileJunk" },
    };

    private const float TileWidth = 5f;
    private const float TileHeight = 5f;
    private const float MarginX = 0.1f;
    private const float MarginY = 0.1f;

    private const int DefaultPuzzleWidth = 8;
    private const int DefaultPuzzleHeight = 12;
    private const float VerticalScrollSpeed = 10f;
    private const int MinimumMatch = 3;

    public GameObject TileContainer;
    public GameObject DragCursor;

    private List<PuzzleTile> m_createdTiles = new List<PuzzleTile>();
    private Vector3 m_tileContainerInitialPosition;
    private int m_lastAddedRow;
    private int m_rowsPastTopOfScreen = 0;

    private PuzzleTile m_currentHoveredTile;

    /// <summary>
    /// Start
    /// </summary>
    void Awake()
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
        UpdateDragCursor();
    }

    /// <summary>
    /// Generate a new puzzle with some rows. This assumes the puzzle is empty.
    /// </summary>
    public void InitializeNewPuzzle(int initialRows)
    {
        int width = DefaultPuzzleWidth;
        int height = DefaultPuzzleHeight;

        // Create a new puzzle.
        PuzzleGrid = new PuzzleTile[width, 50];
        ResetPuzzle();

        // Row 0 is the top, the bottom at the start is equal to the height. Generate 1 row past that.
        int bottomRow = height + 1;

        // Iterate through the first few initial rows.
        int rowsToAdd = initialRows;
        for(int row = bottomRow - rowsToAdd; row <= bottomRow; row ++)
        {
            bool isLastRow = row >= bottomRow;
            GenerateRow(row, width, isLastRow);
        }
    }

    /// <summary>
    /// Reset the puzzle fields to their initial state.
    /// </summary>
    private void ResetPuzzle()
    {
        DestroyOldPuzzleTiles();
        TileContainer.transform.position = m_tileContainerInitialPosition;
        GameActive = true;
    }

    /// <summary>
    /// Generate all of the tiles in a new row.
    /// </summary>
    private void GenerateRow(int row, int width, bool isOffscreen = false)
    {
        // Iterate all of the tiles in this row and create them.
        for (int x = 0; x < width; x++)
        {
            PuzzleTileType tileType = ChooseRandomNonMatchingTile(x, row);
            PuzzleTile newTile = GenerateTile(tileType, x, row);
            newTile.IsOffscreen = isOffscreen;
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
        puzzleTile.PuzzleTileType = tileType;

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
            if(puzzleTile != null)
            {
                GameObject.Destroy(puzzleTile.gameObject);
            }
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
    /// Given a local space position, get the tile matching.
    /// </summary>
    public Vector2 GetTileCoordinateOfLocalPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / (TileWidth + MarginX));
        int y = Mathf.FloorToInt(-position.y / (TileHeight + MarginY));
        return new Vector2(x, y);
    }

    /// <summary>
    /// Get a random tile.
    /// </summary>
    /// <returns></returns>
    private PuzzleTileType ChooseRandomNonMatchingTile(int x, int y)
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

        // Find a tile that would not make a match at the given position.
        PuzzleTileType potentialTile;
        bool foundMatch = false;
        do
        {
            potentialTile = goodTiles[Random.Range(0, goodTiles.Count)];
            foundMatch = GetMatchesFromTypeAtPosition(potentialTile, x, y, true).Count != 0;
            if(foundMatch)
            {
                goodTiles.Remove(potentialTile);
            }

            // If we run out of good tiles, put in a bad tile.
            if(goodTiles.Count == 0)
            {
                potentialTile = PuzzleTileType.Junk;
                break;
            }
        }
        while (foundMatch);

        // Return the tile we found.
        return potentialTile;

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
            int bottomRowOnScreen = m_lastAddedRow;
            GenerateRow(m_lastAddedRow + 1, DefaultPuzzleWidth, true);
            m_rowsPastTopOfScreen = rowsPastTopOfScreen;

            // Set the tiles at the bottom of the screen as Matchable and evaluate them.
            HashSet<PuzzleTile> matchedTiles = new HashSet<PuzzleTile>();
            for (int x = 0; x < DefaultPuzzleWidth; x++)
            {
                PuzzleGrid[x, bottomRowOnScreen].IsOffscreen = false;
                var matchesForTile = GetMatchesFromTypeAtPosition(PuzzleGrid[x, bottomRowOnScreen].PuzzleTileType, x, bottomRowOnScreen);

                // Combine the match coordinates into a single list of tiles.
                matchesForTile.ForEach(v => matchedTiles.Add(PuzzleGrid[(int)v.x, (int)v.y]));
            }
            HandleMatches(new List<PuzzleTile>(matchedTiles));
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

    /// <summary>
    /// If the specified type was at the specified position, return the tiles that would be a match as a result.
    /// </summary>
    private List<Vector2> GetMatchesFromTypeAtPosition(PuzzleTileType type, int x, int y, bool ignoreUnmatchableTag = false)
    {
        HashSet<Vector2> matchingTiles = new HashSet<Vector2>();

        // Check Row
        List<Vector2> matchingInRow = new List<Vector2>();
        int width = DefaultPuzzleWidth;
        for (int checkX = 0; checkX < width; checkX++)
        {
            bool shouldClearList = false;
            if(checkX == x || 
                (   PuzzleGrid[checkX, y] != null && 
                    PuzzleGrid[checkX, y].PuzzleTileType == type && 
                    (ignoreUnmatchableTag || PuzzleGrid[checkX, y].CanBeMatched)))
            {
                matchingInRow.Add(new Vector2(checkX, y));
            }
            else
            {
                shouldClearList = true;
            }

            // If the row matched 3 ,
            // and that match contained our tile,
            if (matchingInRow.Count >= MinimumMatch)
            {
                if (matchingInRow.Any(v => (int)v.x == x))
                {
                    // and we hit the end of a row or a new color or an unmatchable tile, then add what we have as a match.
                    if (checkX == width - 1 ||
                        (checkX != x && PuzzleGrid[checkX, y] == null) ||
                        (checkX != x && PuzzleGrid[checkX, y].PuzzleTileType != type) ||
                        (checkX != x && !ignoreUnmatchableTag && !PuzzleGrid[checkX, y].CanBeMatched))
                    {
                        matchingInRow.ForEach(t => matchingTiles.Add(t));
                        break;
                    }
                }
            }

            if(shouldClearList)
            {
                matchingInRow.Clear();
            }
        }

        // Check Column
        List<Vector2> matchingInColumn = new List<Vector2>();
        int numberToCheck = m_rowsPastTopOfScreen + DefaultPuzzleHeight + 3;
        for(int checkY = 0; checkY < numberToCheck; checkY++)
        {
            bool shouldClearList = false;
            if (checkY == y || 
                (   PuzzleGrid[x, checkY] != null &&
                    PuzzleGrid[x, checkY].PuzzleTileType == type && 
                    (ignoreUnmatchableTag || PuzzleGrid[x, checkY].CanBeMatched)))
            {
                matchingInColumn.Add(new Vector2(x, checkY));
            }
            else
            {
                shouldClearList = true;
            }

            // If the column matched at least 3,
            // and that match contained our tile
            if (matchingInColumn.Count >= MinimumMatch)
            {
                if (matchingInColumn.Any(v => (int)v.y == y))
                {
                    // and we hit the end of a column or a new color or an unmatchable tile, then add what we have as a match.
                    if (checkY == numberToCheck - 1 ||
                    (checkY != y && PuzzleGrid[x, checkY] == null) ||
                    (checkY != y && PuzzleGrid[x, checkY].PuzzleTileType != type) ||
                    (checkY != y && !ignoreUnmatchableTag && !PuzzleGrid[x, checkY].CanBeMatched))
                    {
                        matchingInColumn.ForEach(t => matchingTiles.Add(t));
                        break;
                    }
                }
            }

            if (shouldClearList)
            {
                matchingInColumn.Clear();
            }
        }

        return new List<Vector2>(matchingTiles);
    }

    // Show or hide the cursor.
    public void EnableDragCursor(bool enabled)
    {
        DragCursor.SetActive(enabled);
    }

    /// <summary>
    /// The cursor follows the mouse.
    /// </summary>
    private void UpdateDragCursor()
    {
        var mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        DragCursor.transform.position = new Vector3(mousePosition.x, mousePosition.y, DragCursor.transform.position.z);
    }

    public void SetCurrentHoveredTile(PuzzleTile tile)
    {
        m_currentHoveredTile = tile;
    }

    public void ClearCurrentHoveredTile(PuzzleTile tile)
    {
        if(m_currentHoveredTile == tile)
        {
            m_currentHoveredTile = null;
        }
    }

    public void SwapWithCurrentHoveredTile(PuzzleTile draggedTile)
    {
        if(m_currentHoveredTile == null)
        {
            SwapWithNull(draggedTile);
            return;
        }

        // Can't swap with itself.
        if(draggedTile.X == m_currentHoveredTile.X)
        {
            return;
        }

        // Try to swap with the tile on the same row. Not across rows.
        PuzzleTile tileToSwap = m_currentHoveredTile;
        if(tileToSwap.Y != draggedTile.Y)
        {
            if(PuzzleGrid[tileToSwap.X, draggedTile.Y] != null)
            {
                tileToSwap = PuzzleGrid[tileToSwap.X, draggedTile.Y];
            }
        }

        // Swap the X values
        int temp = draggedTile.X;
        draggedTile.X = tileToSwap.X;
        tileToSwap.X = temp;

        PuzzleGrid[tileToSwap.X, draggedTile.Y] = tileToSwap;
        PuzzleGrid[draggedTile.X, draggedTile.Y] = draggedTile;

        // Update the positions
        draggedTile.transform.localPosition = GetLocalPositionOfTileCoordinate(draggedTile.X, draggedTile.Y);
        tileToSwap.transform.localPosition = GetLocalPositionOfTileCoordinate(tileToSwap.X, tileToSwap.Y);

        // Check for a match
        var matchesForDraggedTile = GetMatchesFromTypeAtPosition(draggedTile.PuzzleTileType, draggedTile.X, draggedTile.Y);
        var matchesForSwappedTile = GetMatchesFromTypeAtPosition(tileToSwap.PuzzleTileType, tileToSwap.X, tileToSwap.Y);
        Debug.Log("Matches 1: " + matchesForDraggedTile.Count + ", Matches 2: " + matchesForSwappedTile.Count);

        // Combine the match coordinates into a single list of tiles.
        HashSet<PuzzleTile> matchedTiles = new HashSet<PuzzleTile>();
        matchesForDraggedTile.ForEach(v => matchedTiles.Add(PuzzleGrid[(int)v.x, (int)v.y]));
        matchesForSwappedTile.ForEach(v => matchedTiles.Add(PuzzleGrid[(int)v.x, (int)v.y]));

        // Handle the matches
        HandleMatches(new List<PuzzleTile>(matchedTiles));
    }

    public void SwapWithNull(PuzzleTile draggedTile)
    {
        int currentX = draggedTile.X;
        int currentY = draggedTile.Y;

        // Figure out where the mouse is right now.
        var mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 nullTileCoordinate = GetTileCoordinateOfLocalPosition(TileContainer.transform.InverseTransformPoint(mousePosition));

        int mouseX = (int)nullTileCoordinate.x + 1;

        // Only drag off edges next to the tile.
        if (Mathf.Abs(mouseX - currentX) != 1)
        {
            return;
        }

        // Move the tile to the null space.
        var slidingTile = MoveTileToNullPosition(currentX, currentY, mouseX, currentY);
        slidingTile.transform.localPosition = GetLocalPositionOfTileCoordinate(mouseX, currentY);

        // Drop the dragged tile.
        for (int y = currentY + 1; y < m_lastAddedRow; y++)
        {
            if(PuzzleGrid[mouseX, y] != null)
            {
                int newY = y - 1;
                // Go down until we find a non-null. Then back it up one and set that as the position.
                var fallingTile = MoveTileToNullPosition(mouseX, currentY, mouseX, newY);
                fallingTile.TargetFallingLocalPosition = GetLocalPositionOfTileCoordinate(mouseX, newY);
                fallingTile.FallIntoPosition(HandleRecursiveTilesWhenFallingComplete);
                break;
            }
        }

        // Drop all tiles above the null space.
        if(PuzzleGrid[currentX, currentY - 1] != null)
        {
            for (int y = currentY - 1; y > 0; y--)
            {
                if (PuzzleGrid[currentX, y] == null)
                {
                    break;
                }

                // Move the tile down one and continue to the next tile.
                int newY = y + 1;
                var fallingTile = MoveTileToNullPosition(currentX, y, currentX, newY);
                fallingTile.TargetFallingLocalPosition = GetLocalPositionOfTileCoordinate(currentX, newY);
                fallingTile.FallIntoPosition(HandleRecursiveTilesWhenFallingComplete);
            }
        }
    }

    /// <summary>
    /// Swap a tile with a null tile.
    /// </summary>
    private PuzzleTile MoveTileToNullPosition(int oldX, int oldY, int newX, int newY)
    {
        // We can only swap with NULL using this function. Return what was already there.
        if (PuzzleGrid[newX, newY] != null)
        {
            return PuzzleGrid[oldX, oldY];
        }

        PuzzleGrid[newX, newY] = PuzzleGrid[oldX, oldY];
        PuzzleGrid[newX, newY].X = newX;
        PuzzleGrid[newX, newY].Y = newY;
        PuzzleGrid[oldX, oldY] = null;
        return PuzzleGrid[newX, newY];
    }

    /// <summary>
    /// Given a list of tiles that were matches, do something with them!
    /// </summary>
    private void HandleMatches(List<PuzzleTile> matchedTiles)
    {
        // If there are no matches, return.
        if(matchedTiles.Count == 0)
        {
            return;
        }

        HashSet<PuzzleTile> fallingTiles = new HashSet<PuzzleTile>();
        foreach(var tile in matchedTiles)
        {
            // Get every tile above this one and move it down.
            int currentX = tile.X;
            int currentY = tile.Y;
            while (currentY - 1 > 0 && PuzzleGrid[currentX, currentY - 1] != null)
            {
                // Update the tile and the grid.
                PuzzleGrid[currentX, currentY] = PuzzleGrid[currentX, currentY - 1];
                PuzzleGrid[currentX, currentY - 1] = null;
                PuzzleGrid[currentX, currentY].Y = currentY;
                PuzzleGrid[currentX, currentY].TargetFallingLocalPosition = GetLocalPositionOfTileCoordinate(currentX, currentY);

                // Add it to a list to check recursive matches later.
                fallingTiles.Add(PuzzleGrid[currentX, currentY]);

                currentY -= 1;
            }

            // Destroy the tile.
            GameObject.Destroy(tile.gameObject);
        }

        // Make the tiles start falling
        foreach (var fallingTile in fallingTiles)
        {
            PuzzleGrid[fallingTile.X, fallingTile.Y].FallIntoPosition(HandleRecursiveTilesWhenFallingComplete);
        }
    }

    private void HandleRecursiveTilesWhenFallingComplete(PuzzleTile fallingTile)
    {
        // Now evaluate all matches on the board from falling blocks as a result of these matches.
        HashSet<PuzzleTile> recursivelyMatchedTiles = new HashSet<PuzzleTile>();
        var matchesForFallingTile = GetMatchesFromTypeAtPosition(fallingTile.PuzzleTileType, fallingTile.X, fallingTile.Y);
        matchesForFallingTile.ForEach(v => recursivelyMatchedTiles.Add(PuzzleGrid[(int)v.x, (int)v.y]));
        HandleMatches(new List<PuzzleTile>(recursivelyMatchedTiles));
    }
}
