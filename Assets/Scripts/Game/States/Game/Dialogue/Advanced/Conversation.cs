/*
 * Project:	Billy's Payback
 * File:	Conversation.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents a single conversation
 */
public class Conversation
{
    // The conversation's ID
    private int m_conversationID;
    public int ConversationID
    {
        get { return m_conversationID; }
        private set { }
    }

    // The ID of the starting text
    private int m_startTextID;
    public int StartTextID
    {
        get { return m_startTextID; }
        private set { m_startTextID = value; }
    }

    // List with all texts
    private Dictionary<int, DialogueText> m_textList = new Dictionary<int,DialogueText>();
    public int TextCount
    {
        get { return m_textList.Count; }
        private set { }
    }

    // Constructor
    public Conversation(int _id, int _startTextID, List<DialogueText> _textList)
    {
        // Copy
        m_conversationID = _id;
        m_startTextID = _startTextID;

        // Add texts to list
        if (_textList != null)
        {
            foreach (DialogueText c in _textList)
                m_textList.Add(c.TextID, c);
        }
    }

    // Returns a text by its ID (can be null!)
    public DialogueText getTextByID(int _id)
    {
        if (m_textList.ContainsKey(_id) == false)
            return null;
        return m_textList[_id];
    }

    // Returns a list with all keys of the texts
    public List<int> getTextIDs()
    {
        // Local variables
        List<int> keys = new List<int>(m_textList.Keys);
        return keys;
    }
}