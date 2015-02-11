﻿/*
 * Project:	Billy's Payback
 * File:	Choice.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents a single choice
 */
public class Choice 
{
    // Enumeration with all valid types for a choice
    public enum ChoiceType
    {
        // Calls the next references text
        CHOICE_NEXT_TEXT,

        // Exits the conversation
        CHOICE_EXIT
    }

    // The choice's type
    private ChoiceType m_choiceType;
    public ChoiceType Type
    {
        get { return m_choiceType; }
        private set { }
    }

    // The choice's ID
    private int m_choiceID;
    public int ChoiceID
    {
        get { return m_choiceID; }
        private set { }
    }

    // The text of this choice
    private string m_text;
    public string Text
    {
        get { return m_text; }
        private set { m_text = value; }
    }

    // The exit value for the conversation
    private string m_exitValue;
    public string ExitValue
    {
        get { return m_exitValue; }
        private set { }
    }

    // Value of the func: enabled
    private string m_funcValueEnabled;

    // Value of the func: next text ID
    private string m_funcValueNextTextID;

    // True if this choice is enabled 
    private System.Func<bool> m_funcIsEnabled;
    public bool IsEnabled
    {
        get { if (m_funcIsEnabled == null) return true; return m_funcIsEnabled(); }
        private set { }
    }

    // Returns the ID of the next text
    private System.Func<int> m_funcNextTextID;
    public int NextTextID
    {
        get { if (m_funcNextTextID == null) return -1; return m_funcNextTextID(); }
        private set { }
    }

    // Returns true if the next text ID is generated by a script
    private bool m_isNextTextIDFunc = false;
    public bool IsNextTextIDFunctionGenerated
    {
        get { return m_isNextTextIDFunc; }
        private set { }
    }
    

    // Constructor
    public Choice(int _id, string _text, string _funcValueNextTextID, string _exitValue, string _funcValueEnabled)
    {
        // Copy
        m_choiceID = _id;
        m_text = _text;
        m_exitValue = _exitValue;
        m_funcValueEnabled = _funcValueEnabled;
        m_funcValueNextTextID = _funcValueNextTextID;

        // Set type
        if (m_exitValue == null || m_exitValue.Length == 0)
            m_choiceType = ChoiceType.CHOICE_NEXT_TEXT;
        else
            m_choiceType = ChoiceType.CHOICE_EXIT;
    }

    // Initializes the choice
    // Returns true on success
    public bool initialize(AdvancedDialogue _dialog)
    {
        // Local variables
        DynamicScript script = null;
        int id = 0;

        // Define function: enabled
        if (m_funcValueEnabled == null || m_funcValueEnabled.Length == 0)
            m_funcIsEnabled = new System.Func<bool>(() => { return true; });
        else if (m_funcValueEnabled.Equals("0") || m_funcValueEnabled.Equals("1"))
            m_funcIsEnabled = new System.Func<bool>(() => { return m_funcValueEnabled.Equals("0") == true ? false : true; });
        else
        {
            // Try to get script
            if(_dialog.DynamicScripts.ContainsKey(m_funcValueEnabled) == false)
                return false;
            script = _dialog.DynamicScripts[m_funcValueEnabled];
            m_funcIsEnabled = new System.Func<bool>(() => { object v = false; Game.Instance.ScriptEngine.executeScript(script, ref v); return (System.Boolean)v; });
        }

        // Define function: next text ID
        if (m_choiceType == ChoiceType.CHOICE_NEXT_TEXT)
        {
            if (m_funcValueNextTextID == null || m_funcValueNextTextID.Length == 0)
            {
                Debug.LogError("Choice with \"next text\" identifiers need an valid ID!");
                return false;
            }
            else if (System.Int32.TryParse(m_funcValueNextTextID, out id) == true)
                m_funcNextTextID = new System.Func<int>(() => { return id; });
            else
            {
                // Try to get script
                if (_dialog.DynamicScripts.ContainsKey(m_funcValueNextTextID) == false)
                {
                    Debug.LogError("Choice with \"next text\" identifiers need an valid ID!");
                    return false;
                }
                script = _dialog.DynamicScripts[m_funcValueNextTextID];
                m_funcNextTextID = new System.Func<int>(() => { object v = null; Game.Instance.ScriptEngine.executeScript(script, ref v); return (System.Int32)v; });

                // Set flag
                m_isNextTextIDFunc = true;
            }
        }

        return true;
    }
}