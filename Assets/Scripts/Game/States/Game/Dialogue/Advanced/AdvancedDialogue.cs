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

    // Stores information about compiled dynamic codes
    public class CompiledDynamicCode
    {
        public object       m_classInstance = null;
        public MethodInfo   m_entryPoint = null;
    }

    // List with all dynamic codes
    private Dictionary<int, CompiledDynamicCode> m_dynamicCodeList = new Dictionary<int, CompiledDynamicCode>();

    // List with all conversations
    private Dictionary<int, Conversation> m_conversationList = new Dictionary<int,Conversation>();

    // Indicates whether the dialogue is valid
    private bool m_isValid = false;
    public bool Valid
    {
        get { return m_isValid; }
        private set { m_isValid = value; }
    }
    
    // Constructor
    public AdvancedDialogue(List<DynamicCode> _dynamicCodeList, List<Conversation> _conversationList)
    {
        // Compile dynamic codes
        if (_dynamicCodeList != null && _dynamicCodeList.Count > 0)
            compileDynamicCodes(_dynamicCodeList);

        // Add conversations to list
        if (_conversationList != null)
        {
            foreach (Conversation c in _conversationList)
                m_conversationList.Add(c.ConversationID, c);
        }

        // Validate
        validate();
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

    // Compiles the dynamic codes
    private void compileDynamicCodes(List<DynamicCode> _codeList)
    {
        // Local variables
        CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });
        CompilerParameters compilerParams = null;
        CompilerResults cResult = null;
        object classInstance = null;
        MethodInfo entryPoint = null;
        CompiledDynamicCode compiledCode = null;

        // Create params
        // Add assemblies: Unity, Game
        compilerParams = new CompilerParameters();
        compilerParams.CompilerOptions = "/target:library /optimize /warn:0";
        compilerParams.GenerateExecutable = false;
        compilerParams.GenerateInMemory = true;
        compilerParams.IncludeDebugInformation = false;
        compilerParams.ReferencedAssemblies.Add(typeof(Transform).Assembly.Location);
        compilerParams.ReferencedAssemblies.Add("System.dll");
        compilerParams.ReferencedAssemblies.Add("System.Core.dll");
        compilerParams.ReferencedAssemblies.Add(typeof(Game).Assembly.Location);

        // Loop through all codes
        foreach(DynamicCode c in _codeList)
        {
            // Compile
            cResult = provider.CompileAssemblyFromSource(compilerParams, @c.Code);

            // Print errors
            if (cResult.Errors.Count != 0)
            {
                for (int i = 0; i < cResult.Errors.Count; ++i)
                    Debug.LogError("Compile dynamic code (" + c.CodeID + "): " + cResult.Errors[i].ErrorText);
                continue;
            }

            // Create class instance
            classInstance = cResult.CompiledAssembly.CreateInstance(c.ClassPath);
            if(classInstance == null)
            {
                Debug.LogError("Compile dynamic code (" + c.CodeID + "): class creation failed!");
                continue;
            }

            // Get entry point
            entryPoint = classInstance.GetType().GetMethod(c.EntryPoint);
            if(entryPoint == null)
            {
                Debug.LogError("Compile dynamic code (" + c.CodeID + "): entry point not found!");
                continue;
            }

            // Add to list
            compiledCode = new CompiledDynamicCode();
            compiledCode.m_classInstance = entryPoint;
            compiledCode.m_entryPoint = entryPoint;
            m_dynamicCodeList.Add(c.CodeID, compiledCode);
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
            if (validateConversation(c.Value) == false)
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
        int tempInt = -1;
        CompiledDynamicCode code = null;

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

            // Next target available?
            else if (choice.NextTextID != -1)
            {
                if (_c.getTextByID(choice.NextTextID) == null)
                {
                    Debug.LogError("Choice's target ID has not been found!");
                    return false;
                }
            }

            // Target for "enabled_func" available?
            if (choice.EnabledFunc != null && choice.EnabledFunc.Length > 0)
            {
                // Try to parse
                try
                {
                    tempInt = int.Parse(choice.EnabledFunc);
                }
                catch (System.FormatException _e)
                {
                    tempInt = -1;
                }

                // Invalid ID? Try standard assets
                if(tempInt == -1)
                {

                }

                // Dynamic code available?
                if(m_dynamicCodeList.ContainsKey(tempInt) == false)
                {
                    Debug.LogError("Choice's enabled_func target has not been found!");
                    return false;
                }
                code = m_dynamicCodeList[tempInt];
                if(code.m_entryPoint.ReturnType != typeof(bool))
                {
                    Debug.LogError("Choice's enabled_func target need a boolean return type!");
                    return false;
                }
                if(code.m_entryPoint.GetParameters().Length != 0)
                {
                    Debug.LogError("Choice's enabled_func target does not support parameters yet!");
                    return false;
                }
            }
        }

        // Auto generated/executed choice available?
        if (_t.AutoChoiceType != DialogueText.ChoiceType.CHOICE_NONE && choiceIDs.Count > 0)
            Debug.LogWarning("Auto generated/executed choice in text (" + _t.TextID + ") will be overwritten by choices");

        return true;
    }
}
