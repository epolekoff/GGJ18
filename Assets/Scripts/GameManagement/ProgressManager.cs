using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : Singleton<ProgressManager> {

    private readonly List<string[]> DialogMap = new List<string[]>()
    {
        { new string[] { "1Trail1" , "1Contraption1" } },
        { new string[] { "2Contraption1" } },
        { new string[] { "3Anime1" , "3Trail2" } },
        { new string[] { "4Anime1" , "4Trail1" } },
        { new string[] { "5Trail1" , "5Contraption1" } },
        { new string[] { "6Trail1" , "6Contraption1" } }
    };

    private int m_conversationProgress = 1;

    
    /// <summary>
    /// Get the filename of the current text conversation.
    /// </summary>
    /// <returns></returns>
    public List<string> GetCurrentTextConversationsForLevel(int levelProgress)
    {
        string[] conversationsForLevel = DialogMap[levelProgress];

        List<string> currentConversations = new List<string>();

        // Find all conversations at this level that match my conversation progress.
        for(int i = 0; i < conversationsForLevel.Length; i++)
        {
            string filename = conversationsForLevel[i];
            if (filename.Substring(filename.Length - 1, 1).Equals(m_conversationProgress.ToString()))
            {
                currentConversations.Add(filename);
            }
        }

        return currentConversations;
    }

    /// <summary>
    /// Move forward to the next dialog.
    /// </summary>
    public void AdvanceDialogProgress()
    {
        m_conversationProgress++;
    }

    /// <summary>
    /// Reset back to the first.
    /// </summary>
    public void ResetDialogProgress()
    {
        m_conversationProgress = 1;
    }
}
