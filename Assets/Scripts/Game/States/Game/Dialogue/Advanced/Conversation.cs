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
    private Dictionary<int, Text> m_textList;

    // Constructor
    public Conversation(int _id, int _startTextID, List<Text> _textList)
    {
        // Copy
        m_conversationID = _id;
        m_startTextID = _startTextID;

        // Add texts to list
        if (_textList != null)
        {
            foreach (Text c in _textList)
                m_textList.Add(c.TextID, c);
        }
    }
}