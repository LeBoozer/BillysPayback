/*
 * Project:	Billy's Payback
 * File:	Text.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Represents a single text
 */
public class DialogueText
{
    // The text's ID
    private int m_textID;
    public int TextID
    {
        get { return m_textID; }
        private set { m_textID = value; }
    }
   
    // List with all text parts
    private List<TextPart> m_textParts = new List<TextPart>();
    public int TextPartCount
    {
        get { return m_textParts.Count; }
        private set { }
    }

    // List with all choices
    private Dictionary<int, Choice> m_choiceList = new Dictionary<int, Choice>();
    public int ChoiceCount
    {
        get { return m_choiceList.Count; }
        private set { }
    }

    // Constructor
    public DialogueText(int _id, List<Choice> _choiceList, List<TextPart> _textParts)
    {
        // Copy
        m_textID = _id;
        m_textParts = _textParts;

        // Add choices to list
        if(_choiceList != null)
        {
            foreach (Choice c in _choiceList)
                m_choiceList.Add(c.ChoiceID, c);
        }
    }

    // Returns a text-part by its index (can be null!)
    public TextPart getTextPartByIndex(int _index)
    {
        if (_index >= m_textParts.Count)
            return null;
        return m_textParts[_index];
    }

    // Returns a choice by its ID (can be null!)
    public Choice getChoiceByID(int _id)
    {
        if (m_choiceList.ContainsKey(_id) == false)
            return null;
        return m_choiceList[_id];
    }

    // Returns a list with all keys of all choices
    public List<int> getChoicesIDs()
    {
        // Local variables
        List<int> keys = new List<int>(m_choiceList.Keys);
        return keys;
    }
}
