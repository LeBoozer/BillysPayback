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

    // Value of the func: enabled
    private string m_funcValueStartTextID;

    // True if this choice is enabled 
    private System.Func<int> m_funcGetStartTextID;
    public int StartTextID
    {
        get { if (m_funcGetStartTextID == null) return -1; return m_funcGetStartTextID(); }
        private set { }
    }

    // List with all texts
    private Dictionary<int, DialogueText> m_textList = new Dictionary<int,DialogueText>();
    public int TextCount
    {
        get { return m_textList.Count; }
        private set { }
    }

    // Constructor
    public Conversation(int _id, string _funcValueStartTextID, List<DialogueText> _textList)
    {
        // Copy
        m_conversationID = _id;
        m_funcValueStartTextID = _funcValueStartTextID;

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

    // Initializes the conversation
    // Returns true on success
    public bool initialize(AdvancedDialogue _dialog)
    {
        // Local variables
        DynamicScript script = null;
        int id = 0;

        // Define function: enabled
        if (m_funcValueStartTextID == null || m_funcValueStartTextID.Length == 0)
            m_funcGetStartTextID = null;
        else if (System.Int32.TryParse(m_funcValueStartTextID, out id) == true)
            m_funcGetStartTextID = new System.Func<int>(() => { return id; });
        else
        {
            // Try to get script
            if (_dialog.DynamicScripts.ContainsKey(m_funcValueStartTextID) == false)
                return false;
            script = _dialog.DynamicScripts[m_funcValueStartTextID];
            m_funcGetStartTextID = new System.Func<int>(() => { object v = null; Game.Instance.ScriptEngine.executeScript(script, ref v); return (System.Int32)v; });
        }

        return true;
    }
}