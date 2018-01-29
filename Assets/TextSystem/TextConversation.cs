using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TextConversation : MonoBehaviour {

    //This will hold each individual conversation. 

    //Who the conversation is with
    public Character character;


    /*
    This list will hold the 'script' for text conversations.
    Commands are as follows:
    !wait number - wait the following number of miliseconds then move to next command
    !say gre/blu string - create a bubble of the chosen color with the text. Green = player side.
    *NYI* !choice choiceID - Calls a function that gives the player a choice, then calls the next subroutine. 
    !end - trigger the ability to end the conversation
    */
    public List<string> textSchedule;

    //The name of the text file holding the script for this conversation
    public string textFileName;

    //These hold the prefabs for the bubble objects
    public TextBubble smGreenBubble;
    public TextBubble lgGreenBubble;
    public TextBubble smBlueBubble;
    public TextBubble lgBlueBubble;

    //This is the max number of characters that can go in a small bubble
    public int smallBubbleLimit = 40;

    //This is the max number of characters that can fit in a large bubble
    public int largeBubbleLimit = 80;

    //This will hold whatever the current line we are on, aka the one that should be played next.  
    private int currentLine = 0;

    //This will hold the canvas we want to use for the messages
    private Canvas targetCanvas;

    //This is how many text bubbles we've spawned this conversation.
    private int numberOfBubbles = 0;

    private bool initialized = false;

    // Use this for initialization
    void Start () {
        if(targetCanvas == null)
            targetCanvas = FindObjectOfType<Canvas>();
        if (targetCanvas == null)
            Debug.LogError(name + " cannot find a loaded Canvas object.");

        string path = "Assets/TextSystem/Dialogue/Resources" + textFileName + ".txt";
        TextAsset asset = Resources.Load(textFileName) as TextAsset;
        //StreamReader reader = new StreamReader(textFileName);
        string input = asset.text;
        //reader.Close();

        textSchedule = input.Split('\n').ToList<string>();
        Debug.Log(textSchedule);

        if (initialized)
        {
            StartCoroutine(PlayConversation());
        }
    }

    public void Initialize(Canvas canvas, string textFileName)
    {
        initialized = true;
        targetCanvas = canvas;
        this.textFileName = textFileName;
    }
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log("pressed a");
            StartCoroutine(PlayConversation());
        }

        if(Debug.isDebugBuild && Input.GetKey(KeyCode.F))
        {
            Time.timeScale = 2f;
        } else if(Debug.isDebugBuild)
        {
            Time.timeScale = 1f;
        }
	}

    public IEnumerator PlayConversation()
    {
        //string command = textSchedule[line];

        foreach(string command in textSchedule)
        {
            Debug.Log(command);

            if (command.StartsWith("!end"))
            {
                //This should probably always be preceeded by a !wait so the player can see what was said.
                TextManager.Instance.EndConversation();
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
                //Debug.Log("say a thing");
            }

            else if (command.StartsWith("!choice"))
            {
                //TODO Used to call a choice
            }

           // else Debug.LogError("Malformed command in textSchedule for " + gameObject.name + ".  The command reads:  " + command);

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
        bool isGreen = false;
        int characterCount = message.Length;

        if(characterCount <= smallBubbleLimit)
        {
            if (color == BubbleColors.BLUE)
            {
                isGreen = false;
                bubble = smBlueBubble;
                //bubble = Instantiate(smBlueBubble);
            }
            else if (color == BubbleColors.GREEN)
            {
                isGreen = true;
                bubble = smGreenBubble;
                //bubble = Instantiate(smGreenBubble);
            }
            else Debug.LogError("Yo you don't have the right color some how. Way to go.");

        }
        else if (characterCount <= largeBubbleLimit)
        {
            if (color == BubbleColors.BLUE)
            {
                isGreen = false;
                bubble = lgBlueBubble;
//              bubble = Instantiate(lgBlueBubble);
            }
            else if (color == BubbleColors.GREEN)
            {
                isGreen = false;
                bubble = lgGreenBubble;
                //bubble = Instantiate(lgGreenBubble);
            }
            else Debug.LogError("Yo you don't have the right color some how. Way to go.");
        }
        else Debug.LogError("The string entered is too long.");

        //bubble.transform.parent = targetCanvas.transform;
        TextManager.Instance.SpawnBox(bubble, message, isGreen);


        UnityEngine.UI.Text textObject = bubble.GetComponentInChildren<UnityEngine.UI.Text>();
        textObject.text = message;
        
    }
}
public enum BubbleColors
{
    GREEN,
    BLUE
}

public enum Character
{
    TRAIL,
    CONTRAPTION,
    ANIMEFAN,
    MYSTERY
}
 