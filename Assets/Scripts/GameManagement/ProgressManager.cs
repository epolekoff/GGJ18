using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : Singleton<ProgressManager> {

    private static Dictionary<int, string> ContraptionMap = new Dictionary<int, string>()
    {
        { 0, "1Contraption1" },
        { 1, "2Contraption1" },
        { 4, "5Contraption1" },
        { 5, "6Contraption1" },
    };

    private static Dictionary<int, string> TrailMap = new Dictionary<int, string>()
    {
        { 0, "1Trail1" },
        { 1, "2Trail1" },
        { 2, "3Trail2" },
        { 3, "4Trail1" },
        { 4, "5Trail1" },
        { 5, "5Trail2" },
        { 6, "6Trail1" },
    };

    private static Dictionary<int, string> AnimeMap = new Dictionary<int, string>()
    {
        { 0, "3Anime1" },
        { 3, "4Anime1" },
    };

    private static Dictionary<int, string>[] Maps = new Dictionary<int, string>[]
    {
        ContraptionMap,
        TrailMap,
        AnimeMap
    };

    private int m_currentProgress;

    
    /// <summary>
    /// Get the filename of the current text conversation.
    /// </summary>
    /// <returns></returns>
    public string GetCurrentTextConversationFile(int personIndex)
    {
        return Maps[personIndex][m_currentProgress];
    }
}
