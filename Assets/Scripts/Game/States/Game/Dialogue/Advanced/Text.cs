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
public class Text
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

    // List with all choices
    private Dictionary<int, Choice> m_choiceList = new Dictionary<int, Choice>();

    // Constructor
    public Text(int _id, List<Choice> _choiceList, List<TextPart> _textParts)
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
}
