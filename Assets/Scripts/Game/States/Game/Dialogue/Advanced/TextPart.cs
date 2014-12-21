/*
 * Project:	Billy's Payback
 * File:	TextPart.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents a single text part
 */
public class TextPart
{
    // The display time for this text part (in seconds)
    private int m_displayTime;
    public int DisplayTime
    {
        get { return m_displayTime; }
        private set { }
    }

    // The speaking character in this text part
    private string m_character;
    public string Character
    {
        get { return m_character; }
        private set { }
    }

    // The text of this text part
    private string m_text;
    public string Text
    {
        get { return m_text; }
        private set { m_text = value; }
    }

    // Constructor
    public TextPart(int _displayTime, string _character, string _text)
    {
        m_displayTime = _displayTime;
        m_character = _character;
        m_text = _text;
    }
}
