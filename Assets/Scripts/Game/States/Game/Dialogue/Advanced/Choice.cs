/*
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

    // ID for attribute: enabled_func
    private string m_enabledFuncID;
    public string EnabledFuncID
    {
        get { return m_enabledFuncID; }
        private set { m_enabledFuncID = value; }
    }

    // The compiled function for: enabled_func
    private AdvancedDialogue.CompiledDynamicCode m_enabledFunc;
    public AdvancedDialogue.CompiledDynamicCode EnabledFunc
    {
        get { return m_enabledFunc; }
        set { m_enabledFunc = value; }
    }
    
    // Constructor
    public Choice(int _id, string _text, int _nextTextID, string _exitValue, string _enabledFuncID)
    {
        // Copy
        m_choiceID = _id;
        m_text = _text;
        m_nextTextID = _nextTextID;
        m_exitValue = _exitValue;
        m_enabledFuncID = _enabledFuncID;

        // Set type
        if (m_exitValue == null || m_exitValue.Length == 0)
            m_choiceType = ChoiceType.CHOICE_NEXT_TEXT;
        else
            m_choiceType = ChoiceType.CHOICE_EXIT;
    }

    // Returns true if this choice is enabled, otherwise false
    public bool isEnabled()
    {
        // Check parameter
        if (m_enabledFunc == null)
            return true;

        // Execute
        return (bool)m_enabledFunc.m_entryPoint.Invoke(m_enabledFunc.m_classInstance, null);
    }
}