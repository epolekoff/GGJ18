using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalWindowText : MonoBehaviour {

    private static string[] ActionLines = new string[]
    {
         "> ./ddos.sh -a 192.168.0.1\n",
         "> pip install reinvent.whl\n",
         "> ping -10 google.com\n",
         "> mv red_tile blue_tile\n",
         "> mv blue_tile yellow_tile\n",
    };

    private static string[] GenericLines = new string[]
    {
        "Uploding Trojan...",
        "Receiving Data...",
        "Bypassing Firewall...",
        "Entring Login Credentials...",
        "Deleting System32...",
        "Rebooting...",
        "Forwarding Ports...",
        "Listening on Port 8080...",
        "Sliding on NOPs...",
        "Injecting SQL...",
        "Reply from 192.54.107.32: bytes=32 time<10ms",
        "Reply from 192.69.142.97: bytes=32 time<100ms",
        "Reply from 192.23.278.54: bytes=32 time<10ms",
        "drwxr-xr-x 1 foo123 foo123 2048 Aug 19 13:00",
        "drwx------ 1 bar456 bar456 1024 Jun 8 19:00",
    };

    private const string TransmissionCompleteLine = "Transmission Complete\n";

    private const float MinLineTime = 2f;
    private const float MaxLineTime = 5f;
    private float m_timer = MinLineTime;

    /// <summary>
    /// Clear the window.
    /// </summary>
    public void ClearText()
    {
        GetComponent<Text>().text = "";
    }

    /// <summary>
    /// When a player does something, print a line.
    /// </summary>
    public void TriggerAction()
    {
        int lineIndex = Random.Range(0, ActionLines.Length);
        PrintLine(ActionLines[lineIndex]);
    }

    /// <summary>
    /// Whenever a transmission is complete, print that line.
    /// </summary>
    public void TriggerTransmissionComplete()
    {
        PrintLine(TransmissionCompleteLine);
    }

    /// <summary>
    /// Player a generic line.
    /// </summary>
    private void TriggerGenericLine()
    {
        int lineIndex = Random.Range(0, GenericLines.Length);
        PrintLine(GenericLines[lineIndex]);
    }

    /// <summary>
    /// Add the line to the window.
    /// </summary>
    /// <param name="line"></param>
    private void PrintLine(string line)
    {
        GetComponent<Text>().text += "\n" + line;
    }

    /// <summary>
    /// Update with text periodically.
    /// </summary>
    void Update()
    {
        if(!PuzzleManager.Instance.GameActive)
        {
            return;
        }

        m_timer -= Time.deltaTime;

        if(m_timer <= 0)
        {
            m_timer = Random.Range(MinLineTime, MaxLineTime);
            TriggerGenericLine();
        }
    }
}
