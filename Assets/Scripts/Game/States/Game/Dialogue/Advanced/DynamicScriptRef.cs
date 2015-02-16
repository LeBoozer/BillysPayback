/*
 * Project:	Billy's Payback
 * File:	DynamicScriptRef.cs
 * Authors:	Byron Worms
 * Editors:	-
 */
using UnityEngine;
using System.Collections;

/*
 * Represents a single reference to a dynamic script
 */
public class DynamicScriptRef
{
    // The script's name
    private string m_scriptName;
    public string ScriptName
    {
        get { return m_scriptName; }
        private set { m_scriptName = value; }
    }

    // The script's run-time
    private string m_runtime = "";
    public string Runtime
    {
        get { return m_runtime; }
        private set { m_runtime = value; }
    }

    // Constructor
    public DynamicScriptRef(string _name, string _runtime)
    {
        // Copy
        m_scriptName = _name;
        m_runtime = _runtime;
    }
}
