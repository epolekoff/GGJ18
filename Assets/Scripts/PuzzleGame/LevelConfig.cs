using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfig {

    public float ScrollSpeed;

    public bool SpawnJunkTiles;

    public bool SpawnAlarmTiles;

    public float TimeLimit;

    public int ScoreToWin;

    public float JunkDropRateSeconds;


    private static readonly LevelConfig[] Levels = new LevelConfig[]
    {
        // Level 1
        new LevelConfig()
        {
            ScrollSpeed = 10f,
            SpawnJunkTiles = false,
            SpawnAlarmTiles = false,
            TimeLimit = 0,
            ScoreToWin = 200,
            JunkDropRateSeconds = 99999,
        },

        // Level 2
        new LevelConfig()
        {
            ScrollSpeed = 12f,
            SpawnJunkTiles = true,
            SpawnAlarmTiles = false,
            TimeLimit = 0,
            ScoreToWin = 250,
            JunkDropRateSeconds = 15,
        },

        // Level 3
        new LevelConfig()
        {
            ScrollSpeed = 13f,
            SpawnJunkTiles = true,
            SpawnAlarmTiles = false,
            TimeLimit = 0,
            ScoreToWin = 300,
            JunkDropRateSeconds = 15,
        },

        // Level 4
        new LevelConfig()
        {
            ScrollSpeed = 14f,
            SpawnJunkTiles = true,
            SpawnAlarmTiles = true,
            TimeLimit = 0,
            ScoreToWin = 300,
            JunkDropRateSeconds = 15,
        },

        // Level 5
        new LevelConfig()
        {
            ScrollSpeed = 14f,
            SpawnJunkTiles = true,
            SpawnAlarmTiles = true,
            TimeLimit = 0,
            ScoreToWin = 350,
            JunkDropRateSeconds = 20,
        },

        // Level 6
        new LevelConfig()
        {
            ScrollSpeed = 15f,
            SpawnJunkTiles = true,
            SpawnAlarmTiles = true,
            TimeLimit = 0,
            ScoreToWin = 350,
            JunkDropRateSeconds = 20,
        }
    };

    /// <summary>
    /// Get a level.
    /// </summary>
    /// <param name="levelNumber"></param>
    /// <returns></returns>
    public static LevelConfig GetLevel(int levelNumber)
    {
        return Levels[levelNumber];
    }
}


