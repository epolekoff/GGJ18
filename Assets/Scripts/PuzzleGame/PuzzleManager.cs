using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuzzleManager : Singleton<PuzzleManager> {

    public delegate void OnTileFallComplete(PuzzleTile tile, int comboDepth);

    public bool GameActive { get; set; }
    public bool ScrollingActive { get; set; }

    public PuzzleTile[,] PuzzleGrid { get; set; }

    public int Score { get; set; }

    private static Dictionary<PuzzleTileType, string> PuzzleTileResourceMap = new Dictionary<PuzzleTileType, string>()
    {
        { PuzzleTileType.Red, "PuzzleGame/PuzzleTileRed" },
        { PuzzleTileType.Blue, "PuzzleGame/PuzzleTileBlue" },
        { PuzzleTileType.Yellow, "PuzzleGame/PuzzleTileYellow" },
        { PuzzleTileType.Green, "PuzzleGame/PuzzleTileGreen" },
        { PuzzleTileType.Purple, "PuzzleGame/PuzzleTilePurple" },
        { PuzzleTileType.Junk, "PuzzleGame/PuzzleTileJunk" },
        { PuzzleTileType.Alarm, "PuzzleGame/PuzzleTileAlarm" },
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

    private const float AlarmJunkTime = 3f;
    private float m_alarmTimer = 0f;
    private Coroutine m_alarmCoroutine;
    private int m_currentAlarmJunk = 0;
    private const int JunkTilesPerAlarm = 5;

    private const float TilesInTopLineTime = 2f;
    private Coroutine m_checkGameOverCoroutine;

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
        ScrollingActive = true;
        Score = 0;
    }

    /// <summary>
    /// Generate all of the tiles in a new row.
    /// </summary>
    private void GenerateRow(int row, int width, bool isOffscreen = false, PuzzleTileType overrideType = PuzzleTileType.None)
    {
        // Iterate all of the tiles in this row and create them.
        for (int x = 0; x < width; x++)
        {
            PuzzleTileType tileType = overrideType;
            if (tileType == PuzzleTileType.None)
            {
                tileType = ChooseRandomNonMatchingTile(x, row);
            }
            PuzzleTile newTile = GenerateTile(tileType, x, row);
            newTile.IsOffscreen = isOffscreen;
        }
        if(row > m_lastAddedRow)
        {
            m_lastAddedRow = row;
        }
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

        PuzzleGrid[x, y] = puzzleTile;
        puzzleTile.X = x;
        puzzleTile.Y = y;
        puzzleTile.PuzzleTileType = tileType;

        // Set the position and check if it should fall.
        puzzleTile.transform.localPosition = GetLocalPositionOfTileCoordinate(x, y);
        CheckUnderBlockAndFall(puzzleTile.X, puzzleTile.Y);

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
        int x = Mathf.FloorToInt((position.x / (TileWidth + MarginX)) + 0.5f);
        int y = Mathf.FloorToInt((-position.y / (TileHeight + MarginY)) - 0.5f);
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

        List<PuzzleTileType> typesToSpawn = new List<PuzzleTileType>();
        // Add the good tiles twice so they are more likely to spawn.
        typesToSpawn.AddRange(goodTiles);
        typesToSpawn.AddRange(goodTiles);
        typesToSpawn.AddRange(badTiles);

        // Find a tile that would not make a match at the given position.
        PuzzleTileType potentialTile;
        bool foundMatch = false;
        do
        {
            potentialTile = typesToSpawn[Random.Range(0, typesToSpawn.Count)];
            foundMatch = GetMatchesFromTypeAtPosition(potentialTile, x, y, true).Count != 0;
            if(foundMatch)
            {
                typesToSpawn.RemoveAll(t => t == potentialTile);
            }

            // If we run out of good tiles, put in a bad tile.
            if(typesToSpawn.Count == 0)
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
        if(!GameActive || !ScrollingActive)
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
                if(PuzzleGrid[x, bottomRowOnScreen] == null)
                {
                    continue;
                }
                PuzzleGrid[x, bottomRowOnScreen].IsOffscreen = false;
                var matchesForTile = GetMatchesFromTypeAtPosition(PuzzleGrid[x, bottomRowOnScreen].PuzzleTileType, x, bottomRowOnScreen);

                // Combine the match coordinates into a single list of tiles.
                matchesForTile.ForEach(v => matchedTiles.Add(PuzzleGrid[(int)v.x, (int)v.y]));
            }
            HandleMatches(new List<PuzzleTile>(matchedTiles), 1);
        }

        // Check if there are any tiles in the top row. If there are, we may lose the game.
        int topRow = m_rowsPastTopOfScreen - 1;
        bool foundTileInTopRow = false;
        for(int x = 0; x < DefaultPuzzleWidth; x++)
        {
            if(topRow >= 0 && PuzzleGrid[x, topRow] != null)
            {
                foundTileInTopRow = true;
                ScrollingActive = false;
                if(m_checkGameOverCoroutine == null)
                {
                    m_checkGameOverCoroutine = StartCoroutine(CheckGameOverTimerCoroutine());
                }
                break;
            }
        }

        // If there are no tiles in the top row, stop trying to end the game.
        if(!foundTileInTopRow)
        {
            if(m_checkGameOverCoroutine != null)
            {
                ScrollingActive = true;
                StopCoroutine(m_checkGameOverCoroutine);
            }
        }
    }

    /// <summary>
    /// Run a timer to check if the game should end. Give the player some time while the line is at the top of the screen.
    /// </summary>
    private IEnumerator CheckGameOverTimerCoroutine()
    {
        float timer = 0;

        while(timer < TilesInTopLineTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GameManager.Instance.GameOver();
    }

    /// <summary>
    /// If the specified type was at the specified position, return the tiles that would be a match as a result.
    /// </summary>
    private List<Vector2> GetMatchesFromTypeAtPosition(PuzzleTileType type, int x, int y, bool ignoreUnmatchableTag = false)
    {
        // Don't allow junk pieces to match.
        if(type == PuzzleTileType.Junk)
        {
            return new List<Vector2>();
        }

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
        HandleMatches(new List<PuzzleTile>(matchedTiles), 1);
    }

    public void SwapWithNull(PuzzleTile draggedTile)
    {
        int currentX = draggedTile.X;
        int currentY = draggedTile.Y;

        // Figure out where the mouse is right now.
        var mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 nullTileCoordinate = GetTileCoordinateOfLocalPosition(TileContainer.transform.InverseTransformPoint(mousePosition));

        int mouseX = (int)nullTileCoordinate.x;

        // Only drag off edges next to the tile.
        if (Mathf.Abs(mouseX - currentX) != 1)
        {
            return;
        }

        // Move the tile to the null space.
        var slidingTile = MoveTileToNullPosition(currentX, currentY, mouseX, currentY);
        slidingTile.transform.localPosition = GetLocalPositionOfTileCoordinate(mouseX, currentY);

        // Drop the dragged tile.
        CheckUnderBlockAndFall(mouseX, currentY);

        // Drop all tiles above the null space.
        if (PuzzleGrid[currentX, currentY - 1] != null)
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
                fallingTile.FallIntoPosition(HandleRecursiveTilesWhenFallingComplete, 1);
            }
        }
    }

    /// <summary>
    /// Look at the blocks under this block and see if it should fall.
    /// If there's nothing below it, set its position to where it is and trigger matches immediately.
    /// </summary>
    private void CheckUnderBlockAndFall(int tileX, int tileY)
    {
        for (int y = tileY + 1; y < m_lastAddedRow; y++)
        {
            if (PuzzleGrid[tileX, y] != null)
            {
                int newY = y - 1;
                // Go down until we find a non-null. Then back it up one and set that as the position.
                var fallingTile = MoveTileToNullPosition(tileX, tileY, tileX, newY);
                if (newY == tileY)
                {
                    fallingTile.transform.localPosition = GetLocalPositionOfTileCoordinate(tileX, newY);
                    HandleRecursiveTilesWhenFallingComplete(fallingTile, 1);
                }
                else
                {
                    fallingTile.TargetFallingLocalPosition = GetLocalPositionOfTileCoordinate(tileX, newY);
                    fallingTile.FallIntoPosition(HandleRecursiveTilesWhenFallingComplete, 1);
                }
                break;
            }
        }
    }

    /// <summary>
    /// Swap a tile with a null tile.
    /// </summary>
    private PuzzleTile MoveTileToNullPosition(int oldX, int oldY, int newX, int newY)
    {
        // If there's no change, ignore it.
        if(oldX == newX && oldY == newY)
        {
            return PuzzleGrid[oldX, oldY];
        }

        // We can only swap with NULL using this function. Return what was already there.
        if (PuzzleGrid[newX, newY] != null)
        {
            Debug.LogError("Attemptint to Null-Swap with a non-null");
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
    private void HandleMatches(List<PuzzleTile> matchedTiles, int comboDepth)
    {
        // If there are no matches, return.
        if(matchedTiles.Count == 0)
        {
            return;
        }

        // Add neighboring junk and alarm tiles
        HashSet<PuzzleTile> neighborBadTiles = new HashSet<PuzzleTile>();
        foreach(var tile in matchedTiles)
        {
            List<PuzzleTile> neighbors = GetAllNeighborsOfTile(tile);
            neighbors.ForEach(t =>
            {
                if( t != null && 
                    (t.PuzzleTileType == PuzzleTileType.Junk || t.PuzzleTileType == PuzzleTileType.Alarm))
                {
                    neighborBadTiles.Add(t);
                }
            });
        }
        matchedTiles.AddRange(neighborBadTiles);

        // Adjust all of the tiles that need to fall as a result.
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
            DestroyTileDuringMatch(tile);
        }

        // Make the tiles start falling
        foreach (var fallingTile in fallingTiles)
        {
            PuzzleGrid[fallingTile.X, fallingTile.Y].FallIntoPosition(HandleRecursiveTilesWhenFallingComplete, comboDepth);
        }

        // Display the score
        Score += GetScoreFromMatch(matchedTiles, comboDepth);
    }

    private void HandleRecursiveTilesWhenFallingComplete(PuzzleTile fallingTile, int comboDepth)
    {
        // Now evaluate all matches on the board from falling blocks as a result of these matches.
        HashSet<PuzzleTile> recursivelyMatchedTiles = new HashSet<PuzzleTile>();
        var matchesForFallingTile = GetMatchesFromTypeAtPosition(fallingTile.PuzzleTileType, fallingTile.X, fallingTile.Y);
        matchesForFallingTile.ForEach(v => recursivelyMatchedTiles.Add(PuzzleGrid[(int)v.x, (int)v.y]));
        HandleMatches(new List<PuzzleTile>(recursivelyMatchedTiles), comboDepth + 1);
    }

    private void DestroyTileDuringMatch(PuzzleTile tile)
    {
        if(tile.PuzzleTileType == PuzzleTileType.Alarm)
        {
            TriggerAlarm();
        }

        GameObject.Destroy(tile.gameObject);
    }

    /// <summary>
    /// Get the neighbors of a tile.
    /// </summary>
    private List<PuzzleTile> GetAllNeighborsOfTile(PuzzleTile tile)
    {
        List<PuzzleTile> neighbors = new List<PuzzleTile>();
        if(tile.X > 0)
            neighbors.Add(PuzzleGrid[tile.X-1, tile.Y]);
        if(tile.X < DefaultPuzzleWidth-1)
            neighbors.Add(PuzzleGrid[tile.X+1, tile.Y]);
        if(tile.Y > 0)
            neighbors.Add(PuzzleGrid[tile.X, tile.Y-1]);
        if (tile.Y < 1000)
            neighbors.Add(PuzzleGrid[tile.X, tile.Y+1]);

        return neighbors;
    }

    /// <summary>
    /// Add more junk to the stacking alarm.
    /// </summary>
    private void TriggerAlarm()
    {
        m_currentAlarmJunk += JunkTilesPerAlarm;
        if (m_alarmCoroutine == null)
        {
            m_alarmCoroutine = StartCoroutine(AlarmCountdownCoroutine());
        }
    }

    /// <summary>
    /// Some number of alarms were triggered, so let them stack up before sending junk.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AlarmCountdownCoroutine()
    {
        m_alarmTimer = 0;
        while(m_alarmTimer < AlarmJunkTime)
        {
            m_alarmTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Drop the accumulated junk tiles on the player.
        DropJunkTilesFromTop(m_currentAlarmJunk);
        m_currentAlarmJunk = 0;

        m_alarmCoroutine = null;
    }

    /// <summary>
    /// Spawn some number of junk tiles at the top of the screen.
    /// </summary>
    /// <param name="numberOfTiles"></param>
    private void DropJunkTilesFromTop(int numberOfTiles)
    {
        int numRows = numberOfTiles / DefaultPuzzleWidth;
        int numLooseTiles = numberOfTiles % DefaultPuzzleWidth;

        int bottomRow = 0;
        for (int i = 0; i < numRows; i++)
        {
            int currentRow = i;
            GenerateRow(currentRow, DefaultPuzzleWidth, false, PuzzleTileType.Junk);
            bottomRow = currentRow + 1;
        }

        List<int> usedPositions = new List<int>();
        for (int i = 0; i < numLooseTiles; i++)
        {
            int x = i;
            do{
                x = Random.Range(0, DefaultPuzzleWidth);
            } while (usedPositions.Contains(x));
            usedPositions.Add(x);
            GenerateTile(PuzzleTileType.Junk, x, bottomRow);
        }
    }

    /// <summary>
    /// Get the score from a set of matches.
    /// </summary>
    private int GetScoreFromMatch(List<PuzzleTile> matches, int comboDepth)
    {
        // Ignore alarms and junk in the score.
        int goodMatches = matches.Count(t =>
            t.PuzzleTileType != PuzzleTileType.Alarm &&
            t.PuzzleTileType != PuzzleTileType.Junk);

        int score = (int)Mathf.Pow(goodMatches, 2) * comboDepth;
        return score;
    }
}
