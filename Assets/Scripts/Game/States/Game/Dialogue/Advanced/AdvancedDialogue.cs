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
    // List with all conversations
    private Dictionary<int, Conversation> m_conversationList;

    // Constructor
    public AdvancedDialogue(List<Conversation> _conversationList)
    {
        // Add conversations to list
        if (_conversationList != null)
        {
            foreach (Conversation c in _conversationList)
                m_conversationList.Add(c.ConversationID, c);
        }
    }
}
