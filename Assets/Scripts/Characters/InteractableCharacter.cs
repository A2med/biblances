using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCharacter : InteractableObject
{
    public CharacterData characterData;

    //Cache the relationship data of the NPC so we can access it
    NPCRelationshipState relationship;

    //The rotation it should be facing by default
    Quaternion defaultRotation;
    //Check if the LookAt coroutine is currently being executed
    bool isTurning = false;

    private void Start()
    {
        relationship = RelationshipStats.GetRelationship(characterData);

        //Cache the original rotation of the characters
        defaultRotation = transform.rotation;
    }

    public override void Pickup()
    {
        LookAtPlayer();
        TriggerDialogue();
    }

    #region Rotation
    void LookAtPlayer()
    {
        //Get the player's transform
        Transform player = FindObjectOfType<PlayerController>().transform;

        //Get a vector for the direction towards the player
        Vector3 dir = player.position - transform.position;
        //Lock the y axis of the vector so the npc doesn't look up or down to the player
        dir.y = 0;
        //Convert the direction vector into a quaternion
        Quaternion lookRot = Quaternion.LookRotation(dir);
        //Look at the player
        transform.rotation = lookRot;
        StartCoroutine(LookAt(lookRot));
    }

    //Coroutine for the character to progressively turn towards a rotation
    IEnumerator LookAt(Quaternion lookRot)
    {
        //Check if the coroutine is already running
        if(isTurning)
        {
            //Stop the coroutine
            isTurning = false;
        }
        else
        {
            isTurning = true;
        }

        while(transform.rotation != lookRot)
        {
            if(!isTurning)
            {
                //Stop coroutine execut
                yield break;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 720 * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        isTurning = false;
    }

    //Rotate back to its original rotation
    void ResetRotation()
    {
        StartCoroutine(LookAt(defaultRotation));
    }
    #endregion

    #region Conversation Interactions
    void TriggerDialogue()
    {
        //Check if the player is holding anything
        if(InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            //Switch over to the Gift Dialogue function
            GiftDialogue();
            return;
        }

        List<DialogueLine> dialogueToHave = characterData.defaultDialogue;

        System.Action onDialogueEnd = null;

        //Have the character reset their rotation after the conversation is over
        onDialogueEnd += ResetRotation;

        //Do the checks to determine which dialogue to put out

        //Is the player meeting for the first time?
        if (RelationshipStats.FirstMeeting(characterData))
        {
            //Assign the first meet dialogue
            dialogueToHave = characterData.onFirstMeet;
            onDialogueEnd += OnFirstMeeting;
        }

        if (RelationshipStats.IsFirstConversationOfTheDay(characterData))
        {
            onDialogueEnd += OnFirstConversation;
        }

        DialogueManager.Instance.StartDialogue(dialogueToHave, onDialogueEnd);
    }

    void GiftDialogue()
    {
        if (!EligibleForGift()) return;

        //Get the ItemSlotData of what the player is holding
        ItemSlotData handSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item);

        List<DialogueLine> dialogueToHave = characterData.neutralGiftDialogue;

        System.Action onDialogueEnd = () =>
        {
            //Mark gift as given for today
            relationship.giftGivenToday = true;

            //Remove the item from the player's hand
            InventoryManager.Instance.ConsumeItem(handSlot);
        };

        //Have the character reset their rotation after the conversation is over
        onDialogueEnd += ResetRotation;

        bool isBirthday = RelationshipStats.IsBirthday(characterData);

        //The friendship points to add from the gift
        int pointsToAdd = 0;

        //Do the checks to determine which dialogue to put out
        switch(RelationshipStats.GetReactionToGift(characterData, handSlot.itemData))
        {
            case RelationshipStats.GiftReaction.Like:
                dialogueToHave = characterData.likedGiftDialogue;
                //80
                pointsToAdd = 80;
                if (isBirthday) dialogueToHave = characterData.birthdayLikedGiftDialogue;
                break;
            case RelationshipStats.GiftReaction.Dislike:
                dialogueToHave = characterData.dislikedGiftDialogue;
                //-20
                pointsToAdd = -20;
                if (isBirthday) dialogueToHave = characterData.birthdayDislikedGiftDialogue;
                break;
            case RelationshipStats.GiftReaction.Neutral:
                //20
                pointsToAdd = 20;
                if (isBirthday) dialogueToHave = characterData.birthdayNeutralGiftDialogue;
                break;
        }

        //Birthday multiplier
        if(isBirthday) pointsToAdd *= 8;

        RelationshipStats.AddFriendPoints(characterData, pointsToAdd);

        DialogueManager.Instance.StartDialogue(dialogueToHave, onDialogueEnd);
    }

    //Check if the character can be given a gift
    bool EligibleForGift()
    {
        //Reject condition: Player has not unlocked this character yet
        if(RelationshipStats.FirstMeeting(characterData))
        {
            DialogueManager.Instance.StartDialogue(DialogueManager.CreateSimpleMessage("You have not unlocked this character yet.")); ;
            return false;
        }

        //Reject condition: Player has already given this character a gift today
        if(RelationshipStats.GiftGivenToday(characterData))
        {
            DialogueManager.Instance.StartDialogue(DialogueManager.CreateSimpleMessage($"You have already given {characterData.name} a gift today."));
            return false;
        }

        return true;
    }

    void OnFirstMeeting()
    {
        //Unlock the character on the relationships
        RelationshipStats.UnlockCharacter(characterData);
        //Update the relationship data
        relationship = RelationshipStats.GetRelationship(characterData);
    }

    void OnFirstConversation()
    {
        Debug.Log("This is the first conversation of the day");
        //Add 20 friend points
        RelationshipStats.AddFriendPoints(characterData, 20);

        relationship.hasTalkedToday = true;
    }
    #endregion
}
