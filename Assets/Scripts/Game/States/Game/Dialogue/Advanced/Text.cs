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
    // Enumeration with all valid types for a auto generated/executed choice
    public enum ChoiceType
    {
        // Do nothing
        CHOICE_NONE,

        // Calls the next references text
        CHOICE_NEXT_TEXT,

        // Exits the conversation
        CHOICE_EXIT
    }

    // The text's ID
    private int m_textID;
    public int TextID
    {
        get { return m_textID; }
        private set { m_textID = value; }
    }
   
    // Type of the auto generated and executed choice (if available)
    private ChoiceType m_autoChoiceType;
    public ChoiceType AutoChoiceType
    {
        get { return m_autoChoiceType; }
        private set { }
    }

    // ID of the next text
    private int m_nextTextID;
    public int NextTextID
    {
        get { return m_nextTextID; }
        private set { }
    }

    // The exit value for the conversation
    private string m_exitValue;
    public string ExitValue
    {
        get { return m_exitValue; }
        private set { }
    }

    // Value of the func: show parts
    private string m_funcValuShowParts;

    // True if the text-parts are enabled
    private System.Func<bool> m_funcShowParts;
    public bool IsShowParts
    {
        get { if (m_funcShowParts == null) return true; return m_funcShowParts(); }
        private set { }
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

    // Constructors
    public DialogueText(int _id, int _nextTextID, string _exitValue, string _funcValueShowParts, List<Choice> _choiceList, List<TextPart> _textParts)
    {
        // Copy
        m_textID = _id;
        m_exitValue = _exitValue;
        m_funcValuShowParts = _funcValueShowParts;
        m_nextTextID = _nextTextID;
        m_textParts = _textParts;

        // Add choices to list
        if (_choiceList != null)
        {
            foreach (Choice c in _choiceList)
                m_choiceList.Add(c.ChoiceID, c);
        }

        // Set type
        if (m_exitValue == null || m_exitValue.Length == 0 && m_nextTextID < 0)
            m_autoChoiceType = ChoiceType.CHOICE_NONE;
        else if (m_exitValue == null || m_exitValue.Length == 0)
            m_autoChoiceType = ChoiceType.CHOICE_NEXT_TEXT;
        else
            m_autoChoiceType = ChoiceType.CHOICE_EXIT;
    }
    public DialogueText(int _id, List<Choice> _choiceList, List<TextPart> _textParts) :
        this(_id, -1, null, null, _choiceList, _textParts)
    { }

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

    // Initializes the text
    // Returns true on success
    public bool initialize(AdvancedDialogue _dialog)
    {
        // Define function: show parts
        if (m_funcValuShowParts == null || m_funcValuShowParts.Length == 0)
        m_funcShowParts = new System.Func<bool>(() => { return true; });
        else if (m_funcValuShowParts.Equals("0") || m_funcValuShowParts.Equals("1"))
            m_funcShowParts = new System.Func<bool>(() => { return m_funcValuShowParts.Equals("0") == true ? false : true; });
        else
        {
            // Try to get script
            if (_dialog.DynamicScripts.ContainsKey(m_funcValuShowParts) == false)
                return false;
            m_funcShowParts = new System.Func<bool>(() =>
            {
                bool v = false;
                Game.Instance.ScriptEngine.executeScript<bool>(_dialog.DynamicScripts[m_funcValuShowParts], ref v);
                return v;
            });
        }

        return true;
    }
}
