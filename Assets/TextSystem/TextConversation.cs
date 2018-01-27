using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextConversation : MonoBehaviour {

    //This will hold each individual schedule. 


    /*
    Commands are as follows:
    !wait number - wait the following number of miliseconds then move to next command
    !say string - create a bubble with the text
    !end - trigger the ability to end the conversation
    */

    public List<string> textSchedule;

    //This will hold whatever the current line we are on, aka the one that should be played next.  
    private int currentLine = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log("pressed a");
            StartCoroutine(PlayConversation());
  //          PlayConversation();
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
                //say the thing
                Debug.Log("say a thing");
            }
            else Debug.LogError("Malformed command in textSchedule for " + gameObject.name);

        }


    }

    void ExecuteLine(int line)
    {

    }
}
