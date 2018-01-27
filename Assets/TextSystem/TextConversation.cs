using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextConversation : MonoBehaviour {

    //This will hold each individual schedule. 

    /*
    This list will hold the 'script' for text conversations.
    Commands are as follows:
    !wait number - wait the following number of miliseconds then move to next command
    !say gre/blu string - create a bubble of the chosen color with the text. Green = player side.
    !end - trigger the ability to end the conversation
    */
    public List<string> textSchedule;


    //These hold the prefabs for the bubble objects
    public TextBubble smGreenBubble;
    public TextBubble lgGreenBubble;
    public TextBubble smBlueBubble;
    public TextBubble lgBlueBubble;

    //This is the max number of characters that can go in a small bubble
    public int smallBubbleLimit = 50;

    //This is the max number of characters that can fit in a large bubble
    public int largeBubbleLimit = 100;

    //This will hold whatever the current line we are on, aka the one that should be played next.  
    private int currentLine = 0;

    //This will hold the canvas we want to use for the messages
    private Canvas targetCanvas;

    //This is how many text bubbles we've spawned this conversation.
    private int numberOfBubbles = 0;


    // Use this for initialization
    void Start () {
        targetCanvas = FindObjectOfType<Canvas>();
        if (targetCanvas == null)
            Debug.LogError(name + " cannot find a loaded Canvas object.");
    }
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log("pressed a");
            StartCoroutine(PlayConversation());
            //PlayConversation();
            //currentLine++;
        }

	}

    public IEnumerator PlayConversation()
    {
        //string command = textSchedule[line];

        foreach(string command in textSchedule)
        {
            if (command.StartsWith("!end"))
            {
                //end the message
                Debug.Log("end it");
            }
            else if (command.StartsWith("!wait"))
            {
                int secondsToWait = 0;
                string input = command.Substring(6);
                if (Int32.TryParse(input, out secondsToWait))
                {
                    float seconds = secondsToWait / 1000f;
                    yield return new WaitForSeconds(seconds);
                    Debug.Log("waiting for " + seconds + " seconds.");
                }
                else
                    Debug.LogError("The command " + command + " would not parse to an int useable by " + name);
            }
            else if (command.StartsWith("!say"))
            {
                //spawn a text bubble into the chat interface
                string input = command.Substring(5);
                string message = input.Substring(4);
                if (input.StartsWith("gre"))
                {
                    SpawnBubble(BubbleColors.GREEN, message);
                }
                else if (input.StartsWith("blu"))
                {
                    SpawnBubble(BubbleColors.BLUE, message);
                }
                else Debug.LogError("The !say command was used without the proper person tag");
                Debug.Log("say a thing");
            }
            else Debug.LogError("Malformed command in textSchedule for " + gameObject.name);

        }
    }


    /*
        public TextBubble smGreenBubble;
    public TextBubble lgGreenBubble;
    public TextBubble smBlueBubble;
    public TextBubble lgBlueBubble;
    */
    void SpawnBubble(BubbleColors color, string message)
    {
        numberOfBubbles++;

        TextBubble bubble = null;
        int characterCount = message.Length;

        if(characterCount <= smallBubbleLimit)
        {
            if (color == BubbleColors.BLUE)
            {
                bubble = Instantiate(smBlueBubble);
            }
            else if (color == BubbleColors.GREEN)
            {
                bubble = Instantiate(smGreenBubble);
            }
            else Debug.LogError("Yo you don't have the right color some how. Way to go.");

        }
        else if (characterCount <= largeBubbleLimit)
        {
            if (color == BubbleColors.BLUE)
            {
                bubble = Instantiate(lgBlueBubble);
            }
            else if (color == BubbleColors.GREEN)
            {
                bubble = Instantiate(lgGreenBubble);
            }
            else Debug.LogError("Yo you don't have the right color some how. Way to go.");
        }
        else Debug.LogError("The string entered is too long.");

        bubble.transform.parent = targetCanvas.transform;
        Vector3 v3 = bubble.transform.position;
        v3.x = 105 * numberOfBubbles;
        v3.y = 480;
        bubble.transform.position = v3;

        UnityEngine.UI.Text textObject = bubble.GetComponentInChildren<UnityEngine.UI.Text>();
        textObject.text = message;


    }
}
public enum BubbleColors
{
    GREEN,
    BLUE
}
 