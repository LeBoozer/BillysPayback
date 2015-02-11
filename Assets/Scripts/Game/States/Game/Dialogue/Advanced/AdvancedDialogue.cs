/*
 * Project:	Billy's Payback
 * File:	AdvancedDialogue.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

/*
 * Represents an advanced dialog
 */
public class AdvancedDialogue
{
    // Constant values
    public static readonly string DIALOGUE_CANCELLED_EXIT_VALUE = "_cancel_";
    public static readonly string DIALOGUE_NO_CHOICE_EXIT_VALUE = "_done_";

    // List with all dynamic scripts
    private Dictionary<string, DynamicScript> m_dynamicScriptList = new Dictionary<string, DynamicScript>();
    public Dictionary<string, DynamicScript> DynamicScripts
    {
        get { return m_dynamicScriptList; }
        private set { }
    }

    // List with all conversations
    private Dictionary<int, Conversation> m_conversationList = new Dictionary<int,Conversation>();

    // Value of the func: override start conversation ID
    private string m_funcValueOverrideStartConvID;

    // Returns the ID of the overwritten start conversion ID or -1 if not used
    private System.Func<int> m_funcOverrideStartConvID;
    public int GetOverrideStartConversationID
    {
        get { if (m_funcOverrideStartConvID == null) return -1; return m_funcOverrideStartConvID(); }
        private set { }
    }

    // Indicates whether the dialogue is valid
    private bool m_isValid = false;
    public bool Valid
    {
        get { return m_isValid; }
        private set { m_isValid = value; }
    }
  
    // Constructor
    public AdvancedDialogue(List<DynamicScript> _dynamicScriptList, List<Conversation> _conversationList, string _funcValueOverrideStartConvID)
    {
        // Copy parameter
        m_funcValueOverrideStartConvID = _funcValueOverrideStartConvID;

        // Copy scripts
        foreach (DynamicScript s in _dynamicScriptList)
            m_dynamicScriptList.Add(s.ScriptName, s);

        // Add conversations to list
        if (_conversationList != null)
        {
            foreach (Conversation c in _conversationList)
                m_conversationList.Add(c.ConversationID, c);
        }

        // Validate
        validate();

        // Initializes the dialogue
        initialize();
    }

    // Returns a conversation by its ID (can be null!)
    public Conversation getConversationByID(int _id)
    {
        if (m_conversationList.ContainsKey(_id) == false)
            return null;
        return m_conversationList[_id];
    }

    // Returns a list with all keys of the conversations
    public List<int> getConversationIDs()
    {
        // Local variables
        List<int> keys = new List<int>(m_conversationList.Keys);
        return keys;
    }

    // Executes all scripts from a given run-type
    public void executeScriptFromRunType(DynamicScript.RunType _type)
    {
        // Local variables
        object nullObj = null;

        // Loop through all scripts
        foreach(var pair in m_dynamicScriptList)
        {
            // Type?
            if(pair.Value.ScriptRunType.Equals(_type) == true)
                Game.Instance.ScriptEngine.executeScript(pair.Value, ref nullObj);
        }
    }

    // Initializes the dialogue
    private void initialize()
    {
        // Local variables
        DynamicScript script = null;
        int id = 0;

        // Define function: override start conversation ID
        if (m_funcValueOverrideStartConvID == null || m_funcValueOverrideStartConvID.Length == 0)
            m_funcOverrideStartConvID = null;
        else if (System.Int32.TryParse(m_funcValueOverrideStartConvID, out id) == true)
            m_funcOverrideStartConvID = new System.Func<int>(() => { return id; });
        else
        {
            // Try to get script
            if (m_dynamicScriptList.ContainsKey(m_funcValueOverrideStartConvID) == false)
                return;
            script = m_dynamicScriptList[m_funcValueOverrideStartConvID];
            m_funcOverrideStartConvID = new System.Func<int>(() => { object v = null; Game.Instance.ScriptEngine.executeScript(script, ref v); return (System.Int32)v; });
        }
    }

    // Validates the parsed dialogue
    private void validate()
    {
        // Conversations available?
        if (m_conversationList.Count == 0)
            return;

        // Validate conversations
        foreach (KeyValuePair<int, Conversation> c in m_conversationList)
        {
            if (c.Value.initialize(this) == false || validateConversation(c.Value) == false)
                return;
        }

        // Set flag
        m_isValid = true;
    }

    // Validates a single conversations
    private bool validateConversation(Conversation _c)
    {
        // Local variables
        List<int> textIDs = null;

        // Check parameter
        if (_c == null)
            return false;

        // Start text available?
        if (_c.getTextByID(_c.StartTextID) == null)
        {
            Debug.LogError("The specified text start-ID of a conversation does not exist!");
            return false;
        }

        // Validate all texts
        textIDs = _c.getTextIDs();
        foreach (int id in textIDs)
        {
            if (validateText(_c, _c.getTextByID(id)) == false)
                return false;
        }

        return true;
    }

    // Validates a single text
    private bool validateText(Conversation _c, DialogueText _t)
    {
        // Local variables
        List<int> choiceIDs = null;
        TextPart part = null;
        Choice choice = null;

        // Check parameter
        if (_t == null)
            return false;

        // Validate all text-parts
        for (int i = 0; i < _t.TextPartCount; ++i)
        {
            // Get text-part
            part = _t.getTextPartByIndex(i);
            if(part == null)
            {
                Debug.LogError("Invalid text-part!");
                return false;
            }

            // Validate time
            if(part.DisplayTime <= 0)
            {
                Debug.LogError("Text-part has an invalid display time: " + part.DisplayTime);
                return false;
            }
        }

        // Validate all choices
        choiceIDs = _t.getChoicesIDs();
        foreach (int id in choiceIDs)
        {
            // Get choice
            choice = _t.getChoiceByID(id);
            if (choice == null)
            {
                Debug.LogError("ID of the choice is invalid!");
                return false;
            }

            // Next text AND exit value defined?
            if (choice.ExitValue != null && choice.ExitValue.Equals("") == false && choice.NextTextID != -1)
            {
                Debug.LogError("Choice can have a next target OR an exit value, not both!");
                return false;
            }

            // Next target available (only if not script generated)?
            else if (choice.IsNextTextIDFunctionGenerated == false && choice.NextTextID != -1)
            {
                if (_c.getTextByID(choice.NextTextID) == null)
                {
                    Debug.LogError("Choice's target ID has not been found!");
                    return false;
                }
            }

            // Initialize choice
            if(choice.initialize(this) == false)
            {
                Debug.LogError("Choice initialization failed!");
                return false;
            }
        }

        // Auto generated/executed choice available?
        if (_t.AutoChoiceType != DialogueText.ChoiceType.CHOICE_NONE && choiceIDs.Count > 0)
            Debug.LogWarning("Auto generated/executed choice in text (" + _t.TextID + ") will be overwritten by choices");

        return true;
    }
}
