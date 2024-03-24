using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Components")]
    public GameObject dialoguePanel;
    public Text speakerText;
    public Text dialogueText;

    //The lines to queue during the dialogue sequence
    Queue<DialogueLine> dialogueQueue;
    Action onDialogueEnd = null;

    bool isTyping = false;

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    //Initialises the dialogue
    public void StartDialogue(List<DialogueLine> dialogueLinesToQueue)
    {
        //Convert the list to a queue
        dialogueQueue = new Queue<DialogueLine>(dialogueLinesToQueue);

        UpdateDialogue();
    }

    //Initialises the dialogue, but with an Action to execute once it finishes
    public void StartDialogue(List<DialogueLine> dialogueLinesToQueue, Action onDialogueEnd)
    {
        //Convert the list to a queue
        StartDialogue(dialogueLinesToQueue);
        this.onDialogueEnd = onDialogueEnd;
    }

    //Cycle through the dialogue lines
    public void UpdateDialogue()
    {
        if(isTyping)
        {
            isTyping = false;
            return;
        }

        //Reset out dialogue text
        dialogueText.text = string.Empty;

        //Check if there are any more lines in the queue
        if(dialogueQueue.Count == 0)
        {
            //If not, end the dialogue
            EndDialogue();
            return;
        }

        //The current dialogue line to put in
        DialogueLine line = dialogueQueue.Dequeue();

        Talk(line.speaker, line.message);
    }

    //Closes the dialogue
    public void EndDialogue()
    {
        //Close the dialogue panel
        dialoguePanel.SetActive(false);

        //Invoke whatever Action queued on dialogue end (if any)
        onDialogueEnd?.Invoke();

        //Reset the Action
        onDialogueEnd = null;
    }

    public void Talk(string speaker, string message)
    {
        //Set the dialogue panel active
        dialoguePanel.SetActive(true);

        //Set the speaker text to the speaker
        speakerText.text = speaker;

        //If there is no speaker, do not show the speaker text panel
        speakerText.transform.parent.gameObject.SetActive(speaker != "");

        //Set the dialogue text to the message
        //dialogueText.text = message;
        StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        //Convert the string to an array of chars
        char[] charsToType = textToType.ToCharArray();
        for(int i = 0; i < charsToType.Length; i++)
        {
            dialogueText.text += charsToType[i];
            yield return new WaitForEndOfFrame();

            //Skip the typing sequence and just show the full text
            if(!isTyping)
            {
                dialogueText.text = textToType;
                //Break out from the loop
                break;
            }
        }

        //Typing sequence complete
        isTyping = false;
    }

    //Converts a simple string into a list of Dialogue lines to put into DialogueManager
    public static List<DialogueLine> CreateSimpleMessage(string message)
    {
        //The Dialogue line we want to output
        DialogueLine messageDialogueLine = new DialogueLine("", message);

        List<DialogueLine> listToReturn = new List<DialogueLine>();

        listToReturn.Add(messageDialogueLine);

        return listToReturn;
    }
}
