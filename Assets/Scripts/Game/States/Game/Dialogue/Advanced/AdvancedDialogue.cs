/*
 * Project:	Billy's Payback
 * File:	AdvancedDialogue.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents an advanced dialog
 */
public class AdvancedDialogue
{
    // Constant values
    public static readonly string DIALOGUE_CANCELLED_EXIT_VALUE = "_cancel_";
    public static readonly string DIALOGUE_NO_CHOICE_EXIT_VALUE = "_done_";

    // List with all conversations
    private Dictionary<int, Conversation> m_conversationList = new Dictionary<int,Conversation>();

    // Indicates whether the dialogue is valid
    private bool m_isValid = false;
    public bool Valid
    {
        get { return m_isValid; }
        private set { m_isValid = value; }
    }
    
    // Constructor
    public AdvancedDialogue(List<Conversation> _conversationList)
    {
        // Add conversations to list
        if (_conversationList != null)
        {
            foreach (Conversation c in _conversationList)
                m_conversationList.Add(c.ConversationID, c);
        }

        // Validate
        validate();
    }

    // Returns a conversation by its ID (can be null!)
    public Conversation getConversationByID(int _id)
    {
        if (m_conversationList.ContainsKey(_id) == false)
            return null;
        return m_conversationList[_id];
    }

    // Returns a list with all keys of the conversations
    public List<int> getConversationIDs()
    {
        // Local variables
        List<int> keys = new List<int>(m_conversationList.Keys);
        return keys;
    }

    // Validates the parsed dialogue
    private void validate()
    {
        // Conversations available?
        if (m_conversationList.Count == 0)
            return;

        // Validate conversations
        foreach (KeyValuePair<int, Conversation> c in m_conversationList)
        {
            if (validateConversation(c.Value) == false)
                return;
        }

        // Set flag
        m_isValid = true;
    }

    // Validates a single conversations
    private bool validateConversation(Conversation _c)
    {
        // Local variables
        List<int> textIDs = null;

        // Check parameter
        if (_c == null)
            return false;

        // Start text available?
        if (_c.getTextByID(_c.StartTextID) == null)
        {
            Debug.LogError("The specified text start-ID of a conversation does not exist!");
            return false;
        }

        // Validate all texts
        textIDs = _c.getTextIDs();
        foreach (int id in textIDs)
        {
            if (validateText(_c, _c.getTextByID(id)) == false)
                return false;
        }

        return true;
    }

    // Validates a single text
    private bool validateText(Conversation _c, DialogueText _t)
    {
        // Local variables
        List<int> choiceIDs = null;
        TextPart part = null;
        Choice choice = null;

        // Check parameter
        if (_t == null)
            return false;

        // Validate all text-parts
        for (int i = 0; i < _t.TextPartCount; ++i)
        {
            // Get text-part
            part = _t.getTextPartByIndex(i);
            if(part == null)
            {
                Debug.LogError("Invalid text-part!");
                return false;
            }

            // Validate time
            if(part.DisplayTime <= 0)
            {
                Debug.LogError("Text-part has an invalid display time: " + part.DisplayTime);
                return false;
            }
        }

        // Validate all choices
        choiceIDs = _t.getChoicesIDs();
        foreach(int id in choiceIDs)
        {
            // Get choice
            choice = _t.getChoiceByID(id);
            if(choice == null)
            {
                Debug.LogError("ID of the choice is invalid!");
                return false;
            }

            // Next text AND exit value defined?
            if(choice.ExitValue != null && choice.ExitValue.Equals("") == false && choice.NextTextID != -1)
            {
                Debug.LogError("Choice can have a next target OR an exit value, not both!");
                return false;
            }

            // Next target available?
            else if(choice.NextTextID != -1)
            {
                if(_c.getTextByID(choice.NextTextID) == null)
                {
                    Debug.LogError("Choice's target ID has not been found!");
                    return false;
                }
            }
        }

        return true;
    }
}
